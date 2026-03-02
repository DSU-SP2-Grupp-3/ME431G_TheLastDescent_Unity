using System.Collections;
using UnityEngine;

public class HealCommand : Command
{
    private float healCost;
    public override float cost => healCost;

    private float amount;

    public HealCommand(WorldAgent invokingAgent, float amount, float healCost) : base(invokingAgent)
    {
        this.amount = amount;
        this.healCost = healCost;
    }

    protected override IEnumerator Execute()
    {
        // play aninmation, heal after or during
        Debug.Log($"heal {amount}");
        yield return null;
    }

    public override void Break()
    {
        // stop animation
    }
}