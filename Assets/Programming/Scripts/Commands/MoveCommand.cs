using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MoveCommand : Command
{
    // todo: calculate ap costs via Command.cost and prevent adding commands that would exceed ap cost in turn based
    public override float cost
    {
        get
        {
            float length = 0;
            for (int i = 1; i < agentPath.corners.Length; i++)
            {
                length += (agentPath.corners[i] - agentPath.corners[i - 1]).magnitude;
            }
            return length * invokingAgent.localStats.movementCostModifier;
        }
    }

    private Vector3 toPosition;

    public readonly NavMeshPath agentPath;
    public readonly bool possible;

    private const float playEndAnimationDistance = 0.5f;
    private const float ignoreMovementDistance = 0.1f;

    public MoveCommand(Vector3 toPosition, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.toPosition = toPosition;
        agentPath = new();
        possible = invokingAgent.navMeshAgent.CalculatePath(toPosition, agentPath);
    } 
    
    public MoveCommand(NavMeshPath path, WorldAgent invokingAgent) : base(invokingAgent)
    {
        agentPath = path;
        possible = path.status == NavMeshPathStatus.PathComplete;
    } 

    public override IEnumerator Execute()
    {
        
        // do not do anything if the path is not valid -se
        if (!possible) yield break;
        
        // todo: move animation only plays once for some reason
        
        invokingAgent.navMeshAgent.SetPath(agentPath);
        if (invokingAgent.navMeshAgent.remainingDistance < ignoreMovementDistance) yield break;
        
        invokingAgent.DrawPath(invokingAgent.navMeshAgent.path); // todo: should be moved to visualiser 
        invokingAgent.animator.SetTrigger("StartMoving");
        yield return new WaitUntil(() => invokingAgent.navMeshAgent.remainingDistance <= playEndAnimationDistance);
        invokingAgent.animator.SetTrigger("StopMoving");
    }

    // todo: move visualisation to seperate Visualiser component and use Command.Visualize to draw them
    public override void Visualize()
    {
        
    }

    public override void Break()
    {
        invokingAgent.animator.SetTrigger("StopMoving");
        invokingAgent.navMeshAgent.ResetPath();
    }
}