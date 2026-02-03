using UnityEngine;
using UnityEngine.AI;

public class Player_Movement : MonoBehaviour
{
    [Tooltip("Required")]
    [SerializeField] private NavMeshAgent nmAgent;
    private NavMeshPath currentPath;
    [SerializeField] private LineRenderer lineRenderer;
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
    }

    private void DrawPath(NavMeshPath path)
    {
        lineRenderer.SetPositions(path.corners);
    }
}
