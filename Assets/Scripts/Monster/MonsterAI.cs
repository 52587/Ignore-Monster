using UnityEngine;
using System.Collections;

public class MonsterAI : MonoBehaviour
{
    public enum MonsterState
    {
        Patrol,
        Idle,
        Chase
    }

    [Header("State")]
    public MonsterState currentState = MonsterState.Patrol;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private Rigidbody2D rb;

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waypointReachDistance = 0.5f;
    [SerializeField] private float idleTimeAtWaypoint = 2f;
    private int currentPatrolIndex = 0;

    [Header("Chase Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float chaseActivationTime = 1.5f; // How long player must look before chase
    [SerializeField] private float chaseDeactivationTime = 3f; // How long to chase after player looks away
    private float lookTimer = 0f;
    private float notLookingTimer = 0f;
    private bool isBeingLookedAt = false;

    [Header("Detection")]
    [SerializeField] private float catchDistance = 1f;

    private Vector2 currentTarget;
    private bool isWaiting = false;

    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (patrolPoints.Length > 0)
        {
            currentTarget = patrolPoints[currentPatrolIndex].position;
        }
    }

    private void Update()
    {
        // Update look timer
        if (isBeingLookedAt)
        {
            lookTimer += Time.deltaTime;
            notLookingTimer = 0f;

            // Transition to chase if looked at long enough
            if (lookTimer >= chaseActivationTime && currentState != MonsterState.Chase)
            {
                TransitionToChase();
            }
        }
        else
        {
            lookTimer = Mathf.Max(0, lookTimer - Time.deltaTime * 0.5f); // Decay slower

            if (currentState == MonsterState.Chase)
            {
                notLookingTimer += Time.deltaTime;

                // Return to patrol if player looks away long enough
                if (notLookingTimer >= chaseDeactivationTime)
                {
                    TransitionToPatrol();
                }
            }
        }

        isBeingLookedAt = false; // Reset, will be set by PlayerVision if still looking

        // Check if caught player
        if (player != null && Vector2.Distance(transform.position, player.position) <= catchDistance)
        {
            OnCatchPlayer();
        }
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case MonsterState.Patrol:
                Patrol();
                break;
            case MonsterState.Idle:
                rb.linearVelocity = Vector2.zero;
                break;
            case MonsterState.Chase:
                ChasePlayer();
                break;
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0 || isWaiting) return;

        Vector2 direction = (currentTarget - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;

        // Rotate to face movement direction
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Check if reached waypoint
        if (Vector2.Distance(transform.position, currentTarget) <= waypointReachDistance)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    private IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        currentState = MonsterState.Idle;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(idleTimeAtWaypoint);

        // Move to next waypoint
        currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        currentTarget = patrolPoints[currentPatrolIndex].position;

        currentState = MonsterState.Patrol;
        isWaiting = false;
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;

        // Rotate to face player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void TransitionToChase()
    {
        currentState = MonsterState.Chase;
        StopAllCoroutines();
        isWaiting = false;
        Debug.Log("Monster is now chasing!");
    }

    private void TransitionToPatrol()
    {
        currentState = MonsterState.Patrol;
        lookTimer = 0f;
        notLookingTimer = 0f;
        
        // Find nearest patrol point
        if (patrolPoints.Length > 0)
        {
            float nearestDist = float.MaxValue;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                float dist = Vector2.Distance(transform.position, patrolPoints[i].position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    currentPatrolIndex = i;
                }
            }
            currentTarget = patrolPoints[currentPatrolIndex].position;
        }
        
        Debug.Log("Monster returned to patrol");
    }

    public void OnPlayerLookingAt()
    {
        isBeingLookedAt = true;
    }

    private void OnCatchPlayer()
    {
        // Check for god mode
        DebugHelper debugHelper = FindObjectOfType<DebugHelper>();
        if (debugHelper != null && debugHelper.IsGodModeActive())
        {
            Debug.Log("Player would be caught but God Mode is active!");
            return;
        }

        Debug.Log("Player caught! Game Over!");
        // This will be handled by GameManager
        GameManager.Instance?.OnPlayerCaught();
    }

    private void OnDrawGizmosSelected()
    {
        // Draw patrol path
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawWireSphere(patrolPoints[i].position, 0.3f);
                    
                    // Draw line to next point
                    int nextIndex = (i + 1) % patrolPoints.Length;
                    if (patrolPoints[nextIndex] != null)
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[nextIndex].position);
                    }
                }
            }
        }

        // Draw catch radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchDistance);
    }
}
