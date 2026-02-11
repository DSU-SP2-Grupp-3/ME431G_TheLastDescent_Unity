using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [SerializeField]
    private WorldAgent agent;
    [SerializeField]
    private BehaviourDefinition behaviourDefinition;

    [SerializeField]
    private AIParameters parameters;
    
    private Locator<RoundClock> roundClock;
    private Locator<AgentManager> agentManager;

    private List<NavMeshAgent> playerNavMeshes;

    private void Start()
    {
        roundClock = new();
        agentManager = new();
        roundClock.Get().RoundProgressed += RoundUpdate;
        playerNavMeshes = agentManager.Get().GetPlayerNavMeshAgents();
    }

    private void RoundUpdate(int round)
    {
        // todo: perhaps don't do anything at all when very far away from the players, to avoid unnecessary calculations
        if (!agent.active) // perform idle behaviour
        {
            Command[] idleCommands = behaviourDefinition.GetIdleBehaviourCommands(agent, parameters).GetCommands();
            agent.OverwriteQueue(idleCommands);
        }
    }

    private void Update()
    {
        if (!agent.active && CheckIfShouldBeActive())
        {
            Debug.Log("activate");
            agent.Activate();
        }
    }
    
    
    private bool CheckIfShouldBeActive()
    {
        foreach (NavMeshAgent playerNavMesh in playerNavMeshes)
        {
            bool unobstructed = !agent.navMeshAgent.Raycast(playerNavMesh.transform.position, out NavMeshHit hit);
            float distance = (transform.position - playerNavMesh.transform.position).magnitude;
            if (unobstructed && distance < parameters.activationDistance)
            {
                return true;
            }
        }
        
        return false;
    }

    public void GetActiveCommands()
    {
        Debug.Assert(agent.queueEmpty);
        Command[] commands = behaviourDefinition.GetActiveBehaviourCommands(agent, parameters).GetCommands();
        agent.QueueCommands(commands);
    }
   

    [Serializable]
    public struct AIParameters
    {
        public float wanderingRadius;
        public float activationDistance;
    }
}