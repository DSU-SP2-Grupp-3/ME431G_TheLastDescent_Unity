using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GetResourceCommand : Command
{
    public override float apCost => 0f;
    public override float resourceCost => 0f;

    private float amount => resource.amount;
    private ResourceManager manager;

    public Resource resource;

    public GetResourceCommand(
        WorldAgent invokingAgent,
        ResourceManager manager,
        Resource resource
    ) : base(invokingAgent)
    {
        this.manager = manager;
        this.resource = resource;

        // check if the object has been queued in turn based and already collected in realtime
        IEnumerable<Resource> collection = invokingAgent.manager.mode switch
        {
            RoundClock.ProgressMode.RealTime => manager.collectedResourceObjects,
            RoundClock.ProgressMode.TurnBased => invokingAgent.ResourceObjectsInQueue(),
            _ => new List<Resource>()
        };

        // if so set the amount of this command to 0 to prevent exploits when spamclicking a resource
        if (collection.Contains(resource))
        {
            status = Status.Invalid;
        }
    }

    protected override IEnumerator Execute()
    {
        Debug.Log("execute");
        manager.GetResource(resource);
        return null;
    }

    public override void Break()
    {
        manager.RemoveCommands(new Command[] { this });
    }
}