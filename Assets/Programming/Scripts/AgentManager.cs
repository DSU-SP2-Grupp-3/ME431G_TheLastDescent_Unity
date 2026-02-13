using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AgentManager : Service<AgentManager>
{
    [SerializeField]
    private WorldAgent[] players;
    private List<WorldAgent> allAgents;
    [SerializeField] 
    private OrthographicCameraMover cameraMover;

    private Locator<InputManager> inputManager;
    private Locator<ModeSwitcher> modeSwitcher;

    private WorldAgent selectedPlayer;
    public DamageManager damageManager;

    private void Awake()
    {
        Register();
        allAgents = new();
        inputManager = new();
        modeSwitcher = new();
    }

    private void Start()
    {
        InputManager im = inputManager.Get();
        im.ClickedOnPlayer += SelectPlayer;
        im.MovePlayerInput += MoveSelectedPlayer;
        im.ClickedEnvironment += ClickedEnvironment;
        im.ClickedOnEnemy += ClickedEnemy;
        SelectPlayer(players[0]);
    }
    
    public void RegisterAgent(WorldAgent agent) 
    {
        allAgents.Add(agent);    
    }

    public void SelectPlayer(WorldAgent playerAgent)
    {
        if (players.Contains(playerAgent))
        {
            // Debug.Log($"Select {playerAgent.name}");
            selectedPlayer = playerAgent;
            // cameraMover.targetGameObject = playerAgent.cameraFocusTransform;
            // todo: camera should move smoothly toward target transform and not follow animations on target -se
        }
    }

    private void MoveSelectedPlayer(Vector3 position)
    {
        MoveCommand movePlayer = new MoveCommand(position, selectedPlayer);
        if (!movePlayer.possible) return;

        RealTimeOrTurnBased(
            () => selectedPlayer.OverwriteQueue(movePlayer),
            () => selectedPlayer.QueueCommand(movePlayer)
        );
    }

    private void ClickedEnvironment(GameObject go)
    {
        InteractionGroup group = go.GetComponentInParent<InteractionGroup>();
        if (group)
        {
            RealTimeOrTurnBased(
                () => group.InteractRealTime(selectedPlayer),
                () => group.InteractTurnBased(selectedPlayer)
            );
        }
        else if (go.TryGetComponent<Interactable>(out Interactable interactable))
        {
            RealTimeOrTurnBased(
                () => interactable.InteractRealTime(selectedPlayer),
                () => interactable.InteractTurnBased(selectedPlayer)
            );
        }
    }

    private void ClickedEnemy(WorldAgent enemyAgent)
    {
        if (enemyAgent.dead) return;
        
        MoveInRangeCommand inRangeCommand = new MoveInRangeCommand(
            enemyAgent.transform.position,
            selectedPlayer.weaponStats.attackRange, 
            selectedPlayer
        );
        AttackCommand attackCommand = new AttackCommand(selectedPlayer, enemyAgent, damageManager);
        Command[] commands = new Command[] { inRangeCommand, attackCommand };
        
        RealTimeOrTurnBased(
            () => selectedPlayer.OverwriteQueue(commands),
            () => selectedPlayer.QueueCommands(commands)
        );
    }

    private void RealTimeOrTurnBased(Action realTime, Action turnBased)
    {
        switch (modeSwitcher.Get().mode)
        {
            case RoundClock.ProgressMode.RealTime:
                realTime.Invoke();
                break;
            case RoundClock.ProgressMode.TurnBased:
                turnBased.Invoke();
                break;
        }
    }

    public List<Vector3> GetPlayerPositions()
    {
        return players.Select(w => w.transform.position).ToList();
    }

    public List<NavMeshAgent> GetPlayerNavMeshAgents()
    {
        return players.Select(w => w.navMeshAgent).ToList();
    }

    /// <summary>
    /// Returns an IEnumerable of all world agents that pass all filters
    /// </summary>
    /// <param name="predicates">A parameterized list of predicate lambdas</param>
    /// <returns>An IEnumerable of all agents that pass all filters</returns>
    public IEnumerable<WorldAgent> GetFilteredAgents(params Func<WorldAgent, bool>[] predicates)
    {
        IEnumerable<WorldAgent> matchingAgents = allAgents;
        foreach (Func<WorldAgent, bool> predicate in predicates)
        {
            matchingAgents = matchingAgents.Where(predicate);
        }
        return matchingAgents;
    }
}