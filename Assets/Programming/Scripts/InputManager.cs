using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : Service<InputManager>
{
    /// <summary>
    /// Triggers every update, passing the raycasthit object if the mouse is not hovering over a ui element,
    /// otherwise null
    /// </summary>
    public event Action<RaycastHit, bool> OnHover;
    /// <summary>
    /// Triggers when the left mouse button is clicked
    /// </summary>
    public event Action OnRightClick;
    public event Action OnLeftClick;
    public event Action OnHold;
    public event Action OnScrollUp;
    public event Action OnScrollDown;

    [SerializeField]
    private LayerMask clickableLayers;
    [SerializeField] private LayerMask[] layerPriority;

    private Camera mainCamera;

    private int UILayer;

    private const float holdDelay = 0.15f;
    private float lastStartHold;

    private void Awake()
    {
        Register();
        mainCamera = Camera.main;
        UILayer = LayerMask.NameToLayer("UI");
    }

    private void OnDestroy()
    {
        Deregister();
    }

    private void FixedUpdate()
    {
        // don't perform physics raycast if the mouse is over a ui element
        if (IsPointerOverUIElement())
        {
            OnHover?.Invoke(default(RaycastHit), false);
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] AllRayReturns = Physics.RaycastAll(ray, float.MaxValue, clickableLayers);
        var hit = GetRayPriority(AllRayReturns, out bool didHit);
        OnHover?.Invoke(hit, didHit);
    }
    //-Ma. For the future, we might do an inital raycast that finds a point and then performs this check with limited distance to avoid raycasts being considered across the entire map.
    // will likely be needed because of elevation.
    /// <summary>
    /// Gets the ray closest to the camera with the highest found layer priority.
    /// </summary>
    /// <param name="allRayReturns"></param>
    /// <param name="didHit"></param>
    /// <returns></returns>
    private RaycastHit GetRayPriority(RaycastHit[] allRayReturns, out bool didHit)
    {
        allRayReturns = BubbleSortRayCast(allRayReturns);
        foreach (LayerMask priorityLayer in layerPriority)
        {
            foreach (RaycastHit hit in allRayReturns)
            {
                //Debug.DrawRay(hit.point, Vector3.up, Color.blue, 3, false);
                if (((1 << hit.collider.gameObject.layer) & priorityLayer) != 0)
                {
                    didHit = true;
                    return hit;
                }
            }
        }
        didHit = false;
        return new RaycastHit();
    }
    //-Ma. Might want to change this to quicksort or merge sort if it becomes too expensive
    private RaycastHit[] BubbleSortRayCast(RaycastHit[] raycastAll)
    {
        var n = raycastAll.Length;
        while (n > 0)
        {
            for (int i = 1; i < n; i++)
            {
                if (raycastAll[i - 1].distance > raycastAll[i].distance)
                {

                    RaycastHit temp = raycastAll[i - 1];
                    raycastAll[i - 1] = raycastAll[i];
                    raycastAll[i] = temp;
                }
            }
            n--;
        }
        return raycastAll;
    }
    private void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            OnScrollUp?.Invoke();
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            OnScrollDown?.Invoke();
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnRightClick?.Invoke();
            lastStartHold = Time.time;
        }
        if (Input.GetMouseButtonDown(1))
        {
            OnLeftClick?.Invoke();
        }

        if (Input.GetMouseButton(0))
        {
            if (lastStartHold + holdDelay < Time.time) OnHold?.Invoke();
        }
    }

    #region Check if mouse is over UI

    // https://discussions.unity.com/t/how-to-detect-if-mouse-is-over-ui/821330

    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    //Returns 'true' if we touched or hovering on Unity UI element.
    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }

    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    #endregion
}