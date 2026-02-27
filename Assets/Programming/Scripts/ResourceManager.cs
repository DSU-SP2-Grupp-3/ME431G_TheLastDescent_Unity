using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceManager", menuName = "Manager/Resource Manager")]
public class ResourceManager : ScriptableObject
{
    public event Action<float> ResourcesChanged;

    private float _collectedResources;
    public float collectedResources
    {
        get => _collectedResources;
        private set
        {
            _collectedResources = value;
            ResourcesChanged?.Invoke(_collectedResources);
        }
    }

    public void GetResource(float amount)
    {
        Debug.Log($"Get {amount} of resources");
        collectedResources += amount;
    }
}