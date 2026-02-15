using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AgentManager : Service<AgentManager>
{
    public event Action<WorldAgent> AgentRegistered;

    private List<WorldAgent> players;
    private List<WorldAgent> allAgents;
    private Locator<OrthographicCameraMover> cameraMover;

    private Locator<InputManager> inputManager;
    private Locator<ModeSwitcher> modeSwitcher;

    private WorldAgent selectedPlayer;
    public DamageManager damageManager;

    private void Awake()
    {
        Register();
        players = new();
        allAgents = new();
        inputManager = new();
        modeSwitcher = new();
        cameraMover = new();
    }

    private void Start()
    {
        InputManager im = inputManager.Get();
        im.ClickedOnPlayer += SelectPlayer;
        im.MovePlayerInput += MoveSelectedPlayer;
        im.ClickedEnvironment += ClickedEnvironment;
        im.ClickedOnEnemy += ClickedEnemy;
    }

    public void RegisterAgent(WorldAgent agent)
    {
        allAgents.Add(agent);
        AgentRegistered?.Invoke(agent);
        if (agent.team == WorldAgent.Team.Player)
        {
            players.Add(agent);
            if (!selectedPlayer && agent.defaultSelected)
            {
                SelectPlayer(agent);
            }
        }
    }

    public void SelectPlayer(WorldAgent playerAgent)
    {
        if (players.Contains(playerAgent) && !playerAgent.dead)
        {
            // Debug.Log($"Select {playerAgent.name}");
            selectedPlayer = playerAgent;
            // cameraMover.targetGameObject = playerAgent.cameraFocusTransform;
            // todo: camera should move smoothly toward target transform and not follow animations on target -se
            cameraMover.Get().SetCameraTarget(selectedPlayer.cameraFocusTransform);
        }
    }

    private void MoveSelectedPlayer(Vector3 position)
    {
        MoveCommand movePlayer = new MoveCommand(position, selectedPlayer);

        if (!movePlayer.possible) return;

        RealTimeOrTurnBased(
            () => selectedPlayer.OverwriteQueue(movePlayer),
            () =>
            {
                if (!CanQueueCommand(movePlayer)) return;
                selectedPlayer.QueueCommand(movePlayer);
            }
        ); 
    }

    private void ClickedEnvironment(GameObject go)
    {
        Interactable.InteractionResult result;
        InteractionGroup group = go.GetComponentInParent<InteractionGroup>();
        if (group)
        {
            group.UnwrapInteractableCommands(selectedPlayer, out result);
        }
        else if (go.TryGetComponent<Interactable>(out Interactable interactable))
        {
            interactable.UnwrapInteractableCommands(selectedPlayer, out result);
        }
        else
        {
            // didn't click on interactable, return
            return;
        }

        // queue the right commands based on turn based or real time
        RealTimeOrTurnBased(
            () =>
            {
                selectedPlayer.OverwriteQueue(result.invokingAgentCommands);
                selectedPlayer.QueueCommand(result.QueueInteractablesCommand(selectedPlayer));
            },
            () =>
            {
                InvokeActionCommand queueInteractables = result.QueueInteractablesCommand(selectedPlayer);
                if (CanQueueCommands(result.invokingAgentCommands) && CanQueueCommand(queueInteractables))
                {
                    selectedPlayer.QueueCommands(result.invokingAgentCommands);
                    selectedPlayer.QueueCommand(result.QueueInteractablesCommand(selectedPlayer));
                }
            }
        );
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
            () =>
            {
                if (!CanQueueCommands(commands)) return;
                selectedPlayer.QueueCommands(commands);
            }
        );
    }

    private bool CanQueueCommand(Command command)
    {
        // don't queue null command, obviously
        if (command == null) return false;
        float remainingAP = selectedPlayer.localStats.actionPoints - selectedPlayer.TotalCommandQueueCost();
        if (remainingAP < command.cost)
        {
            Debug.Log($"Not enough AP remaining to queue command {command}, " +
                      $"ap remaining = {remainingAP}, command cost: {command.cost}");
            return false;
        }
        return true;
    }

    private bool CanQueueCommands(Command[] commands)
    {
        // don't queue empty commands
        if (commands.Length == 0) return false;
        float queueCost = 0f;
        foreach (Command command in commands)
        {
            queueCost += command.cost;
            if (selectedPlayer.TotalCommandQueueCost() + queueCost > selectedPlayer.localStats.actionPoints)
            {
                Debug.Log($"Not enough AP remaining to queue commands starting with {commands[0]}");
                return false;
            }
        }
        // check if queueing command will exceed the allowed ap cost for the player based on WorldAgent.localStats
        return true;
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

    public List<WorldAgent> GetPlayerAgents() => players;

    public List<Vector3> GetPlayerPositions()
    {
        return players.Select(w => w.transform.position).ToList();
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