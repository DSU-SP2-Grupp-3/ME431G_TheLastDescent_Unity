using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class AgentManager : Service<AgentManager>
{
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
        SelectPlayer(players[0]);
    }

    public void RegisterAgent(WorldAgent agent)
    {
        allAgents.Add(agent);
        if (agent.team == WorldAgent.Team.Player)
        {
            players.Add(agent);
        }
    }

    private void SelectPlayer(WorldAgent playerAgent)
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
            () => selectedPlayer.QueueCommand(movePlayer)
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
                selectedPlayer.QueueCommands(result.invokingAgentCommands);
                selectedPlayer.QueueCommand(result.QueueInteractablesCommand(selectedPlayer));
            });
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

    private bool CanQueueCommand(Command command)
    {
        // don't queue null command, obviously
        if (command == null) return false;
        // check if queueing command will exceed the allowed ap cost for the player based on WorldAgent.localStats
        return true;
        // Debug.Log($"Not enough AP remaining to queue command {command}");
    }

    private bool CanQueueCommands(Command[] commands)
    {
        // don't queue empty commandsw
        if (commands.Length == 0) return false;
        // check if queueing command will exceed the allowed ap cost for the player based on WorldAgent.localStats
        return true;
        // Debug.Log($"Not enough AP remaining to queue commands starting with {commands[0]}");
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