using UnityEngine;

public class GetResourcesCommandWrapper : CommandWrapper
{
    [SerializeField]
    private float resourceAmount;
    [SerializeField]
    private ResourceManager resourceManager;

    /// <inheritdoc />
    public override Command UnwrapCommand(WorldAgent agent)
    {
        return new GetResourceCommand(agent, resourceManager, resourceAmount);
    }
}