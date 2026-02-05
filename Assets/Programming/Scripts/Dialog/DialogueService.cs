using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using System;

public class DialogueService : Service<DialogueService>
{
    private Queue<Dialogue> dialogues = new();

    private Queue<string> Sentences = new();
    private Locator<DialogueService> dialogueService;
    [SerializeField]
    private DialogueScriptable tempDialogueScriptable;
    [SerializeField]
    private TextMeshProUGUI textField;


    void Start()
    {
        Register();

        //-Ma. For testing purposes, I call this here.
        InitializeDialouge(tempDialogueScriptable.GetDialogues());
    }
    private void InitializeDialouge(Dialogue[] dialogues)
    {
        this.dialogues.Clear();
        foreach (Dialogue dialogue in dialogues)
        {
            this.dialogues.Enqueue(dialogue);
        }

        StartCoroutine(RunDialogue());
    }

    private IEnumerator RunDialogue()
    {
        while (dialogues.Count > 0)
        {
            Dialogue ActiveSpeaker = dialogues.Dequeue();

            Sentences.Clear();
            foreach (string sentence in ActiveSpeaker.sentences)
            {
                Sentences.Enqueue(sentence);
            }

            yield return StartCoroutine(DisplayNextSentence());

        }
        yield return null;
    }
    public IEnumerator DisplayNextSentence()
    {
        Queue<Char> letters = new();
        while (Sentences.Count > 0)
        {
            string sentence = Sentences.Dequeue();

            letters.Clear();
            foreach (char letter in sentence)
            {
                letters.Enqueue(letter);
            }

            string WrittenSentence = "";
            while (letters.Count > 0)
            {
                WrittenSentence += letters.Dequeue();
                textField.text = WrittenSentence;
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(2f);
        }


        yield return new WaitForSeconds(2f);
    }
}
