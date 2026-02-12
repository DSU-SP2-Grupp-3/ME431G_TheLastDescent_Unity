using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
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
    
    [HideInInspector] public EventInstance muInstance; //Music
    [HideInInspector] public EventInstance ambInstance; //Ambiance
    
    [Header("Debug Mode")]
    public bool debug = false;
    public UnityEvent PreloadUECSFiles;
    
    
   void Awake()
    {
        Register();
        PreloadUECSFiles.Invoke();
    } 
}
