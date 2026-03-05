using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CinematicAnimationCommand : Command
{
    private List<CinematicAnimationInfo> cinematicAnimationInfo;
    private Locator<CinematicKitService> cinematicKitLocator;
    public CinematicAnimationCommand(List<CinematicAnimationInfo> cinematicMoveInfos, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.cinematicAnimationInfo = cinematicMoveInfos;
    }
    protected override IEnumerator Execute()
    {
        List<IEnumerator> enumerator = new();
        cinematicKitLocator = new();
        Locator<TurnManager> turnManagerLocator = new();
        foreach (CinematicAnimationInfo agentinfo in cinematicAnimationInfo)
        {
            WorldAgent worldAgent = cinematicKitLocator.Get().GetActor(agentinfo.ActorID);
            TriggerInfo triggerInfo = new TriggerInfo(agentinfo.startTrigger, agentinfo.endTrigger);
            enumerator.Add(worldAgent.OverwriteQueueIEnumerator(new PlayAnimationCommand(worldAgent, agentinfo.animator, triggerInfo, agentinfo.hasEndAnimation)));
        }
        yield return turnManagerLocator.Get().WaitForAll(enumerator);
    }
    public override void Break() { }
    public override float apCost { get; }
    /// <inheritdoc />
    public override float resourceCost => 0f;
}
