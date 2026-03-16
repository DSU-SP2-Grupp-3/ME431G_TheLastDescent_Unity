using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ResourceCounter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text counter, queueCounter;
    [SerializeField]
    private ResourceManager resourceManager;
    [SerializeField]
    private Color positiveColor = Color.green;
    [SerializeField]
    private Color negativeColor = Color.red;

    private void Awake()
    {
        resourceManager.collectedResources.Changed += OnResourcesChanged;
        resourceManager.queuedPayResourceCommands.Changed += OnQueuedPayResourceCommandsChanged;
    }

    private void OnResourcesChanged(float amount)
    {
        counter.text = $"{amount:0.}";
    }

    private void OnQueuedPayResourceCommandsChanged(List<Command> newQueue)
    {
        float amount = TotalQueueAmount(newQueue);
        if (amount == 0) queueCounter.text = "";
        else if (amount < 0)
        {
            queueCounter.color = positiveColor;
            queueCounter.text = $"+{Mathf.Abs(amount):0}";
        }
        else
        {
            queueCounter.color = negativeColor;
            queueCounter.text = $"-{amount:0}";
        }
    }

    private float TotalQueueAmount(List<Command> queue)
    {
        return queue.Select(p => p.resourceCost).Sum();
    }
}