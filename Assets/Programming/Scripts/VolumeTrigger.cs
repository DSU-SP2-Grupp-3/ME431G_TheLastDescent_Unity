using UnityEngine;
using UnityEngine.Events;

public class VolumeTrigger : MonoBehaviour
{
    public UnityEvent TriggerEnter;
    public UnityEvent TriggerExit;
    public UnityEvent TriggerStay;

    public void OnTriggerEnter(Collider collider)
    {
        TriggerEnter?.Invoke();
        Debug.Log(collider.gameObject.name);
    }

    public void OnTriggerExit(Collider _)
    {
        TriggerExit?.Invoke();
    }

    public void OnTriggerStay(Collider _)
    {
        TriggerStay?.Invoke();
    }

}
