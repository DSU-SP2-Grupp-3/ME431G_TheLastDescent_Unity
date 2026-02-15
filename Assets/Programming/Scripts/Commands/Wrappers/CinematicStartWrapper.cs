using UnityEngine;

public class CinematicStartWrapper : CommandWrapper
{
    [SerializeField]
    [Min(1)]
    private int[] ActorID;
    public override Command UnwrapCommand(WorldAgent agent)
    {
        return new CinematicStartCommand(ActorID, agent);
    }
}
