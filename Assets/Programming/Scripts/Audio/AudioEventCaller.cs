using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

public class AudioEventCaller : MonoBehaviour
{
    public string eventPlayerName;
    public string ParameterName;
    public EventScriptable eventScriptable;
    public EventMono eventMono;
    public void Play()
    {
        Locator<AudioManager> locator = new();
        if (locator.TryGet(out AudioManager locatedService))
        {
            if (eventMono != null)
            {
                locatedService.PlayAudioEvent(eventMono);
            }
            else if (eventScriptable != null)
            {
                locatedService.PlayAudioEvent(eventScriptable);
            }
            else
            {
                locatedService.PlayAudioEvent(eventPlayerName);
            }
        }
    }

    public void SetValueAndPlay(float value)
    {
        Locator<AudioManager> locator = new();
        if (locator.TryGet(out AudioManager locatedService))
        {
            if (eventMono != null)
            {
                locatedService.PlayAudioEvent(eventMono);
                locatedService.RunInstanceModification(eventMono, ParameterName, value);
            }
            else if (eventScriptable != null)
            {
                locatedService.PlayAudioEvent(eventScriptable);
                locatedService.RunInstanceModification(eventScriptable, ParameterName, value);
            }
            else
            {
                locatedService.PlayAudioEvent(eventPlayerName);
                locatedService.RunInstanceModification(eventPlayerName, ParameterName, value);
            }
        }
    }
    public void TryPlayAndSetValue(float value)
    {
        Locator<AudioManager> locator = new();
        if (locator.TryGet(out AudioManager locatedService))
        {
            /*
            if (eventMono != null)
            {
                if (locatedService.TryGet(eventPlayerName, out _)) locatedService.PlayAudioEvent(eventMono);
                locatedService.RunInstanceModification(eventMono, ParameterName, value);
            }
            */
            if (eventScriptable != null)
            {
                if (!locatedService.TryGet(eventScriptable.eventReference, out _)) locatedService.PlayAudioEvent(eventScriptable);
                locatedService.RunInstanceModification(eventScriptable, ParameterName, value);
            }
            else
            {
                if (!locatedService.TryGet(eventScriptable.eventReference, out _)) locatedService.PlayAudioEvent(eventPlayerName);
                locatedService.RunInstanceModification(eventPlayerName, ParameterName, value);
            }
        }
    }
}
