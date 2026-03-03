using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ScreenFade : MonoBehaviour
{
    //-Ma. ok, I might have forgotten about animator.
    private Image image;
    public bool isOn;
    public UnityEvent OnComplete;
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
        float i = 3;
        float time = 3;
        Color color = new();
        Debug.Log("y01");
        while (time > 0)
        {
            Debug.Log("ya");
            time -= Time.deltaTime;
            var currentFade = time / i;
            color.a = currentFade;
            image.color = color;
            yield return new WaitForSeconds(1f);
        }

        color.a = 0;
        image.color = color;
        OnComplete?.Invoke();
    }
    private IEnumerator FadeOut()
    {
        float i = 3;
        float time = 0;
        Color color = new();
        Debug.Log("y02");
        while (time < i)
        {
            Debug.Log("y0");
            time += Time.deltaTime;
            var currentFade = time / i;
            color.a = currentFade;
            image.color = color;
            yield return new WaitForSeconds(0.01f);
        }

        color.a = 1;
        image.color = color;
        OnComplete?.Invoke();
    }

    
}
