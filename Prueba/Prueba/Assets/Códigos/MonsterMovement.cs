using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 250f;
    //[SerializeField] private float avoidanceAngle = 45f;
    //[SerializeField] private float groundMargin = 2f;
    [SerializeField] private Transform player; // Referencia al jugador
    [SerializeField] private float detectionRadius = 10f; // Radio de detecci?n
    [SerializeField] private float followSpeed = 5f; // Velocidad al seguir al jugador
    [SerializeField] private float deathRadius = 2f; // Radio de muerte (m?s peque?o)

    private Rigidbody rb;
    private bool isFollowing = false;
    private Vector3 currentDirection;
    private float changeDirectionTime = 2f; // Tiempo en el que cambia la direcci?n
    private float timer;

    void Start()
    {
        // Configuraci?n del Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;  // Desactivamos la gravedad
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY; // Fijamos la posici?n y rotaci?n en Y
        }

        // Inicializar la direcci?n aleatoria
        SetRandomDirection();
    }

    void Update()
    {
        // Verificar si el jugador est? dentro del rango de detecci?n
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

        // Verificar si el jugador est? dentro del rango de muerte
        if (Vector3.Distance(transform.position, player.position) <= deathRadius)
        {
            // Al llegar al rango de muerte, mostrar el mensaje de depuraci?n
            Debug.Log("?Est?s muerto!");
            // Aqu? puedes agregar m?s acciones, como finalizar el juego, etc.
        }
    }

    void FollowPlayer()
    {
        // Calcular la direcci?n hacia el jugador (en el plano horizontal)
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Asegurarse de que la direcci?n solo afecta al plano horizontal (x, z)

        // Mover al monstruo hacia el jugador sin afectar la altura (eje Y)
        Vector3 newPosition = transform.position + direction * followSpeed * Time.deltaTime;
        newPosition.y = transform.position.y; // Mantener la misma altura

        // Actualizar la posici?n del monstruo
        rb.position = newPosition;

        // Rotaci?n suave para alinear el monstruo con la nueva direcci?n
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void RandomMovement()
    {
        // Control del temporizador para cambiar la direcci?n aleatoria
        timer += Time.deltaTime;
        if (timer > changeDirectionTime)
        {
            SetRandomDirection();
            timer = 0f; // Resetear el temporizador
        }

        // Mover al monstruo en la direcci?n actual
        rb.velocity = currentDirection * moveSpeed;

        // Rotaci?n suave para alinear el monstruo con la nueva direcci?n
        Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void SetRandomDirection()
    {
        // Generar una nueva direcci?n aleatoria en el plano horizontal
        currentDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Al colisionar con un objeto, cambiar la direcci?n aleatoria
        SetRandomDirection();
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius); // Muestra el radio de detecci?n

        // Mostrar el rango de muerte
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, deathRadius); // Muestra el radio de muerte
    }
}
