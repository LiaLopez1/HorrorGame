using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPersonaje : MonoBehaviour
{
    public float walkSpeed = 10f;          // Velocidad de caminar
    public float runSpeed = 20f;           // Velocidad de correr
    public float crouchHeight = 0.5f;      // Altura al agacharse
    public float mouseSensitivity = 600f;  // Sensibilidad del mouse
    public Transform cameraTransform;      // Referencia a la cámara
    public float raycastDistance = 5f;     // Distancia del raycast

    private float originalHeight;          // Altura original del personaje
    private Rigidbody rb;                  // Componente Rigidbody
    private CapsuleCollider capsuleCollider; // Componente Collider

    private float rotationX = 0f;          // Rotación en el eje X (para la cámara)

    // Variables para agacharse de forma suave
    private Vector3 escalaNormal;          // Escala normal del personaje
    private Vector3 escalaAgachado;        // Escala al agacharse
    public float tiempoAgachado = 0.1f;    // Tiempo de transición al agacharse
    private bool agachado = false;         // Estado de agachado

    void Start()
    {
        // Inicialización de componentes
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Bloquear el cursor en el centro de la pantalla
        Cursor.lockState = CursorLockMode.Locked;

        // Guardar la altura original del personaje
        originalHeight = transform.localScale.y;

        // Inicializar la rotación de la cámara
        rotationX = 0f;
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Configurar escalas para agacharse
        escalaNormal = transform.localScale;
        escalaAgachado = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
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

        // Raycast para apuntar
        Raycast();
    }

    void Move()
    {
        // Obtener la entrada del teclado
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calcular la dirección del movimiento
        Vector3 moveDirection = transform.right * moveHorizontal + transform.forward * moveVertical;

        // Determinar la velocidad (caminar o correr)
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

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

            // Opcional: Interactuar con el objeto apuntado
            //if (hit.collider != null)
            //{
            //    Debug.Log("Apuntando a: " + hit.collider.gameObject.name);
            //}
        }
    }
}