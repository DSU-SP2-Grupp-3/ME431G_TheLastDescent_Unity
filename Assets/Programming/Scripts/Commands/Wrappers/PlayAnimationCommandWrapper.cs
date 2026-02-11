using UnityEngine;

public class PlayAnimationCommandWrapper : CommandWrapper
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private string startTrigger, endTrigger;
    [SerializeField]
    private bool hasEndAnimation;

    public override Command UnwrapCommand(WorldAgent agent)
    {
        TriggerInfo triggerInfo = new TriggerInfo(startTrigger, endTrigger);
        return new PlayAnimationCommand(agent, animator, triggerInfo, hasEndAnimation);
    }
}