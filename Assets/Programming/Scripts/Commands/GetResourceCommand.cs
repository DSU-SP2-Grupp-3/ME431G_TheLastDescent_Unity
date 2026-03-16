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

        // check if the object has been queued or already collected
        bool collectedOrQueued = 
            manager.collectedResourceObjects.Union(invokingAgent.ResourceObjectsInQueue()).Contains(resource);

        // if so, set status to invalid to prevent queueing
        if (collectedOrQueued)
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