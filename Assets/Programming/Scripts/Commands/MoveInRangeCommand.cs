using System;
using System.Collections;
using System.Linq;
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
            return length * costModifier;
        }
    }

    private float costModifier => invokingAgent.localStats.movementCostModifier / invokingAgent.localStats.movement;

    private Vector3 toPosition;
    private Vector3 fromPosition;
    private float range;

    public readonly NavMeshPath agentPath;
    public bool possible { get; private set; }
    public bool noMovement { get; private set; }

    private const float playEndAnimationDistance = 0.5f;
    private const float ignoreMovementDistance = 0.1f;
    private const float interruptTime = 5f;
    private const int trimSampleResoltion = 5;
    private const int findCompletePathIterations = 10;

    public Vector3 ToPosition() => toPosition;

    public MoveInRangeCommand(Vector3 toPosition, float range, WorldAgent invokingAgent) :
        base(invokingAgent)
    {
        fromPosition = invokingAgent.GetLastMoveCommandToPosition();
        noMovement = Vector3.Distance(fromPosition, toPosition) <= range;
        this.range = range;
        agentPath = new();
        NavMesh.CalculatePath(fromPosition, toPosition, NavMesh.AllAreas, agentPath);
        if (agentPath.status != NavMeshPathStatus.PathComplete)
        {
            if (!FindCompletePath(fromPosition, toPosition, ref agentPath))
            {
                possible = false;
                return;
            }
        }
        float lackingDistance = Vector3.Distance(agentPath.corners.Last(), toPosition);
        possible = agentPath.status != NavMeshPathStatus.PathInvalid || lackingDistance > range;
        if (possible)
        {
            TrimToLength(ref agentPath, range, toPosition);
            this.toPosition = agentPath.corners.Last();
        }
    }

    protected override IEnumerator Execute()
    {
        if (agentPath == null)
        {
            status = Status.Failed;
            yield break;
        }
        invokingAgent.navMeshAgent.SetPath(agentPath);

        invokingAgent.animator.ResetTrigger("StopMoving");
        invokingAgent.animator.SetTrigger("StartMoving");
        float interrupt = Time.time + interruptTime;
        yield return new WaitUntil(WaitUntilArrivedOrInterrupted(interrupt));
        invokingAgent.animator.SetTrigger("StopMoving");
        invokingAgent.animator.ResetTrigger("StartMoving");
        invokingAgent.navMeshAgent.ResetPath();
    }

    private Func<bool> WaitUntilArrivedOrInterrupted(float interrupt)
    {
        return () =>
        {
            if (invokingAgent.navMeshAgent.remainingDistance < playEndAnimationDistance) return true;
            else if (Time.time > interrupt)
            {
                status = Status.Failed;
                return true;
            }
            return false;
        };
    }

    public override void VisualizeInQueue(Visualizer visualizer)
    {
        visualizer.AppendQueuedPath(agentPath, invokingAgent);
    }

    public override void VisualizeExecution(Visualizer visualizer)
    {
        NavMeshPath remainingPath = new();
        NavMesh.CalculatePath(invokingAgent.navMeshAgent.nextPosition, toPosition, NavMesh.AllAreas, remainingPath);
        visualizer.DrawExecutingPath(remainingPath, invokingAgent);
    }

    public override void VisualizePreview(Visualizer visualizer)
    {
        visualizer.DrawPreviewPath(agentPath);
    }

    private void TrimToLength(ref NavMeshPath path, float minDistance, Vector3 target)
    {
        if (path.corners.Length < 2) return;
        // starting from the target, check which point first exists min distance
        for (int i = path.corners.Length - 1; i >= 0; i--)
        {
            float distance = Vector3.Distance(path.corners[i], target);
            if (distance > minDistance)
            {
                // sample along last corner pair to see if a complete path can be drawn closer to target position
                float tIncrement = 1f / trimSampleResoltion;
                for (float t = 0f; t < 1; t += tIncrement)
                {
                    Vector3 samplePoint = Vector3.Lerp(path.corners[i], path.corners[i + 1], t);
                    distance = Vector3.Distance(samplePoint, target);
                    if (distance <= minDistance)
                    {
                        NavMesh.CalculatePath(path.corners[0], samplePoint, NavMesh.AllAreas, path);
                        return;
                    }
                }
            }
        }
    }

    private bool FindCompletePath(Vector3 from, Vector3 to, ref NavMeshPath path)
    {
        NavMeshPath candidatePath = new();
        float tIncrement = 1f / findCompletePathIterations;
        for (float t = 0f; t < 1f; t += tIncrement)
        {
            Vector3 linearSample = Vector3.Lerp(to, from, t);
            if (NavMesh.SamplePosition(linearSample, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                NavMesh.CalculatePath(from, hit.position, NavMesh.AllAreas, candidatePath);
                if (candidatePath.status == NavMeshPathStatus.PathComplete)
                {
                    path = candidatePath;
                    return true;
                }
            }
        }
        return false;
    }

    public override void Break()
    {
        invokingAgent.animator.SetTrigger("StopMoving");
        invokingAgent.animator.ResetTrigger("StartMoving");
        invokingAgent.navMeshAgent.ResetPath();
    }
}