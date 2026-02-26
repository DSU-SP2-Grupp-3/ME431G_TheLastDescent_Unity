using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// -Ma. Monobehaviors act as event catalysts
/// They always contain a runtime instance of a EventPlayer.
/// Cannot trigger persistent sounds.
/// </summary>
public class EventMono : MonoBehaviour
{

    [SerializeField]
    public EventReference eventReference;
    public EventInstance eventInstance;
    public EventPlayer eventPlayer;
    void Start()
    {
        eventPlayer = new(eventReference);
        eventInstance = eventPlayer.eventInstance;
    }
    public void RunInstanceModification(string name, float value)
    {
        eventInstance.setParameterByName(name, value);
    }

}