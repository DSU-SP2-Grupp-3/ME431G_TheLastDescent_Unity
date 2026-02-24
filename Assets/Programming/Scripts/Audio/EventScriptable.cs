using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "EventScriptable", menuName = "Scriptable Objects/EventScriptable")]
public class EventScriptable : ScriptableObject
{
    public string eventName;

    [SerializeField]
    public EventReference fmodEvent;
    public EventInstance eventInstance;

    public void CreateInstance()
    {
        eventInstance = RuntimeManager.CreateInstance(fmodEvent);
    }

    public void PlayEvent()
    {
        eventInstance.start();
    }
}
