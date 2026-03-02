using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

public class AudioEventCaller : MonoBehaviour
{
    public string eventPlayerName;
    public string ParameterName;
    public EventScriptable eventScriptable;
    public EventMono eventMono;
    public UnityEvent unityEvent;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
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
                locatedService.RunInstanceModification(eventPlayerName, ParameterName, value);
            }
            else
            {
                locatedService.PlayAudioEvent(eventPlayerName);
                locatedService.RunInstanceModification(eventPlayerName, ParameterName, value);
            }
        }
    }
    public void SetValue(float value)
    {
        Locator<AudioManager> locator = new();
        if (locator.TryGet(out AudioManager locatedService))
        {
            if (eventMono != null)
            {
                locatedService.RunInstanceModification(eventMono, ParameterName, value);
            }
            else if (eventScriptable != null)
            {
                locatedService.RunInstanceModification(eventPlayerName, ParameterName, value);
            }
            else
            {
                locatedService.RunInstanceModification(eventPlayerName, ParameterName, value);
            }
        }
    }
    public void PlayEvent()
    {
        unityEvent?.Invoke();
    }
}
