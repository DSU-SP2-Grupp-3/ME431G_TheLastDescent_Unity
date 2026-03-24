using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            if (agent.TryGetComponent<AI>(out AI ai))
            {
                ai.GetActiveCommands();
            }
            queues.Add(agent.ExecuteCommandQueue());
        }

        return queues;
    }

    public bool GroupDead()
    {
        return agents.All(a => a.dead);
    }

    public WorldAgent GetCameraTarget()
    {
        IEnumerable<WorldAgent> alive = agents.Where(a => !a.dead);
        if (alive.Any()) return alive.First();
        else return null;
    }
}