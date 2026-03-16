using System;
using TMPro;
using UnityEngine;

public class TextUpdater : MonoBehaviour
{
    [SerializeField] private SettingsStorage storedSettings;
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] [Min(0)] private float fadeRate;
    [SerializeField] [Min(0)] private float riseRate;
    public Vector3 Target;

    private void Start()
    {
        transform.position = Camera.main.WorldToScreenPoint(Target);
    }

    private void Update()
    {
        textComponent.alpha -= fadeRate * Time.deltaTime;
        textComponent.transform.position = new Vector3(
            transform.position.x, 
            transform.position.y + (riseRate * fadeRate * 100 * Time.deltaTime),
            transform.position.z);
        if (textComponent.alpha <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(float number)
    {
        textComponent.color = number > 0 ? storedSettings.DamageColor : storedSettings.HealColor;
        if (number < 0)
        {
            textComponent.text = $"{MathF.Abs(number):F0} HEAL";

        }
        else
        {
            textComponent.text = $"{MathF.Abs(number):F0} DMG";
        }
    }
}
