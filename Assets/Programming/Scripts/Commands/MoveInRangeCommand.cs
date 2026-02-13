using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MoveInRangeCommand : Command, IMoveCommand
{
    public override float cost
    {
        get
        {
            float length = 0;
            for (int i = 1; i < agentPath.corners.Length; i++)
            {
                length += (agentPath.corners[i] - agentPath.corners[i - 1]).magnitude;
            }
            return (length - range) * invokingAgent.localStats.movementCostModifier;
        }
    }

    private Vector3 toPosition;
    private Vector3 fromPosition;
    private float range;

    public readonly NavMeshPath agentPath;
    public readonly bool possible;

    private const float playEndAnimationDistance = 0.5f;
    private const float ignoreMovementDistance = 0.1f;

    public Vector3 ToPosition() => toPosition;

    public MoveInRangeCommand(Vector3 toPosition, float range, WorldAgent invokingAgent) :
        base(invokingAgent)
    {
        this.fromPosition = invokingAgent.GetLastMoveCommandToPosition();
        this.toPosition = toPosition;
        this.range = range;
        agentPath = new();
        NavMesh.CalculatePath(fromPosition, toPosition, NavMesh.AllAreas, agentPath);
        possible = agentPath.status != NavMeshPathStatus.PathInvalid;
    }

    public override IEnumerator Execute()
    {
        // do not do anything if the path is not valid -se
        // if (!possible) yield break;

        // todo: should fix so it calculates a valid path in constructor using from and toPosition, not SetDestination
        invokingAgent.navMeshAgent.SetDestination(toPosition);
        if (WithinDistance())
        {
            invokingAgent.navMeshAgent.ResetPath();
            yield break;
        }

        invokingAgent.animator.SetTrigger("StartMoving");
        yield return new WaitUntil(WithinDistance);
        invokingAgent.animator.SetTrigger("StopMoving");
        invokingAgent.navMeshAgent.ResetPath();
    }

    public override void Visualize(Visualizer visualizer)
    {
        visualizer.DrawPath(agentPath, invokingAgent);
    }

    public override void Break()
    {
        invokingAgent.animator.SetTrigger("StopMoving");
        invokingAgent.navMeshAgent.CalculatePath(invokingAgent.navMeshAgent.transform.position, agentPath);
        invokingAgent.navMeshAgent.SetPath(agentPath);
    }

    private bool WithinDistance()
    {
        float distanceToTarget = (toPosition - invokingAgent.transform.position).sqrMagnitude;
        return distanceToTarget <= range * range;
    }
}