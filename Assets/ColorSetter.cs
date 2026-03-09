using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorSetter : MonoBehaviour
{
    [SerializeField] private SettingsStorage storedSettings;
    [SerializeField] private RawImage myImage;
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
    
    public void SetColor() //updates the color stored in the settings
    {
        switch (colorType)
        {
            //change stored color to color of button, send out event to all listeners
            //-3 honestly a pretty bad way of doing it probably, but i would rather have it done poorly than not at all :<
            case ColorType.HP:
                storedSettings.PlayerHpColor = myImage.color;
                break;
            case ColorType.AP:
                storedSettings.PlayerAPColor = myImage.color;
                break;
            case ColorType.Heat:
                storedSettings.PlayerHeatColor = myImage.color;
                break;
            case ColorType.Path:
                storedSettings.PathColor = myImage.color;
                break;
            case ColorType.Indicator:
                storedSettings.IndicatorColor = myImage.color;
                break;
            
            //these two dont need events as the created object always inherits the color correctly <3
            case ColorType.Damage:
                storedSettings.DamageColor = myImage.color;
                break;
            case ColorType.Heal:
                storedSettings.HealColor = myImage.color;
                break;

            default:
                Debug.Log("HOW?? enum set to impossible option");
                break;
        }
    }
}
