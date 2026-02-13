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
    private Queue<Char> letters = new();
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
            string sentence = Sentences.Dequeue();

            letters.Clear();
            foreach (char letter in sentence)
            {
                letters.Enqueue(letter);
            }

            //-Ma. This sucks, but it works

            skipping = false;
            WrittenSentence = "";
            ClickCheck = StartCoroutine(OnMouseClick());
            yield return StartCoroutine(DisplayNextLetter());
            textField.text = sentence;
            if (ClickCheck != null) StopCoroutine(ClickCheck);
            yield return ClickCheck = StartCoroutine(OnMouseClick());
        }
    }
    public IEnumerator DisplayNextLetter()
    {
        while (letters.Count > 0)
        {
            if (skipping == true)
            {
                skipping = false;
                yield break;
            }
            unityEvent.Invoke();
            WrittenSentence += letters.Dequeue();
            textField.text = WrittenSentence;
            yield return new WaitForSeconds(0.04f);
        }
    }
    public IEnumerator OnMouseClick()
    {
        yield return new WaitForEndOfFrame();
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
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
}
