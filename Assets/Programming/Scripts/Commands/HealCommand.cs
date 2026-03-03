using System.Collections;
using UnityEngine;

public class HealCommand : Command
{
    private float healCost;
    public override float cost => healCost;

    private float amount;
    private DamageManager damageManager;

    private bool animationEnded;

    public HealCommand(WorldAgent invokingAgent, DamageManager damageManager, float amount, float healCost) :
        base(invokingAgent)
    {
        this.amount = amount;
        this.healCost = healCost;
        this.damageManager = damageManager;
    }

    protected override IEnumerator Execute()
    {
        invokingAgent.AnimationEventTriggered += CaptureAnimationEvent;
        invokingAgent.animator.SetTrigger("StartHeal");
        yield return new WaitUntil(() => animationEnded);
        invokingAgent.AnimationEventTriggered -= CaptureAnimationEvent;
    }

    public override void Break()
    {
        invokingAgent.animator.SetTrigger("StopHeal");
        invokingAgent.AnimationEventTriggered -= CaptureAnimationEvent;
    }

    private void CaptureAnimationEvent(string trigger)
    {
        if (trigger == "heal")
        {
            PerformHeal();
        }
        if (trigger == "end") animationEnded = true;
    }

    private void PerformHeal()
    {
        damageManager.DealDamage(-amount, invokingAgent);
    }
}