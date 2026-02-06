using System;
using UnityEngine;

public class InputManager : Service<InputManager>
{
    public event Action<WorldAgent> ClickedOnPlayer;
    public event Action<Vector3> MovePlayerInput;
    public event Action<GameObject> ClickedEnvironment;

    [SerializeField]
    private LayerMask clickableLayers;

    private Camera mainCamera;

    private void Awake()
    {
        Register();
        mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        Deregister();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, clickableLayers))
            {
                ProcessClick(hit);
            }
        }
    }

    private void ProcessClick(RaycastHit hit)
    {
        if (HitLayer(hit, "Environment"))
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
    }

    private bool HitLayer(RaycastHit hit, string layerName)
    {
        return hit.collider.gameObject.layer == LayerMask.NameToLayer(layerName);
    }
}