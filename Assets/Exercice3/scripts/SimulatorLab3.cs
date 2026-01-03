using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatorLab3 : MonoBehaviour
{
    public float timeStep = 0.025f;

    [SerializeField]
    private List<AgentLab3> agents;

    // hide constructor
    private SimulatorLab3() { 
        agents = new List<AgentLab3>();
    }

    private static SimulatorLab3 instance;

    public static SimulatorLab3 getInstance()
    {
        if(instance == null)
        {
            // create a new GO
            GameObject goInstance = new GameObject("Simulator");
            // add a simulator component and set it to our singleton instance
            instance = goInstance.AddComponent<SimulatorLab3>();
        }
        return instance;
    }

    public void AddAgent(AgentLab3 agent)
    {
        agents.Add(agent);
    }

    public void RemoveNLastAgents(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Destroy(agents[agents.Count-1].gameObject);
            agents.RemoveAt(agents.Count-1);
        }
    }

    public void RemoveAgent(AgentLab3 agent)
    {
        // If this is often used, we should change the type of agents to a linkedlist for example
        agents.Remove(agent);
    }

    public int AgentCount()
    {
        return agents.Count;
    }

    public void UpdateSimulator(float deltaTime)
    {
        foreach (AgentLab3 agent in agents)
        {
            PathManagerLab3 pathManager = agent.pathManager;
            
            // Agent might not be ready yet
            if (pathManager == null) continue;
            
            Vector3 goal = pathManager.currentGoal;
            Vector3 toGoal = goal - agent.rb.position;
                
            Vector3 dir = toGoal.normalized;

            float speed = Mathf.Min(agent.speed, toGoal.magnitude / deltaTime);
            agent.velocity = dir * speed;
        }
    }

    public void Start()
    {
        StartCoroutine(SimulationCoroutine());
    }

    IEnumerator SimulationCoroutine()
    {
        while (true)
        {
            UpdateSimulator(timeStep);
            yield return new WaitForSeconds(timeStep);
        }
    }
}
