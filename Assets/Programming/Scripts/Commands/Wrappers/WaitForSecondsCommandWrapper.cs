using UnityEngine;

public class WaitForSecondsCommandWrapper : CommandWrapper
{

    [SerializeField]
    private float seconds;
    public override Command UnwrapCommand(WorldAgent agent)
    {
        return new WaitForSecondsCommand(seconds, agent);
    }
}


