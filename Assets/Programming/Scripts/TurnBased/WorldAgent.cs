using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldAgent : MonoBehaviour
{
    public enum Team
    {
        Player, Ally, Enemy, Neutral 
    }

    public Team team;

    public Animator animator;
    public NavMeshAgent navMeshAgent;
    public Transform cameraFocusTransform;
    
    /// <summary>
    /// True if this agent should enter into the turn order when turn based mode is activated
    /// </summary>
    private bool active;
    private Locator<ModeSwitcher> modeSwitcher;
        
    private Queue<Command> commandQueue;
    private Command currentlyExecutingCommand;
    private Coroutine currentExecutingCommandCoroutine;
    
    private void Awake()
    {
        commandQueue = new();
        modeSwitcher = new();
        if (team == Team.Player) active = true;
    }

    private void Start()
    {
        modeSwitcher.Get().OnEnterTurnBased += RegisterInTurnManager;
        StartCoroutine(ExecuteCommandQueue());
    }

    private void OnDestroy()
    {
        modeSwitcher.Get().OnEnterTurnBased -= RegisterInTurnManager;
    }

    private void RegisterInTurnManager(TurnManager turnManager)
    {
        if (active)
        {
            turnManager.RegisterAgentInTeam(team, this);
            InterruptCommandQueue();
            
        }
    }

    public void QueueCommand(Command command)
    {
        commandQueue.Enqueue(command);
    }

    public void OverwriteCommand(Command command)
    {
        InterruptCommandQueue();
        commandQueue.Enqueue(command);
        StartCoroutine(ExecuteCommandQueue());
    }

    private void InterruptCommandQueue()
    {
        currentlyExecutingCommand?.Break();
        currentlyExecutingCommand = null;
        StopAllCoroutines();
        commandQueue.Clear();
    }
    
    public IEnumerator ExecuteCommandQueue()
    {
        while (commandQueue.TryDequeue(out Command command))
        {
            currentlyExecutingCommand = command;
            currentExecutingCommandCoroutine = StartCoroutine(command.Execute());
            yield return currentExecutingCommandCoroutine;
            currentExecutingCommandCoroutine = null;
            currentlyExecutingCommand = null;
        }
    }

    // visualise command queue /se
    // can afford new command /se
}