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

    [Tooltip("Temporary!! how much damage it deals when it attacks")] public int damageAmount;
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
            int expensiveCorner = 0;
            float accumulatedDistance = 0;
            float remainingDistance = 0;
            //this loop should go through the list and set the expensiveCorner int to the corner that it is too expensive to path to
            for (int i = 0; i < agent.navMeshAgent.path.corners.Length - 1; i++)
            {
                accumulatedDistance += Vector3.Distance(agent.navMeshAgent.path.corners[i], agent.navMeshAgent.path.corners[i + 1]);
                if (accumulatedDistance > moveDistance)
                {
                    accumulatedDistance -= Vector3.Distance(agent.navMeshAgent.path.corners[i], agent.navMeshAgent.path.corners[i + 1]) ;
                    remainingDistance = agent.localStats.movement - accumulatedDistance;
                    expensiveCorner = i + 1;
                    break;
                }
            }
            
            
            
            return trimmedPath;
        }
    }
    
    private void DealDamage()
    {
        //the gameObject needs to be replaced with the thing that is being harmed, thus i need to find the thing i wish to harm
        agent.agentManager.Get().damageManager.DealDamage(damageAmount, gameObject);
    }
}