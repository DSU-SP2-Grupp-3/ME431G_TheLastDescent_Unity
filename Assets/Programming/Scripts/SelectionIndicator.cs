using UnityEngine;

public class SelectionIndicator : MonoBehaviour
{
    [Tooltip("The target Transform to move the camera from, should be a model and not an actor if possible")]
    public Transform targetGameObject;
    public Vector3 offset;

    public bool active { get; private set; }

    public void SetIndicatorTarget(Transform target)
    {
        targetGameObject = target;
        active = true;
    }
    public void DisableIndicator()
    {
        targetGameObject = null;
        transform.position = new Vector3(0, -100, 0);
        active = false;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = targetGameObject.position;
        gameObject.transform.position = targetPosition + offset;
    }
}