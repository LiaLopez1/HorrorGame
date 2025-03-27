using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class PruebaControlador : MonoBehaviour
{
    public float walkSpeed = 10f;
    public float runSpeed = 20f;
    public float crouchHeight = 0.2f;
    public float mouseSensitivity = 600f;
    public Transform cameraTransform;
    public float raycastDistance = 5f;  // Distancia del raycast

    private float originalHeight;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;
    private float rotationX = 0f;

    // Variables para agacharse de forma suave
    private Vector3 escalaNormal;
    private Vector3 escalaAgachado;
    public float tiempoAgachado = 0.1f;
    private bool agachado = false;

    // Variables para FMOD
    private FMODEvents fmodEvents;
    private AudioManager audioManager;
    private EventInstance footstepsInstance;
    private float walkCrouchRunValue = 0f; // 0 = walk, 1 = crouch, 2 = run
    private PARAMETER_ID floorParamID;
    private int floorValue = 0; // Valor del material (0 = tile, 1 = blood, 3 = glass)
    private LayerMask floorLayerMask;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
        originalHeight = transform.localScale.y;

        rotationX = 0f;
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        escalaNormal = transform.localScale;
        escalaAgachado = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);

        // Inicialización de FMOD
        fmodEvents = FMODEvents.instance;
        audioManager = AudioManager.instance;

        // Configurar el parámetro de floor en FMOD
        EventDescription eventDesc = RuntimeManager.GetEventDescription(fmodEvents.Footsteps);
        PARAMETER_DESCRIPTION paramDesc;
        eventDesc.getParameterDescriptionByName("floor", out paramDesc);
        floorParamID = paramDesc.id;

        floorLayerMask = LayerMask.GetMask("Floor");
    }

    void Update()
    {
        // Movimiento
        Move();

        // Rotación de la cámara y el jugador
        Look();

        // Agacharse
        Crouch();

        rb.AddForce(Vector3.down * 10f, ForceMode.Acceleration);

        // Raycast para detectar el material debajo
        Raycast();

        // Manejo de sonidos de pasos
        HandleFootsteps();
    }

    void HandleFootsteps()
    {
        // Actualizar parámetros de FMOD según el movimiento
        UpdateFMODParameters();

        // Controlar reproducción de sonido de pasos
        if (rb.velocity.magnitude > 0.1f && IsGrounded())
        {
            if (!footstepsInstance.isValid())
            {
                PlayFootsteps();
            }
        }
        else
        {
            if (footstepsInstance.isValid())
            {
                StopFootsteps();
            }
        }
    }

    void UpdateFMODParameters()
    {
        // Determinar el estado de movimiento (walk/crouch/run)
        if (agachado)
        {
            walkCrouchRunValue = 1f; // Crouch
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            walkCrouchRunValue = 2f; // Run
        }
        else
        {
            walkCrouchRunValue = 0f; // Walk
        }

        // Aplicar parámetros a FMOD
        if (footstepsInstance.isValid())
        {
            footstepsInstance.setParameterByName("WalkCrouchRun", walkCrouchRunValue);
            footstepsInstance.setParameterByID(floorParamID, floorValue);
        }
    }

    void PlayFootsteps()
    {
        footstepsInstance = audioManager.CreateEventInstance(fmodEvents.Footsteps);
        RuntimeManager.AttachInstanceToGameObject(footstepsInstance, transform, rb);
        footstepsInstance.start();
    }

    void StopFootsteps()
    {
        footstepsInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        footstepsInstance.release();
    }

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
        
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            transform.Rotate(Vector3.up * mouseX);

            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -80f, 80f);
            cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
        }
    }

    bool IsGrounded()
    {
        float distanceToGround = capsuleCollider.bounds.extents.y;
        return Physics.Raycast(transform.position, Vector3.down, distanceToGround + 0.1f);
    }

    void Raycast()
    {
        // Raycast para detectar objetos al frente
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            Debug.DrawRay(cameraTransform.position, cameraTransform.forward * raycastDistance, Color.red);

            if (hit.collider != null)
            {
                //Debug.Log("Apuntando a: " + hit.collider.gameObject.name);
            }
        }

        // Raycast para detectar el material debajo (para FMOD)
        RaycastHit surfaceHit;
        Vector3 rayStartPosition = transform.position + Vector3.down * (capsuleCollider.height / 2);

        if (Physics.Raycast(rayStartPosition, Vector3.down, out surfaceHit, 1f, floorLayerMask))
        {
            switch (surfaceHit.collider.tag)
            {
                case "Tile":
                    floorValue = 0;
                    Debug.Log("Está sobre un Tile");
                    break;
                case "Blood":
                    floorValue = 1;
                    Debug.Log("Está sobre agua (Blood)");
                    break;
                case "Glass":
                    floorValue = 3;
                    Debug.Log("Está sobre vidrio (Glass)");
                    break;
                default:
                    floorValue = 0;
                    Debug.Log("Está sobre una superficie desconocida");
                    break;
            }
        }
    }

    void Crouch()
    {
        if (Input.GetKey(KeyCode.C))
        {
            agachado = true;
        }
        else
        {
            agachado = false;
        }

        transform.localScale = Vector3.Lerp(transform.localScale, agachado ? escalaAgachado : escalaNormal, Time.deltaTime / tiempoAgachado);
    }

    void OnDestroy()
    {
        // Limpieza de FMOD al destruir el objeto
        if (footstepsInstance.isValid())
        {
            footstepsInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            footstepsInstance.release();
        }
    }
}