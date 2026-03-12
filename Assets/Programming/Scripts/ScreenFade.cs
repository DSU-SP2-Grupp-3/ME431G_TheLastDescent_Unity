using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    //-Ma. ok, I might have forgotten about animator.
    private Image image;
    public bool isOn;
    public UnityEvent OnFadeIn;
    public UnityEvent OnFadeOut;
    public float fadeTime;
    void Awake()
    {
        image = gameObject.GetComponent<Image>();
        Color color = new();
        if (isOn)
        {
            color.a = 1;
            image.color = color;
        }
        else
        {
            color.a = 0;
            image.color = color;
        }
    }
    public void CallFade(bool state)
    {
        switch (state)
        {
            case true:
                StartCoroutine(Fadein());
                break;
            case false:
                StartCoroutine(FadeOut());
                break;
        }
    }
    private IEnumerator Fadein()
    {
        float i = fadeTime;
        float time = fadeTime;
        Color color = new();
        while (time > 0)
        {
            time -= Time.deltaTime;
            var currentFade = time / i;
            color.a = currentFade;
            image.color = color;
            yield return null;
        }

        color.a = 0;
        image.color = color;
        OnFadeIn?.Invoke();
    }
    private IEnumerator FadeOut()
    {
        float i = fadeTime;
        float time = 0;
        Color color = new();
        while (time < i)
        {
            time += Time.deltaTime;
            var currentFade = time / i;
            color.a = currentFade;
            image.color = color;
            yield return null;
        }

        color.a = 1;
        image.color = color;
        OnFadeOut?.Invoke();
    }

    
}
