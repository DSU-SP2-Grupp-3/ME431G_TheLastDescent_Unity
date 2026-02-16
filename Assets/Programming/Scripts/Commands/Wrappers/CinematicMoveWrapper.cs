using System.Collections.Generic;
using UnityEngine;

public class CinematicMoveWrapper : CommandWrapper
{
    [SerializeField, Tooltip("Position relative to the game object that this component is attached to")]
    private List<CinematicMoveInfo> ActorsMove;
    private Locator<CinematicKitService> cinematicKitLocator = new();
    public override Command UnwrapCommand(WorldAgent agent)
    {
        return new CinematicMoveCommand(ActorsMove, agent);
    } 

}
