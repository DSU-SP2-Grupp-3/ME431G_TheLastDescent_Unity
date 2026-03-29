using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SettingsStorage", menuName = "Scriptable Objects/SettingsStorage")]
public class SettingsStorage : ScriptableObject
{
    
    //-E this is literally just used to store variables across scenes so that settings changed in the menu will stay changed in the game <3
    [Header("Sound/FMOD")] public float masterVolume;
    public float effectVolume;
    public float musicVolume;
    public float ambienceVolume;
    public float dialogueVolume;

    [Header("Color")] 
    public Color PlayerHpColor;
    public Color PlayerApColor;
    public Color PlayerHeatColor;

    public Color PathColor;
    public Color IndicatorColor;

    public Color DamageColor;
    public Color HealColor;

    [Header("Display")]
    public float UIScale;
    
    [HideInInspector]
    public bool resolutionInitialized;
    
    public int resolutionIndex { get; private set; }
    public (int width, int height) resolution => resolutions[resolutionIndex];
    public int fullScreenModeIndex { get; private set; }
    public FullScreenMode fullScreenMode => modes[fullScreenModeIndex];

    // these should match the order in which they are dislplayed in the settings menu
    private static readonly (int width, int height)[] resolutions =
    {
        (1920, 1080),
        (2560, 1440),
        (3440, 1440)
    };

    private static readonly FullScreenMode[] modes =
    {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed
    };
    
    //-E i have to make events :(
    public void TriggerAll() //just triggers all the events <3
    {
        SetPlayerHpColor();
        SetPlayerApColor();
        SetPlayerHeatColor();
        SetPathColor();
        SetIndicatorColor();
        SetDamageColor();
        SetHealColor();
    }
    //player HP
    public UnityAction PlayerHpColorEvent = delegate{}; public void SetPlayerHpColor() { PlayerHpColorEvent(); }
    
    //player AP
    public UnityAction PlayerApColorEvent = delegate{}; public void SetPlayerApColor() { PlayerApColorEvent(); }
    
    //player Heat ;) <- silly
    public UnityAction PlayerHeatColorEvent = delegate{}; public void SetPlayerHeatColor() { PlayerHeatColorEvent(); }
    
    //path color
    public UnityAction PathColorEvent = delegate{}; public void SetPathColor() { PathColorEvent(); }
    
    //indicator color (no idea how this one will work tbh
    public UnityAction IndicatorColorEvent = delegate{}; public void SetIndicatorColor() { IndicatorColorEvent(); }
    
    //damage pop, might not get used but it now exists at least?
    public UnityAction DamagePopColorEvent = delegate{}; public void SetDamageColor() { DamagePopColorEvent(); }
    
    //heal pop, might not get used but it now exists at least?
    public UnityAction HealPopColorEvent = delegate{}; public void SetHealColor() { HealPopColorEvent(); }

    public UnityAction UIScaleEvent = delegate { };
    public void SetUIScale() { UIScaleEvent(); }

    public void SetResolution(int newResolution, int newFullScreenMode)
    {
        resolutionInitialized = true;
        resolutionIndex = Math.Min(newResolution, resolutions.Length);
        fullScreenModeIndex = Math.Min(newFullScreenMode, modes.Length);
        Screen.SetResolution(resolution.width, resolution.height, fullScreenMode);
    }

    public void SetResolutionPixels(int newResolution)
    {
        SetResolution(newResolution, fullScreenModeIndex);
    }

    public void SetResultionMode(int newFullScreenMode)
    {
        SetResolution(resolutionIndex, newFullScreenMode);
    }
}