using UnityEngine;
using UnityEngine.Events;

public class PlayAudioCommandWrapper : CommandWrapper
{
    //-Ma. Even though this is a unity event trigger only, do not use it for anything but audio!
    [SerializeField]
    private UnityEvent unityEvent;
    public override Command UnwrapCommand(WorldAgent agent)
    {
        return new PlayAudioCommand(unityEvent, agent);
    }
}
