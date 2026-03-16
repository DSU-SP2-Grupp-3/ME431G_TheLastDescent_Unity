using UnityEngine;
using UnityEngine.Events;

public class AudioTrigger : MonoBehaviour
{
    public UnityEvent TriggerEnter;
    public UnityEvent TriggerExit;
    public UnityEvent TriggerStay;
    public LayerMask audiolayer;
    public void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == audiolayer)
        Debug.Log("Yalla yanni");
            TriggerEnter?.Invoke();
    }

    public void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == audiolayer)
            TriggerExit?.Invoke();
    }

    public void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.layer == audiolayer)
            TriggerStay?.Invoke();
    }
}
