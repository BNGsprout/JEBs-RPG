using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [Header("References")]
    public Transform player; // Drag your Player GameObject here
    public Transform[] patrolPoints;

    [Header("Settings")]
    public float chaseRange = 5f;      // Distance to start chasing
    public float stopChaseRange = 7f;  // Distance to stop chasing (prevents jitter)
    public float waitTime = 2f;
    public float chaseSpeed = 6f;      // Faster speed when chasing
    public float patrolSpeed = 3f;     // Slower speed when patrolling

    private NavMeshAgent agent;
    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;
    
    // State tracking
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; 
        agent.updateUpAxis = false;
        agent.speed = patrolSpeed; // Start with patrol speed

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // Ensure we are on the mesh before starting
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }

        if (patrolPoints.Length > 0)
        {
            SetDestination(patrolPoints[0].position);
        }
    }

    void Update()
    {
        if (patrolPoints.Length == 0 || !agent.isOnNavMesh || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // STATE MACHINE LOGIC
        if (distanceToPlayer <= chaseRange)
        {
            StartChase();
        }
        else if (distanceToPlayer >= stopChaseRange)
        {
            StopChase();
        }

        // EXECUTE CURRENT STATE
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void StartChase()
    {
        if (!isChasing)
        {
            isChasing = true;
            agent.speed = chaseSpeed;
            // Optional: Play alert sound or animation here
        }
    }

    void StopChase()
    {
        if (isChasing)
        {
            isChasing = false;
            agent.speed = patrolSpeed;
            
            // CRITICAL FIX: Force reset of waiting state and immediately find next point
            isWaiting = false; 
            waitTimer = 0f;
            
            // If we stopped chasing, we might be far from the original patrol point.
            // Recalculate path to the CURRENT target immediately.
            if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
            {
                 AdvancePatrol(); // Move to next point immediately if we are "stuck"
            }
        }
    }

    void ChasePlayer()
    {
        // Continuously update destination to player's current position
        agent.SetDestination(player.position);
    }

    void Patrol()
    {
        // CRITICAL: Do not return if pathPending is true; just wait for it to finish.
        // The original code returned here, preventing the rest of the logic from running 
        // if the path was still calculating, which can happen after a chase.
        
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0)
            {
                isWaiting = false;
                AdvancePatrol();
            }
        }
        else
        {
            // Only check arrival if we aren't already calculating a new path
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                isWaiting = true;
                waitTimer = waitTime;
            }
        }
    }

    void AdvancePatrol()
    {
        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        SetDestination(patrolPoints[currentPointIndex].position);
    }

    void SetDestination(Vector3 target)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 1.0f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    // Visualize ranges in Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopChaseRange);
    }
}   