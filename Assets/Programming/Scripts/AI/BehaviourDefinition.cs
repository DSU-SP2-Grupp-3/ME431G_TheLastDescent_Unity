using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewBehaviourDefintion", menuName = "AI/Behaviour Definition", order = 0)]
public class BehaviourDefinition : ScriptableObject
{
    [SerializeField] private float attackRange, hitPoints, movement;
    [SerializeField] [Min(0)] private float minDamage;
    [SerializeField] [Min(1)] private float maxDamage;
    [SerializeField] private AIBehaviourType behaviourType;

    #region Fetch Nearest Player
    public NavMeshPath FetchPath(NavMeshAgent agent, List<Vector3> playerPositions)
    {
        //this should return the path to the closest player. it just needs to be fed a navmesh agent and the player itself
        NavMeshPath outputPath = new NavMeshPath();
        NavMeshPath temporaryPath = new NavMeshPath();
        if (playerPositions != null)
        {
            agent.CalculatePath(playerPositions[0], outputPath);
            agent.CalculatePath(playerPositions[1], temporaryPath);
            if (GetPathLength(temporaryPath) < GetPathLength(outputPath))
            {
                outputPath = temporaryPath;
            }
            agent.CalculatePath(playerPositions[2], temporaryPath);
            if (GetPathLength(temporaryPath) < GetPathLength(outputPath))
            {
                outputPath = temporaryPath;
            }
            //for (int i = 1; i < playerPositions.Count; i++)
            //{
            //    agent.CalculatePath(playerPositions[i], temporaryPath);
            //    if (GetPathLength(outputPath) > GetPathLength(temporaryPath))
            //    {
            //        outputPath = temporaryPath;
            //    }
            //}
        }

        return outputPath;
    }

    private float GetPathLength(NavMeshPath path)
    {
        float output = new float();

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            output += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return output;
    }
    #endregion
    
}