using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Command
{
    private float attackCost;
    public override float cost => attackCost;

    private WorldAgent receivingAgent;
    private DamageManager damageManager;

    private bool animationEnded;

    private string attackEventName;

    public AttackCommand(WorldAgent invokingAgent,
                         WorldAgent receivingAgent,
                         DamageManager damageManager,
                         float attackCost,
                         string attackEventName)
        : base(invokingAgent)
    {
        this.receivingAgent = receivingAgent;
        this.damageManager = damageManager;
        this.attackEventName = attackEventName;
        this.attackCost = attackCost;
    }

    protected override IEnumerator Execute()
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

    public override void VisualizeInQueue(Visualizer visualizer) { }

    private void CaptureAnimationEvent(string trigger)
    {
        if (trigger == "attack")
        {
            audioManager.PlayAudioEvent(attackEventName);
            PerformAttack();
        }
        if (trigger == "end") animationEnded = true;
    }

    private void PerformAttack()
    {
        Debug.Log("performed attack");
        float damage = invokingAgent.weaponStats.GetDamage();
        damageManager.DealDamageEvent(damage, receivingAgent);
    }
}