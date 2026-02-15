using UnityEngine;
using UnityEngine.AI;

public class Player_Movement : MonoBehaviour
{
    [Tooltip("Required")]
    [SerializeField] private NavMeshAgent nmAgent;
    private NavMeshPath currentPath;
    [SerializeField] private LineRenderer lineRenderer;
    [Tooltip("How close the agent needs to get to remove the path behind it.")]
    [SerializeField] private float pathcutoff;
    void Start()
    {
        if (nmAgent == null)
        {
            Debug.Log( "missing NavMeshAgent reference", this);
        }

        currentPath = new NavMeshPath();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                nmAgent.CalculatePath(hit.point, currentPath);
                DrawPath(currentPath);
                
                //this next bit could be done in a later execute action thing
                nmAgent.SetPath(currentPath);
            }
        }

        if (Vector3.Distance(transform.position, nmAgent.destination) 
            < pathcutoff && Mathf.Abs(transform.position.y - nmAgent.destination.y) < 0.3f)  /*if distance between transform and destination small*/
        {
            lineRenderer.positionCount = 0;
        }
    }

    private void DrawPath(NavMeshPath path)
    {
        //needs to be improved
        lineRenderer.positionCount = path.corners.Length;
        lineRenderer.SetPosition(0, transform.position);
        for (int i = 1; i < path.corners.Length; i++)
        {
            lineRenderer.SetPosition(i, path.corners[i]);
        }
    }
}
