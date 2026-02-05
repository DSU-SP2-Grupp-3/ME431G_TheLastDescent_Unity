using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldAgent : MonoBehaviour
{
    public enum Team
    {
        Ally, Enemy, Neutral 
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
        if (active) turnManager.RegisterAgentInTeam(team, this);
    }

    public void QueueCommand(Command command)
    {
        commandQueue.Enqueue(command);
    }

    public void OverwriteCommand(Command command)
    {
        currentlyExecutingCommand?.Break();
        currentlyExecutingCommand = null;
        if (currentExecutingCommandCoroutine != null) StopCoroutine(currentExecutingCommandCoroutine);
        currentExecutingCommandCoroutine = null;
        commandQueue.Clear();
        commandQueue.Enqueue(command);
        StartCoroutine(ExecuteCommandQueue());
    }

    public IEnumerator ExecuteCommandQueue()
    {
        while (true)
        {
            if (commandQueue.TryDequeue(out Command command))
            {
                currentlyExecutingCommand = command;
                currentExecutingCommandCoroutine = StartCoroutine(command.Execute());
                yield return currentExecutingCommandCoroutine;
                currentExecutingCommandCoroutine = null;
                currentlyExecutingCommand = null;
            }
            else
            {
                yield return null;
            }
        }
    }

    // visualise command queue /se
    // can afford new command /se
}