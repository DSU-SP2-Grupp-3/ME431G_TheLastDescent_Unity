using System;
using UnityEngine;

public class DialogueRequest
{
    public Dialogue[] dialogues;
    public Action onComplete;

    public DialogueRequest(Dialogue[] dialogues, Action onComplete)
    {
        this.dialogues = dialogues;
        this.onComplete = onComplete;
    }
}
