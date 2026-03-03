using System.Collections;
using UnityEngine;

public class HeatUpCommand : Command
{
    private float heatUpApCost;
    public override float apCost => heatUpApCost;

    private float heatUpResourceCost;
    public override float resourceCost => heatUpResourceCost;

    private float amount;
    private bool animationEnded;


    public HeatUpCommand(
        WorldAgent invokingAgent,
        float amount,
        float heatUpApCost,
        float heatUpResourceCost
    ) : base(invokingAgent)
    {
        this.amount = amount;
        this.heatUpApCost = heatUpApCost;
        this.heatUpApCost = heatUpResourceCost;
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
            PerformHeatUp();
        }
        if (trigger == "end") animationEnded = true;
    }

    private void PerformHeatUp()
    {

    }
}