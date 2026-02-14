using System.Collections;
using UnityEngine;

// -se:
/// <summary>
/// A command represents a unit of action for a world agent. If it seems reasonable to break down a command into smaller 
/// commands, do so. A command should represent an action that cannot be split up more. A command that involves moving
/// and then shooting should thus actually be two commands given in sequence, one for moving and then one for shooting.
/// </summary>
public abstract class Command
{
    protected WorldAgent invokingAgent;
    public abstract float cost { get; }
    public abstract IEnumerator Execute();
    public abstract void Break();
    public virtual void VisualizeInQueue(Visualizer visualizer) { }
    public virtual void VisualizeExecution(Visualizer visualizer) { }

    public Command(WorldAgent invokingAgent)
    {
        this.invokingAgent = invokingAgent;
    }

    protected IEnumerator WaitForEndOfAnimation(Animator animator)
    {
        // https://discussions.unity.com/t/wait-until-an-animation-is-finished/699955/6
        yield return new WaitForSeconds(0.1f); // -se: wait short time for animator to enter correct animation
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    }
}