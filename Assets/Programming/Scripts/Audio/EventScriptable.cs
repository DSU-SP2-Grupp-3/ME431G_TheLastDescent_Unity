using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[CreateAssetMenu(fileName = "EventScriptable", menuName = "Scriptable Objects/EventScriptable")]
public class EventScriptable : ScriptableObject
{
    public string eventName;

    [SerializeField]
    public EventReference eventReference;
}