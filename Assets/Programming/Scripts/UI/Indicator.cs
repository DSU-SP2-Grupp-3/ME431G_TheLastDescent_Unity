using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Indicator : Service<Indicator>
{
    [SerializeField]
    private SelectionIndicator selectionIndicatorPrefab;
    private List<SelectionIndicator> selectionIndicators;
    private Dictionary<Transform, SelectionIndicator> activeIndicators;

    private void Awake()
    {
        Register();
        selectionIndicators = new();
        activeIndicators = new();
    }

    public void GetIndicator(Transform target)
    {
        if (activeIndicators.ContainsKey(target)) return;
        SelectionIndicator selectionIndicator;
        IEnumerable<SelectionIndicator> availableIndicators = selectionIndicators.Where(s => !s.active);
        if (availableIndicators.Any())
        {
            selectionIndicator = availableIndicators.First();
        }
        else
        {
            selectionIndicator = AddSelectionIndicator();
        }

        selectionIndicator.SetIndicatorTarget(target);
        activeIndicators.Add(target, selectionIndicator);
    }

    public void DisableIndicator(Transform target)
    {
        if (!activeIndicators.ContainsKey(target)) return;
        activeIndicators[target].DisableIndicator();
        activeIndicators.Remove(target);
    }

    private SelectionIndicator AddSelectionIndicator()
    {
        SelectionIndicator selectionIndicator = Instantiate(selectionIndicatorPrefab, transform);
        selectionIndicator.DisableIndicator();
        selectionIndicators.Add(selectionIndicator);
        return selectionIndicator;
    }
}