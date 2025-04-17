using UnityEngine;
using FMODUnity;
using FMOD.Studio;

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
    [SerializeField] private float crouchDBBoost = 0f;
    [SerializeField] private float bloodDBBoost = 4f;
    [SerializeField] private float glassDBBoost = 5f;

    [Header("UI")]
    [SerializeField] private GameObject deathCanvas;

    [Header("Crouch Settings")]
    public float tiempoAgachado = 0.1f;

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

    // FMOD
    private FMODEvents fmodEvents;
    private EventInstance footstepsInstance;
    private PARAMETER_ID floorParamID;

    void Start()
    {
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
        UpdateDBBoost(); // Nuevo: Actualiza los dB adicionales
    }

    // --- Sistema de Movimiento (sin cambios) ---
    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 velocity = moveDirection * speed;
        velocity.y = rb.velocity.y;
        rb.velocity = velocity;
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

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
            // Aquí puedes añadir lógica para interactuar con objetos

            if (hit.collider != null)
            {
                //Debug.Log("Apuntando a: " + hit.collider.gameObject.name);
            }
        }

        // Raycast adicional para detectar superficies (opcional)
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
        if (rb.velocity.magnitude > 0.1f && IsGrounded())
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
        // Lógica simplificada para caminar(0), agacharse(1) o correr(2)
        float walkCrouchRunValue = 0f; // Por defecto caminando

        if (Input.GetKey(KeyCode.C)) // Prioridad al agacharse si ambas teclas están presionadas
        {
            walkCrouchRunValue = 1f; // Agachado
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            walkCrouchRunValue = 2f; // Corriendo
        }

        footstepsInstance.setParameterByName("WalkCrouchRun", walkCrouchRunValue);
        footstepsInstance.setParameterByID(floorParamID, currentFloorValue);
    }

    // --- Nuevo: Sistema de dB Adicionales ---
    void UpdateDBBoost()
    {
        float movementBoost = agachado ? crouchDBBoost :
                            Input.GetKey(KeyCode.LeftShift) ? runDBBoost : walkDBBoost;

        float surfaceBoost = currentFloorValue switch
        {
            1 => bloodDBBoost, // Blood
            3 => glassDBBoost, // Glass
            _ => 0f
        };

        MicrophoneCapture.externalDBBoost = (rb.velocity.magnitude > 0.1f) ?
                                          movementBoost + surfaceBoost : 0f;
    }

    // --- Funciones de apoyo ---
    bool IsGrounded()
    {
        float distanceToGround = capsuleCollider.bounds.extents.y;
        return Physics.Raycast(transform.position, Vector3.down, distanceToGround + 0.1f);
    }

    /*void OnDrawGizmos()
    {
        // Color del gizmo (verde si está en suelo, rojo si no)
        bool isGrounded = IsGrounded();
        Gizmos.color = isGrounded ? Color.green : Color.red;

        // Calcula la posición de origen del raycast (igual que en el código)
        float currentHeight = agachado ? crouchHeight : capsuleCollider.height;
        Vector3 rayStartPosition = transform.position + Vector3.down * (currentHeight / 2);

        // Dibuja el rayo hacia abajo
        float rayDistance = agachado ? 1.5f : 1f;
        Gizmos.DrawRay(rayStartPosition, Vector3.down * rayDistance);

        // Opcional: Dibuja una esfera pequeña en el origen del rayo
        Gizmos.DrawSphere(rayStartPosition, 0.05f);
    }*/

    void OnDestroy()
    {
        if (footstepsInstance.isValid())
        {
            footstepsInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            footstepsInstance.release();
        }
    }
}