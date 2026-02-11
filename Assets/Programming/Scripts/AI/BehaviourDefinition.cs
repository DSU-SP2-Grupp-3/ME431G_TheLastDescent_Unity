using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public abstract class BehaviourDefinition : ScriptableObject
{
    public abstract BehaviourCommands GetIdleBehaviourCommands(WorldAgent aiAgent, AI.AIParameters parameters);
    public abstract BehaviourCommands GetActiveBehaviourCommands(WorldAgent aiAgent, AI.AIParameters parameters);
    
    protected NavMeshPath GetPathToNearestAgent(NavMeshAgent agent, List<Vector3> agentPositions)
    {
        //this should return the path to the closest player. it just needs to be fed a navmesh agent and the player itself
        NavMeshPath outputPath = new NavMeshPath();
        NavMeshPath temporaryPath = new NavMeshPath();
        if (agentPositions != null)
        {
            agent.CalculatePath(agentPositions[0], outputPath);
            agent.CalculatePath(agentPositions[1], temporaryPath);
            if (GetPathLength(temporaryPath) < GetPathLength(outputPath))
            {
                outputPath = temporaryPath;
            }
            agent.CalculatePath(agentPositions[2], temporaryPath);
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

    protected float GetPathLength(NavMeshPath path)
    {
        float output = new float();

        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            output += Vector3.Distance(path.corners[i], path.corners[i + 1]);
        }

        return output;
    }
    
    protected NavMeshPath TrimPathToMoveRange(WorldAgent agent, NavMeshPath inputPath, float moveDistance)
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
    
    // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AI.NavMesh.SamplePosition.html
    protected bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            // maybe 1.0f should be range here, could be expensive if range is large then
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
    
    public class BehaviourCommands
    {
        private List<Command> behaviourCommands;

        public BehaviourCommands()
        {
            behaviourCommands = new();
        }

        public void AddCommand(Command command)
        {
            behaviourCommands.Add(command);
        }

        public Command[] GetCommands() => behaviourCommands.ToArray();
    }
}