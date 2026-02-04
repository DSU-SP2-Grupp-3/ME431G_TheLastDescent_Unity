using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MoveCommand : Command
{
    // todo: -se: borde inte vara hårdkodad, borde bero på hur långt man går
    public override float cost => 1f;

    private NavMeshAgent agent;
    private Vector3 toPosition;
    public NavMeshPath agentPath => agent.path;

    public MoveCommand(Vector3 toPosition, NavMeshAgent agent)
    {
        this.toPosition = toPosition;
        this.agent = agent;
        // todo: probably get reference to animator too
    }

    /// <inheritdoc />
    public override IEnumerator Execute()
    {
        
        agent.CalculatePath()
    }
    /// <inheritdoc />
    public override void Break()
    {

    }
}