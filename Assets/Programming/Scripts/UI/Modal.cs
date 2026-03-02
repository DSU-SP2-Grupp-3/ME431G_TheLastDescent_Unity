using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Modal : Service<Modal>
{
    [SerializeField]
    private TMP_Text question;

    private bool answered;
    private bool yes;

    private Coroutine activeCoroutine;

    public void Awake()
    {
        Register();
        gameObject.SetActive(false);
    }

    public void Prompt(string questionText, Action yesAction, Action noAction)
    {
        if (activeCoroutine != null) return;
        gameObject.SetActive(true);
        answered = false;
        yes = false;
        question.text = questionText;
        activeCoroutine = StartCoroutine(AwaitAnswer(yesAction, noAction));
    }

    private IEnumerator AwaitAnswer(Action yesAction, Action noAction)
    {
        Debug.Log("await");
        yield return new WaitUntil(() => answered);
        if (yes) yesAction?.Invoke();
        else noAction?.Invoke();
        gameObject.SetActive(false);
        activeCoroutine = null;
    }

    public void Answer(bool yes)
    {
        answered = true;
        this.yes = yes;
    }

}
