using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEngine.InputSystem.HID.HID;

public class Simulator : MonoBehaviour
{
    public float timeStep = 0.025f;

    [SerializeField]
    private List<Agent> agents;

    // hide constructor
    private Simulator() { 
        agents = new List<Agent>();
    }

    private static Simulator instance;

    public static Simulator getInstance()
    {
        if(instance == null)
        {
            // create a new GO
            GameObject goInstance = new GameObject("Simulator");
            // add a simulator component and set it to our singleton instance
            instance = goInstance.AddComponent<Simulator>();
        }
        return instance;
    }

    public void AddAgent(Agent agent)
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

    public void RemoveAgent(Agent agent)
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
        foreach (Agent agent in agents)
        {
            PathManager pathManager = agent.pathManager;
            
            // Agent might not be ready yet
            if (pathManager == null) continue;
            
            Vector3 goal = pathManager.goal;
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
