using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MoveInRangeCommand : Command, IMoveCommand
{
    public override float apCost
    {
        get
        {
            float length = 0;
            if (agentPath.corners.Length == 0) return length;
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
    private float range;

    public readonly NavMeshPath agentPath;
    public bool possible { get; private set; }
    public bool noMovement { get; private set; }

    private const float playEndAnimationDistance = 0.5f;
    private const float ignoreMovementDistance = 0.1f;
    private const float interruptTime = 20f;
    
    private const float sampleRadiusStepSize = 0.1f;
    private const float sampleAngleStepSize = 10f * Mathf.Deg2Rad;
    private const float samplePointRange = 0.2f;
    private const float samplePathStepSize = 0.2f;

    private WorldAgent lineOfSightTarget;

    public Vector3 ToPosition() => toPosition;

    public MoveInRangeCommand(Vector3 toPosition, float range, WorldAgent invokingAgent, WorldAgent lineOfSightTarget) :
        base(invokingAgent)
    {
        fromPosition = invokingAgent.GetLastMoveCommandToPosition();
        this.range = range;
        this.lineOfSightTarget = lineOfSightTarget;
        
        agentPath = new();
        NavMeshPath candidatePath = new();
        
        // search points within radius, find the point inside the radius with shortest complete path to agent
        for (float sampleRadius = range; sampleRadius > 0; sampleRadius -= sampleRadiusStepSize)
        {
            for (float angle = 0; angle < Mathf.PI * 2; angle += sampleAngleStepSize)
            {
                Vector3 delta = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * sampleRadius;
                Vector3 samplePosition = toPosition + delta;
                NavMesh.CalculatePath(fromPosition, samplePosition, NavMesh.AllAreas, candidatePath);
                if (candidatePath.status != NavMeshPathStatus.PathComplete) continue;
                if (lineOfSightTarget && TargetObstructed(candidatePath.corners.Last())) continue;
                if (agentPath.status == NavMeshPathStatus.PathInvalid || PathLength(candidatePath) < PathLength(agentPath))
                {
                    NavMesh.CalculatePath(fromPosition, samplePosition, NavMesh.AllAreas, agentPath);
                }
            }
            // if a valid path has been found, don't check smaller radiuses
            if (agentPath.status == NavMeshPathStatus.PathComplete) break;
        }
        
        if (agentPath.status == NavMeshPathStatus.PathInvalid)
        {
            possible = false;
            return;
        }
        
        possible = true;
        this.toPosition = agentPath.corners.Last();
        float distance = (fromPosition - toPosition).magnitude;
        noMovement = distance <= range;
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
        invokingAgent.AnimationEventTriggered += CaptureStepEvent;

        yield return new WaitUntil(ArrivedOrInterrupted(interrupt));

        invokingAgent.AnimationEventTriggered -= CaptureStepEvent;
        invokingAgent.animator.SetTrigger("StopMoving");
        invokingAgent.animator.ResetTrigger("StartMoving");
        invokingAgent.navMeshAgent.ResetPath();
    }

    private void CaptureStepEvent(string trigger, GameObject gameObject)
    {
        if (trigger == "step")
        {
            audioManager.PlayAudioEvent("Footstep", gameObject);
        }
    }

    private Func<bool> ArrivedOrInterrupted(float interrupt)
    {
        return () =>
        {
            if (lineOfSightTarget)
            {
                if (invokingAgent.navMeshAgent.Raycast(toPosition, out NavMeshHit hit)) return false;
                if (invokingAgent.navMeshAgent.remainingDistance < range) return true;
            }
            else if (invokingAgent.navMeshAgent.remainingDistance < playEndAnimationDistance) return true;
            else if (Time.time > interrupt)
            {
                status = Status.Failed;
                return true;
            }
            return false;
        };
    }

    private float PathLength(NavMeshPath path)
    {
        float length = 0f;
        for (int i = 1; i < path.corners.Length; i++)
        {
            length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return length;
    }

    private bool TargetObstructed(Vector3 from)
    {
        return Physics.Linecast(
            from, 
            lineOfSightTarget.transform.position, 
            LayerMask.GetMask("Environment")
        );
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
    

    public override void Break()
    {
        invokingAgent.animator.SetTrigger("StopMoving");
        invokingAgent.animator.ResetTrigger("StartMoving");
        invokingAgent.navMeshAgent.ResetPath();
        invokingAgent.AnimationEventTriggered -= CaptureStepEvent;
    }
}