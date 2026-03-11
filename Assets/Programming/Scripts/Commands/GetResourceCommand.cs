using System.Collections;
using UnityEngine;

public class GetResourceCommand : Command
{
    public override float apCost { get; }
    public override float resourceCost => -amount;

    private float amount;
    private ResourceManager manager;

    public GetResourceCommand(WorldAgent invokingAgent, ResourceManager manager, float amount) : base(invokingAgent)
    {
        this.amount = amount;
        this.manager = manager;
    }

    protected override IEnumerator Execute()
    {
        // todo: can click multiple times on one resource to get a bunch of resources
        manager.PayResource(this);
        yield return null;
    }

    public override void Break()
    {
        manager.RemoveCommands(new Command[] { this });
    }
}