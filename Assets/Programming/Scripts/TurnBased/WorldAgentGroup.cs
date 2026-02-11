using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAgentGroup
{
    private List<WorldAgent> agents;

    public int Count => agents.Count;

    public WorldAgent.Team team => agents[0].team;
    
    public WorldAgentGroup()
    {
        agents = new();
    }

    public WorldAgentGroup(WorldAgent initialAgent)
    {
        agents = new() { initialAgent };
    }

    public void AddAgent(WorldAgent agent)
    {
        agents.Add(agent);
    }
    
    public List<IEnumerator> GetGroupCommandQueues()
    {
        List<IEnumerator> queues = new();
        foreach (WorldAgent agent in agents)
        {
            queues.Add(agent.ExecuteCommandQueue());
        }

        return queues;
    }
}