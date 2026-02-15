using System.Collections;
using UnityEngine;

public class CinematicStartCommand : Command
{
    private int[] actorIds;
    public CinematicStartCommand(int[] actorIds, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.actorIds = actorIds;
    }
    public override IEnumerator Execute()
    {
        yield return new WaitUntil(() => new Locator<CinematicKitService>().Get().FindActors(actorIds));
    }
    public override void Break() { }
    public override float cost { get; }
}
