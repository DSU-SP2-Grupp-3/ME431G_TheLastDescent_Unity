using System;
using System.Collections;
using UnityEngine;

public class InvokeActionCommand : Command
{
    public override float cost => 0f;
    
    private Action action;

    public InvokeActionCommand(WorldAgent invokingAgent, Action action) : base(invokingAgent)
    {
        this.action = action;
    }
    
    public override IEnumerator Execute()
    {
        yield return null;
        action.Invoke();
        action = null;
    }
    
    public override void Break() { }
}