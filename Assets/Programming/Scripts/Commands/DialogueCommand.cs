using System.Collections;
using UnityEngine;

public class DialogueCommand : Command
{
    private Locator<DialogueService> dialogueServiceLocator;
    private DialogueService dialogueService;
    private DialogueScriptable dialogueScriptable;
    public DialogueCommand(DialogueScriptable dialogueScriptable, WorldAgent invokingAgent) : base(invokingAgent)
    {
        //-Ma. if This is in push, remove please
        Debug.Log("DialogueStarted");
        this.dialogueScriptable = dialogueScriptable;

    }
    public override IEnumerator Execute()
    {
        dialogueServiceLocator = new();
        yield return null;
        if (dialogueServiceLocator.TryGet(out dialogueService))
        {
            dialogueService.InitializeDialouge(dialogueScriptable.GetDialogues());
        }

    }
    public override void Break() { }
    public override float cost { get; }
}



