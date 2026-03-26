using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SlideShow : MonoBehaviour
{
    public UnityEvent SlideEnd;

    public void TriggerSlideEnd()
    {
        SlideEnd?.Invoke();
    }
}