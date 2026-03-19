using System;
using FMOD.Studio;
using FMODUnity;
using NUnit.Framework.Constraints;
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
#if UNITY_EDITOR
        var result = ParameterReference.GetParameters(eventReference);
        string parameter = null;
        foreach (PARAMETER_DESCRIPTION param in result)
        {
            if (param.name == name)
            {
                parameter = param.name;
                break;
            }
        }
        if (parameter == null)
        {
            Debug.Log($"The Parameter: '{name}' does not exist on EventPlayer, referencePath: {eventReference.Path}. \nThe available parameters on this player are the following:");
            ParameterReference.ShowParameters(eventReference);
        }
#endif
        eventInstance.setParameterByName(name, value);
    }
    public void PlayEvent()
    {
        eventInstance.start();
        ParameterReference.GetParameters(eventReference);
    }
    public bool IsFinished()
    {
        eventInstance.getPlaybackState(out PLAYBACK_STATE state);
        return state == PLAYBACK_STATE.STOPPED;
    }
    public bool IsPlaying()
    {
        if (!eventInstance.isValid()) return false;

        PLAYBACK_STATE state;
        eventInstance.getPlaybackState(out state);

        Debug.Log(state);

        return state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.STARTING;
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
