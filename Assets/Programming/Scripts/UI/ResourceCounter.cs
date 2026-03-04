using TMPro;
using UnityEngine;

public class ResourceCounter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text counter, queueCounter;
    [SerializeField]
    private ResourceManager resourceManager;

    private void Awake()
    {
        resourceManager.collectedResources.Changed += OnResourcesChanged;
        resourceManager.queuedResources.Changed += OnQueuedResourcesChanged;
    }

    private void OnResourcesChanged(float amount)
    {
        counter.text = $"{amount:0.}";
    }

    private void OnQueuedResourcesChanged(float amount)
    {
        if (amount <= 0) queueCounter.text = "";
        else queueCounter.text = $"-{amount:0}";
    }
}