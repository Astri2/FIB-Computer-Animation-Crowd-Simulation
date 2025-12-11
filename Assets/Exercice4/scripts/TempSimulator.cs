using UnityEngine;

public class TempSimulator : MonoBehaviour
{
    [SerializeField] private Agent agent;

    [SerializeField] private Transform target;

    // glhf with finding a good value for that one
    [SerializeField] private float maxForce;

    [Header("Steering methods")]
    [SerializeField] private bool doSeek = true;
    [SerializeField] private float seekWeight = 1f;
    [SerializeField] private bool doFlee = true;
    [SerializeField] private float fleeWeight = 1f;

    [Header("Steering parameters")]
    [SerializeField] private float fleeRadius = 10f;
    [SerializeField] private bool drawFleeRadius = true;

    // Update is called once per frame
    public void FixedUpdate()
    {
        Vector3 steeringForce = Vector3.zero;

        if(doSeek) steeringForce += seekWeight * Seek(agent, target.position);
        if (doFlee) steeringForce += fleeWeight * Flee(agent, target.position, fleeRadius);

        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);

        Vector3 acceleration = steeringForce / agent.rb.mass;

        agent.velocity += acceleration * Time.deltaTime;
        agent.velocity = Vector3.ClampMagnitude(agent.velocity, agent.speed);
    }

    private Vector3 Seek(Agent agent, Vector3 target)
    {
        Vector3 currentVelocity = agent.tracker.m_worldVelocity;

        Vector3 desiredVelocity = (target - agent.tracker.m_worldPos).normalized * agent.speed;

        Vector3 steeringForce = desiredVelocity - currentVelocity;

        return steeringForce;
    }

    private Vector3 Flee(Agent agent, Vector3 target, float fleeingRadius)
    {
        Vector3 currentVelocity = agent.tracker.m_worldVelocity;

        Vector3 diff = agent.tracker.m_prevWorldPos - target;

        Vector3 desiredVelocity;
        if (Vector3.SqrMagnitude(diff) > fleeingRadius * fleeingRadius) 
            desiredVelocity = 0.75f * currentVelocity;
        else desiredVelocity = (diff).normalized * agent.speed;

        Vector3 steeringForce = desiredVelocity - currentVelocity;

        return steeringForce;
    }

    public void OnDrawGizmos()
    {
        if(!Application.isPlaying) return;

        if(doFlee && drawFleeRadius)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position, fleeRadius);
        }
    }
}
