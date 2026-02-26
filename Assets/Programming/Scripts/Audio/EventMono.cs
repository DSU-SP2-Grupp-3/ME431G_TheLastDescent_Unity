using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// -Ma. Monobehaviors act as event catalysts
/// They always contain a runtime instance of a EventPlayer.
/// Cannot trigger persistent sounds.
/// </summary>
public class EventMono : MonoBehaviour, IEventInst
{
    public string eventName;

    [SerializeField]
    public EventReference eventReference;
    public EventInstance eventInstance {get; set;}
    public EventPlayer eventPlayer;
    private EventMono()
    {
        eventPlayer = new(eventReference);
        eventPlayer.eventInstance = eventInstance;
        eventPlayer.AttachToGameObject(gameObject);
    }
    public void RunInstanceModification(string name, float value)
    {
        eventInstance.setParameterByName(name, value);
    }
    void OnValidate()
    {
        eventName.Trim().ToLower();
    }

}