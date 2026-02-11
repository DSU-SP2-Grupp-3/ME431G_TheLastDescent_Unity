using UnityEngine;

public class OrthographicCameraMover : MonoBehaviour
{

    [Tooltip("The target Transform to move the camera from, should be a model and not an actor if possible")]
    public Transform targetGameObject;
    [Tooltip("Zoom / Distance to target")]
    public float zoom;

    [SerializeField] [Tooltip("Används i princip istället för att sätta positionen på kameran")]
    private Vector3 offset;
    
    
    private void Update()
    {
        transform.position = new Vector3(
            targetGameObject.position.x + offset.x, 
            targetGameObject.position.y + offset.y,
            targetGameObject.position.z + offset.z);
    }
}