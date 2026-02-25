using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CinematicLookAtCommand : Command
{
    private List<CinematicLookAtInfo> cinematicLookAtInfos;
    private Locator<CinematicKitService> cinematicKitLocator;
    public CinematicLookAtCommand(List<CinematicLookAtInfo> cinematicLookAtInfos, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.cinematicLookAtInfos = cinematicLookAtInfos;
    }
    public override IEnumerator Execute()
    {
        List<IEnumerator> enumerator = new();
        cinematicKitLocator = new();
        Locator<TurnManager> turnManagerLocator = new();
        foreach(CinematicLookAtInfo agentinfo in cinematicLookAtInfos)
        {
            WorldAgent StartAgent = cinematicKitLocator.Get().GetActor(agentinfo.IDStart);
            WorldAgent EndAgent = cinematicKitLocator.Get().GetActor(agentinfo.IDStart);
            enumerator.Add(StartAgent.OverwriteQueueIEnumerator(new LookAtCommand(StartAgent, EndAgent)));
        }
        yield return turnManagerLocator.Get().WaitForAll(enumerator);
    }
    public override void Break() { }
    public override float cost { get; }
}
