using UnityEngine;

public class OrthographicCameraMover : MonoBehaviour
{

    [Tooltip("The target Transform to move the camera from, should be a model and not an actor if possible")]
    public Transform targetGameObject;
    [Tooltip("Zoom / Distance to target")]
    public float zoom;

    [SerializeField]
    private Vector3 offset;
    
    
    private void Update()
    {
        transform.position = targetGameObject.position + offset;
    }
}