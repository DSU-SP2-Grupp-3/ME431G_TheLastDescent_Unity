using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorSetter : MonoBehaviour
{
    [SerializeField] private SettingsStorage storedSettings;
    [SerializeField] private RawImage targetImage;
    private enum ColorType
    {
        HP, 
        AP, 
        Heat,
        Path, 
        Indicator, 
        Damage, 
        Heal
    }
    
    [SerializeField] private ColorType colorType;
    private Color temporaryColor;
    public void UpdateColor()
    {
        switch (colorType)
        {
            case ColorType.HP:
                targetImage.color = storedSettings.PlayerHpColor;
                break;
            case ColorType.AP:
                targetImage.color = storedSettings.PlayerAPColor;
                break;
            case ColorType.Heat:
                targetImage.color = storedSettings.PlayerHeatColor;
                break;
            case ColorType.Path:
                targetImage.color = storedSettings.PathColor;
                break;
            case ColorType.Indicator:
                targetImage.color = storedSettings.IndicatorColor;
                break;
            case ColorType.Damage:
                targetImage.color = storedSettings.DamagePopColor;
                break;
            case ColorType.Heal:
                targetImage.color = storedSettings.HealPopColor;
                break;

            default:
                Debug.Log("HOW?? enum set to impossible option");
                break;
        }
    }

    public void SetColor()
    {
        switch (colorType)
        {
            case ColorType.HP:
                storedSettings.PlayerHpColor = targetImage.color;
                break;
            case ColorType.AP:
                storedSettings.PlayerAPColor = targetImage.color;
                break;
            case ColorType.Heat:
                storedSettings.PlayerHeatColor = targetImage.color;
                break;
            case ColorType.Path:
                storedSettings.PathColor = targetImage.color;
                break;
            case ColorType.Indicator:
                storedSettings.IndicatorColor = targetImage.color;
                break;
            case ColorType.Damage:
                storedSettings.DamagePopColor = targetImage.color;
                break;
            case ColorType.Heal:
                storedSettings.HealPopColor = targetImage.color;
                break;

            default:
                Debug.Log("HOW?? enum set to impossible option");
                break;
        }
    }
}
