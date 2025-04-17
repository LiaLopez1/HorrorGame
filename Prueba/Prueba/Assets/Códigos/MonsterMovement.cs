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

    [HideInInspector] public bool playerInExtendedArea = false;
    [HideInInspector] public bool playerInNormalArea = false;

    [Header("Game State")]
    public bool isPlayerDead = false;

    [Header("FMOD Events")]
    [SerializeField] private EventReference deathScreamEvent;


    private Rigidbody rb;
    private Vector3 currentDirection;
    private float changeDirectionTime = 2f;
    private float timer;
    private float currentSpeed;
    private float noiseDetectionTimer = 0f;
    private bool isNoiseAlertActive = false;
    private MusicController musicController;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY |
                            RigidbodyConstraints.FreezeRotationX |
                            RigidbodyConstraints.FreezeRotationZ |
                            RigidbodyConstraints.FreezeRotationY;
        }

        // FORZAR ESTADO INICIAL (radio pequeño)
        isNoiseAlertActive = false;
        noiseDetectionTimer = searchDuration; // Timer completo = desactivado
        playerInExtendedArea = false; // Resetear área extendida
        playerInNormalArea = false;   // Resetear área normal

        currentSpeed = normalSpeed;
        SetRandomDirection();
        musicController = FindObjectOfType<MusicController>();
    }


    void Update()
    {
        HandleNoiseDetection();
        bool shouldFollow = ShouldFollowPlayer();

        if (shouldFollow)
        {
            FollowPlayer();

            if (isNoiseAlertActive)
            {
                playerInExtendedArea = true;
                playerInNormalArea = false;
            }
            else
            {
                playerInExtendedArea = false;
                playerInNormalArea = true;
            }
        }
        else
        {
            RandomMovement();
            playerInExtendedArea = false;
            playerInNormalArea = false;
        }

        if (!isPlayerDead && Vector3.Distance(transform.position, player.position) <= deathRadius)
        {
            isPlayerDead = true;

            // Reproduce el grito de muerte con FMOD
            RuntimeManager.PlayOneShot(deathScreamEvent, player.position);

            DeathHandler deathHandler = FindObjectOfType<DeathHandler>();
            if (deathHandler != null)
            {
                deathHandler.TriggerDeath(player.position);
            }

            Debug.Log("¡Estás muerto!");
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
        // SIEMPRE usar radio normal al inicio (ignorar isNoiseAlertActive hasta que haya ruido real)
        float currentRadius = normalDetectionRadius;

        // Solo usar radio extendido si isNoiseAlertActive ES VERDADERO Y hay ruido actual
        if (isNoiseAlertActive && MicrophoneCapture.currentDB >= noiseThreshold)
        {
            currentRadius = boostedDetectionRadius;
        }

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