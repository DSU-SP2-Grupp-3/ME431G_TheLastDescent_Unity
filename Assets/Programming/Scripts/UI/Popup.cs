using TMPro;
using UnityEngine;

public class Popup : MonoBehaviour
{
    private TMP_Text text;
    [SerializeField]
    private Color textColor;
    private Color currentColor;
    [SerializeField, Tooltip("The opacity lost per second")]
    private float fadeRate;

    private void Start()
    {
        text = GetComponent<TMP_Text>();
        gameObject.SetActive(false);
    }

    private void Update()
    {
        currentColor.a -= fadeRate * Time.deltaTime;
        text.color = currentColor;
        if (text.color.a <= 0f)
        {
            gameObject.SetActive(false);
        }
    }

    public void PopUp(string popUpText)
    {
        gameObject.SetActive(true);
        text.text = popUpText;
        currentColor = textColor;
        currentColor.a = 1;
        text.color = currentColor;
    }
}