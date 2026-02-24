using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class AgentManager : Service<AgentManager>
{
    public event Action<WorldAgent> AgentRegistered;
    public event Action<CommandManager.CommandPackage> PreviewUpdated;
    public UnityEvent NotEnoughAP;

    private List<WorldAgent> players;
    private List<WorldAgent> allAgents;
    private Locator<OrthographicCameraMover> cameraMover;
    private Locator<SelectionIndicator> indicator;

    private Locator<InputManager> inputManager;
    private Locator<ModeSwitcher> modeSwitcher;
    private Locator<TurnManager> turnManager;

    private WorldAgent selectedPlayer;
    private WorldAgent defaultPlayer;
    public DamageManager damageManager;

    private bool allPlayersSelected;

    private CommandManager.CommandPackage currentCommandPackage;

    private void Awake()
    {
        Register();
        players = new();
        allAgents = new();
        inputManager = new();
        modeSwitcher = new();
        turnManager = new();
        cameraMover = new();
        indicator = new();
    }

    private void Start()
    {
        InputManager im = inputManager.Get();
        im.OnHover += PreviewCommand;
        im.OnClick += ProcessClick;
        modeSwitcher.Get().OnEnterTurnBased += (_) => allPlayersSelected = false;
    }
    
    private void PreviewCommand(RaycastHit hit, bool didHit)
    {
        if (!didHit || turnManager.Get().executingTurn)
        {
            currentCommandPackage = CommandManager.EmptyPackage();
            PreviewUpdated?.Invoke(currentCommandPackage);
            return;
        }

        GameObject go = hit.collider.gameObject;
        currentCommandPackage = (LayerMask.LayerToName(go.layer)) switch
        {
            "Interactable" => CommandManager.GetInteractionCommands(selectedPlayer, go),
            "Player" => CommandManager.SelectPlayerPackage(go.GetComponentInParent<WorldAgent>()),
            "Ground" => CommandManager.GetMoveCommand(selectedPlayer, hit.point),
            "Enemy" => CommandManager.AttackEnemyPackage(selectedPlayer, go.GetComponent<WorldAgent>(), damageManager),
            _ => CommandManager.EmptyPackage()
        };
        PreviewUpdated?.Invoke(currentCommandPackage);

    }

    private void ProcessClick()
    {
        if (currentCommandPackage.empty) return;
        else if (currentCommandPackage.clickOnAgentOnly) SelectPlayer(currentCommandPackage.agent);
        else
        {
            if (!currentCommandPackage.QueueCommands(modeSwitcher.Get().mode)) NotEnoughAP?.Invoke();
            
            // move other characters if select all is active
            if (allPlayersSelected && currentCommandPackage.type == "move")
            {
                IMoveCommand moveCommand = currentCommandPackage.commands[0] as IMoveCommand;
                foreach (WorldAgent agent in players)
                {
                    if (agent == selectedPlayer) continue;
                    MoveInRangeCommand moveInRangeCommand = new MoveInRangeCommand(moveCommand.ToPosition(), 3f, agent);
                    agent.OverwriteQueue(moveInRangeCommand);
                }
            }
        }
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
                defaultPlayer = agent;
            }
        }
    }

    public void SelectPlayer(WorldAgent playerAgent)
    {
        allPlayersSelected = false;
        if (players.Contains(playerAgent) && !playerAgent.dead)
        {
            selectedPlayer = playerAgent;
            cameraMover.Get().SetCameraTarget(selectedPlayer.cameraFocusTransform);
            indicator.Get().SetIndicatorTarget(selectedPlayer.transform);
        }
    }

    public void SelectAllPlayers()
    {
        if (modeSwitcher.Get().mode == RoundClock.ProgressMode.RealTime)
        {
            SelectPlayer(defaultPlayer);
            allPlayersSelected = true;
        }
    }

    public void UndoLatestCommand()
    {
        if (modeSwitcher.Get().mode == RoundClock.ProgressMode.TurnBased)
        {
            selectedPlayer.UndoLastestCommand();
        }
    }
    

    public List<WorldAgent> GetPlayerAgents() => players;
    public List<WorldAgent> GetAllAgents() => allAgents;

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