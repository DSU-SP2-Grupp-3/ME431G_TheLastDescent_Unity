using System.Collections;
using UnityEngine;

public class HeatUpCommand : Command
{
    private float heatUpApCost;
    public override float apCost => heatUpApCost;

    private float heatUpResourceCost;
    public override float resourceCost => heatUpResourceCost;

    private float amount;
    private ResourceManager resourceManager;
    private bool animationEnded;

    public HeatUpCommand(
        WorldAgent invokingAgent,
        float amount,
        ResourceManager resourceManager,
        float heatUpApCost,
        float heatUpResourceCost
    ) : base(invokingAgent)
    {
        this.amount = amount;
        this.resourceManager = resourceManager;
        this.heatUpApCost = heatUpApCost;
        this.heatUpResourceCost = heatUpResourceCost;
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
            PerformHeatUp();
        }
        if (trigger == "end") animationEnded = true;
    }

    private void PerformHeatUp()
    {
        resourceManager.PayResource(this);
        // maybe make a temperature manager like the damageManager and use that here
        // could be useful for managing debuffs
        invokingAgent.localStats.temperature.value += amount;
    }
}