using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MoveCommand : Command, IMoveCommand
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
            return length * costModifier;
        }
    }

    private float costModifier => invokingAgent.localStats.movementCostModifier / invokingAgent.localStats.movement;

    private Vector3 toPosition;
    private Vector3 fromPosition;

    public readonly NavMeshPath agentPath;
    public readonly bool possible;

    private const float playEndAnimationDistance = 0.5f;
    private const float ignoreMovementDistance = 0.1f;

    public Vector3 ToPosition() => toPosition;

    public MoveCommand(Vector3 fromPosition, Vector3 toPosition, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.toPosition = toPosition;
        this.fromPosition = fromPosition;
        agentPath = new();
        NavMesh.CalculatePath(fromPosition, toPosition, NavMesh.AllAreas, agentPath);
        possible = agentPath.status == NavMeshPathStatus.PathComplete;
    }

    public MoveCommand(Vector3 toPosition, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.fromPosition = invokingAgent.GetLastMoveCommandToPosition();
        this.toPosition = toPosition;
        agentPath = new();
        NavMesh.CalculatePath(fromPosition, toPosition, NavMesh.AllAreas, agentPath);
        possible = agentPath.status == NavMeshPathStatus.PathComplete;
    }

    public MoveCommand(NavMeshPath path, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.fromPosition = path.corners[0];
        this.toPosition = path.corners.Last();
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

        //Visualize();

        invokingAgent.animator.SetTrigger("StartMoving");
        yield return new WaitUntil(() => invokingAgent.navMeshAgent.remainingDistance <= playEndAnimationDistance);
        invokingAgent.animator.SetTrigger("StopMoving");
    }

    public override void Visualize(Visualizer visualizer)
    {
        // visualizer.DrawPath(agentPath, invokingAgent);
    }

    public override void Break()
    {
        // todo: rework this so stop moving is not triggered when being overwritten with another move command
        // causes slight visual bug when rapidly sending move commands and sometimes makes it so stopmoving is set 
        // when the idle animation starts, causing the move animation to be immediately stopped on the next move command
        invokingAgent.animator.SetTrigger("StopMoving");
        invokingAgent.navMeshAgent.ResetPath();
    }
}