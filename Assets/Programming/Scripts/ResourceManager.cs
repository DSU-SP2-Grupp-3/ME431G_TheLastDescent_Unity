using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceManager", menuName = "Manager/Resource Manager")]
public class ResourceManager : ScriptableObject
{
    public Watcher<float> collectedResources;
    public Watcher<float> queuedResources;

    public void GetResource(float amount)
    {
        collectedResources.value += amount;
    }

    public void LoseResource(float lost)
    {
        collectedResources.value -= lost;
    }

    public void PayQueue()
    {
        Debug.Assert(collectedResources.value >= queuedResources.value);
        collectedResources.value -= queuedResources.value;
        ResetQueue();
    }

    public void QueueResource(float queue)
    {
        queuedResources.value += queue;
    }

    public void ResetQueue()
    {
        queuedResources.value = 0f;
    }

    private void OnEnable()
    {
        collectedResources = new Watcher<float>(0, GreaterThanZero);
        queuedResources = new Watcher<float>(0, GreaterThanZero);
    }

    private float GreaterThanZero(float value)
    {
        return Mathf.Max(value, 0f);
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

    public bool CanQueuePackage(CommandManager.CommandPackage package)
    {
        return TotalCommandCollectionResourceCost(package.commands) <= collectedResources - queuedResources;
    }

    public float TotalCommandCollectionResourceCost(IEnumerable<Command> commands)
    {
        float totalResourceCost = 0f;
        foreach (Command command in commands)
        {
            totalResourceCost += command.resourceCost;
        }
        return totalResourceCost;
    }
}