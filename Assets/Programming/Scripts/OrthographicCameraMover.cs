using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

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
    public float FreeMoveSpeed;
    public float FreeDistanceCap;
    public Transform freeMoveFocus;
    private Camera mainCamera;
    private AgentManager am;
    private Transform savedCharacter;

    private Vector2 freeMoveInitialMousePosition;

    private void Awake()
    {
        Register();
        modeSwitcher = new();
        thisCamera = GetComponent<Camera>();
        targetZoomSize = thisCamera.orthographicSize;
        mainCamera = Camera.main;
    }

    private void Start()
    {
        savedCharacter = targetGameObject;
        InputManager im = new Locator<InputManager>().Get();
        am = new Locator<AgentManager>().Get();
        im.OnScrollUp += ZoomIn;
        im.OnScrollDown += ZoomOut;
        im.OnRightClick += OnRightClick;
        im.OnRightHold += OnRightHold;
        im.OnRightUp += OnRightClickDropped;
    }
    
    private void OnRightClick()
    {
        //Mouse.current.WarpCursorPosition(new Vector2(Screen.width/2,Screen.height/2));
        freeMoveInitialMousePosition = Input.mousePosition;

        freeMoveFocus.position = targetGameObject.position;
        savedCharacter = targetGameObject;
        targetGameObject = freeMoveFocus;
    }
    private void OnRightHold()
    {
        // Vector2 mousePos = (Vector2)Input.mousePosition - new Vector2(Screen.width/2,Screen.height/2);
        Vector2 mousePos = (Vector2)Input.mousePosition - freeMoveInitialMousePosition;
        mousePos = new Vector2(mousePos.x/Screen.width*0.5f,mousePos.y/Screen.height*0.5f);
        Vector2 cameraForce = mousePos * FreeMoveSpeed;

        Transform cam = mainCamera.transform;

        Vector3 move = new Vector3(0.5f, 0, -0.5f) * cameraForce.x + new Vector3(0.5f, 0, 0.5f) * cameraForce.y;

        freeMoveFocus.position += move * FreeMoveSpeed;
        var player = am.GetSelectedPlayer();
        if(Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(freeMoveFocus.position.x, freeMoveFocus.position.z)) >= FreeDistanceCap)
        {
            freeMoveFocus.position = player.transform.position + (freeMoveFocus.position - player.transform.position).normalized * FreeDistanceCap;
        }
    }

    private void OnRightClickDropped()
    {
        targetGameObject = savedCharacter;
    }

    public void SetCameraTarget(Transform target)
    {
        targetGameObject = target;
    }

    private void LateUpdate()
    {
        if (targetGameObject)
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