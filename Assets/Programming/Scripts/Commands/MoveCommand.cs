using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MoveCommand : Command, IMoveCommand
{
    public override float apCost
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
    /// <inheritdoc />
    public override float resourceCost => 0f;

    private float costModifier => invokingAgent.localStats.movementCostModifier / invokingAgent.localStats.movement;

    private Vector3 toPosition;
    private Vector3 fromPosition;

    public readonly NavMeshPath agentPath;
    public bool possible { get; private set; }
    public bool noMovement { get; private set; }

    private const float playEndAnimationDistance = 0.5f;
    private const float ignoreMovementDistance = 0.1f;
    private const float interruptTime = 20f;

    public Vector3 ToPosition() => toPosition;

    public MoveCommand(Vector3 fromPosition, Vector3 toPosition, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.toPosition = toPosition;
        this.fromPosition = fromPosition;
        noMovement = Vector3.Distance(toPosition, fromPosition) <= ignoreMovementDistance;
        agentPath = new();
        NavMesh.CalculatePath(fromPosition, toPosition, NavMesh.AllAreas, agentPath);
        possible = agentPath.status == NavMeshPathStatus.PathComplete;
    }

    public MoveCommand(Vector3 toPosition, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.fromPosition = invokingAgent.GetLastMoveCommandToPosition();
        this.toPosition = toPosition;
        noMovement = Vector3.Distance(toPosition, fromPosition) <= ignoreMovementDistance;
        agentPath = new();
        NavMesh.CalculatePath(fromPosition, toPosition, NavMesh.AllAreas, agentPath);
        possible = agentPath.status == NavMeshPathStatus.PathComplete;
    }

    public MoveCommand(NavMeshPath path, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.fromPosition = path.corners[0];
        this.toPosition = path.corners.Last();
        noMovement = Vector3.Distance(toPosition, fromPosition) <= ignoreMovementDistance;
        agentPath = path;
        possible = path.status == NavMeshPathStatus.PathComplete;
    }

    protected override IEnumerator Execute()
    {
        invokingAgent.navMeshAgent.SetPath(agentPath);

        invokingAgent.animator.ResetTrigger("StopMoving");
        invokingAgent.animator.SetTrigger("StartMoving");
        invokingAgent.AnimationEventTriggered += CaptureStepEvent;
        float interrupt = Time.time + interruptTime;

        yield return new WaitUntil(WaitUntilArrivedOrInterrupted(interrupt));

        invokingAgent.AnimationEventTriggered -= CaptureStepEvent;
        invokingAgent.animator.SetTrigger("StopMoving");
        invokingAgent.animator.ResetTrigger("StartMoving");
        invokingAgent.navMeshAgent.ResetPath();
    }

    private Func<bool> WaitUntilArrivedOrInterrupted(float interrupt)
    {
        return () =>
        {
            if (invokingAgent.navMeshAgent.remainingDistance <= playEndAnimationDistance) return true;
            else if (Time.time > interrupt)
            {
                status = Status.Failed;
                return true;
            }
            return false;
        };
    }

    private void CaptureStepEvent(string trigger, GameObject gameObject)
    {
        if (trigger == "step")
        {
            audioManager.PlayAudioEvent(invokingAgent.moveEventName, gameObject);
        }
    }

    public override void VisualizeInQueue(Visualizer visualizer)
    {
        // todo: drawn path will intersect slopes, might have to do raycast between each corner to check for intersections with floor
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
        NavMeshPath remainingPath = new();
        NavMesh.CalculatePath(fromPosition, toPosition, NavMesh.AllAreas, remainingPath);
        visualizer.DrawPreviewPath(remainingPath);
    }

    public override void Break()
    {
        invokingAgent.animator.SetTrigger("StopMoving");
        invokingAgent.animator.ResetTrigger("StartMoving");
        invokingAgent.navMeshAgent.ResetPath();
        invokingAgent.AnimationEventTriggered -= CaptureStepEvent;
    }
}