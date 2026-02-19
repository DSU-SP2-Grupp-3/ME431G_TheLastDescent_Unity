using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class SimpleEventPlayer : MonoBehaviour
{
    public string eventName;

    [SerializeField]
    private EventReference fmodEvent;
    private EventInstance eventInsance;

    private void Start()
    {
        eventInsance = RuntimeManager.CreateInstance(fmodEvent);
    }

    public void PlayEvent()
    {
        eventInsance.start();
    }
}