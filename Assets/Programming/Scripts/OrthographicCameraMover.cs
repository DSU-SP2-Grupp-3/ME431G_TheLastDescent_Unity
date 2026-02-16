using UnityEngine;

public class OrthographicCameraMover : Service<OrthographicCameraMover>
{
    [Tooltip("The target Transform to move the camera from, should be a model and not an actor if possible")]
    public Transform targetGameObject;
    [Tooltip("Zoom / Distance to target")]
    public float zoom;

    [SerializeField]
    [Tooltip("Används i princip istället för att sätta positionen på kameran")]
    private Vector3 offset;

    [SerializeField, Range(0f, 1f)]
    private float smoothing;

    private void Awake()
    {
        Register();
    }

    public void SetCameraTarget(Transform target)
    {
        targetGameObject = target;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = targetGameObject.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing);
    }
}