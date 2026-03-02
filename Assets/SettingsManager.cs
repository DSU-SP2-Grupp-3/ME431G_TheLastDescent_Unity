using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsManager : Service<SettingsManager>
{
    public UnityEvent open;
    public UnityEvent close;
    //-Ma. Temp solution.
    public Slider slider;
        public Slider slider1;
            public Slider slider2;

    public void Open()
    {
        open?.Invoke();
    }
    public void Close()
    {
        close?.Invoke();
    }
    public void MasterVol()
    {
        AudioSettings.Instance.masterVolume = slider.value;
    }    
    public void SFXVol()
    {
        AudioSettings.Instance.effectVolume = slider1.value;
    }
    public void MusicVol()
    {
        AudioSettings.Instance.musicVolume = slider2.value;
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) Open();
    }
}

