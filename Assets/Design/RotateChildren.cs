using System.Collections.Generic;
using UnityEngine;

public class RotateChildren : MonoBehaviour
{
    private List<Transform> children;
    public Vector3 RotationSpeed = new Vector3(0, 0, 0);

    void Start()
    {
        children = new();

        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }
    }

    void Update()
    {
        foreach (Transform child in children)
        {
            child.Rotate(RotationSpeed * Time.deltaTime);
        }
    }
}