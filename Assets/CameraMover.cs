using System;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraMover : MonoBehaviour
{
   
    [Tooltip("The target Transform to move the camera from")]
    public Transform targetGameObject;
    [Tooltip("Zoom / Distance to target")]
    public float zoom;

    private void Update()
    {
        /*needs to account for zoom/distance to object
         which means vector math i bet :(*/
        transform.position = new Vector3(targetGameObject.position.x - zoom, targetGameObject.position.y + zoom,
            targetGameObject.position.z - zoom);
    }
}
