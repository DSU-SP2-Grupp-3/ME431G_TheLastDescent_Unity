using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Command
{
    private float attackCost;
    public override float apCost => attackCost;
    /// <inheritdoc />
    public override float resourceCost => 0f;

    private WorldAgent receivingAgent;
    private DamageManager damageManager;

    private bool animationEnded;
    private bool receiverDied;

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
        if (receivingAgent.dead) yield break;
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

    private void CaptureAnimationEvent(string trigger, GameObject gameObject)
    {
        if (trigger == "attack")
        {
            PerformAttack();
        }
        if (trigger == "end") animationEnded = true;
    }

    private void PerformAttack()
    {
        audioManager.PlayAudioEvent(attackEventName);
        float damage = invokingAgent.weaponStats.GetDamage() * receivingAgent.localStats.receivedDamageModifier;
        damageManager.DealDamageEvent(damage, receivingAgent);
    }
}