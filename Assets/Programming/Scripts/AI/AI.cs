using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [SerializeField]
    private WorldAgent agent;
    [SerializeField]
    private BehaviourDefinition behaviourDefinition;

    private List<Vector3> playerPositions;

    

    private void Update()
    {
        Movement();
        DealDamage();
    }

    private void DealDamage()
    {
        agent.agentManager.Get().damageManager.DealDamage(3, gameObject);
    }
    private void Movement()
    {
        playerPositions = agent.agentManager.Get().GetPlayerPositions();
        //sets the ais agent movement
        NavMeshPath path = behaviourDefinition.FetchPath(agent.navMeshAgent, playerPositions);
        MoveCommand aiMovement = new MoveCommand(path, agent); 
        agent.QueueCommand(aiMovement);
        agent.ForceStartCommandQueueExecution();
    }

    private NavMeshPath TrimPathToMoveRange()
    {
        NavMeshPath trimmedPath = new NavMeshPath();
        
        return trimmedPath;
    }
}