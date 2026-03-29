using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueService : Service<DialogueService>
{
    private Queue<DialogueRequest> requestQueue = new();
    private bool isRunning = false;
    public bool dialogueRunning => isRunning;
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
    private PopupService popupService;
    //Temp fix
    public bool isDone;
    public UnityEvent unityEvent;

    private bool turbo = false;

    private void Awake()
    {
        Register();
    }
    private void Start()
    {
        popupService = new Locator<PopupService>().Get();
    }
    public void EnqueueDialogue(Dialogue[] dialogues, Action onComplete)
    {
        requestQueue.Enqueue(new DialogueRequest(dialogues, onComplete));

        if (!isRunning)
        {
            StartCoroutine(ProcessQueue());
        }
    }
    private IEnumerator ProcessQueue()
    {
        isRunning = true;
        ani.SetTrigger("DialogueStart");

        while (requestQueue.Count > 0)
        {
            DialogueRequest request = requestQueue.Dequeue();

            yield return StartCoroutine(RunDialogueSequence(request.dialogues));

            request.onComplete?.Invoke();
        }

        ani.SetTrigger("DialogueEnd");
        isRunning = false;
    }
    public IEnumerator RunDialogueSequence(Dialogue[] dialogues)
    {

        // todo: maybe don't clear dialogues here to prevent funky stuff if triggering two dialogues at the same time??
        this.dialogues.Clear();
        foreach (Dialogue dialogue in dialogues)
        {
            this.dialogues.Enqueue(dialogue);
        }
        yield return StartCoroutine(RunDialogue());

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

            nameField.text = activeSpeaker.name;
            if (activeSpeaker.portrait != null) portrait.sprite = activeSpeaker.portrait;
            if (activeSpeaker.sfx != null) new Locator<AudioManager>().Get().PlayAudioEvent(activeSpeaker.sfx);
            if (activeSpeaker.Popup != null) yield return popupService.Open(activeSpeaker.Popup);

            yield return StartCoroutine(DisplayNextSentence());

            nameField.text = "";
            textField.text = "";

            if (activeSpeaker.Popup != null) yield return popupService.Close();
        }

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
            textField.text = sentence;
            textField.maxVisibleCharacters = 0;
            ClickCheck = StartCoroutine(OnMouseClick());
            yield return StartCoroutine(DisplayNextLetter());
            WrittenSentence = sentence;
            textField.maxVisibleCharacters = int.MaxValue;
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
            textField.maxVisibleCharacters += 1;
            yield return turbo ? null : new WaitForSecondsRealtime(0.04f);
        }
    }
    public IEnumerator OnMouseClick()
    {
        yield return turbo ? null : new WaitForSecondsRealtime(0.01f);
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
