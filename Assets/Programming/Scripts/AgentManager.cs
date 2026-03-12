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
    public UnityEvent NotEnouchResources;
    public UnityEvent InvalidTarget;
    public UnityEvent EnterSpectator;

    private List<WorldAgent> players;
    private List<WorldAgent> allAgents;

    private int numberOfAlivePlayers;

    private Locator<OrthographicCameraMover> cameraMover;
    private Locator<InputManager> inputManager;
    private Locator<ModeSwitcher> modeSwitcher;
    private Locator<TurnManager> turnManager;

    public RoundClock.ProgressMode mode => modeSwitcher.Get().mode;

    private WorldAgent selectedPlayer;
    private WorldAgent defaultPlayer;

    public DamageManager damageManager;
    public ResourceManager resourceManager;

    private bool allPlayersSelected;

    private CommandManager.CommandPackage currentCommandPackage;
    private ClickAbility currentClickAbility;
    private WorldAgent portraitAgent;

    private bool agentInputActive = true;

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
        im.OnLeftClick += ProcessRightClick;
        im.OnLeftHold += ProcessHold;
        im.OnRightClick += () => currentClickAbility = null;
        modeSwitcher.Get().OnEnterTurnBased += (_) => allPlayersSelected = false;
    }

    private void PreviewCommand(RaycastHit hit, bool didHit)
    {
        if (!agentInputActive || selectedPlayer.dead)
        {
            currentCommandPackage = CommandManager.EmptyPackage();
            PreviewUpdated?.Invoke(currentCommandPackage);
            return;
        }

        if (currentClickAbility != null)
        {
            if (portraitAgent)
            {
                currentCommandPackage = CommandManager.GetClickAbilityPackage(
                    hit, didHit, portraitAgent, currentClickAbility
                );
            }
            else currentCommandPackage = CommandManager.GetClickAbilityPackage(hit, didHit, null, currentClickAbility);

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
            "Player" => CommandManager.GetSelectPlayerPackage(go.GetComponentInParent<WorldAgent>()),
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
        if (!agentInputActive) return;
        if (currentCommandPackage.empty) return;
        else if (currentCommandPackage.type == "click")
        {
            if (!currentClickAbility.valid) InvalidTarget?.Invoke();

            // if this is not the final click of the click ability then return and wait for future clicks
            if (!currentClickAbility.Click()) return;
            currentCommandPackage = CommandManager.GetFinalizedClickAbilityPackage(currentClickAbility);
        }
        else if (currentCommandPackage.type == "select") SelectPlayer(currentCommandPackage.agent);
        if (currentCommandPackage.commands.Count > 0) QueueCurrentPackage();
    }

    private void ProcessHold()
    {
        if (!agentInputActive) return;
        if (currentCommandPackage.type == "move" && modeSwitcher.Get().mode == RoundClock.ProgressMode.RealTime)
        {
            QueueCurrentPackage();
        }
    }

    private void QueueCurrentPackage()
    {
        currentClickAbility = null;
        if (!resourceManager.CanQueuePackage(currentCommandPackage))
        {
            NotEnouchResources?.Invoke();
            return;
        }
        if (!currentCommandPackage.QueueCommands(modeSwitcher.Get().mode))
        {
            NotEnoughAP?.Invoke();
            return;
        }

        foreach (Command command in currentCommandPackage.commands)
        {
            resourceManager.QueueResource(command);
        }

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

    private void UpdateNumberOfAlivePlayers(int number)
    {
        numberOfAlivePlayers += number;
        if (numberOfAlivePlayers <= 0)
        {
            new Locator<Modal>().Get().Prompt(
                "Everyone has died.\nDo you wish to try again?",
                () => { new Locator<SceneChanger>().Get().GoToScene("Tutorial Level"); },
                () => { new Locator<SceneChanger>().Get().GoToScene("MainMenu"); }
            );
        }
    }

    private void SelectAlivePlayer()
    {
        List<WorldAgent> remainingPlayers =
            GetFilteredAgents(
                a => { return !a.dead && a.team == WorldAgent.Team.Player; }
            ).ToList();
        if (remainingPlayers.Any())
        {
            SelectPlayer(remainingPlayers.First());
        }
    }

    public void SetAgentInputActive(bool active)
    {
        agentInputActive = active;
        currentClickAbility = null;
    }

    public void SetClickAbility(ClickAbility clickAbility)
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
            UpdateNumberOfAlivePlayers(1);
            agent.OnDeath += () => UpdateNumberOfAlivePlayers(-1);
            agent.OnRevive += () => UpdateNumberOfAlivePlayers(1);
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
            if (selectedPlayer) selectedPlayer.OnDeath -= SelectAlivePlayer;
            selectedPlayer = playerAgent;
            selectedPlayer.OnDeath += SelectAlivePlayer;
            cameraMover.Get().SetCameraTarget(selectedPlayer.cameraFocusTransform);
        }

#if UNITY_EDITOR
        if (inputManager.Get().KillFlag())
        {
            damageManager.DealDamageEvent(10000, playerAgent);
        }
#endif
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
            selectedPlayer.UndoLastestCommand(resourceManager);
        }
    }

    public List<WorldAgent> GetPlayerAgents() => players;
    public List<WorldAgent> GetAllAgents() => allAgents;
    public WorldAgent GetSelectedPlayer() => selectedPlayer;
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