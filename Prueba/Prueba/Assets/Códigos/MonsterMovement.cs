using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 250f;
    //[SerializeField] private float avoidanceAngle = 45f;
    //[SerializeField] private float groundMargin = 2f;
    [SerializeField] private Transform player; // Referencia al jugador
    [SerializeField] private float detectionRadius = 10f; // Radio de detección
    [SerializeField] private float followSpeed = 5f; // Velocidad al seguir al jugador
    [SerializeField] private float deathRadius = 2f; // Radio de muerte (más pequeño)

    private Rigidbody rb;
    private bool isFollowing = false;
    private Vector3 currentDirection;
    private float changeDirectionTime = 2f; // Tiempo en el que cambia la dirección
    private float timer;

    void Start()
    {
        // Configuración del Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;  // Desactivamos la gravedad
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY; // Fijamos la posición y rotación en Y
        }

        // Inicializar la dirección aleatoria
        SetRandomDirection();
    }

    void Update()
    {
        // Verificar si el jugador está dentro del rango de detección
        if (Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            // Comienza a seguir al jugador
            isFollowing = true;
        }
        else
        {
            // Deja de seguir al jugador y comienza el movimiento aleatorio
            isFollowing = false;
        }

        if (isFollowing)
        {
            FollowPlayer();
        }
        else
        {
            // Comportamiento por defecto (movimiento aleatorio)
            RandomMovement();
        }

        // Verificar si el jugador está dentro del rango de muerte
        if (Vector3.Distance(transform.position, player.position) <= deathRadius)
        {
            // Al llegar al rango de muerte, mostrar el mensaje de depuración
            Debug.Log("¡Estás muerto!");
            // Aquí puedes agregar más acciones, como finalizar el juego, etc.
        }
    }

    void FollowPlayer()
    {
        // Calcular la dirección hacia el jugador (en el plano horizontal)
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Asegurarse de que la dirección solo afecta al plano horizontal (x, z)

        // Mover al monstruo hacia el jugador sin afectar la altura (eje Y)
        Vector3 newPosition = transform.position + direction * followSpeed * Time.deltaTime;
        newPosition.y = transform.position.y; // Mantener la misma altura

        // Actualizar la posición del monstruo
        rb.position = newPosition;

        // Rotación suave para alinear el monstruo con la nueva dirección
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void RandomMovement()
    {
        // Control del temporizador para cambiar la dirección aleatoria
        timer += Time.deltaTime;
        if (timer > changeDirectionTime)
        {
            SetRandomDirection();
            timer = 0f; // Resetear el temporizador
        }

        // Mover al monstruo en la dirección actual
        rb.velocity = currentDirection * moveSpeed;

        // Rotación suave para alinear el monstruo con la nueva dirección
        Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void SetRandomDirection()
    {
        // Generar una nueva dirección aleatoria en el plano horizontal
        currentDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Al colisionar con un objeto, cambiar la dirección aleatoria
        SetRandomDirection();
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Muestra el radio de detección

        // Mostrar el rango de muerte
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, deathRadius); // Muestra el radio de muerte
    }
}
