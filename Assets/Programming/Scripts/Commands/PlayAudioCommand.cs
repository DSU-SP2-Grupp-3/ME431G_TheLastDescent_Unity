using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayAudioCommand : Command
{
    private UnityEvent unityEvent;
    public PlayAudioCommand(UnityEvent unityEvent, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.unityEvent = unityEvent;

    }
    protected override IEnumerator Execute()
    {
        unityEvent.Invoke();
        yield return null;

    }
    public override void Break() { }
    public override float cost { get; }
}



