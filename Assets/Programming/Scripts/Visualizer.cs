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

    public void DrawPath(NavMeshPath inputPath, WorldAgent agent)
    {
        //needs to be improved
        if (!agent.lineRenderer) return;
        agent.lineRenderer.positionCount = inputPath.corners.Length;
        agent.lineRenderer.SetPosition(0, transform.position);
        for (int i = 1; i < inputPath.corners.Length; i++)
        {
            agent.lineRenderer.SetPosition(i, inputPath.corners[i]);

        }
    }
}
