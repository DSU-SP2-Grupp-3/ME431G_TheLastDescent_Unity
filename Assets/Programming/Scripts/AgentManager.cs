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

    private Locator<InputManager> inputManager;
    private Locator<ModeSwitcher> modeSwitcher;
    private Locator<TurnManager> turnManager;

    private WorldAgent selectedPlayer;
    private WorldAgent defaultPlayer;
    public DamageManager damageManager;

    private bool allPlayersSelected;

    private CommandManager.CommandPackage currentCommandPackage;
    private ResourceManager.ClickAbility currentClickAbility;
    private WorldAgent portraitAgent;

    private void Awake()
    {
        Register();
        players = new();
        allAgents = new();
        inputManager = new();
        modeSwitcher = new();
        turnManager = new();
        cameraMover = new();
    }

    private void Start()
    {
        InputManager im = inputManager.Get();
        im.OnHover += PreviewCommand;
        im.OnRightClick += ProcessRightClick;
        im.OnHold += ProcessHold;
        im.OnLeftClick += () => currentClickAbility = null;
        modeSwitcher.Get().OnEnterTurnBased += (_) => allPlayersSelected = false;
    }

    private void PreviewCommand(RaycastHit hit, bool didHit)
    {
        if (currentClickAbility != null)
        {
            if (didHit && hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                WorldAgent hoveredAgent = hit.collider.GetComponentInParent<WorldAgent>();
                currentCommandPackage = CommandManager.GetSelectPlayerPackage(hoveredAgent, currentClickAbility);
            }
            else if (portraitAgent)
            {
                currentCommandPackage = CommandManager.GetSelectPlayerPackage(portraitAgent, currentClickAbility);
            }
            else
            {
                currentCommandPackage.SetCursor(currentClickAbility.invalidCursorPath);
            }
            PreviewUpdated?.Invoke(currentCommandPackage);
            return;
        }

        if (!didHit || turnManager.Get().executingTurn)
        {
            currentCommandPackage = CommandManager.EmptyPackage();
            PreviewUpdated?.Invoke(currentCommandPackage);
            return;
        }

        GameObject go = hit.collider.gameObject;
        currentCommandPackage = (LayerMask.LayerToName(go.layer)) switch
        {
            "Interactable" => CommandManager.GetInteractionPackage(selectedPlayer, go),
            "Player" => CommandManager.GetSelectPlayerPackage(
                go.GetComponentInParent<WorldAgent>(),
                null
            ),
            "Ground" => CommandManager.GetMovePackage(selectedPlayer, hit.point),
            "Enemy" => CommandManager.GetAttackEnemyPackage(
                selectedPlayer,
                go.GetComponentInParent<WorldAgent>(),
                damageManager
            ),
            _ => CommandManager.EmptyPackage()
        };

        PreviewUpdated?.Invoke(currentCommandPackage);
    }

    private void ProcessRightClick()
    {
        if (currentCommandPackage.empty) return;
        else if (currentCommandPackage.type == "select") SelectPlayer(currentCommandPackage.agent);
        if (currentCommandPackage.commands.Count > 0) QueueCurrentPackage();
    }

    private void ProcessHold()
    {
        if (currentCommandPackage.type == "move" && modeSwitcher.Get().mode == RoundClock.ProgressMode.RealTime)
        {
            QueueCurrentPackage();
        }
    }

    private void QueueCurrentPackage()
    {
        currentClickAbility = null;
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

    public void SetClickAbility(ResourceManager.ClickAbility clickAbility)
    {
        currentClickAbility = clickAbility;
    }

    public void SetPortraitAgent(WorldAgent agent)
    {
        portraitAgent = agent;
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
        }
    }

    public void SelectAllPlayers()
    {
        if (modeSwitcher.Get().mode == RoundClock.ProgressMode.RealTime)
        {
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