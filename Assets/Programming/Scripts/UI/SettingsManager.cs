using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsManager : Service<SettingsManager>
{
    public UnityEvent open;
    public UnityEvent close;
    
    public Slider MasterSlider;
    public Slider SFXSlider;
    public Slider MusicSlider;
    public Slider AmbianceSlider;
    public Slider DialogueSlider;

    public void Open() { open?.Invoke(); }
    public void Close() { close?.Invoke(); }
    public void MasterVol() { AudioSettings.Instance.masterVolume = MasterSlider.value; }    
    public void SFXVol() { AudioSettings.Instance.effectVolume = SFXSlider.value; }
    public void MusicVol() { AudioSettings.Instance.musicVolume = MusicSlider.value; }
    public void AmbianceVol() { AudioSettings.Instance.ambianceVolume = AmbianceSlider.value; }
    public void DialogVol() { AudioSettings.Instance.dialogVolume = DialogueSlider.value; }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (transform.position.y < -10) { Open(); } else { Close(); }
        }
    }
}

