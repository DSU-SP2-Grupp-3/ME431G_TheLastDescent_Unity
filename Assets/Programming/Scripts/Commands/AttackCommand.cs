using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Command
{
    // todo: this should probably be variable
    public override float cost => 1f;

    private WorldAgent receivingAgent;
    private DamageManager damageManager;

    private bool animationEnded;

    public AttackCommand(WorldAgent invokingAgent, WorldAgent receivingAgent, DamageManager damageManager)
        : base(invokingAgent)
    {
        this.receivingAgent = receivingAgent;
        this.damageManager = damageManager;
    }

    public override IEnumerator Execute()
    {
        invokingAgent.AnimationEventTriggered += CaptureAnimationEvent;
        invokingAgent.animator.SetTrigger("StartAttack");
        yield return new WaitUntil(() => animationEnded);
        invokingAgent.AnimationEventTriggered -= CaptureAnimationEvent;
    }

    public override void Break()
    {
        invokingAgent.animator.SetTrigger("StopAttack");
        invokingAgent.AnimationEventTriggered -= CaptureAnimationEvent;
    }

    public override void Visualize(Visualizer visualizer) { }

    private void CaptureAnimationEvent(string trigger)
    {
        if (trigger == "attack") PerformAttack();
        if (trigger == "end") animationEnded = true;
    }

    private void PerformAttack()
    {
        float damage = invokingAgent.weaponStats.GetDamage();
        damageManager.DealDamageEvent(damage, receivingAgent);
    }
}