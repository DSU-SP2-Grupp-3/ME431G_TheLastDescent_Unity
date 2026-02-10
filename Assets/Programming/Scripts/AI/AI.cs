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
    }

    private void Movement()
    {
        //sets the ais agent movement
        NavMeshPath path = behaviourDefinition.FetchPath(agent.navMeshAgent, playerPositions);
        MoveCommand aiMovement = new MoveCommand(path, agent); 
        agent.QueueCommand(aiMovement);
    }
}