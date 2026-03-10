using System.Collections;
using UnityEngine;

public class CostCommand : Command
{
    /// <inheritdoc />
    public override float apCost => ap;
    /// <inheritdoc />
    public override float resourceCost => resource;

    private float ap;
    private float resource;

    /// <inheritdoc />
    public CostCommand(WorldAgent invokingAgent, float ap, float resource) : base(invokingAgent)
    {
        this.ap = ap;
        this.resource = resource;
    }

    /// <inheritdoc />
    protected override IEnumerator Execute()
    {
        yield return null;
    }

    /// <inheritdoc />
    public override void Break() { }
}