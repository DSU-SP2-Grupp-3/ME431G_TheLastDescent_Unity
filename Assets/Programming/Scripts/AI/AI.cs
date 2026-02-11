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
        
        //should be removed! forces all commands to be run
        agent.ForceStartCommandQueueExecution();
    }
    
    private void Movement()
    {
        playerPositions = agent.agentManager.Get().GetPlayerPositions();
        
        //sets the ais world navMeshAgent to a new path
        NavMeshPath path = behaviourDefinition.FetchPath(agent.navMeshAgent, playerPositions);
        path = TrimPathToMoveRange(path, agent.localStats.movement);
        
        //create and queue a movecommand using the path and the agent
        MoveCommand aiMovement = new MoveCommand(path, agent); 
        agent.QueueCommand(aiMovement);
        
        
    }

    private NavMeshPath TrimPathToMoveRange(NavMeshPath inputPath, float moveDistance)
    {
        if (agent.navMeshAgent.remainingDistance <= moveDistance)
        {
            return inputPath;
        }
        else
        {
            NavMeshPath trimmedPath = new NavMeshPath();
            //trim path here
            
            return trimmedPath;
        }
    }
    
    private void DealDamage()
    {
        //the gameObject needs to be replaced with the thing that is being harmed, thus i need to find the thing i wish to harm
        agent.agentManager.Get().damageManager.DealDamage(3, gameObject);
    }
}