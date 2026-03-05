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
        if (number > 0)
        {
            textComponent.color = storedSettings.DamagePopColor;
        }
        else
        {
            textComponent.color = storedSettings.HealPopColor;
        }
        textComponent.text = $"{MathF.Abs(number):0,0} DMG";
    }
}
