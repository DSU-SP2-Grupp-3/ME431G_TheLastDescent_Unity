using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewBehaviourDefintion", menuName = "AI/Behaviour Definition", order = 0)]
public class BehaviourDefinition : ScriptableObject
{
    [SerializeField]
    private Stats stats;
    [SerializeField] private AIBehaviourType behaviourType;

    public float attackRange => stats.attackRange;
    public float hitPoints => stats.hitPoints;
    public float movement => stats.movement;
    public float activationDistance => stats.activationDistance;

    public AIBehaviourType.BehaviourResults GetIdleBehaviourResults(WorldAgent agent)
    {
        return behaviourType.GetIdleBehaviourResults(stats, agent);
    }

    public AIBehaviourType.BehaviourResults GetActiveBehaviourResults(WorldAgent agent)
    {
        return behaviourType.GetActiveBehaviourResults(stats, agent);
    }
    
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

    [Serializable]
    public struct Stats
    {
        public float attackRange, hitPoints, movement;
        [Tooltip("The distance within which if the agent can see a player it should register itself with the turn manager")]
        public float activationDistance;
        [Min(0)] public float minDamage;
        [Min(1)] public float maxDamage;
        public float wanderingRadius;
    }
}