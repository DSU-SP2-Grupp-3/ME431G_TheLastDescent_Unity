using System.Collections;
using UnityEngine;

public class CinematicEndCommand : Command
{
    public CinematicEndCommand(WorldAgent invokingAgent) : base(invokingAgent)
    {

    }
    public override IEnumerator Execute()
    {
        new Locator<CinematicKitService>().Get().ClearCinematicScene();
        yield return null;
    }
    public override void Break() { }
    public override float cost { get; }
}
