using UnityEngine;

public struct TriggerInfo
{
    private string startTrigger;
    private string endTrigger;

    public TriggerInfo(string startTrigger, string endTrigger)
    {
        this.startTrigger = startTrigger;
        this.endTrigger = endTrigger;
    }

    public string StartTrigger() => startTrigger;
    public string EndTrigger() => endTrigger;
}