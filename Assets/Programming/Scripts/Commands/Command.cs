using System.Collections;
using UnityEngine;

public abstract class Command
{
    public abstract float cost { get; }
    public abstract IEnumerator Execute();
    public virtual void Visualize() { }
}