using System.Collections;
using UnityEngine;

public class HealCommand : Command
{
    private float healApCost;
    public override float apCost => healApCost;

    private float healResourceCost;
    public override float resourceCost => healResourceCost;

    private float amount;
    private DamageManager damageManager;
    private ResourceManager resourceManager;

    private bool animationEnded;

    public HealCommand(
        WorldAgent invokingAgent,
        DamageManager damageManager,
        ResourceManager resourceManager,
        float amount,
        float healApCost,
        float healResourceCost
    ) : base(invokingAgent)
    {
        this.amount = amount;
        this.healApCost = healApCost;
        this.healResourceCost = healResourceCost;
        this.damageManager = damageManager;
        this.resourceManager = resourceManager;
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
        resourceManager.RemoveCommands(new Command[] { this });
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
        resourceManager.PayResource(this);
        damageManager.DealDamage(-amount, invokingAgent);
    }
}