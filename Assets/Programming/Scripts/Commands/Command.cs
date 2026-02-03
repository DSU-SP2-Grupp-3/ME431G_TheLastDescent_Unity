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
    public abstract float cost { get; }
    public abstract IEnumerator Execute();
    public abstract void Break();
    public virtual void Visualize() { }
}