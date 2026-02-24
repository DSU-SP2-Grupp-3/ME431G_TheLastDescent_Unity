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
    public enum Status
    {
        Pending, // command has not been executed yet, but has been constructed
        Executing, // command is currently being executed
        Successful, // command has stopped executing and completed successfully
        Failed, // command has stopped executing and did not complete successfully
    }
    public Status status { get; set; }
    protected WorldAgent invokingAgent;
    public abstract float cost { get; }
    public IEnumerator ExecuteCommand()
    {
        status = Status.Executing;
        yield return Execute();
        if (status != Status.Failed) status = Status.Successful;
    }
    protected abstract IEnumerator Execute();
    public abstract void Break();
    public virtual void VisualizeInQueue(Visualizer visualizer) { }
    public virtual void VisualizeExecution(Visualizer visualizer) { }
    public virtual void VisualizePreview(Visualizer visualizer) { }

    public Command(WorldAgent invokingAgent)
    {
        this.invokingAgent = invokingAgent;
        status = Status.Pending;
    }

    protected IEnumerator WaitForEndOfAnimation(Animator animator)
    {
        // https://discussions.unity.com/t/wait-until-an-animation-is-finished/699955/6
        yield return new WaitForSeconds(0.1f); // -se: wait short time for animator to enter correct animation
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    }
}