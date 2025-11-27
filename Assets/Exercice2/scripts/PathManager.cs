using UnityEngine;

public class PathManager : MonoBehaviour
{

    public Vector3 goal;

    // to be set by the instancier
    public Vector3 worldBboxMin;
    public Vector3 worldBboxMax;

    private Agent agent;
    
    private void SetNewGoal()
    {
        float x = Random.Range(worldBboxMin.x, worldBboxMax.x);
        float y = Random.Range(worldBboxMin.y, worldBboxMax.y);
        float z = Random.Range(worldBboxMin.z, worldBboxMax.z);

        goal = new Vector3(x, y, z);
    }

    private void Start()
    {
        agent = GetComponent<Agent>();

        if(worldBboxMin == null || worldBboxMax == null)
        {
            Debug.Log("Warning: In Pathmanager, World Bounding box is null, did you forget to set it?");
            return;
        }

        SetNewGoal();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distanceToGoal = new Vector2(agent.rb.position.x - goal.x, agent.rb.position.z - goal.z).magnitude;
        if(distanceToGoal <= agent.velocity.magnitude * Time.fixedDeltaTime / 2.0f)
        {
            SetNewGoal();
            agent.RandomizeColorAndSpeed();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) { return; }
        if(agent == null) { return; }

        Color col = Color.red;
        Gizmos.color = col;
        Gizmos.DrawLine(agent.rb.position, goal);
    }
}
