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

    public Slider UIScaleSlider;
    
    [SerializeField] private Material pathMaterial;
    [SerializeField] private Material indicatorMaterial;
    private Locator<AgentManager> agentManager;
    
    public void Open()
    {
        open?.Invoke(); 
        Time.timeScale = 0f; 
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        if (agentManager.TryGet(out AgentManager am)) { am.LockAgentInputActive(gameObject); }
    }

    public void Close()
    {
        close?.Invoke(); 
        Time.timeScale = 1f;
        if (agentManager.TryGet(out AgentManager am)) { am.UnlockAgentInputActive(gameObject); }
    }
    
    public void MasterVol() { AudioSettings.Instance.masterVolume = MasterSlider.value; AudioSettings.Instance.UpdateVolumes(); AudioSettings.Instance.SetVolumes();}    
    public void SFXVol() { AudioSettings.Instance.effectVolume = SFXSlider.value; AudioSettings.Instance.UpdateVolumes(); AudioSettings.Instance.SetVolumes();}
    public void MusicVol() { AudioSettings.Instance.musicVolume = MusicSlider.value; AudioSettings.Instance.UpdateVolumes(); AudioSettings.Instance.SetVolumes();}
    public void AmbianceVol() { AudioSettings.Instance.ambienceVolume = AmbienceSlider.value; AudioSettings.Instance.UpdateVolumes(); AudioSettings.Instance.SetVolumes();}
    public void DialogVol() { AudioSettings.Instance.dialogueVolume = DialogueSlider.value; AudioSettings.Instance.UpdateVolumes(); AudioSettings.Instance.SetVolumes();}

    public void SetUIScale()
    {
        // todo: slider don't work
        storedSettings.UIScale = UIScaleSlider.value;
        storedSettings.SetUIScale();
    }

    private bool agentManagerExsists = false;
    private void Start()
    {
        agentManager = new Locator<AgentManager>();
        storedSettings.TriggerAll();
        MasterSlider.value = storedSettings.masterVolume;
        SFXSlider.value = storedSettings.effectVolume;
        MusicSlider.value = storedSettings.musicVolume;
        AmbienceSlider.value = storedSettings.ambienceVolume;
        DialogueSlider.value = storedSettings.dialogueVolume;

        UIScaleSlider.value = storedSettings.UIScale;
        
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

    public void SetResolutionHD()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
    }
}

