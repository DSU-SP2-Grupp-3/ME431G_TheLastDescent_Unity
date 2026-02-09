using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField]
    private WorldAgent agent;
    [SerializeField]
    private BehaviourDefinition behaviourDefinition;

    private List<Vector3> playerPositions;
    private void Movement()
    { 
        agent.navMeshAgent.SetPath(behaviourDefinition.FetchNearestPlayer(agent.navMeshAgent, playerPositions));
    }
}