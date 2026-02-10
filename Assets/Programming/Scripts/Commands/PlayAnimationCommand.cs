using System.Collections;
using UnityEngine;

public class PlayAnimationCommand : Command
{

    public override float cost => 0f;

    private Animator animator;
    private TriggerInfo triggerInfo;
    private bool hasEndAnimation;

    public PlayAnimationCommand(WorldAgent _, Animator animator, TriggerInfo triggerInfo, bool hasEndAnimation = false) : base(_)
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
            // https://discussions.unity.com/t/wait-until-an-animation-is-finished/699955/6
            yield return null; // -se: wait one frame for animator to update internal state
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }
    }

    public override void Break()
    {
        animator.SetTrigger(triggerInfo.EndTrigger());
    }
}
