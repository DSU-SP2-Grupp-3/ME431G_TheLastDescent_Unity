using System.Collections;
using UnityEngine;

public class CinematicEndCommand : Command
{
    public CinematicEndCommand(WorldAgent invokingAgent) : base(invokingAgent)
    {

    }
    protected override IEnumerator Execute()
    {
        new Locator<CinematicKitService>().Get().ClearCinematicScene();
        yield return null;
    }
    public override void Break() { }
    public override float apCost { get; }
    /// <inheritdoc />
    public override float resourceCost => 0f;
}
