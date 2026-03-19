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
    private const float samplePointStepSize = 0.1f;
    private const float samplePathStepSize = 0.2f;

    private WorldAgent lineOfSightTarget;

    public Vector3 ToPosition() => toPosition;

    public MoveInRangeCommand(Vector3 toPosition, float range, WorldAgent invokingAgent) :
        base(invokingAgent)
    {
        fromPosition = invokingAgent.GetLastMoveCommandToPosition();
        float distance = Vector3.Distance(fromPosition, toPosition);
        noMovement = distance <= range;
        this.range = range;
        agentPath = new();
        
        Vector3 toFrom = (fromPosition - toPosition).normalized;
        float sampleLength = 0f;
        
        // find a valid path as close to the target as possible
        // alternatively these points could be user defined by the move in range command wrapper
        // check points between to and from position until a path is found
        while (sampleLength <= distance && agentPath.status == NavMeshPathStatus.PathInvalid)
        {
            // a point at distance sampleLength from toPosition toward fromPosition
            Vector3 samplePosition = toPosition + toFrom * sampleLength;
            if (NavMesh.SamplePosition(samplePosition, out NavMeshHit hit, 0f, NavMesh.AllAreas))
            {
                // if this point is on the NavMesh, calculate a path to it and break
                // if the target is on the NavMesh this should be the same as calculating the path between from and to position
                NavMesh.CalculatePath(fromPosition, hit.position, NavMesh.AllAreas, agentPath);
                break;
            }
            
            sampleLength += samplePointStepSize;
        }

        // if no point between to and from is valid this command is invalid and movement is not possible
        if (agentPath.status == NavMeshPathStatus.PathInvalid)
        {
            possible = false;
            status = Status.Invalid;
            return;
        }
        
        // calculate point in this path that is nearest from position while still within range from toPosition
        // if the distance from the end of this path is farther than range from toPosition, then the move is impossible
        
        // invert path for sampling (NavMesh.CalculatePath(agentPath.corners.Last, agentPath.corners[0], ...)
        // use inverted path and samplePathStepSize to move toward fromPosition
        // whenever the sampled path position is further away from toPosition than range, the previously sampled position is our target
        // recalculate agentPath to the target
        
        
        
        
        // NavMesh.CalculatePath(fromPosition, toPosition, NavMesh.AllAreas, agentPath);
        // if (agentPath.status != NavMeshPathStatus.PathComplete)
        // {
        //     if (!FindCompletePath(fromPosition, toPosition, ref agentPath))
        //     {
        //         possible = false;
        //         return;
        //     }
        // }
        // float lackingDistance = Vector3.Distance(agentPath.corners.Last(), toPosition);
        // possible = agentPath.status != NavMeshPathStatus.PathInvalid || lackingDistance > range;
        //
        // if (possible)
        // {
        //     TrimToLength(ref agentPath, range, toPosition);
        //     this.toPosition = agentPath.corners.Last();
        // }
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

    public void SetLineOfSightTarget(WorldAgent target)
    {
        lineOfSightTarget = target;
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