using System.Collections;
using UnityEngine;

public class GetResourceCommand : Command
{
    public override float cost { get; }

    private float amount;
    private ResourceManager manager;

    public GetResourceCommand(WorldAgent invokingAgent, ResourceManager manager, float amount) : base(invokingAgent)
    {
        this.amount = amount;
        this.manager = manager;
    }

    protected override IEnumerator Execute()
    {
        manager.GetResource(amount);
        yield return null;
    }

    public override void Break() { }
}