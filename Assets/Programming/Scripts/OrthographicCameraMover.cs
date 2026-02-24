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

    public GameObject rangeIndicator;
    private Locator<ModeSwitcher> modeSwitcher;

    private void Awake()
    {
        Register();
        modeSwitcher = new();
    }

    public void SetCameraTarget(Transform target)
    {
        targetGameObject = target;
    }

    private void LateUpdate()
    {
        if (modeSwitcher.Get().mode == RoundClock.ProgressMode.TurnBased)
        {
            rangeIndicator.transform.position = targetGameObject.transform.position;
        }
        else
        {
            rangeIndicator.transform.position = new Vector3(0, -100, 0);
        }
        Vector3 targetPosition = targetGameObject.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime * 100f);
    }
}