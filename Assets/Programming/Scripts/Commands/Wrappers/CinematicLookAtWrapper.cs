using System.Collections.Generic;
using UnityEngine;

public class CinematicLookAtWrapper : CommandWrapper
{
    [SerializeField, Tooltip("Position relative to the game object that this component is attached to")]
    private List<CinematicLookAtInfo> ActorsLookAt;
    private Locator<CinematicKitService> cinematicKitLocator = new();
    public override Command UnwrapCommand(WorldAgent agent)
    {
        return new CinematicLookAtCommand(ActorsLookAt, agent);
    } 

}
