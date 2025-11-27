using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

public class CrowdGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject agentPrefab;

    [SerializeField]
    private uint agentNumber = 0;

    public Vector3 worldBboxMin = Vector3.zero;
    public Vector3 worldBboxMax = Vector3.zero;

    private void spawnAgents(uint n)
    {
        Simulator simulator = Simulator.getInstance();
        
        // this parameter should prevent the spawner to get stuck if it can't find a suitable spawn point
        const int maxSpawnIteration = 20;
        
        for (int i = 0; i < n; i++)
        {
            // Instanciate the GO
            GameObject agentGO = Instantiate(agentPrefab, this.transform);
            agentGO.SetActive(false); // prevent self intersection
            
            // Store it in simulator's list
            Agent agent = agentGO.GetComponent<Agent>();
            simulator.AddAgent(agent);
            
            // Init pathManager
            PathManager pathManager = agentGO.GetComponent<PathManager>();
            pathManager.worldBboxMin = worldBboxMin;
            pathManager.worldBboxMax = worldBboxMax;

            // will store spawn location information
            float x, y, z, yYaw;
            Vector3 spawnPoint;
            Quaternion rotation;

            int spawnIteration = 0;
            bool collision = false;
            do
            {
                x = Random.Range(worldBboxMin.x, worldBboxMax.x);
                y = Random.Range(worldBboxMin.y, worldBboxMax.y);
                z = Random.Range(worldBboxMin.z, worldBboxMax.z);
                yYaw = Random.Range(0, 360);

                spawnPoint = new Vector3(x, y, z);
                rotation = Quaternion.Euler(new Vector3(0, yYaw, 0));

                collision = Physics.CheckSphere(spawnPoint + Vector3.up * (agent.colliderRadius + 0.01f), agent.colliderRadius);
                
                // fallback to prevent infinite loops
                if(spawnIteration++ >= maxSpawnIteration)
                {
                    Debug.Log("Could not find a collision free location. Spawning anyway.");
                    break;
                }

            } while (collision);

            agentGO.transform.position = spawnPoint;
            agentGO.transform.rotation = rotation;
            agentGO.SetActive(true);
        }
    }

    void Start()
    {
        if (worldBboxMin ==  Vector3.zero && worldBboxMax == Vector3.zero)
        {
            Debug.Log("Warning: both worldBboxMin & worldBboxMax are zero vectors");
        }

        spawnAgents(agentNumber);
    }

    // Update is called once per frame
    void Update()
    {
        Simulator simulator = Simulator.getInstance();
        // remove exessive agents
        if(simulator.AgentCount() > agentNumber) {
            simulator.RemoveNLastAgents((int)(simulator.AgentCount() - agentNumber));
        }

        // add missing agents
        if (simulator.AgentCount() < agentNumber) { 
            spawnAgents((uint)(agentNumber - simulator.AgentCount()));
        }
    }
}
