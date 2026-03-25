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

    private float queuedGet;
    private float queuedPay;
    private float deltaQueue => queuedGet - queuedPay;

    private void Awake()
    {
        resourceManager.collectedResources.Changed += OnResourcesChanged;
        resourceManager.queuedPayResourceCommands.Changed += TotalQueuePayAmount;
        resourceManager.queuedGetResources.Changed += TotalQueueGetAmount;
        UpdateQueueAmount();
        OnResourcesChanged(resourceManager.collectedResources);
    }

    private void OnResourcesChanged(float amount)
    {
        counter.text = $"{amount:0.}";
    }

    private void UpdateQueueAmount()
    {
        if (deltaQueue == 0) queueCounter.text = "";
        else if (deltaQueue > 0)
        {
            queueCounter.color = positiveColor;
            queueCounter.text = $"+{Mathf.Abs(deltaQueue):0}";
        }
        else
        {
            queueCounter.color = negativeColor;
            queueCounter.text = $"-{Mathf.Abs(deltaQueue):0}";
        }
    }

    private void TotalQueuePayAmount(List<Command> queue)
    {
        queuedPay = queue.Select(p => p.resourceCost).Sum();
        UpdateQueueAmount();
    }

    private void TotalQueueGetAmount(List<Resource> queue)
    {
        queuedGet = queue.Select(r => r.amount).Sum();
        UpdateQueueAmount();
    }
}