using UnityEngine;
using UnityEngine.AI;

public class RoamBehaviour : MonoBehaviour
{
    [SerializeField] private float roamRadius = 15f;
    [SerializeField] private float pauseDuration = 2f;
    [SerializeField] private float agentSpeed = 3f;  // ← new

    private NavMeshAgent agent;
    private float pauseTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = agentSpeed;  // ← new  
    }

    void OnEnable()
    {
        agent.SetDestination(GetRandomPoint());
    }

    void Update()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            if (pauseTimer <= 0f)
            {
                pauseTimer = pauseDuration;
            }
            else
            {
                pauseTimer -= Time.deltaTime;
                if (pauseTimer <= 0f)
                {
                    agent.SetDestination(GetRandomPoint());
                }
            }
        }
    }

    private Vector3 GetRandomPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomOffset = Random.insideUnitSphere * roamRadius;
            randomOffset.y = 0f;
            Vector3 randomPos = transform.position + randomOffset;

            NavMeshHit hit;
            if (!NavMesh.SamplePosition(randomPos, out hit, roamRadius, NavMesh.AllAreas))
                continue;

            var path = new NavMeshPath();
            if (NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, path))
            {
                if (path.status == NavMeshPathStatus.PathComplete)
                    return hit.position;
            }
        }

        return transform.position;
    }
}   