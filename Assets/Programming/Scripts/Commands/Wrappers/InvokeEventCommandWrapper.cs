using UnityEngine;
using UnityEngine.Events;

public class InvokeEventCommandWrapper : CommandWrapper
{
    public UnityEvent commandEvent;

    public override Command UnwrapCommand(WorldAgent agent)
    {
        return new InvokeEventCommand(agent, commandEvent);
    }
}