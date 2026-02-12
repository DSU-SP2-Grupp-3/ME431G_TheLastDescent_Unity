using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : Service<TurnManager>
{
    private Dictionary<int, WorldAgentGroup> groups;
    private List<WorldAgentGroup> orderedGroups;
    private List<(WorldAgent, int)> slackers;
    
    private Coroutine cycle;
    public WorldAgentGroup activeGroup { get; private set; }
    
    private bool playerReady;

    [SerializeField]
    private Events turnManagerEvents;

    private Locator<RoundClock> roundClock;
    private Locator<AgentManager> agentManager;
    private Locator<ModeSwitcher> modeSwitcher;

    private bool executingTurn;
    
    private void Awake()
    {
        Register();
        groups = new();
        orderedGroups = new();
        slackers = new();
        roundClock = new();
        agentManager = new();
        modeSwitcher = new();
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
        groups.Clear();
    }

    public void RegisterAgentInGroup(int team, WorldAgent agent)
    {
        if (executingTurn)
        {
            slackers.Add((agent, team));
        }
        else if (!groups.TryAdd(team, new WorldAgentGroup(agent)))
        {
            groups[team].AddAgent(agent);
        }
    }

    public void RegisterAgentInGroup(WorldAgent.Team team, WorldAgent agent)
    {
        RegisterAgentInGroup((int)team, agent);
    }

    public void RegisterAgentAsOneManTeam(WorldAgent agent)
    {
        if (groups.Count == 0)
        {
            RegisterAgentInGroup(-1, agent);
        }
        else
        {
            int min = groups.Keys.Min();
            RegisterAgentInGroup(min, agent);
        }
    }

    private void OrderGroups()
    {
        orderedGroups.Clear();
        WorldAgentGroup players = groups.Where((pair) => pair.Key == (int)WorldAgent.Team.Player).First().Value;
        List<WorldAgentGroup> theRest = groups
                                        .Where((pair) => pair.Key != (int)WorldAgent.Team.Player)
                                        .Select(pair => pair.Value)
                                        .ToList();
        orderedGroups.Add(players);
        orderedGroups.AddRange(theRest);
    }

    private void AddSlackersToGroups()
    {
        foreach ((WorldAgent agent, int team) slacker in slackers)
        {
            RegisterAgentInGroup(slacker.team, slacker.agent);
        }
        slackers.Clear();
    }
    
    private IEnumerator TurnCycle()
    {
        while (true)
        {
            playerReady = false;
            yield return new WaitUntil((() => playerReady == true));
            
            turnManagerEvents.StartExecutingTurn?.Invoke();
            executingTurn = true;
            
            OrderGroups();
            for (int i = 0; i < orderedGroups.Count; i++)
            {
                activeGroup = orderedGroups[i];
                yield return WaitForAll(activeGroup.GetGroupCommandQueues());
            }
            
            activeGroup = null;
            

            turnManagerEvents.FinishExecutingTurn?.Invoke();
            executingTurn = false;

            AddSlackersToGroups();
            
            if (AllActiveEnemiesDead())
            {
                // force entrance into real time when all enemies have been defeated
                modeSwitcher.Get().TryEnterRealTime(true);
            }
            
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

    private bool AgentIsNotDead(WorldAgent agent) => !agent.dead;
    private bool AgentIsActive(WorldAgent agent) => agent.active;
    private bool AgentIsEnemy(WorldAgent agent) => agent.team == WorldAgent.Team.Enemy;
    
    private bool AllActiveEnemiesDead()
    {
        // if there are any enemy agents that are active and not dead return false, otherwise true
        return !agentManager.Get().GetFilteredAgents(AgentIsNotDead, AgentIsActive, AgentIsEnemy).Any();
    }
    

    [Serializable]
    public struct Events
    {
        public UnityEvent StartExecutingTurn, FinishExecutingTurn;
    }
    
}