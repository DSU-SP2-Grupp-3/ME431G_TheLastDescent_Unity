using TMPro;
using UnityEngine;

public class ResourceCounter : MonoBehaviour
{
    [SerializeField]
    private TMP_Text counter;
    [SerializeField]
    private ResourceManager resourceManager;

    private void Start()
    {
        resourceManager.ResourcesChanged += OnResourcesChanged;
        counter.text = "0";
    }

    private void OnResourcesChanged(float amount)
    {
        counter.text = $"{amount:0.}";
    }
}