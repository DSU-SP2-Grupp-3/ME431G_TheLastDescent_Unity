using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MoveCommand : Command
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
            return length * costPerUnit;
        }
    }

    private NavMeshAgent navMeshAgent;
    private Vector3 toPosition;
    private Animator animator;
    private float costPerUnit;
    
    public readonly NavMeshPath agentPath;
    public readonly bool possible;

    private const float playEndAnimationDistance = 0.5f;
    
    public MoveCommand(Vector3 toPosition, WorldAgent agent, float costPerUnit = 1f)
    {
        this.toPosition = toPosition;
        navMeshAgent = agent.navMeshAgent;
        animator = agent.animator;
        agentPath = new();
        possible = navMeshAgent.CalculatePath(toPosition, agentPath);
    }

    public override IEnumerator Execute()
    {
        // do not do anything if the path is not valid -se
        if (!possible) yield return null;

        navMeshAgent.SetPath(agentPath);
        animator.SetTrigger("StartMoving");
        yield return new WaitUntil(() => navMeshAgent.remainingDistance <= playEndAnimationDistance);
        animator.SetTrigger("StopMoving");

    }

    public override void Visualize()
    {
        // lineRenderer.SetPositions(path.corners);
    }

    public override void Break()
    {
        animator.SetTrigger("StopMoving");
        navMeshAgent.CalculatePath(navMeshAgent.transform.position, agentPath);
        navMeshAgent.SetPath(agentPath);
    }
}