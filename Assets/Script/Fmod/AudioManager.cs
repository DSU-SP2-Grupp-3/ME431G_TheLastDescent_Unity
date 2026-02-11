using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;


public class AudioManager : Service<AudioManager>
{
    [Header("Music Events")] 
    public EventReference[] muRef;
    [Header("Ambiance Events")]
    public EventReference[] ambRef;
    [Header("Sfx Events")] 
    [Header("Character Sound Effects")]
    public EventReference[] characterRef;
    [Header("Enemy Sound Effects")]
    public EventReference[] enemyRef;
    [Header("UI Sound Effects")]
    public EventReference[] uiRef;
    [Header("Interactables Sound Effects")]
    public EventReference[] interactRef;
    
    public List<EventReference> muEvents = new List<EventReference>();
    public List<EventReference> ambEvents = new List<EventReference>();
    public List<EventReference> characterEvents = new List<EventReference>();
    public List<EventReference> enemyEvents  = new List<EventReference>();
    public List<EventReference> uiEvents = new List<EventReference>();
    public List<EventReference> interactEvents = new List<EventReference>();
    
    [HideInInspector] public EventInstance muInstance; //Music
    [HideInInspector] public EventInstance ambInstance; //Ambiance
    
   void Awake()
    {
        Register();
    } 
    public void UpdateReferences()
    {
        muEvents.Clear();
        ambEvents.Clear();
        characterEvents.Clear();
        enemyEvents.Clear();
        uiEvents.Clear();
        interactEvents.Clear();
        
        var events = EventManager.Events;
        foreach (var x in events)
        {
            EventReference eventRef = new EventReference();
            
            eventRef.Path = x.Path;
            if (x.Path.Contains("/Music/"))
            {
                muEvents.Add(eventRef);
            }
            else if (x.Path.Contains("/AMB/"))
            {
                ambEvents.Add(eventRef);
            }
            else if (x.Path.Contains("/Character/"))
            {
                characterEvents.Add(eventRef);
            }
            else if (x.Path.Contains("/Enemies/"))
            {
                enemyEvents.Add(eventRef);
            }
            else if (x.Path.Contains("/UI/"))
            {
                uiEvents.Add(eventRef);
            }
            else if (x.Path.Contains("/Interactables/"))
            {
                interactEvents.Add(eventRef);
            }
        }

        muRef = muEvents.ToArray();
        ambRef = ambEvents.ToArray();
        characterRef = characterEvents.ToArray();
        enemyRef = enemyEvents.ToArray();
        uiRef = uiEvents.ToArray();
        interactRef = interactEvents.ToArray();

    }
}
