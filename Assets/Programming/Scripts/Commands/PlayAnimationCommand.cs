using System.Collections;
using UnityEngine;

public class PlayAnimationCommand : Command
{

    public override float cost => 0f;

    private Animator animator;
    private TriggerInfo triggerInfo;
    private bool hasEndAnimation;

    public PlayAnimationCommand(WorldAgent _, Animator animator, TriggerInfo triggerInfo, bool hasEndAnimation = false) 
         : base(_)
    {
        this.animator = animator;
        this.triggerInfo = triggerInfo;
        this.hasEndAnimation = hasEndAnimation;
    }

    public override IEnumerator Execute()
    {
        animator.SetTrigger(triggerInfo.StartTrigger());
        if (!hasEndAnimation)
        {
            yield return null;
        }
        else
        {
            yield return WaitForEndOfAnimation(animator);
        }
    }

    public override void Break()
    {
        animator.SetTrigger(triggerInfo.EndTrigger());
    }
}
