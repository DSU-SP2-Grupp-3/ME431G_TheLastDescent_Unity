using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueCommand : Command
{
    private Locator<DialogueService> dialogueServiceLocator;
    private DialogueService dialogueService;
    private DialogueScriptable dialogueScriptable;
    private AgentManager agentManager;
    public DialogueCommand(DialogueScriptable dialogueScriptable, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.dialogueScriptable = dialogueScriptable;
    }
    protected override IEnumerator Execute()
    {
        agentManager = new Locator<AgentManager>().Get();
        ToggleTurnBasedButton turnBasedButton = new Locator<ToggleTurnBasedButton>().Get();
        RoundClock roundClock = new Locator<RoundClock>().Get();
        bool dialoguePausedRoundClock = false;

        dialogueServiceLocator = new();

        if (dialogueServiceLocator.TryGet(out dialogueService))
        {
            if (roundClock.currentMode == RoundClock.ProgressMode.RealTime)
            {
                dialoguePausedRoundClock = true;
                roundClock.EnterTurnBased();
            }

            agentManager.LockAgentInputActive(this);
            yield return dialogueService.StartCoroutine(
                dialogueService.InitializeDialouge(dialogueScriptable.GetDialogues()));
            agentManager.UnlockAgentInputActive(this);

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