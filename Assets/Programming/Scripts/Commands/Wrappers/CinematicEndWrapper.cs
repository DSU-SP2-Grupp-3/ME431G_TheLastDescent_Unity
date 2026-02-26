using UnityEngine;

public class CinematicEndWrapper : CommandWrapper
{
    public override Command UnwrapCommand(WorldAgent agent)
    {
        return new CinematicEndCommand(agent);
    }
}
