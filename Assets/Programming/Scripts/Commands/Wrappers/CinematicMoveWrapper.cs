using System.Collections.Generic;
using UnityEngine;

public class CinematicMoveWrapper : CommandWrapper
{
    [SerializeField, Tooltip("Position relative to the game object that this component is attached to")]
    private CinematicMoveInfo ActorsMove;
    private Locator<CinematicKitService> cinematicKitLocator = new();
    public override Command UnwrapCommand(WorldAgent agent)
    {
        WorldAgent worldAgent = cinematicKitLocator.Get().GetActor(ActorsMove.ID);
        return new MoveCommand(agent.transform.position - ActorsMove.relativePosition, worldAgent);
    } 

}
