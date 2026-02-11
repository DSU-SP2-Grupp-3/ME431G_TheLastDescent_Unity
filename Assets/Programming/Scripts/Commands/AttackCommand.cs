using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Command
{
    public override float cost { get; }

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
        Debug.Log("start attack");
        invokingAgent.AnimationEventTriggered += CaptureAnimationEvent;
        invokingAgent.animator.SetTrigger("StartAttack");
        yield return new WaitUntil(() => animationEnded);
        invokingAgent.AnimationEventTriggered -= CaptureAnimationEvent;
        Debug.Log("finish attack");
    }

    public override void Break()
    {
        invokingAgent.animator.SetTrigger("StopAttack");
        invokingAgent.AnimationEventTriggered -= CaptureAnimationEvent;
    }

    public override void Visualize() { }

    private void CaptureAnimationEvent(string trigger)
    {
        Debug.Log($"animation event: {trigger}");
        if (trigger == "attack") PerformAttack();
        if (trigger == "end") animationEnded = true;
    }

    private void PerformAttack()
    {
        float damage = invokingAgent.weaponStats.GetDamage();
        Debug.Log($"Deal {damage} damage to {receivingAgent.name}");
        damageManager.DealDamageEvent(damage, receivingAgent);
    }
}
