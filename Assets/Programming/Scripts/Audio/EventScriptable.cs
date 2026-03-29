using FMODUnity;
using UnityEngine;

/// <summary>
/// -Ma. ScriptableObjects act exclusivly as memory loaders and bank pointers.
/// They may NOT carry ANY event instances during runtime.
/// </summary>

[CreateAssetMenu(fileName = "EventScriptable", menuName = "Scriptable Objects/EventScriptable")]
public class EventScriptable : ScriptableObject
{
    public string eventName;
    public enum Override
    {
        persistent,
        multi
    }
    public Override type;
    public bool isMusic;
    [SerializeField]
    public EventReference eventReference;
    void OnValidate()
    {
        eventName = eventName.Trim().ToLower();
    }
}