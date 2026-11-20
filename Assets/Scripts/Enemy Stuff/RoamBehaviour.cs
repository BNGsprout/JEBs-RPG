using UnityEngine;

public class RoamBehaviour : MonoBehaviour
{
    public float speed = 3f;
    public float roamRadius = 5f;
    public float waitTime = 2f;
    public float lookAhead = 2f;        // how far ahead to cast
    public float sideAngle = 45f;       // spread of side rays
    public int rayCount = 5;            // more rays = smoother avoidance
    public LayerMask obstacleMask;      // set this in the inspector

    Rigidbody2D rb;
    Vector2 homePos;
    Vector2 target;
    float waitTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        homePos = transform.position;
        PickNewTarget();
    }

    void FixedUpdate()
    {
        if (waitTimer > 0)
        {
            waitTimer -= Time.fixedDeltaTime;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = ((Vector2)target - rb.position).normalized;

        if (Vector2.Distance(rb.position, target) < 0.2f)
        {
            waitTimer = waitTime;
            PickNewTarget();
            return;
        }

        // Obstacle avoidance
        dir = AvoidObstacles(dir);

        rb.linearVelocity = dir * speed;
    }

    Vector2 AvoidObstacles(Vector2 desiredDir)
    {
        Vector2 avoidForce = Vector2.zero;

        for (int i = 0; i < rayCount; i++)
        {
            float t = (float)i / (rayCount - 1) - 0.5f; // -0.5 to 0.5
            Vector2 rayDir = Quaternion.Euler(0, 0, t * sideAngle * 2f) * desiredDir;

            RaycastHit2D hit = Physics2D.Raycast(rb.position, rayDir, lookAhead, obstacleMask);

            if (hit)
            {
                // Closer hit = stronger avoidance
                float weight = 1f - (hit.distance / lookAhead);
                // Steer along the wall normal
                avoidForce += hit.normal * weight;
            }
        }

        if (avoidForce.sqrMagnitude > 0.01f)
        {
            // Blend desired direction with avoidance
            return (desiredDir + avoidForce.normalized * 0.8f).normalized;
        }

        return desiredDir;
    }

    void PickNewTarget()
    {
        Vector2 offset = Random.insideUnitCircle * roamRadius;
        target = homePos + offset;
    }
}   