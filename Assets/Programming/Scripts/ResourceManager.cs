using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceManager", menuName = "Manager/Resource Manager")]
public class ResourceManager : ScriptableObject
{
    public Watcher<float> collectedResources;
    public Watcher<List<Command>> queuedResourceCommands;

    public void PayResource(Command command)
    {
        if (queuedResourceCommands.value.Contains(command))
        {
            queuedResourceCommands.value.Remove(command);
            queuedResourceCommands.MarkChanged();
            collectedResources.value -= command.resourceCost;
        }
        else
        {
            Debug.LogError($"Commmand {nameof(command)} is not registered to pay resources");
        }
    }

    public void QueueResource(Command command)
    {
        queuedResourceCommands.value.Add(command);
        queuedResourceCommands.MarkChanged();
    }

    public void RemoveCommands(IEnumerable<Command> commands)
    {
        queuedResourceCommands.value.RemoveAll((c) => commands.Contains(c));
        queuedResourceCommands.MarkChanged();
    }

    public void ResetResourceCommands()
    {
        queuedResourceCommands.value.Clear();
        queuedResourceCommands.MarkChanged();
    }

    private void OnEnable()
    {
        collectedResources = new(0, GreaterThanZero);
        queuedResourceCommands = new();
        queuedResourceCommands.MarkChanged();
    }

    private float GreaterThanZero(float value)
    {
        return Mathf.Max(value, 0f);
    }

    public bool CanQueuePackage(CommandManager.CommandPackage package)
    {
        float queuedTotal = queuedResourceCommands.value.Select(c => c.resourceCost).Sum();
        return TotalCommandCollectionResourceCost(package.commands) <= collectedResources - queuedTotal;
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

    public bool InDeficit()
    {
        float totalQueued = queuedResourceCommands.value.Select(c => c.resourceCost).Sum();
        return collectedResources < totalQueued;
    }
}

