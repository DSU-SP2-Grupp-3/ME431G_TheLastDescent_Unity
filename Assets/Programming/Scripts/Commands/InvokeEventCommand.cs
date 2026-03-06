using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InvokeEventCommand : Command
{
    public override float apCost => 0f;
    /// <inheritdoc />
    public override float resourceCost => 0f;

    private UnityEvent unityEvent;

    public InvokeEventCommand(WorldAgent invokingAgent, UnityEvent unityEvent) : base(invokingAgent)
    {
        this.unityEvent = unityEvent;
    }

    protected override IEnumerator Execute()
    {
        yield return null;
        unityEvent.Invoke();
        unityEvent = null;
    }
    
    public override void Break() { }
}