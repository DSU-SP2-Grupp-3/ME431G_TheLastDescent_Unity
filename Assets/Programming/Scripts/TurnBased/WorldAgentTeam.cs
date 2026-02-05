using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAgentTeam
{
    private List<WorldAgent> agents;

    public int Count => agents.Count;
    
    public WorldAgentTeam()
    {
        agents = new();
    }

    public WorldAgentTeam(WorldAgent initialAgent)
    {
        agents = new() { initialAgent };
    }

    public void AddAgent(WorldAgent agent)
    {
        agents.Add(agent);
    }
    
    public List<IEnumerator> GetTeamCommandQueues()
    {
        List<IEnumerator> queues = new();
        foreach (WorldAgent agent in agents)
        {
            queues.Add(agent.ExecuteCommandQueue());
        }

        return queues;
    }
    
    
}