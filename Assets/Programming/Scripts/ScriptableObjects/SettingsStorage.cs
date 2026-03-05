using UnityEngine;

[CreateAssetMenu(fileName = "SettingsStorage", menuName = "Scriptable Objects/SettingsStorage")]
public class SettingsStorage : ScriptableObject
{
    //-E this is literally just used to store variables across scenes so that settings changed in the menu will stay changed in the game <3
    [Header("Sound/FMOD")] 
    public float Master;
    public float Sound;
    public float Music;
    public float Dialogue;
    public float Ambience;
    
    [Header("Color")] 
    public Color PlayerHpColor;
    public Color PlayerAPColor;
    public Color DamagePopColor;
    public Color PathColor;
    public Color IndicatorColor;
}
