using System;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIScaler : MonoBehaviour
{
    public SettingsStorage settings;
    public RectTransform rectTransform;
    
    private void Start()
    {
        SetScale();
        settings.UIScaleEvent += SetScale;
    }

    private void OnDestroy()
    {
        settings.UIScaleEvent -= SetScale;
    }

    private void SetScale()
    {
        rectTransform.localScale = Vector3.one * settings.UIScale;
    } 
}
