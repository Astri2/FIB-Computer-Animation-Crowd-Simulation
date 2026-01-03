using System.Collections.Generic;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UIElements;

public class CrowdGeneratorLab3 : MonoBehaviour
{
    [SerializeField] 
    Transform floorTransform;

    [SerializeField] 
    GridGenerator gridGenerator;

    private Grid grid;

    [SerializeField]
    GameObject agentPrefab;

    [SerializeField]
    private uint agentNumber = 0;

    private void spawnAgents(uint n)
    {
        SimulatorLab3 simulator = SimulatorLab3.getInstance();

        // this parameter should prevent the spawner to get stuck if it can't find a suitable spawn point
        const int maxSpawnIteration = 20;

        for (int i = 0; i < n; i++)
        {
            // Instanciate the GO
            GameObject agentGO = Instantiate(agentPrefab, this.transform);
            agentGO.SetActive(false); // prevent self intersection

            // Store it in simulator's list
            AgentLab3 agent = agentGO.GetComponent<AgentLab3>();
            simulator.AddAgent(agent);

            // Init pathManager
            float minX = floorTransform.position.x - 5 * floorTransform.lossyScale.x;
            float maxX = floorTransform.position.x + 5 * floorTransform.lossyScale.x;
            float minZ = floorTransform.position.z - 5 * floorTransform.lossyScale.z;
            float maxZ = floorTransform.position.z + 5 * floorTransform.lossyScale.z;
            float height = floorTransform.position.y;
            Vector3 worldBboxMin = new Vector3(minX, height, minZ);
            Vector3 worldBboxMax = new Vector3(maxX, height, maxZ);

            PathManagerLab3 pathManager = agentGO.GetComponent<PathManagerLab3>();
            pathManager.grid = grid;

            // will store spawn location information
            
            Vector3 spawnPoint;
            Quaternion rotation;

            int spawnIteration = 0;
            bool collision, cellOccupied;
            do
            {
                float x, y, z, yYaw;
                x = Random.Range(worldBboxMin.x, worldBboxMax.x);
                y = Random.Range(worldBboxMin.y, worldBboxMax.y);
                z = Random.Range(worldBboxMin.z, worldBboxMax.z);
                yYaw = Random.Range(0, 360);

                spawnPoint = new Vector3(x, y, z);
                rotation = Quaternion.Euler(new Vector3(0, yYaw, 0));

                collision = Physics.CheckSphere(spawnPoint + Vector3.up * (agent.colliderRadius + 0.01f), agent.colliderRadius);
                cellOccupied = grid.getNode(grid.CoordToId(spawnPoint)).occupied;

                // fallback to prevent infinite loops
                if (spawnIteration++ >= maxSpawnIteration)
                {
                    Debug.Log("Could not find a collision free location. Spawning anyway.");
                    break;
                }

            } while (collision || cellOccupied);

            agentGO.transform.position = spawnPoint;
            agentGO.transform.rotation = rotation;
            agentGO.SetActive(true);
        }
    }

    public void Start()
    {
        // Don't spawn agents yet, becasue grid will most likely not exist yet
    }

    // Update is called once per frame
    public void Update()
    {
        // retrieve grid until it exists
        if (grid == null)
        {
            grid = gridGenerator.grid;
            if (grid == null) return;
        }

        SimulatorLab3 simulator = SimulatorLab3.getInstance();
        // remove exessive agents
        if (simulator.AgentCount() > agentNumber)
        {
            simulator.RemoveNLastAgents((int)(simulator.AgentCount() - agentNumber));
        }

        // add missing agents
        if (simulator.AgentCount() < agentNumber)
        {
            spawnAgents((uint)(agentNumber - simulator.AgentCount()));
        }
    }
}
