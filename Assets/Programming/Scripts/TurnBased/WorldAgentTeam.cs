using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAgentTeam
{
    private List<WorldAgent> agents;

    private bool teamReady;

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
    
    public void ReadyTeam()
    {
        teamReady = true;
    }
    
    public IEnumerator ExecuteTeamActions()
    {
        yield return new WaitUntil(() => teamReady);

        foreach (WorldAgent agent in agents)
        {
            yield return agent.ExecuteCommandQueue();
        }

        teamReady = false;
    }

}