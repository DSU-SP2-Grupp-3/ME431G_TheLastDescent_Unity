using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GetResourceCommand : Command
{
    public override float apCost { get; }
    public override float resourceCost => -amount;

    private float amount;
    private ResourceManager manager;

    public GameObject resourceObject;

    public GetResourceCommand(
        WorldAgent invokingAgent,
        ResourceManager manager,
        float amount,
        GameObject resourceObject
    ) : base(invokingAgent)
    {
        this.amount = amount;
        this.manager = manager;
        this.resourceObject = resourceObject;

        // check if the object has been queued in turn based and already collected in realtime
        IEnumerable<GameObject> collection = invokingAgent.manager.mode switch
        {
            RoundClock.ProgressMode.RealTime => manager.collectedResourceObjects,
            RoundClock.ProgressMode.TurnBased => invokingAgent.ResourceObjectsInQueue()
        };

        // if so set the amount of this command to 0 to prevent exploits when spamclicking a resource
        if (collection.Contains(resourceObject)) this.amount = 0f;

    }

    protected override IEnumerator Execute()
    {
        // todo: can click multiple times on one resource to get a bunch of resources
        manager.PayResource(this, resourceObject);
        return null;
    }

    public override void Break()
    {
        manager.RemoveCommands(new Command[] { this });
    }
}