using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceManager", menuName = "Manager/Resource Manager")]
public class ResourceManager : ScriptableObject
{
    public Watcher<float> collectedResources;
    public Watcher<Queue<Command>> queuedResources;

    public void PayResource(Command command)
    {
        Command nextCommand = queuedResources.value.Peek();
        if (nextCommand == command)
        {
            queuedResources.value.Dequeue();
            queuedResources.MarkChanged();
            collectedResources.value -= nextCommand.resourceCost;
        }
        else
        {
            Debug.LogError($"Commmand {nameof(command)} is not the next command to pay resources");
        }
    }

    public void QueueResource(Command command)
    {
        queuedResources.value.Enqueue(command);
        queuedResources.MarkChanged();
    }

    public void RemoveCommandsFromQueue(IEnumerable<Command> commands)
    {
        Queue<Command> newQueue = new();
        Command[] existingCommands = queuedResources.value.ToArray();
        queuedResources.value.Clear();
        for (int i = 0; i < existingCommands.Length; i++)
        {
            if (!commands.Contains(existingCommands[i])) newQueue.Enqueue(existingCommands[i]);
        }
        queuedResources.value = newQueue;
        queuedResources.MarkChanged();
    }

    public void ResetQueue()
    {
        queuedResources.value.Clear();
        queuedResources.MarkChanged();
    }

    private void OnEnable()
    {
        collectedResources = new(0, GreaterThanZero);
        queuedResources = new();
        queuedResources.MarkChanged();
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
        float queuedTotal = queuedResources.value.Select(c => c.resourceCost).Sum();
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
        float totalQueued = queuedResources.value.Select(c => c.resourceCost).Sum();
        return collectedResources < totalQueued;
    }
}