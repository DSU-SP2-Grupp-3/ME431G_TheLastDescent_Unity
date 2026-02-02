using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMover : MonoBehaviour
{
    [Header("Targets, written in order of priority")]
    
    [Tooltip("The target Transform to move the camera from")]
    public Transform targetGameObject;
    [Tooltip("The point in space that the camera is focused on.")]
    public Vector3 targetLocation;

    private void Update()
    {
        /*needs to account for zoom/distance to object
         which means vector math i bet :(*/
        if (targetGameObject != null)
        {
            transform.position = targetGameObject.transform.position;
        }
        else
        {
            transform.position = targetLocation;
        }
    }
}
