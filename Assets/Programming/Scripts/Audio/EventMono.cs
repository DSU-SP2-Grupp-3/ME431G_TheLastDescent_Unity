using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// -Ma. Monobehaviors act as runtime instance containers and .
/// They always contain a runtime instance of a EventPlayer.
/// May not produce an omni-present sound.
/// </summary>
public class EventMono : MonoBehaviour
{

    [SerializeField]
    public EventReference eventReference;
    public EventInstance eventInstance;
    public EventPlayer eventPlayer;
    void Awake()
    {
        eventPlayer = new(eventReference);
        eventInstance = eventPlayer.eventInstance;
        RuntimeManager.AttachInstanceToGameObject(eventInstance, gameObject);
    }
    public void RunInstanceModification(string name, float value)
    {
        eventPlayer.eventInstance.setParameterByName(name, value);
    }

}