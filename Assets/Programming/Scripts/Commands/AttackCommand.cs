using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Command
{
    public override float cost { get; }

    private WorldAgent receivingAgent;
    private DamageManager damageManager;

    public AttackCommand(WorldAgent invokingAgent, WorldAgent receivingAgent, DamageManager damageManager) 
         : base(invokingAgent)
    {
        this.receivingAgent = receivingAgent;
        this.damageManager = damageManager;
    }
    
    public override IEnumerator Execute()
    {
        invokingAgent.AnimationEventTriggered += CaptureAttackEvent;
        invokingAgent.animator.SetTrigger("StartAttack");
        yield return WaitForEndOfAnimation(invokingAgent.animator);
        invokingAgent.AnimationEventTriggered -= CaptureAttackEvent;
    }

    public override void Break()
    {
        invokingAgent.animator.SetTrigger("StopAttack");
        invokingAgent.AnimationEventTriggered -= CaptureAttackEvent;
    }

    public override void Visualize() { }

    private void CaptureAttackEvent(string trigger)
    {
        if (trigger == "attack") PerformAttack();
    }

    private void PerformAttack()
    {
        float damage = invokingAgent.weaponStats.GetDamage();
        damageManager.DealDamageEvent(damage, receivingAgent);
        Debug.Log($"Deal {damage} damage to {receivingAgent.name}");
    }
}
