using System;
using TMPro;
using UnityEngine;

public class TextUpdater : MonoBehaviour
{
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] [Min(0)] private float fadeRate;
    [SerializeField] [Min(0)] private float riseRate;
    private float TemporaryX;

    private void Start()
    {
        TemporaryX = transform.position.x;
        textComponent.color = Color.magenta;
    }

    private void Update()
    {
        textComponent.alpha -= fadeRate * Time.deltaTime;
        textComponent.transform.position = new Vector3(
            TemporaryX, 
            transform.position.y + (riseRate * fadeRate * 100 * Time.deltaTime),
            transform.position.z);
        if (textComponent.alpha <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text)
    {
        textComponent.text = text;
    }
}
