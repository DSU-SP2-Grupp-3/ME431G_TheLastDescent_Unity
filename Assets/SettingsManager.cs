using UnityEngine;
using UnityEngine.Events;

public class SettingsManager : Service<SettingsManager>
{
    public UnityEvent open;
    public UnityEvent close;
    public void Open()
    {
        open?.Invoke();
    }
    public void Close()
    {
        close?.Invoke();
    }
}

