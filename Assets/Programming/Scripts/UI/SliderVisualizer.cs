using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderVisualizer : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text text;

    private void LateUpdate()
    {
        text.text = $"{slider.value*100:N0}";
    }
}
