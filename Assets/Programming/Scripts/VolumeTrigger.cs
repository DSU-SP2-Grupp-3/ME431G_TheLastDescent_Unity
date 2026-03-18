using UnityEngine;
using UnityEngine.Events;

public class VolumeTrigger : MonoBehaviour
{
    public RoundClock.ProgressMode triggerInMode = RoundClock.ProgressMode.RealTime;
    public Locator<RoundClock> roundClock;

    public UnityEvent TriggerEnter;
    public UnityEvent TriggerExit;
    public UnityEvent TriggerStay;
    
    private void Awake()
    {
        roundClock = new();
    }

    public void OnTriggerEnter(Collider _)
    {
        if (roundClock.Get().currentMode != triggerInMode) return;
        TriggerEnter?.Invoke();
    }

    public void OnTriggerExit(Collider _)
    {
        if (roundClock.Get().currentMode != triggerInMode) return;
        TriggerExit?.Invoke();
    }

    public void OnTriggerStay(Collider _)
    {
        if (roundClock.Get().currentMode != triggerInMode) return;
        TriggerStay?.Invoke();
    }
}