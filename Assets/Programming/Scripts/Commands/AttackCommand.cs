using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Command
{
    public override float cost { get; }

    public override IEnumerator Execute()
    {
        //not implemented
        
        yield break;
    }

    public override void Break()
    {
        
    }
    public override void Visualize() { }

    public AttackCommand(WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.invokingAgent = invokingAgent;
    }
}
