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
    public Color PlayerAPColor;
    public Color PlayerHeatColor;

    public Color PathColor;
    public Color IndicatorColor;

    public Color DamageColor;
    public Color HealColor;
    
    //-E i have to make events :(
    public void TriggerAll() //just triggers all the events <3
    {
        SetPlayerHpColor(PlayerHpColor);
        SetPlayerAPColor(PlayerAPColor);
        SetPlayerHeatColor(PlayerHeatColor);
        SetPathColor(PathColor);
        SetIndicatorColor(IndicatorColor);
        SetDamagePopColor(DamageColor);
        SetHealPopColor(HealColor);
    }
    //player HP
    public UnityAction<Color> PlayerHpColorEvent = delegate{}; public void SetPlayerHpColor(Color input) { PlayerHpColorEvent(input); }
    
    //player AP
    public UnityAction<Color> PlayerAPColorEvent = delegate{}; public void SetPlayerAPColor(Color input) { PlayerAPColorEvent(input); }
    
    //player Heat ;) <- silly
    public UnityAction<Color> PlayerHeatColorEvent = delegate{}; public void SetPlayerHeatColor(Color input) { PlayerHeatColorEvent(input); }
    
    //path color
    public UnityAction<Color> PathColorEvent = delegate{}; public void SetPathColor(Color input) { PathColorEvent(input); }
    
    //indicator color (no idea how this one will work tbh
    public UnityAction<Color> IndicatorColorEvent = delegate{}; public void SetIndicatorColor(Color input) { IndicatorColorEvent(input); }
    
    //damage pop, might not get used but it now exists at least?
    public UnityAction<Color> DamagePopColorEvent = delegate{}; public void SetDamagePopColor(Color input) { DamagePopColorEvent(input); }
    
    //heal pop, might not get used but it now exists at least?
    public UnityAction<Color> HealPopColorEvent = delegate{}; public void SetHealPopColor(Color input) { HealPopColorEvent(input); }
}