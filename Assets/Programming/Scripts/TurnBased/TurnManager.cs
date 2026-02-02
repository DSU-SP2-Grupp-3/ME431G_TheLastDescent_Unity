using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private Dictionary<int, WorldAgentTeam> teams;
    
    private Coroutine cycle;
    public WorldAgentTeam activeTeam { get; private set; }

    private void Awake()
    {
        teams = new();
    }

    public void Activate()
    {
        cycle = StartCoroutine(TurnCycle());
    }

    public void Deactivate()
    {
        if (cycle != null) StopCoroutine(cycle);
        teams.Clear();
    }

    public void RegisterAgentInTeam(int team, WorldAgent agent)
    {
        if (!teams.TryAdd(team, new WorldAgentTeam(agent)))
        {
            teams.Add(team, new WorldAgentTeam(agent));
        }
    }

    public void RegisterAgentInTeam(WorldAgent.Team team, WorldAgent agent)
    {
        RegisterAgentInTeam((int)team, agent);
    }

    private List<WorldAgentTeam> ConvertTeamsToList()
    {
        return teams.Values.ToList();
    }

    private IEnumerator TurnCycle()
    {
        while (true)
        {
            foreach (WorldAgentTeam team in ConvertTeamsToList())
            {
                activeTeam = team;
                yield return team.ExecuteTeamActions();
            }
            activeTeam = null;
        }
    }
    
}