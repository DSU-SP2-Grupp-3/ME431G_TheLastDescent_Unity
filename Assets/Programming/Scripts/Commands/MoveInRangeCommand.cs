using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MoveInRangeCommand : Command, IMoveCommand
{
    // todo: -se: borde inte vara hårdkodad, borde bero på hur långt man går
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
        // todo: don't think this works if either position is outside navmesh
        // SetDestination seems to still work but can't precalculate path
        // fixing this is not super high priority but if we can fix it before st1 would be nice
        NavMeshPath path = new();
        NavMesh.CalculatePath(fromPosition, toPosition, NavMesh.AllAreas, path);
        visualizer.DrawPath(path, invokingAgent);
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