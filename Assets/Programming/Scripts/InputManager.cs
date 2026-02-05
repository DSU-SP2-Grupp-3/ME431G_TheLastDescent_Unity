using System;
using UnityEngine;

public class InputManager : Service<InputManager>
{
    public event Action<WorldAgent> ClickedOnPlayer;
    public event Action<Vector3> MovePlayerInput;

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
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    ClickedOnPlayer?.Invoke(hit.collider.GetComponentInParent<WorldAgent>());
                }
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    MovePlayerInput?.Invoke(hit.point);
                }
            }
        }
    }
}