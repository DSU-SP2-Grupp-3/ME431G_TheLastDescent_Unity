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
        float duration = 0.5f;
        float t = 0;
        Quaternion startRotation = invokingAgent.transform.rotation;
        Quaternion targetRotation = LookAt();


        while (t < duration)
        {
            var time = t / duration;
            invokingAgent.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, time);
            t += Time.deltaTime;
            yield return null;
        }
        invokingAgent.transform.rotation = targetRotation;
    }

    public override void Break()
    { }

    public override void Visualize(Visualizer visualizer) { }
    public Quaternion LookAt()
    {
        Vector3 direction = receivingAgent.transform.position - invokingAgent.transform.position;
        direction.y = 0f;
        return Quaternion.LookRotation(direction);
    }
}
