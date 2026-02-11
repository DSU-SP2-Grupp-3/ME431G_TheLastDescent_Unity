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
        if (!agent.active) // perform idle behaviour
        {
            Command[] idleCommands = behaviourDefinition.GetIdleBehaviourResults(agent).GetCommands();
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
    
    private void Movement()
    {
        playerPositions = agent.agentManager.Get().GetPlayerPositions();
        
        //sets the ais world navMeshAgent to a new path
        NavMeshPath path = behaviourDefinition.FetchPath(agent.navMeshAgent, playerPositions);
        path = TrimPathToMoveRange(path, agent.localStats.movement);
        
        //create and queue a movecommand using the path and the agent
        MoveCommand aiMovement = new MoveCommand(path, agent); 
        agent.OverwriteQueue(aiMovement);
        
        
    }

    private bool CheckIfShouldBeActive()
    {
        foreach (NavMeshAgent playerNavMesh in playerNavMeshes)
        {
            bool unobstructed = !agent.navMeshAgent.Raycast(playerNavMesh.transform.position, out NavMeshHit hit);
            float distance = (transform.position - playerNavMesh.transform.position).magnitude;
            if (unobstructed && distance < behaviourDefinition.activationDistance)
            {
                return true;
            }
        }
        
        return false;
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
            float accumulatedDistance = 0;
            int expensiveCorner = 0;
            float remainingDistance = 0;
            //this loop should go through the list and set the expensiveCorner int to the corner that it is too expensive to path to
            for (int i = 1; i < inputPath.corners.Length; i++)
            {
                accumulatedDistance += Vector3.Distance(inputPath.corners[i-1],inputPath.corners[i]);
                if (accumulatedDistance > moveDistance)
                {
                    accumulatedDistance -= Vector3.Distance(inputPath.corners[i-1], inputPath.corners[i]) ;
                    remainingDistance = agent.localStats.movement - accumulatedDistance;
                    expensiveCorner = i;
                    break;
                }
            }

            float distanceRatio = remainingDistance / Vector3.Distance(inputPath.corners[expensiveCorner - 1], inputPath.corners[expensiveCorner]);
            //x3 = x1 + t(x2-x1), t=d/D
            Vector3 newDestination = 
                new Vector3(
                    inputPath.corners[expensiveCorner - 1].x + (distanceRatio * (inputPath.corners[expensiveCorner].x - inputPath.corners[expensiveCorner - 1].x)),
                    inputPath.corners[expensiveCorner - 1].y + (distanceRatio * (inputPath.corners[expensiveCorner].y - inputPath.corners[expensiveCorner - 1].y)),
                    inputPath.corners[expensiveCorner - 1].z + (distanceRatio * (inputPath.corners[expensiveCorner].z - inputPath.corners[expensiveCorner - 1].z))
                            );
            
            agent.navMeshAgent.CalculatePath(newDestination, trimmedPath);
            
            return trimmedPath;
        }
    }
    
    private void DealDamage()
    {
        //the gameObject needs to be replaced with the thing that is being harmed, thus i need to find the thing i wish to harm
        agent.agentManager.Get().damageManager.DealDamage(damageAmount, gameObject);
    }
}