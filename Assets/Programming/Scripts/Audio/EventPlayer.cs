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

    public void AttachToGameObject(GameObject gameObject)
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
    }
    public bool IsFinished()
    {
        eventInstance.getPlaybackState(out PLAYBACK_STATE state);
        return state == PLAYBACK_STATE.STOPPED;
    }
    
    public void Stop(bool allowFadeout = true)
    {
        eventInstance.stop(allowFadeout ? FMOD.Studio.STOP_MODE.ALLOWFADEOUT : FMOD.Studio.STOP_MODE.IMMEDIATE);
        eventInstance.release();
    }
    public bool isOneshot()
    {
        eventInstance.getDescription(out EventDescription description);
        description.isOneshot(out bool result);
        return result;
    }

}
