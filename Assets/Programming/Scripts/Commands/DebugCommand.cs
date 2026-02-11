using System.Collections;
using UnityEngine;

public class DebugCommand : Command
{
    public override float cost => 0f;

    private string debugString;
    private float waitTime1, waitTime2;

    public DebugCommand(WorldAgent invokingAgent, string debugString, float waitTime1 = 1f, float waitTime2 = 2f) : base(invokingAgent)
    {
        this.debugString = debugString;
        this.waitTime1 = waitTime1;
        this.waitTime2 = waitTime2;
    }
    
    public override IEnumerator Execute()
    {
        Debug.Log($"Debug Command Start: {debugString}");
        yield return new WaitForSeconds(waitTime1);
        Debug.Log($"Debug Command Middle: {debugString}");
        yield return new WaitForSeconds(waitTime2);
        Debug.Log($"Debug Command End: {debugString}");
    }
    public override void Break()
    {
        Debug.Log($"Debug Command Break: {debugString}");
    }
}