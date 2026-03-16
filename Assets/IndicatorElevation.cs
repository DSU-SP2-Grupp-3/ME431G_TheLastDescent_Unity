using System;
using UnityEngine;

public class IndicatorElevation : MonoBehaviour
{
    public LayerMask ScalableLayer;
    private Vector3 TargetElevation = new();

    void Update()
    {
        if(Physics.Raycast(transform.position, Vector3.up, out RaycastHit hitInfoUp, Mathf.Infinity, ScalableLayer))
        {
            TargetElevation = hitInfoUp.point;
            //Debug.DrawRay(TargetElevation, Vector3.up, Color.blue, 1f);
        }
        else if(Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hitInfoDown, Mathf.Infinity, ScalableLayer))
        {
            TargetElevation = hitInfoDown.point;
            //Debug.DrawRay(TargetElevation, -Vector3.up, Color.red, 1f);
        }
        transform.position = new Vector3(transform.position.x, TargetElevation.y, transform.position.z);

    }
}
