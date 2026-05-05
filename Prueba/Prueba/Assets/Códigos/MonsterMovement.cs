using UnityEngine;
using UnityEngine.AI;
using FMOD.Studio;
using FMODUnity;

public class MonsterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 3f;
    [SerializeField] private float chaseSpeed = 7f;
    [SerializeField] private float deathRadius = 2f;

    [Header("Detection Settings")]
    [SerializeField] private float normalDetectionRadius = 10f;
    [SerializeField] private float boostedDetectionRadius = 20f;
    [SerializeField] private float noiseThreshold = -30f;
    [SerializeField] private float searchDuration = 10f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] patrolPoints;

    [Header("Game State")]
    public bool isPlayerDead = false;

    [Header("FMOD Events")]
    [SerializeField] private EventReference deathScreamEvent;

    private NavMeshAgent agent;
    private Animator animator;
    private int currentPatrolIndex = 0;

    private float noiseDetectionTimer = 0f;
    private bool isNoiseAlertActive = false;

    private bool wasChasing = false;


     // 🔹 Propiedades públicas para MusicController
    public bool IsPlayerInNormalRange => Vector3.Distance(transform.position, player.position) <= normalDetectionRadius;

    public bool IsExtendedZoneTriggered =>
        Vector3.Distance(transform.position, player.position) <= boostedDetectionRadius && isNoiseAlertActive;

    public bool IsNoiseAlertActive => isNoiseAlertActive;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.speed = normalSpeed;
        GoToNextPatrolPoint();
    }

    void Update()
    {
        HandleNoiseDetection();
        bool shouldFollow = ShouldFollowPlayer();

        if (shouldFollow)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
            wasChasing = true;

        }
        else
        {
            agent.speed = normalSpeed;

            if (wasChasing)
            {
                GoToNextPatrolPoint();
                wasChasing = false;
            }

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                GoToNextPatrolPoint();
            }


        }

        //lógica de muerte
        if (!isPlayerDead && Vector3.Distance(transform.position, player.position) <= deathRadius)
        {
            isPlayerDead = true;
            RuntimeManager.PlayOneShot(deathScreamEvent, player.position);

            DeathHandler deathHandler = Object.FindFirstObjectByType<DeathHandler>();

            if (deathHandler != null)
            {
                deathHandler.TriggerDeath(player.position);
            }

            Debug.Log("¡Estás muerto!");
        }

        ActualizarAnimaciones();
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
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= normalDetectionRadius)
            return true;

        if (distance <= boostedDetectionRadius && isNoiseAlertActive)
            return true;

        return false;
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.destination = patrolPoints[currentPatrolIndex].position;
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
    }

    void ActualizarAnimaciones()
    {
        bool persiguiendo = ShouldFollowPlayer();
        animator.SetBool("Persiguiendo", persiguiendo);
    }

    void OnDrawGizmosSelected()
    {
        float currentRadius = isNoiseAlertActive ? boostedDetectionRadius : normalDetectionRadius;
        Gizmos.color = isNoiseAlertActive ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, currentRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, deathRadius);
    }
}