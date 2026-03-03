using System.Collections;
using UnityEngine;

public class WaitForSecondsCommand : Command
{
    private float seconds;
    public WaitForSecondsCommand(float seconds, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.seconds = seconds;

    }
    protected override IEnumerator Execute()
    {
        yield return new WaitForSeconds(seconds);
    }
    public override void Break() { }
    public override float apCost { get; }
    /// <inheritdoc />
    public override float resourceCost => 0f;
}
