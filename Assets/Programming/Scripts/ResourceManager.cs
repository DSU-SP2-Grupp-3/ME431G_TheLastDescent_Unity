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
        collectedResources += amount;
    }

    public bool PayResource(float cost)
    {
        Debug.Log($"pay {cost} resources");
        return true;
    }

    private void OnEnable()
    {
        collectedResources = 0;
    }

    public class ClickAbility
    {
        public readonly string validCursorPath;
        public readonly string invalidCursorPath;
        public readonly Command[] commands;
        public readonly float resourceCost;

        public ClickAbility(Command[] commands, float resourceCost, string validCursorPath, string invalidCursorPath)
        {
            this.commands = commands;
            this.resourceCost = resourceCost;
            this.validCursorPath = validCursorPath;
            this.invalidCursorPath = invalidCursorPath;
        }

        public ClickAbility(Command command, float resourceCost, string validCursorPath, string invalidCursorPath)
        {
            this.commands = new Command[] { command };
            this.resourceCost = resourceCost;
            this.validCursorPath = validCursorPath;
            this.invalidCursorPath = invalidCursorPath;
        }
    }
}