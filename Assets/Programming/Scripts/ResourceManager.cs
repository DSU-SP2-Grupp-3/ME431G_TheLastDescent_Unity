using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceManager", menuName = "Manager/Resource Manager")]
public class ResourceManager : ScriptableObject
{
    public Watcher<float> collectedResources;
    public Watcher<List<Command>> queuedResourceCommands;
    public HashSet<GameObject> collectedResourceObjects;

    public void PayResource(Command command, GameObject resourceObject = null)
    {
        // if this resource has already been picked up don't get it's resources again. 
        // abilities that use resources pass null
        if (resourceObject && collectedResourceObjects.Contains(resourceObject)) return;
        
        // zero cost commands are not queued so must be ignored here
        if (command.resourceCost == 0f) return;
        if (queuedResourceCommands.value.Contains(command))
        {
            queuedResourceCommands.value.Remove(command);
            queuedResourceCommands.MarkChanged();
            collectedResources.value -= command.resourceCost;
            if (resourceObject) collectedResourceObjects.Add(resourceObject);
        }
        else
        {
            Debug.LogError($"Commmand {nameof(command)} is not registered to pay resources");
        }
    }

    public void QueueResource(Command command)
    {
        if (command.resourceCost == 0) return;
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
        collectedResourceObjects = new();
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

