using System.Collections;
using UnityEngine;

public class WaitForSecondsCommand : Command
{
    private float seconds;
    public WaitForSecondsCommand(float seconds, WorldAgent invokingAgent) : base(invokingAgent)
    {
        this.seconds = seconds;

    }
    public override IEnumerator Execute()
    {
        yield return new WaitForSeconds(seconds);
    }
    public override void Break() { }
    public override float cost { get; }
}
