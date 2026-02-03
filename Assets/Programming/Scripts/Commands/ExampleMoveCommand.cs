using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

// -se:
/// <summary>
/// Example of a move command. To issue a command just instantiate a MoveCommand with new, pass in the relevant
/// references and then pass the instance to the WorldAgent's QueueCommand() method.
/// </summary>
public class ExampleMoveCommand : Command
{
    /// <inheritdoc />
    public override float cost => 0f;

    private Vector3 position;
    private NavMeshAgent agent;
    private Animator animator;

    public ExampleMoveCommand(Vector3 toPosition, NavMeshAgent movingAgent, Animator agentAnimator)
    {
        position = toPosition;
        agent = movingAgent;
        animator = agentAnimator;
    }

    /// <inheritdoc />
    public override IEnumerator Execute()
    {
        // set agent destination
        // trigger 'startmoving' animation on animator
        // yield return wait until agent has arrived at position
        // trigger 'stopmoving' animation on animator
        // yield return wait until 'stopmoving' animation is finished

        yield return null;
    }

    public override void Break()
    {
        // stop agent movement
        // trigger 'stopmoving' animation on animator
    }
}