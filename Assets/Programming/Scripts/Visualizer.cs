using UnityEngine;
using UnityEngine.AI;

public class Visualizer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // todo: made this static for now, should not be static, fix later so it calls all command.Visualize methods instead
    public static void DrawPath(NavMeshPath inputPath, WorldAgent agent)
    {
        //needs to be improved
        if (!agent.lineRenderer) return;
        agent.lineRenderer.positionCount = inputPath.corners.Length;
        agent.lineRenderer.SetPosition(0, agent.lineRenderer.transform.position);
        for (int i = 1; i < inputPath.corners.Length; i++)
        {
            agent.lineRenderer.SetPosition(i, inputPath.corners[i]);

        }
    }

}
