using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class ControlPersonaje : MonoBehaviour
{
    public float walkSpeed = 5f;          // Velocidad de caminar
    public float runSpeed = 7f;           // Velocidad de correr
    public float crouchSpeed = 1f;        // Velocidad de agacharse
    public float crouchHeight = 0.2f;     // Altura al agacharse
    public float mouseSensitivity = 1000f; // Sensibilidad del mouse
    public Transform cameraTransform;     // Referencia a la cámara
    public float raycastDistance = 5f;    // Distancia del raycast

    private float originalHeight;         // Altura original del personaje
    private Rigidbody rb;                 // Componente Rigidbody
    private CapsuleCollider capsuleCollider; // Componente Collider
    private float rotationX = 0f;         // Rotación en el eje X (para la cámara)

    // Variables para agacharse de forma suave
    private Vector3 escalaNormal;         // Escala normal del personaje
    private Vector3 escalaAgachado;       // Escala al agacharse
    public float tiempoAgachado = 0.1f;   // Tiempo de transición al agacharse
    private bool agachado = false;        // Estado de agachado

    // Referencias para el audio de FMOD
    private FMODEvents fmodEvents;
    private AudioManager audioManager;
    private EventInstance footstepsInstance; // Instancia del evento de pasos
    private float walkCrouchRunValue = 0f;  // Valor del parámetro WalkCrouchRun (0 = caminar, 1 = agacharse, 2 = correr)

    private PARAMETER_ID floorParamID;     // ID del parámetro "floor"
    private int floorValue;                // Valor numérico del parámetro "floor" según el material debajo del jugador
    private RaycastHit rh;                 // Raycast hit para obtener información del objeto impactado
    private float distance = 1f;           // Distancia para el raycast (ajustada a 1f para mejor detección)
    private LayerMask lm;                  // LayerMask para los objetos sobre los que se puede hacer raycast

    void Start()
    {
        // Inicialización de componentes
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        fmodEvents = FMODEvents.instance; // Accede al FMODEvents
        audioManager = AudioManager.instance; // Accede al AudioManager

        // Inicialización de la rotación de la cámara
        rotationX = 0f;
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Guardar la altura original del personaje
        originalHeight = transform.localScale.y;

        // Configurar escalas para agacharse
        escalaNormal = transform.localScale;
        escalaAgachado = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);

        // Obtener el parámetro "floor" desde FMOD
        EventDescription eventDesc = RuntimeManager.GetEventDescription(fmodEvents.Footsteps);
        PARAMETER_DESCRIPTION paramDesc;
        eventDesc.getParameterDescriptionByName("floor", out paramDesc);
        floorParamID = paramDesc.id;

        lm = LayerMask.GetMask("Floor"); // Ahora la layer es "Floor" para filtrar superficies del suelo
    }

    void Update()
    {
        // Movimiento del personaje
        Move();

        // Rotación de la cámara y el personaje
        Look();

        // Agacharse
        Crouch();

        // Aplicar gravedad adicional para evitar flotar
        rb.AddForce(Vector3.down * 10f, ForceMode.Acceleration);

        // Modificar el parámetro WalkCrouchRun basado en las teclas presionadas
        HandleWalkCrouchRun();

        // Si el personaje está en movimiento, reproducir el sonido de los pasos
        if (rb.velocity.magnitude > 0.1f) // Si hay movimiento
        {
            if (!footstepsInstance.isValid()) // Si no se está reproduciendo el sonido, iniciar el evento
            {
                PlayFootsteps();
            }
        }
        else // Si no hay movimiento
        {
            if (footstepsInstance.isValid()) // Si se está reproduciendo el sonido, detenerlo
            {
                StopFootsteps();
            }
        }
    }

    void HandleWalkCrouchRun()
    {
        // Cambiar el parámetro según la acción del jugador
        if (Input.GetKey(KeyCode.C)) // Si el jugador está agachado
        {
            walkCrouchRunValue = 1f; // Agacharse
        }
        else if (Input.GetKey(KeyCode.LeftShift)) // Si el jugador está corriendo
        {
            walkCrouchRunValue = 2f; // Correr
        }
        else // Si el jugador está caminando
        {
            walkCrouchRunValue = 0f; // Caminar
        }

        // Asegurarse de que la instancia del evento está activa
        if (footstepsInstance.isValid()) // Si la instancia del evento es válida
        {
            // Actualizamos el parámetro WalkCrouchRun en FMOD
            footstepsInstance.setParameterByName("WalkCrouchRun", walkCrouchRunValue);
            Debug.Log("Updated WalkCrouchRun parameter: " + walkCrouchRunValue); // Verificar en consola
        }

        // Cambiar el parámetro "floor" según la superficie detectada
        MaterialCheck();
        if (footstepsInstance.isValid()) // Si la instancia del evento es válida
        {
            footstepsInstance.setParameterByID(floorParamID, floorValue, false);
            Debug.Log("Updated floor parameter: " + floorValue); // Verificar en consola
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Verificamos si estamos en contacto con una superficie de la Layer "Floor"
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            switch (collision.gameObject.tag)
            {
                case "Tile":
                    floorValue = 0; // Asignar valor 0 para Tile
                    break;
                case "Blood":
                    floorValue = 1; // Asignar valor 1 para WaterTile
                    break;
                case "Glass":
                    floorValue = 3; // Asignar valor 3 para CristalTile
                    break;
                default:
                    floorValue = 0; // Por defecto a Tile (valor 0)
                    break;
            }

            // Asegurarse de que la instancia del evento está activa
            if (footstepsInstance.isValid())
            {
                // Actualizamos el parámetro "floor" en FMOD
                footstepsInstance.setParameterByID(floorParamID, floorValue, false);
                Debug.Log("Updated floor parameter: " + floorValue); // Verificar en consola
            }
        }
    }

    void MaterialCheck()
    {
        // Realizar un raycast desde el centro de la cápsula (ligeramente más abajo)
        Vector3 rayStartPosition = transform.position + Vector3.down * (capsuleCollider.height / 2); // Desde la base de la cápsula

        // Visualizamos el raycast en la escena (rojo para que sea fácil de ver)
        Debug.DrawRay(rayStartPosition, Vector3.down * distance, Color.red);

        if (Physics.Raycast(rayStartPosition, Vector3.down, out rh, distance, lm))
        {
            // Verifica los tags para asignar el valor correspondiente al parámetro "floor"
            switch (rh.collider.tag)
            {
                case "Tile":
                    floorValue = 0; // Asignar valor 0 para Tile
                    break;
                case "Blood":
                    floorValue = 1; // Asignar valor 1 para WaterTile
                    break;
                case "Glass":
                    floorValue = 3; // Asignar valor 3 para CristalTile
                    break;
                default:
                    floorValue = 0; // Por defecto a Tile (valor 0)
                    break;
            }
        }
    }

    void PlayFootsteps()
    {
        // Crea la instancia del evento Footsteps
        footstepsInstance = audioManager.CreateEventInstance(fmodEvents.Footsteps);
        RuntimeManager.AttachInstanceToGameObject(footstepsInstance, transform, GetComponent<Rigidbody>());

        // Inicia el evento
        footstepsInstance.start();
    }

    void StopFootsteps()
    {
        // Detiene el evento de los pasos
        footstepsInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        footstepsInstance.release(); // Libera la instancia del evento
    }

    void Move()
    {
        // Obtener la entrada del teclado
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calcular la dirección del movimiento
        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;

        // Determinar la velocidad (caminar o correr)
        float speed = walkSpeed; // Predeterminado es caminar

        if (Input.GetKey(KeyCode.LeftShift)) // Si el jugador está corriendo
        {
            speed = runSpeed; // Aumenta la velocidad si está corriendo
        }
        else if (Input.GetKey(KeyCode.C)) // Si el jugador está agachado
        {
            speed = crouchSpeed; // Reducimos la velocidad al agacharse
        }

        // Aplicar la velocidad al Rigidbody
        Vector3 velocity = moveDirection * speed;
        velocity.y = rb.velocity.y; // Mantener la velocidad en el eje Y (gravedad)
        rb.velocity = velocity;
    }

    void Look()
    {
        // Obtener la entrada del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotar el personaje en el eje Y
        transform.Rotate(Vector3.up * mouseX);

        // Rotar la cámara en el eje X (arriba y abajo)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -80f, 80f); // Limitar la rotación
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);
    }

    void Crouch()
    {
        // Verificar si se presiona la tecla para agacharse
        if (Input.GetKey(KeyCode.C))
        {
            agachado = true;
        }
        else
        {
            agachado = false;
        }

        // Interpolación suave para agacharse
        transform.localScale = Vector3.Lerp(transform.localScale, agachado ? escalaAgachado : escalaNormal, Time.deltaTime / tiempoAgachado);
    }

    void Raycast()
    {
        // Crear un rayo desde la cámara
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        // Lanzar el rayo y verificar si golpea algo
        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            // Dibujar el rayo en la escena para visualización
            Debug.DrawRay(cameraTransform.position, cameraTransform.forward * raycastDistance, Color.red);
        }
    }
}
