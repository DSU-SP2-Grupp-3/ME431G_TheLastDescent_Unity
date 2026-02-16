using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CinematicMoveCommand : Command
{
    private List<CinematicMoveInfo> cinematicMoveInfos;
    private Locator<CinematicKitService> cinematicKitLocator;
    public CinematicMoveCommand(List<CinematicMoveInfo> cinematicMoveInfos, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.cinematicMoveInfos = cinematicMoveInfos;
    }
    public override IEnumerator Execute()
    {
        cinematicKitLocator = new();
        foreach(CinematicMoveInfo agentinfo in cinematicMoveInfos)
        {

        

            WorldAgent worldAgent = cinematicKitLocator.Get().GetActor(agentinfo.ID);
            worldAgent.OverwriteQueue(new MoveCommand(invokingAgent.transform.position - agentinfo.relativePosition, worldAgent));
            Debug.Log(worldAgent);
            
        }
        yield return null;
    }
    public override void Break() { }
    public override float cost { get; }
}
