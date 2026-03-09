using System.Collections;
using UnityEngine;

public class ReviveCommand : Command
{
    /// <inheritdoc />
    public override float apCost => thisApCost;
    /// <inheritdoc />
    public override float resourceCost => thisResourceCost;

    private WorldAgent revivee;
    private DamageManager damageManager;
    private float amount;
    private float thisResourceCost;
    private float thisApCost;

    private bool animationFinished;

    /// <inheritdoc />
    public ReviveCommand(
        WorldAgent invokingAgent,
        WorldAgent revivee,
        DamageManager damageManager,
        float amount,
        float resourceCost,
        float apCost
    ) : base(invokingAgent)
    {
        this.revivee = revivee;
        this.damageManager = damageManager;
        this.amount = amount;
        thisApCost = apCost;
        thisResourceCost = resourceCost;
    }

    /// <inheritdoc />
    protected override IEnumerator Execute()
    {
        revivee.AnimationEventTriggered += CaptureEndTrigger;
        revivee.animator.SetTrigger("Revive");
        yield return new WaitUntil(() => animationFinished);

        revivee.Revive();
        damageManager.DealDamageEvent(-amount, revivee);

        revivee.AnimationEventTriggered -= CaptureEndTrigger;
    }

    private void CaptureEndTrigger(string trigger, GameObject _)
    {
        if (trigger == "end")
        {
            animationFinished = true;
        }
    }

    /// <inheritdoc />
    public override void Break() { }
}