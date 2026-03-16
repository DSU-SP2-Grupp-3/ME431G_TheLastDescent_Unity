using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceManager", menuName = "Manager/Resource Manager")]
public class ResourceManager : ScriptableObject
{
    public Watcher<float> collectedResources;
    public Watcher<List<Command>> queuedPayResourceCommands;
    public Watcher<List<Resource>> queuedGetResources;
    public HashSet<Resource> collectedResourceObjects;

    public void PayResource(Command command)
    {
        // zero cost commands are not queued so must be ignored here
        if (!ValidCommand(command)) return;

        if (queuedPayResourceCommands.value.Contains(command))
        {
            queuedPayResourceCommands.value.Remove(command);
            queuedPayResourceCommands.MarkChanged();
            collectedResources.value -= command.resourceCost;
        }
        else
        {
            Debug.LogError($"Commmand {nameof(command)} is not registered to pay resources");
        }
    }

    public void GetResource(Resource resource)
    {
        if (collectedResourceObjects.Contains(resource) || !queuedGetResources.value.Contains(resource)) return;
        collectedResources.value += resource.amount;
        collectedResourceObjects.Add(resource);
        queuedGetResources.value.Remove(resource);
        queuedGetResources.MarkChanged();

        resource.Collect();
    }

    public void ProcessCommand(Command command)
    {
        if (command is GetResourceCommand getResourceCommand) QueueGetResource(getResourceCommand.resource);
        else if (command.resourceCost > 0f) QueuePayResource(command);
    }

    public void ProcessCommands(IEnumerable<Command> commands)
    {
        foreach (Command command in commands)
        {
            ProcessCommand(command);
        }
    }
    
    private void QueuePayResource(Command command)
    {
        if (!ValidCommand(command)) return;
        queuedPayResourceCommands.value.Add(command);
        queuedPayResourceCommands.MarkChanged();
    }

    private void QueueGetResource(Resource resource)
    {
        if (queuedGetResources.value.Contains(resource) || collectedResourceObjects.Contains(resource)) return;
        queuedGetResources.value.Add(resource);
        queuedGetResources.MarkChanged();
    }
    
    public void RemoveCommands(IEnumerable<Command> commands)
    {
        queuedPayResourceCommands.value.RemoveAll((c) => commands.Contains(c));
        queuedPayResourceCommands.MarkChanged();

        IEnumerable<Resource> getResources = commands
            .Where(c => c is GetResourceCommand)
            .Select(c => (c as GetResourceCommand).resource);
        
        queuedGetResources.value.RemoveAll(r => getResources.Contains(r));
        queuedGetResources.MarkChanged();
    }

    public void ResetResourceCommands()
    {
        queuedPayResourceCommands.value.Clear();
        queuedPayResourceCommands.MarkChanged();
    }

    private void OnEnable()
    {
        collectedResources = new(0, GreaterThanZero);
        queuedPayResourceCommands = new();
        queuedPayResourceCommands.MarkChanged();
        queuedGetResources = new();
        queuedGetResources.MarkChanged();
        collectedResourceObjects = new();
    }

    private float GreaterThanZero(float value)
    {
        return Mathf.Max(value, 0f);
    }

    public bool CanQueuePackage(CommandManager.CommandPackage package)
    {
        float queuedTotal = queuedPayResourceCommands.value.Select(c => c.resourceCost).Sum();
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
        float totalQueued = queuedPayResourceCommands.value.Select(c => c.resourceCost).Sum();
        return collectedResources < totalQueued;
    }

    /// <summary>
    /// Only to be used for debugging purposes, like in the cheater object
    /// </summary>
    public void DebugGetResource(float amount)
    {
        collectedResources.value += amount;
    }

    private bool ValidCommand(Command command)
    {
        Debug.Log($"res cost: {command.resourceCost} | status: {command.status}");
        return command.resourceCost != 0f && command.status != Command.Status.Invalid;
    }
}