using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : Service<InputManager>
{
    public event Action<WorldAgent> ClickedOnPlayer;
    public event Action<WorldAgent> ClickedOnEnemy;
    public event Action<Vector3> MovePlayerInput;
    public event Action<GameObject> ClickedEnvironment;

    [SerializeField]
    private LayerMask clickableLayers;

    private Camera mainCamera;

    private int UILayer;

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

    private void Update()
    {
        // todo: refactor input manager so that we can preview what commands are about to be added to the selected player
        
        if (Input.GetMouseButtonDown(0))
        {
            // don't perform physics raycast if the mouse is over a ui element
            if (IsPointerOverUIElement()) return;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, clickableLayers))
            {
                ProcessClick(hit);
            }
        }
    }

    private void ProcessClick(RaycastHit hit)
    {
        if (HitLayer(hit, "Interactable"))
        {
            ClickedEnvironment?.Invoke(hit.collider.gameObject);
        }
        if (HitLayer(hit, "Player"))
        {
            ClickedOnPlayer?.Invoke(hit.collider.GetComponentInParent<WorldAgent>());
        }
        if (HitLayer(hit, "Ground"))
        {
            MovePlayerInput?.Invoke(hit.point);
        }
        if (HitLayer(hit, "Enemy"))
        {
            ClickedOnEnemy?.Invoke(hit.collider.GetComponentInParent<WorldAgent>());
        }
    }

    private bool HitLayer(RaycastHit hit, string layerName)
    {
        return hit.collider.gameObject.layer == LayerMask.NameToLayer(layerName);
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