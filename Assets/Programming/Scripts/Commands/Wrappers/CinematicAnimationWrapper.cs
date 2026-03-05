using System.Collections.Generic;
using UnityEngine;

public class CinematicAnimationWrapper : CommandWrapper
{
    [SerializeField, Tooltip("Position relative to the game object that this component is attached to")]
    private List<CinematicAnimationInfo> ActorsAnimation;
    private Locator<CinematicKitService> cinematicKitLocator = new();
    public override Command UnwrapCommand(WorldAgent agent)
    {
        return new CinematicAnimationCommand(ActorsAnimation, agent);
    } 

}
