using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class EventPlayer
{
    public EventReference eventReference;
    public EventInstance eventInstance;
    public EventPlayer(EventReference fmodEvent)
    {
        eventReference = fmodEvent;
        eventInstance = RuntimeManager.CreateInstance(fmodEvent);
    }

    public void AttachToGameObejct(GameObject gameObject)
    {
        RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject);
    }
    public void RunInstanceModification(string name, float value)
    {
        eventInstance.setParameterByName(name, value);
    }
    public void PlayEvent()
    {
        eventInstance.start();
        eventInstance.release();
    }
}
