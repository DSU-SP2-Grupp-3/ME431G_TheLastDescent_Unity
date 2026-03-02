using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrthographicCameraMover : Service<OrthographicCameraMover>
{
    private Camera thisCamera;

    [Tooltip("The target Transform to move the camera from, should be a model and not an actor if possible")]
    public Transform targetGameObject;

    [SerializeField, Tooltip("Zoom grejer, min size är hur mycket man kan zoom ut, max är hur mycket man zoomar in")]
    private float minSize, maxSize;

    [SerializeField, Tooltip("size change per scroll tick")]
    private float zoomSpeed;

    private float targetZoomSize;
    [SerializeField]
    private float zoomSmoothing;

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
        thisCamera = GetComponent<Camera>();
        targetZoomSize = thisCamera.orthographicSize;
    }

    private void Start()
    {
        InputManager im = new Locator<InputManager>().Get();
        im.OnScrollUp += ZoomIn;
        im.OnScrollDown += ZoomOut;
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

        thisCamera.orthographicSize = Mathf.Lerp(
            thisCamera.orthographicSize,
            targetZoomSize,
            zoomSmoothing * Time.deltaTime * 100f
        );
    }

    private void ZoomIn()
    {
        float newSize = targetZoomSize - zoomSpeed;
        targetZoomSize = Mathf.Max(newSize, minSize);
    }

    private void ZoomOut()
    {
        float newSize = targetZoomSize + zoomSpeed;
        targetZoomSize = Mathf.Min(newSize, maxSize);
    }
}