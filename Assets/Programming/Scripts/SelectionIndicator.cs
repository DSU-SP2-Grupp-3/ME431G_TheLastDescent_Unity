using UnityEngine;

public class SelectionIndicator : Service<SelectionIndicator>
{
    

    [Tooltip("The target Transform to move the camera from, should be a model and not an actor if possible")]
    public Transform targetGameObject;
    public Vector3 offset;
    private Locator<ModeSwitcher> modeSwitcher;
    private void Awake()
    {
        Register();
        modeSwitcher = new();
    }
    public void SetIndicatorTarget(Transform target)
    {
        targetGameObject = target;
    }
    private void LateUpdate()
    {
        if (modeSwitcher.Get().mode == RoundClock.ProgressMode.TurnBased)
        {
            gameObject.transform.position = targetGameObject.transform.position + offset;
        }
        else
        {
            gameObject.transform.position = new Vector3(0, -100, 0);
        }
        Vector3 targetPosition = targetGameObject.position;
    }

    private void Update()
    {
        /*needs to account for zoom/distance to object
         which means vector math i bet :(*/
        transform.position = new Vector3(targetGameObject.position.x, targetGameObject.position.y,
            targetGameObject.position.z);
    }
}
