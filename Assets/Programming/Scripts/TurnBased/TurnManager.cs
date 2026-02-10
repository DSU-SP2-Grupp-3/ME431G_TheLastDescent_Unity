using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    private Dictionary<int, WorldAgentTeam> teams;
    
    private Coroutine cycle;
    public WorldAgentTeam activeTeam { get; private set; }
    
    private bool playerReady;

    [SerializeField]
    private Events turnManagerEvents;
    
    private void Awake()
    {
        teams = new();
    }

    private void Start()
    {
        Locator<ReadyButton> readyButton = new();
        readyButton.Get().ReadyButtonPressed += () => playerReady = true;
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
            teams[team].AddAgent(agent);
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
            playerReady = false;
            yield return new WaitUntil((() => playerReady == true));
            
            turnManagerEvents.StartExecutingTurn?.Invoke();
            
            foreach (WorldAgentTeam team in ConvertTeamsToList())
            {
                activeTeam = team;
                yield return WaitForAll(team.GetTeamCommandQueues());
            }
            activeTeam = null;
            
            turnManagerEvents.FinishExecutingTurn?.Invoke();
        }
    }
    
    // https://www.reddit.com/r/Unity3D/comments/11imces/wait_for_all_coroutines_to_finish/
    public IEnumerator WaitForAll(List<IEnumerator> coroutines)
    {
        int coroutineTally = 0;

        for (int i = 0; i < coroutines.Count; i++)
        {
            StartCoroutine(RunAwaitedCoroutine(coroutines[i]));
        }

        while (coroutineTally > 0)
        {
            yield return null;
        }

        IEnumerator RunAwaitedCoroutine(IEnumerator coroutine)
        {
            coroutineTally++;
            yield return StartCoroutine(coroutine);
            coroutineTally--;
        }
    }
    

    [Serializable]
    public struct Events
    {
        public UnityEvent StartExecutingTurn, FinishExecutingTurn;
    }
    
}