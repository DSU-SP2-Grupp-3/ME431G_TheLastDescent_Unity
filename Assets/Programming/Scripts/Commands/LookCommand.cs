using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCommand : Command
{
    public override float cost { get; }

    private WorldAgent receivingAgent;

    public LookAtCommand(WorldAgent invokingAgent, WorldAgent receivingAgent)
         : base(invokingAgent)
    {
        this.receivingAgent = receivingAgent;
    }

    public override IEnumerator Execute()
    {
        float t = 2f;
        float tMax = t;
        Quaternion newRotation = LookAt();
        Debug.Log(invokingAgent.transform.rotation.eulerAngles.y);
        Vector3 start = invokingAgent.transform.rotation.eulerAngles;
        
        while(t > 0)
        {
            var time = t/tMax;
                    Debug.Log(start.y);
                    Debug.Log(newRotation.eulerAngles.y);
            var rotation =Vector3.Lerp(new(0, newRotation.eulerAngles.y + 90, 0), new(0, start.y, 0), time);
            invokingAgent.transform.rotation = Quaternion.Euler(rotation);
            t -= Time.deltaTime;
            yield return new();
        }
        yield return null;
    }

    public override void Break()
    { }

    public override void Visualize(Visualizer visualizer) { }
    public Quaternion LookAt()
    {
        var a = (invokingAgent.transform.position - receivingAgent.transform.position).normalized;
        return Quaternion.FromToRotation(invokingAgent.transform.forward, a);
    }
}
