using UnityEngine;
using System.Collections;

public class MonsterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 250f;
    [SerializeField] private float avoidanceAngle = 45f;
    [SerializeField] private Transform groundContainer;
    [SerializeField] private float groundMargin = 2f;

    private Rigidbody rb;
    private Bounds combinedGroundBounds;
    private Collider[] groundColliders;
    private Vector3 currentForward;
    private bool isTurning = false;

    void Start()
    {
        // Configuración del Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Obtener colliders del piso
        if (groundContainer != null)
        {
            groundColliders = groundContainer.GetComponentsInChildren<Collider>();
            CalculateCombinedBounds();
        }

        // Dirección inicial (hacia donde mira el modelo)
        currentForward = transform.forward;
    }

    void CalculateCombinedBounds()
    {
        if (groundColliders == null || groundColliders.Length == 0) return;

        combinedGroundBounds = groundColliders[0].bounds;
        foreach (Collider col in groundColliders)
        {
            combinedGroundBounds.Encapsulate(col.bounds);
        }
    }

    void FixedUpdate()
    {
        if (!isTurning)
        {
            MoveStraight();
            CheckGroundBounds();
        }
    }

    void MoveStraight()
    {
        // Movimiento constante en la dirección actual
        rb.velocity = -currentForward * moveSpeed;

        // Rotación gradual para alinear el modelo con la dirección
        if (Vector3.Angle(transform.forward, currentForward) > 2f)
        {
            Quaternion targetRot = Quaternion.LookRotation(currentForward);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    void CheckGroundBounds()
    {
        Vector3 pos = transform.position;
        bool outOfBounds =
            pos.x < (combinedGroundBounds.min.x + groundMargin) ||
            pos.x > (combinedGroundBounds.max.x - groundMargin) ||
            pos.z < (combinedGroundBounds.min.z + groundMargin) ||
            pos.z > (combinedGroundBounds.max.z - groundMargin);

        if (outOfBounds)
        {
            Vector3 centerDir = (combinedGroundBounds.center - pos).normalized;
            StartCoroutine(ChangeDirection(centerDir));
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!IsGroundCollider(collision.collider) && !isTurning)
        {
            Vector3 reflectDir = Vector3.Reflect(currentForward, collision.contacts[0].normal).normalized;
            StartCoroutine(ChangeDirection(reflectDir));
        }
    }

    bool IsGroundCollider(Collider col)
    {
        foreach (Collider groundCol in groundColliders)
        {
            if (col == groundCol) return true;
        }
        return false;
    }

    IEnumerator ChangeDirection(Vector3 newDirection)
    {
        isTurning = true;

        // Añadir aleatoriedad controlada
        float angleVariation = Random.Range(-avoidanceAngle, avoidanceAngle);
        newDirection = Quaternion.Euler(0, angleVariation, 0) * newDirection;

        // Suavizar transición
        float angle;
        do
        {
            currentForward = Vector3.RotateTowards(currentForward, newDirection, rotationSpeed * Mathf.Deg2Rad * Time.deltaTime, 0f);
            angle = Vector3.Angle(currentForward, newDirection);
            yield return null;
        } while (angle > 5f);

        isTurning = false;
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, currentForward * 3);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(combinedGroundBounds.center, combinedGroundBounds.size);
    }
}