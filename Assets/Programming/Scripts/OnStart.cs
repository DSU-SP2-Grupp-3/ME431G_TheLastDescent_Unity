using UnityEngine;
using UnityEngine.Events;

public class OnStart : MonoBehaviour
{
    public UnityEvent OnStartEvent;

    void Start()
    {
        OnStartEvent?.Invoke();
    }
}
