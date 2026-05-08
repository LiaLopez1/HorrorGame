using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.UI;

public class PruebaControlador : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 10f;
    public float runSpeed = 20f;
    public float crouchHeight = 0.2f;
    public float mouseSensitivity = 600f;
    public Transform cameraTransform;
    public float raycastDistance = 5f;  // Para detectar objetos al frente

    [Header("Sound DB Boost")]
    [SerializeField] private float runDBBoost = 6f;
    [SerializeField] private float walkDBBoost = 2f;
    [SerializeField] private float crouchDBBoost = 2f; // Cambiado de 0f a 2f para que se escuchen los pasos al agacharse
    [SerializeField] private float bloodDBBoost = 4f;
    [SerializeField] private float glassDBBoost = 5f;

    [Header("Crouch Settings")]
    public float tiempoAgachado = 0.1f;
    public float crouchVelocityThreshold = 0.03f; // Umbral menor para detectar movimiento al agacharse
    public float groundCheckOffset = 0.1f; // Offset adicional para detectar el suelo cuando estamos agachados

    [Header("UI")]
    [SerializeField] private GameObject deathCanvas;

    [Header("Mouse Sensitivity UI")]
    [SerializeField] private Slider mouseSensitivitySlider;

    // Componentes privados
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private float rotationX = 0f;
    private Vector3 escalaNormal;
    private Vector3 escalaAgachado;
    private bool agachado = false;
    private int currentFloorValue = 0;
    private LayerMask floorLayerMask; // Para el raycast de superficies
    private MonsterMovement monster;

    private OutlineController currentOutline;

    // FMOD
    private FMODEvents fmodEvents;
    private EventInstance footstepsInstance;
    private PARAMETER_ID floorParamID;
    private float walkCrouchRunValue = 0f;

    void Start()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivity);

        if (mouseSensitivitySlider != null)
        {
            mouseSensitivitySlider.value = mouseSensitivity;
            mouseSensitivitySlider.onValueChanged.AddListener(ChangeMouseSensitivity);
        }


        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;

        // Configuración de escalas para agacharse
        escalaNormal = transform.localScale;
        escalaAgachado = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);

        // Inicialización FMOD
        fmodEvents = FMODEvents.instance;
        EventDescription eventDesc = RuntimeManager.GetEventDescription(fmodEvents.Footsteps);
        PARAMETER_DESCRIPTION paramDesc;
        eventDesc.getParameterDescriptionByName("floor", out paramDesc);
        floorParamID = paramDesc.id;

        // Capa para el raycast de superficies
        floorLayerMask = LayerMask.GetMask("Floor");

        monster = FindObjectOfType<MonsterMovement>();
    }

    void Update()
    {
        if (monster != null && monster.isPlayerDead)
        {
            // Mostrar el canvas de muerte y no permitir movimiento
            if (deathCanvas != null && !deathCanvas.activeSelf)
            {
                deathCanvas.SetActive(true);
            }
            return;
        }

        Move();
        Look();
        Crouch();
        Raycast(); // Detección de objetos al frente
        HandleFootsteps();
        UpdateDBBoost(); // Actualiza los dB adicionales
    }



    // --- Sistema de Movimiento ---
    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        
        // Reduce la velocidad cuando estamos agachados
        if (agachado)
        {
            speed *= 0.5f;  // Mitad de velocidad cuando estamos agachados
        }

        Vector3 velocity = moveDirection * speed;
        velocity.y = rb.linearVelocity.y;
        rb.linearVelocity = velocity;
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity ;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity ;

        transform.Rotate(Vector3.up * mouseX);
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    void Crouch()
    {
        agachado = Input.GetKey(KeyCode.C);
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            agachado ? escalaAgachado : escalaNormal,
            Time.deltaTime / tiempoAgachado
        );
    }

    // --- Sistema de Raycast (para objetos al frente) ---
    void Raycast()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            Debug.DrawRay(cameraTransform.position, cameraTransform.forward * raycastDistance, Color.red);

            // Intentamos obtener el OutlineController del objeto que estamos mirando
            OutlineController outline = hit.collider.GetComponentInParent<OutlineController>();

            // Si es un objeto nuevo al que apuntamos
            if (outline != null && outline != currentOutline)
            {
                // Ocultamos el anterior (si existe)
                if (currentOutline != null)
                    currentOutline.HideOutline();

                // Activamos el nuevo
                outline.ShowOutline();
                currentOutline = outline;
            }
            else if (outline == null && currentOutline != null)
            {
                // Si dejamos de mirar un objeto interactuable
                currentOutline.HideOutline();
                currentOutline = null;
            }
        }
        else
        {
            // Si no estamos mirando nada, ocultamos el contorno si hay uno activo
            if (currentOutline != null)
            {
                currentOutline.HideOutline();
                currentOutline = null;
            }
        }

        // Raycast adicional para detectar superficies
        RaycastHit surfaceHit;
        Vector3 rayStartPosition = transform.position + Vector3.down * (capsuleCollider.height / 2);

        if (Physics.Raycast(rayStartPosition, Vector3.down, out surfaceHit, 1f, floorLayerMask))
        {
            switch (surfaceHit.collider.tag)
            {
                case "Blood":
                    currentFloorValue = 1;
                    break;
                case "Glass":
                    currentFloorValue = 3;
                    break;
                default:
                    currentFloorValue = 0;
                    break;
            }
        }
    }

    // --- Sistema de Sonido Ambiental (modificado) ---
    void HandleFootsteps()
    {
        // Usa un umbral menor para detectar movimiento cuando estamos agachados
        float currentThreshold = agachado ? crouchVelocityThreshold : 0.1f;
        
        // ARREGLO CRÍTICO: Verifica solo la velocidad horizontal (x, z) cuando estamos agachados
        // Esto evita que la velocidad vertical afecte a la detección de movimiento
        float horizontalVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
        
        // Usa la velocidad horizontal para verificaciones de movimiento cuando estamos agachados
        float velocityToCheck = agachado ? horizontalVelocity : rb.linearVelocity.magnitude;
        
        // Reproducimos pasos si nos estamos moviendo Y estamos en el suelo O estamos agachados y moviéndonos
        // Esto permite que los sonidos de pasos se reproduzcan incluso cuando la detección de suelo falla al agacharse
        bool shouldPlayFootsteps = (velocityToCheck > currentThreshold) && 
                                  (IsGrounded() || agachado);
        
        if (shouldPlayFootsteps)
        {
            if (!footstepsInstance.isValid())
            {
                footstepsInstance = RuntimeManager.CreateInstance(fmodEvents.Footsteps);
                RuntimeManager.AttachInstanceToGameObject(footstepsInstance, transform, rb);
                footstepsInstance.start();
            }
            UpdateFMODParameters();
        }
        else
        {
            if (footstepsInstance.isValid())
            {
                footstepsInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                footstepsInstance.release();
            }
        }
    }

    void UpdateFMODParameters()
    {
        // Siempre establece el valor correcto cuando estamos agachados
        // Esto asegura que FMOD use el sonido correcto basado en el estado de movimiento
        walkCrouchRunValue = agachado ? 1f : (Input.GetKey(KeyCode.LeftShift) ? 2f : 0f);
        
        footstepsInstance.setParameterByName("WalkCrouchRun", walkCrouchRunValue);
        footstepsInstance.setParameterByID(floorParamID, currentFloorValue);
    }

    // --- Sistema de dB Adicionales (modificado) ---
    void UpdateDBBoost()
    {
        float movementBoost = 0f;

        // Usa velocidad horizontal para verificar el movimiento cuando estamos agachados
        float horizontalVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
        float velocityToCheck = agachado ? horizontalVelocity : rb.linearVelocity.magnitude;
        float currentThreshold = agachado ? crouchVelocityThreshold : 0.1f;
        
        // Solo aplica el aumento de dB si realmente nos estamos moviendo
        if (velocityToCheck > currentThreshold)
        {
            if (agachado)
            {
                movementBoost = crouchDBBoost; // Ahora usa crouchDBBoost correctamente
            }
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                movementBoost = runDBBoost;
            }
            else
            {
                movementBoost = walkDBBoost;
            }
        }

        float surfaceBoost = currentFloorValue switch
        {
            1 => bloodDBBoost, // Blood
            3 => glassDBBoost, // Glass
            _ => 0f
        };

        // Establece el valor total de DB boost para MicrophoneCapture
        MicrophoneCapture.externalDBBoost = movementBoost + surfaceBoost;
    }

    // --- Funciones de apoyo ---
    bool IsGrounded()
    {
        // ARREGLADO: Añade distancia extra para verificar el suelo cuando estamos agachados
        // Esto resuelve problemas de detección de suelo debido a la menor altura del personaje
        float extraDistance = agachado ? groundCheckOffset : 0f;
        float distanceToGround = capsuleCollider.bounds.extents.y + extraDistance;
        
        // Usa una distancia de rayo más corta cuando estamos agachados para evitar fallar la detección del suelo
        return Physics.Raycast(transform.position, Vector3.down, distanceToGround + 0.1f);
    }

    public void ChangeMouseSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.Save();
    }

    /* Función de visualización de depuración - desactivada en la versión final
    void OnDrawGizmos()
    {
        // Color del gizmo (verde si está en suelo, rojo si no)
        bool isGrounded = IsGrounded();
        Gizmos.color = isGrounded ? Color.green : Color.red;

        // Calcula la posición de origen del raycast
        Vector3 rayStartPosition = transform.position;

        // Dibuja el rayo hacia abajo
        float extraDistance = agachado ? groundCheckOffset : 0f;
        float distanceToGround = (capsuleCollider != null) ? 
            capsuleCollider.bounds.extents.y + extraDistance + 0.1f : 1f;
            
        Gizmos.DrawRay(rayStartPosition, Vector3.down * distanceToGround);

        // Dibuja una esfera pequeña en el origen del rayo
        Gizmos.DrawSphere(rayStartPosition, 0.05f);
        
        // Draw velocity direction
        if (rb != null)
        {
            // Draw horizontal velocity
            Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            if (horizontalVelocity.magnitude > 0.1f)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(transform.position, horizontalVelocity.normalized);
                Gizmos.DrawSphere(transform.position + horizontalVelocity.normalized, 0.05f);
            }
        }
    }
    */

    void OnDestroy()
    {
        if (footstepsInstance.isValid())
        {
            footstepsInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            footstepsInstance.release();
        }
    }
}