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
        TurnManager turnManager = new Locator<TurnManager>().Get();
        bool dialoguePausedRoundClock = false;

        dialogueServiceLocator = new();

        if (dialogueServiceLocator.TryGet(out dialogueService))
        {
            if (roundClock.currentMode == RoundClock.ProgressMode.Automatic)
            {
                dialoguePausedRoundClock = true;
                roundClock.Pause();
            }
            agentManager.LockAgentInputActive(this);

            bool isDone = false;

            dialogueService.EnqueueDialogue(dialogueScriptable.GetDialogues(), () => isDone = true);

            yield return new WaitUntil(() => isDone);

            agentManager.UnlockAgentInputActive(this);

            if (dialoguePausedRoundClock && !turnManager.active)
            {
                roundClock.Unpause();
            }
        }
    }
    public override void Break() { }
    public override float apCost { get; }
    /// <inheritdoc />
    public override float resourceCost => 0f;
}