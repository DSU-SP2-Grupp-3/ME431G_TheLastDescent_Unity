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
    private List<(WorldAgent agent, int team)> slackers;

    private Coroutine cycle;
    public WorldAgentGroup activeGroup { get; private set; }

    private bool playerReady;

    [SerializeField]
    private Events turnManagerEvents;

    private Locator<RoundClock> roundClock;
    private Locator<AgentManager> agentManager;
    private Locator<ModeSwitcher> modeSwitcher;
    private Locator<Modal> modalLocator;
    private Locator<InputManager> inputManager;
    private Locator<OrthographicCameraMover> camera;

    [SerializeField]
    private ResourceManager resourceManager;

    public bool executingTurn { get; private set; }
    public bool active => cycle != null;

    private void Awake()
    {
        Register();
        groups = new();
        orderedGroups = new();
        slackers = new();
        roundClock = new();
        agentManager = new();
        modeSwitcher = new();
        modalLocator = new();
        inputManager = new();
        camera = new();
    }

    private void Start()
    {
        inputManager.Get().OnIKeyPressed += GlobalInterrupt;
    }

    public void Ready()
    {
        if (resourceManager.InDeficit())
        {
            turnManagerEvents.ResourceDeficitRejection?.Invoke();
            return;
        }
        if (AllPlayersAPUsed()) playerReady = true;
        else
        {
            modalLocator.Get().Prompt(
                "Some characters have unused AP. Do you still want to end your turn?",
                () => playerReady = true,
                () => playerReady = false
            );
        }
    }

    public void Activate()
    {
        cycle = StartCoroutine(TurnCycle());
    }

    public void Deactivate(bool dontClearGroups = false)
    {
        if (cycle != null)
        {
            StopCoroutine(cycle);
            cycle = null;
        }
        if (!dontClearGroups) groups.Clear();
        else
        {
            turnManagerEvents.FinishExecutingTurn?.Invoke();
        }
        executingTurn = false;
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
        int min = -1;
        if (groups.Count > 0) min = Math.Min(groups.Keys.Min(), -1);
        if (slackers.Count > 0)
        {
            int slackersMin = slackers.Select(pair => pair.team).Min();
            min = Math.Min(min, slackersMin);
        }
        RegisterAgentInGroup(min - 1, agent);
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

            camera.Get().SetPanningLocked(true);

            turnManagerEvents.StartExecutingTurn?.Invoke();
            executingTurn = true;

            camera.Get().SetCameraTarget(agentManager.Get().playerMiddleTransform);

            OrderGroups();
            for (int i = 0; i < orderedGroups.Count; i++)
            {
                activeGroup = orderedGroups[i];
                if (activeGroup.GroupDead()) continue;
                if (activeGroup.team != WorldAgent.Team.Player)
                {
                    camera.Get().SetCameraTarget(activeGroup.GetCameraTarget().cameraFocusTransform);
                }
                yield return WaitForAll(activeGroup.GetGroupCommandQueues());
            }

            agentManager.Get().SelectPlayer(agentManager.Get().GetSelectedPlayer());

            activeGroup = null;

            turnManagerEvents.FinishExecutingTurn?.Invoke();
            executingTurn = false;

            AddSlackersToGroups();

            if (AllActiveEnemiesDead())
            {
                modeSwitcher.Get().EnterRealTime();
            }

            camera.Get().SetPanningLocked(false);
        }
    }

    private void GlobalInterrupt()
    {
        foreach (WorldAgent agent in agentManager.Get().GetAllAgents())
        {
            agent.InterruptCommandQueue();
        }
        Deactivate(true);
        Activate();
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

    private bool AllPlayersAPUsed()
    {
        bool used = true;
        foreach (WorldAgent playerAgent in agentManager.Get().GetPlayerAgents())
        {
            if (playerAgent.localStats.actionPoints >= 0.05f)
            {
                used = false;
            }
        }
        return used;
    }

    [Serializable]
    public struct Events
    {
        public UnityEvent StartExecutingTurn, FinishExecutingTurn, ResourceDeficitRejection;
    }
}