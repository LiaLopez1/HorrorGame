using UnityEngine;
using FMOD.Studio;
using FMODUnity; 

public class MonsterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float chaseSpeed = 7f;
    [SerializeField] private float acceleration = 0.5f;
    [SerializeField] private float rotationSpeed = 250f;
    [SerializeField] private float deathRadius = 2f;

    [Header("Detection Settings")]
    [SerializeField] private float normalDetectionRadius = 10f;
    [SerializeField] private float boostedDetectionRadius = 20f;
    [SerializeField] private float noiseThreshold = -30f;
    [SerializeField] private float searchDuration = 10f;

    [Header("References")]
    [SerializeField] private Transform player;

    private Rigidbody rb;
    private Vector3 currentDirection;
    private float changeDirectionTime = 2f;
    private float timer;
    private float currentSpeed;
    private float noiseDetectionTimer = 0f;
    private bool isNoiseAlertActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            // Bloquea TODOS los movimientos y rotaciones no deseados
            rb.constraints = RigidbodyConstraints.FreezePositionY |
                            RigidbodyConstraints.FreezeRotationX |
                            RigidbodyConstraints.FreezeRotationZ |
                            RigidbodyConstraints.FreezeRotationY;
        }

        currentSpeed = normalSpeed;
        SetRandomDirection();
    }

    void Update()
    {
        HandleNoiseDetection();
        bool shouldFollow = ShouldFollowPlayer();

        if (shouldFollow)
        {
            FollowPlayer();
        }
        else
        {
            RandomMovement();
        }

        if (Vector3.Distance(transform.position, player.position) <= deathRadius)
        {
            Debug.Log("�Est�s muerto!");
        }
    }

    void HandleNoiseDetection()
    {
        if (MicrophoneCapture.currentDB >= noiseThreshold && !isNoiseAlertActive)
        {
            isNoiseAlertActive = true;
            noiseDetectionTimer = 0f;
        }

        if (isNoiseAlertActive)
        {
            noiseDetectionTimer += Time.deltaTime;
            if (noiseDetectionTimer >= searchDuration)
            {
                isNoiseAlertActive = false;
            }
        }
    }

    bool ShouldFollowPlayer()
    {
        float currentRadius = isNoiseAlertActive ? boostedDetectionRadius : normalDetectionRadius;
        return Vector3.Distance(transform.position, player.position) <= currentRadius;
    }

    void FollowPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        currentSpeed = Mathf.Lerp(currentSpeed, chaseSpeed, acceleration * Time.deltaTime);
        Vector3 newPosition = transform.position + direction * currentSpeed * Time.deltaTime;

        // Fuerza posici�n Y = 0 (evita elevaci�n)
        newPosition.y = 0;
        rb.position = newPosition;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void RandomMovement()
    {
        timer += Time.deltaTime;
        if (timer > changeDirectionTime)
        {
            SetRandomDirection();
            timer = 0f;
        }

        currentSpeed = Mathf.Lerp(currentSpeed, normalSpeed, acceleration * Time.deltaTime);

        // Fuerza movimiento en plano XZ (Y=0)
        Vector3 movement = currentDirection * currentSpeed * Time.deltaTime;
        movement.y = 0;
        rb.MovePosition(transform.position + movement);

        Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void SetRandomDirection()
    {
        currentDirection = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Fuerza posici�n Y=0 tras colisi�n
        Vector3 fixedPosition = transform.position;
        fixedPosition.y = 0;
        transform.position = fixedPosition;

        // Rebote aleatorio
        SetRandomDirection();
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        float currentRadius = isNoiseAlertActive ? boostedDetectionRadius : normalDetectionRadius;
        Gizmos.color = isNoiseAlertActive ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, currentRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, deathRadius);
    }
}