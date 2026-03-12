using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCommand : Command
{
    private Locator<DialogueService> dialogueServiceLocator;
    private DialogueService dialogueService;
    private DialogueScriptable dialogueScriptable;
    private InputManager inputManager;
    public DialogueCommand(DialogueScriptable dialogueScriptable, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.dialogueScriptable = dialogueScriptable;
    }
    protected override IEnumerator Execute()
    {
        inputManager = new Locator<InputManager>().Get();
        Button turnBasedButton = new Locator<ToggleTurnBasedButton>().Get().GetComponent<Button>();
        RoundClock roundClock = new Locator<RoundClock>().Get();
        bool dialoguePausedRoundClock = false;

        dialogueServiceLocator = new();

        if (dialogueServiceLocator.TryGet(out dialogueService))
        {
            if (turnBasedButton) turnBasedButton.interactable = false;
            if (roundClock.currentMode == RoundClock.ProgressMode.RealTime)
            {
                dialoguePausedRoundClock = true;
                roundClock.EnterTurnBased();
            }
            inputManager.enabled = false;
            yield return dialogueService.StartCoroutine(
                dialogueService.InitializeDialouge(dialogueScriptable.GetDialogues()));
            Debug.Log("Done");
            inputManager.enabled = true;
            if (turnBasedButton) turnBasedButton.interactable = true;

            if (dialoguePausedRoundClock)
            {
                roundClock.EnterRealTime();
            }
        }
    }
    public override void Break() { }
    public override float apCost { get; }
    /// <inheritdoc />
    public override float resourceCost => 0f;
}