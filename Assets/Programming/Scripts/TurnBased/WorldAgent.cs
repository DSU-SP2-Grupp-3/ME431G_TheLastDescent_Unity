using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldAgent : MonoBehaviour
{
    public enum Team
    {
        Ally, Enemy, Neutral 
    }
    
    public Team team { get; private set; }

    private bool active;
    private Locator<ModeSwitcher> modeSwitcher;
        
    private Queue<Command> commandQueue;
    
    private void Awake()
    {
        commandQueue = new();
        modeSwitcher = new();
        modeSwitcher.Get().OnEnterTurnBased += RegisterInTurnManager;
    }

    private void OnDisable()
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

    public IEnumerator ExecuteCommandQueue()
    {
        while (commandQueue.TryDequeue(out Command command))
        {
            yield return command.Execute();
        }
    }

    // visualise command queue /se
    // can afford new command /se
}