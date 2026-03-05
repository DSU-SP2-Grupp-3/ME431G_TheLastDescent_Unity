using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CinematicMoveCommand : Command
{
    private List<CinematicMoveInfo> cinematicMoveInfos;
    private Locator<CinematicKitService> cinematicKitLocator;
    public CinematicMoveCommand(List<CinematicMoveInfo> cinematicMoveInfos, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.cinematicMoveInfos = cinematicMoveInfos;
    }
    protected override IEnumerator Execute()
    {
        List<IEnumerator> enumerator = new();
        cinematicKitLocator = new();
        Locator<TurnManager> turnManagerLocator = new();
        foreach(CinematicMoveInfo agentinfo in cinematicMoveInfos)
        {
            WorldAgent worldAgent = cinematicKitLocator.Get().GetActor(agentinfo.ID);
            enumerator.Add(worldAgent.OverwriteQueueIEnumerator(new MoveCommand(invokingAgent.transform.position - agentinfo.relativePosition, worldAgent)));
        }
        yield return turnManagerLocator.Get().WaitForAll(enumerator);
    }
    public override void Break() { }
    public override float apCost { get; }
    /// <inheritdoc />
    public override float resourceCost => 0f;
}
