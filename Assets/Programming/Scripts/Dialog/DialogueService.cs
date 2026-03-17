using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueService : Service<DialogueService>
{
    private Queue<Dialogue> dialogues = new();

    private Queue<string> Sentences = new();
    private string sentence;
    string WrittenSentence = "";
    private Coroutine ClickCheck = null;
    private bool skipping;
    [SerializeField]
    private TextMeshProUGUI textField;
    [SerializeField]
    private TextMeshProUGUI nameField;
    [SerializeField]
    private Image portrait;
    [SerializeField]
    private Animator ani;
    //Temp fix
    public bool isDone;
    public UnityEvent unityEvent;

    private bool turbo = false;

    private void Awake()
    {
        Register();
    }
    public IEnumerator InitializeDialouge(Dialogue[] dialogues)
    {
        ani.Play("DialogueStart");
        this.dialogues.Clear();
        foreach (Dialogue dialogue in dialogues)
        {
            this.dialogues.Enqueue(dialogue);
        }

        yield return StartCoroutine(RunDialogue());
        ani.Play("DialogueEnd");
    }

    private IEnumerator RunDialogue()
    {
        while (dialogues.Count > 0)
        {
            Dialogue activeSpeaker = dialogues.Dequeue();

            Sentences.Clear();
            foreach (string sentence in activeSpeaker.sentences)
            {
                Sentences.Enqueue(sentence);
            }


            if (activeSpeaker.portrait != null) portrait.sprite = activeSpeaker.portrait;

            nameField.text = activeSpeaker.name;
            yield return StartCoroutine(DisplayNextSentence());

        }
        nameField.text = "";
        textField.text = "";
        EndDialogue();
    }
    public IEnumerator DisplayNextSentence()
    {

        while (Sentences.Count > 0)
        {
            sentence = Sentences.Dequeue();
            if (sentence == "") break;
            textField.text = sentence;

            skipping = false;
            ClickCheck = StartCoroutine(OnMouseClick());
            yield return StartCoroutine(DisplayNextLetter());
            textField.maxVisibleCharacters = sentence.Length;
            if (ClickCheck != null) StopCoroutine(ClickCheck);
            yield return ClickCheck = StartCoroutine(OnMouseClick());
        }
    }
    public IEnumerator DisplayNextLetter()
    {
        for (int i = 0; i <= sentence.Length; i++)
        {
            if (skipping == true)
            {
                skipping = false;
                yield break;
            }
            unityEvent.Invoke();
            textField.maxVisibleCharacters = i;
            yield return turbo ? null : new WaitForSeconds(0.04f);
        }
    }
    public IEnumerator OnMouseClick()
    {
        yield return turbo ? null : new WaitForSeconds(0.01f);
        while (true)
        {
            if (Input.GetMouseButtonDown(1) || turbo)
            {
                skipping = true;
                yield break;
            }
            yield return null;
        }

    }
    private void EndDialogue()
    {
        //-Ma. Ran dialogue
    }

    public void Turbo(bool value)
    {
        turbo = value;
    }
}
