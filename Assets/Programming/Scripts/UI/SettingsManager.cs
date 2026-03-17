using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsManager : Service<SettingsManager>
{
    [SerializeField] private SettingsStorage storedSettings;
    public UnityEvent open;
    public UnityEvent close;
    
    public Slider MasterSlider;
    public Slider SFXSlider;
    public Slider MusicSlider;
    public Slider AmbienceSlider;
    public Slider DialogueSlider;
    
    [SerializeField] private Material pathMaterial;
    [SerializeField] private Material indicatorMaterial;

    public void Open()
    {
        open?.Invoke(); 
        Time.timeScale = 0f; 
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void Close()
    {
        close?.Invoke(); 
        Time.timeScale = 1f; 
        
    }
    public void MasterVol() { AudioSettings.Instance.masterVolume = MasterSlider.value; AudioSettings.Instance.UpdateVolumes(); AudioSettings.Instance.SetVolumes();}    
    public void SFXVol() { AudioSettings.Instance.effectVolume = SFXSlider.value; AudioSettings.Instance.UpdateVolumes(); AudioSettings.Instance.SetVolumes();}
    public void MusicVol() { AudioSettings.Instance.musicVolume = MusicSlider.value; AudioSettings.Instance.UpdateVolumes(); AudioSettings.Instance.SetVolumes();}
    public void AmbianceVol() { AudioSettings.Instance.ambienceVolume = AmbienceSlider.value; AudioSettings.Instance.UpdateVolumes(); AudioSettings.Instance.SetVolumes();}
    public void DialogVol() { AudioSettings.Instance.dialogueVolume = DialogueSlider.value; AudioSettings.Instance.UpdateVolumes(); AudioSettings.Instance.SetVolumes();}

    private void Start()
    {
        storedSettings.TriggerAll();
        MasterSlider.value = storedSettings.masterVolume;
        SFXSlider.value = storedSettings.effectVolume;
        MusicSlider.value = storedSettings.musicVolume;
        AmbienceSlider.value = storedSettings.ambienceVolume;
        DialogueSlider.value = storedSettings.dialogueVolume;
        
        storedSettings.PathColorEvent += UpdatePathColor;
        storedSettings.IndicatorColorEvent += UpdateIndicatorColor;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { if (transform.position.y < -10) { Open(); } else { Close(); } }
    }

    private void UpdatePathColor()
    {
        pathMaterial.color = storedSettings.PathColor;
    }

    private void UpdateIndicatorColor()
    {
        indicatorMaterial.color = storedSettings.IndicatorColor;
    }
}

