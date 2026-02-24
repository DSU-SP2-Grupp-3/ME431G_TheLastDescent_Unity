using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class EventPlayer : MonoBehaviour
{
    public string eventName;

    [SerializeField]
    public EventReference fmodEvent;
    public EventInstance eventInstance;

    private void Start()
    {
        eventInstance = RuntimeManager.CreateInstance(fmodEvent);
    }

    public void PlayEvent()
    {
        
    }
}