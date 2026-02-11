using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class UniversalEventController : MonoBehaviour
{
    #region Definitioner
    //Enumerables
    
    public enum EventTypeSelect
    {
        Music,
        Sfx,
        Ambience
    }

    public enum ActionTypeSelect
    {
        Play,
        Stop,
        SetParameter
    }

    public enum SfxTypeSelect
    {
        Characters,
        Enemy,
        UI,
        Interactable
    }
    
    
    //Lists
    public List<String> musicEventSelectList = new List<String>();
    public List<String> charEventSelectList = new List<String>();
    public List<String> enemyEventSelectList = new List<String>();
    public List<String> uiEventSelectList = new List<String>();
    public List<String> interEventSelectList = new List<String>();
    public List<String> ambienceEventSelectList = new List<String>();
    
    //Structs
    [System.Serializable]
    public struct EventParameter
    {
        public string name;
        public float value;
    }
    
    #endregion
    
    #region Deklarationer
    
    //Misc
    private GameObject aM;
    private AudioManager audioManager;
    public EventInstance SfxInstance;
    
    //Boolean
    [SerializeField] private bool playOnStart;
    [SerializeField] private bool startWithParameters;
    [SerializeField] private bool allowFadeout;
    [SerializeField] private bool toggleRelease;
    [SerializeField] private bool positionStatic;
    public bool is3DEvent = false;
    private bool initiated;
    
    [SerializeField] private bool debugMode;
    [Tooltip("Debug Initiate Method.")]
    [SerializeField] private bool dbInit;
    [Tooltip("Debug SelectedEventTypeMethod.")]
    [SerializeField] private bool dbSET;
    [Tooltip("Debug SelectedActionType Method.")]
    [SerializeField] private bool dbSAT;
    [Tooltip("Debug SelectedSfxType Method.")]
    [SerializeField] private bool dbSST;
    [Tooltip("Debug SetEventParameters Method.")]
    [SerializeField] private bool dbSEP;
    [Tooltip("Debug SetEventIndex Method.")]
    [SerializeField] private bool dbSEI;
    [Tooltip("Debug PlayEvent Method.")]
    [SerializeField] private bool dbPlay;
    [Tooltip("Debug StopEvent Method.")]
    [SerializeField] private bool dbStop;
    [Tooltip("Debug CreateInstance Method.")]
    [SerializeField] private bool dbCreate;
    
    //Enum
    [SerializeField] private EventTypeSelect eventTypeSelect;
    [SerializeField] private ActionTypeSelect actionTypeSelect;
    [SerializeField] private SfxTypeSelect sfxTypeSelect;
    
    //Array
    [SerializeField] private EventParameter[] parameters;
    
    //Intergers
    private int eventIndex;
    [SerializeField] private int muIndex;
    [SerializeField] private int ambIndex;
    [SerializeField] private int charIndex;
    [SerializeField] private int enemyIndex;
    [SerializeField] private int uiIndex;
    [SerializeField] private int interIndex;
    
    #endregion

    private void OnEnable()
    {
        
        aM = GameObject.FindGameObjectWithTag("AudioManager");
        audioManager = aM.GetComponent<AudioManager>();
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateInstance();
        if (playOnStart)
        {
            Initiate();
            if (debugMode)
            {
                Debug.Log("[" + gameObject.name + "] " + " was initiated on Start().");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (is3DEvent && !positionStatic)
        {
            RuntimeManager.AttachInstanceToGameObject(SfxInstance, gameObject.GetComponent<Transform>(), false);
        }
    }

    public void Initiate()
    {
        if (SelectedEventType() == 2) //Skriv förklaring
        {
            if (startWithParameters)
            {
                SetEventParameters();
                if (debugMode && dbInit)
                {
                    Debug.Log("[" + gameObject.name + "] " + " was initiated with StartWithParameters");
                }
            }
            PlayEvent();
            if (debugMode && dbInit)
            {
                Debug.Log("[" + gameObject.name + "] " + " Initiated SFX Event");
            }
        }
        else
        {
            if (SelectedActionType() == 1) //Play
            {
                if (startWithParameters)
                {
                    SetEventParameters();
                    if (debugMode && dbInit)
                    {
                        Debug.Log("[" + gameObject.name + "] " + " was initiated with StartWithParameters.");
                    }
                }
                PlayEvent();
                if (debugMode && dbInit)
                {
                    Debug.Log("[" + gameObject.name + "] " + " was initiated with Play.");
                }
            }
            else if (SelectedActionType() == 2) //Stop
            {
                StopEvent();
                if (debugMode && dbInit)
                {
                    Debug.Log("[" + gameObject.name + "] " + " was initiated with Stop.");
                }
            }
            else if (SelectedActionType() == 3) //Set Parameter
            {
                SetEventParameters();
                if (debugMode && dbInit)
                {
                    Debug.Log("[" + gameObject.name + "] " + " was initiated with SetParameter.");
                }
            }
        }
        
    }

    private int SelectedEventType()
    {
        int eventType = 0;
        switch (eventTypeSelect)
        {
            case EventTypeSelect.Music:
                eventType = 1;
                break;
            case EventTypeSelect.Ambience:
                eventType = 2;
                break;
            case EventTypeSelect.Sfx:
                eventType = 3;
                break;
        }
        if (debugMode && dbSET)
        {
            Debug.Log("[" + gameObject.name + "] " + "Event Type set to " + eventTypeSelect);
        }
        return eventType;
    }

    private int SelectedActionType()
    {
        int actionType = 0;
        switch (actionTypeSelect)
        {
            case ActionTypeSelect.Play:
                actionType = 1;
                break;
            case ActionTypeSelect.Stop:
                actionType = 2;
                break;
            case ActionTypeSelect.SetParameter:
                actionType = 3;    
                break;
        }
        if (debugMode && dbSAT)
        {
            Debug.Log("[" + gameObject.name + "] " + "Action Type set to " + actionTypeSelect);
        }
        return actionType;
    }

    private int SelectedSfxType()
    {
        int sfxType = 0;
        switch (sfxTypeSelect)
        {
            case SfxTypeSelect.Characters:
                sfxType = 1;
                break;
            case SfxTypeSelect.Enemy:
                sfxType = 2;
                break;
            case SfxTypeSelect.UI:
                sfxType = 3;
                break;
            case SfxTypeSelect.Interactable:
                sfxType = 4;
                break;
        }

        if (debugMode && dbSST)
        {
            Debug.Log("[" + gameObject.name + "] " + "Sfx Type set to " + sfxTypeSelect);
        }
        return sfxType;
    }

    private void SetEventParameters()
    {
        if (parameters == null || parameters.Length == 0)
        {
            if (debugMode && dbSEP){}
            Debug.LogError("[" + gameObject.name + "] " + "Parameter array empty or null!");
        }
        else
        {
            if (SelectedEventType() == 1) //Music
            {
                audioManager.muInstance.getDescription(out EventDescription desc);
                desc.getParameterDescriptionCount(out int count);

                foreach (var x in parameters)
                {
                    bool matchFound = false;
                    bool valOver = true;

                    for (int i = 0; i < count; i++)
                    {
                        //Kollar om parametervärdena name och value överensstämmer med det aktiva eventet.
                        desc.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION paraDesc);
                        if (paraDesc.name == x.name)
                        {
                            matchFound = true;
                        }

                        if (x.value <= paraDesc.maximum)
                        {
                            valOver = false;
                        }
                        
                    }
                    
                    if (matchFound && valOver == false)
                    {
                        //Vid korrekt namngiven parameter och värdet är inom parameterns maximala värde.
                        audioManager.muInstance.setParameterByName(x.name, x.value);
                        if (debugMode && dbSEP){Debug.Log("[" + gameObject.name + "] " + x.name + " is set to " + x.value);}
                        
                    }
                    else if (matchFound && valOver) 
                    {
                        //Om parametervärdet överskrids
                        if (debugMode && dbSEP){Debug.LogError("[" + gameObject.name + "] " + x.name+ " found, but its maximum value was outside of allowed range.");}
                        
                    }
                    else
                    {
                        //Om parameternamnet ej hittas i eventet
                        if (debugMode && dbSEP){Debug.LogError("[" + gameObject.name + "] " + "Could not set parameter!" + x.name + " not found.");}
                        
                    
                    }
                    
                }
            }
            else if (SelectedEventType() == 2) //Ambiance
            {
                audioManager.ambInstance.getDescription(out EventDescription desc);
                desc.getParameterDescriptionCount(out int count);

                foreach (var x in parameters)
                {
                    bool matchFound = false;
                    bool valOver = true;

                    for (int i = 0; i < count; i++)
                    {
                        //Kollar om parametervärdena name och value överensstämmer med det aktiva eventet.
                        desc.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION paraDesc);
                        if (paraDesc.name == x.name)
                        {
                            matchFound = true;
                        }

                        if (x.value <= paraDesc.maximum)
                        {
                            valOver = false;
                        }
                        
                    }
                    
                    if (matchFound && valOver == false)
                    {
                        //Vid korrekt namngiven parameter och värdet är inom parameterns maximala värde.
                        audioManager.ambInstance.setParameterByName(x.name, x.value);
                        if (debugMode && dbSEP){Debug.Log("[" + gameObject.name + "] " + x.name + " is set to " + x.value);}
                    }
                    else if (matchFound && valOver) 
                    {
                        //Om parametervärdet överskrids
                        if (debugMode && dbSEP){Debug.LogError("[" + gameObject.name + "] " + x.name+ " found, but its maximum value was outside of allowed range.");}
                    }
                    else
                    {
                        //Om parameternamnet ej hittas i eventet
                        if (debugMode && dbSEP){Debug.LogError("[" + gameObject.name + "] " + "Could not set parameter!" + x.name + " not found.");}
                    
                    }
                    
                }
            }
            else if (SelectedEventType() == 3) //Sfx
            {
                SfxInstance.getDescription(out EventDescription desc);
                desc.getParameterDescriptionCount(out int count);

                foreach (var x in parameters)
                {
                    bool matchFound = false;
                    bool valOver = true;

                    for (int i = 0; i < count; i++)
                    {
                        //Kollar om parametervärdena name och value överensstämmer med det aktiva eventet.
                        desc.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION paraDesc);
                        if (paraDesc.name == x.name)
                        {
                            matchFound = true;
                        }

                        if (x.value <= paraDesc.maximum)
                        {
                            valOver = false;
                        }
                        
                    }
                    
                    if (matchFound && valOver == false)
                    {
                        //Vid korrekt namngiven parameter och värdet är inom parameterns maximala värde.
                        SfxInstance.setParameterByName(x.name, x.value);
                        if (debugMode && dbSEP){Debug.Log("[" + gameObject.name + "] " + x.name + " is set to " + x.value);}
                    }
                    else if (matchFound && valOver) 
                    {
                        //Om parametervärdet överskrids
                        if (debugMode && dbSEP){Debug.LogError("[" + gameObject.name + "] " + x.name+ " found, but its maximum value was outside of allowed range.");}
                    }
                    else
                    {
                        //Om parameternamnet ej hittas i eventet
                        if (debugMode && dbSEP){Debug.LogError("[" + gameObject.name + "] " + "Could not set parameter!" + x.name + " not found.");}
                    
                    }
                    
                }
            }
        }
        
    }
    
    private void SetEventIndex()
    {
        if (SelectedEventType() == 1) //Music
        {
            for (int i = 0; i < audioManager.muRef.Length; i++)
            {
                if (audioManager.muRef[i].Path.Contains(musicEventSelectList[muIndex]))
                {
                    eventIndex = i;
                    break;
                }
            }
        }
        else if (SelectedEventType() == 2) //Ambiance
        {
            for (int i = 0; i < audioManager.ambRef.Length; i++)
            {
                if (audioManager.ambRef[i].Path.Contains(ambienceEventSelectList[ambIndex]))
                {
                    eventIndex = i;
                    break;
                }
            }
        }
        else if (SelectedEventType() == 3) //Sfx
        {
            if (SelectedSfxType() == 1) //Character sfx
            {
                for (int i = 0; i < audioManager.characterRef.Length; i++)
                {
                    if (audioManager.characterRef[i].Path.Contains(charEventSelectList[charIndex]))
                    {
                        eventIndex = i;
                        break;
                    }
                }
            }
            else if (SelectedSfxType() == 2) //Enemy sfx
            {
                for (int i = 0; i < audioManager.enemyRef.Length; i++)
                {
                    if (audioManager.enemyRef[i].Path.Contains(enemyEventSelectList[enemyIndex]))
                    {
                        eventIndex = i;
                        break;
                    }
                }
            }
            else if (SelectedSfxType() == 3) //UI sfx
            {
                for (int i = 0; i < audioManager.uiRef.Length; i++)
                {
                    if (audioManager.uiRef[i].Path.Contains(uiEventSelectList[uiIndex]))
                    {
                        eventIndex = i;
                        break;
                    }
                }
            }
            else if (SelectedSfxType() == 4) //Interactable sfx
            {
                for (int i = 0; i < audioManager.interactRef.Length; i++)
                {
                    if (audioManager.interactRef[i].Path.Contains(interEventSelectList[interIndex]))
                    {
                        eventIndex = i;
                        break;
                    }
                }
            }
        }
        
        if (debugMode && dbSEI)
        {
            Debug.Log("[" + gameObject.name + "] " + "Event Index set to " + eventIndex);
        }
    }

    private void PlayEvent()
    {
        if (SelectedEventType() == 1) //Music
        {
            audioManager.muInstance.getPlaybackState(out PLAYBACK_STATE state);
            audioManager.muInstance.getDescription(out EventDescription desc);
            desc.getPath(out string path);
            if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
            {
            
                if (debugMode && dbSET){Debug.Log("[" + gameObject.name + "] " + "Playing: " + path);}
                
                audioManager.muInstance.start();
            }
            else
            {
                if (debugMode && dbSET){Debug.Log("[" + gameObject.name + "] " + path + " is already playing!");}
            }
        }
        else if (SelectedEventType() == 2) //Ambiance
        {
            audioManager.ambInstance.getPlaybackState(out PLAYBACK_STATE state);
            audioManager.ambInstance.getDescription(out EventDescription desc);
            desc.getPath(out string path);
            if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
            {
                if (debugMode && dbSET){Debug.Log("[" + gameObject.name + "] " + "Playing: " + path);}
                audioManager.ambInstance.start();
            }
            else
            {
                if (debugMode && dbSET){Debug.Log("[" + gameObject.name + "] " + path + " is already playing!");}
            }
        }
        else if (SelectedEventType() == 3) //Sfx
        {
            SfxInstance.getPlaybackState(out PLAYBACK_STATE state);
            SfxInstance.getDescription(out EventDescription desc);
            desc.getPath(out string path);
            if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
            {
                if (debugMode && dbSET){Debug.Log("[" + gameObject.name + "] " + "Playing: " + path);}
                SfxInstance.start();
                if (toggleRelease)
                {
                    if (debugMode && dbSET){Debug.Log("[" + gameObject.name + "] " + "Release toggled ON for: " + path);}
                    SfxInstance.release();
                }
            }
            else
            {
                if (debugMode && dbSET) {Debug.Log("[" + gameObject.name + "] " + path + " is already playing!");}
            }
        }
        
    }

    private void StopEvent()
    {
        if (SelectedEventType() == 1) //Music
        {
            audioManager.muInstance.getPlaybackState(out PLAYBACK_STATE state);
            audioManager.muInstance.getDescription(out EventDescription desc);
            desc.getPath(out string path);
            if (state == PLAYBACK_STATE.PLAYING)
            {
                if (debugMode && dbStop){Debug.Log("[" + gameObject.name + "] " + "Stopping: " + path);}
                audioManager.muInstance.stop(allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
                audioManager.muInstance.release();
            }
            else
            {
                if (debugMode && dbStop){Debug.Log("[" + gameObject.name + "] " + path + " is already stopped!");}
            }
        }
        else if (SelectedEventType() == 2) //Ambiance
        {
            audioManager.ambInstance.getPlaybackState(out PLAYBACK_STATE state);
            audioManager.ambInstance.getDescription(out EventDescription desc);
            desc.getPath(out string path);
            if (state == PLAYBACK_STATE.PLAYING)
            {
                if (debugMode && dbStop){Debug.Log("[" + gameObject.name + "] " + "Stopping: " + path);}
                audioManager.ambInstance.stop(allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
                audioManager.ambInstance.release();
            }
            else
            {
                if (debugMode && dbStop){Debug.Log("[" + gameObject.name + "] " + path + " is already stopped!");}
            }
        }
    }

    private void CreateInstance()
    {
        SetEventIndex();
        if (SelectedEventType() == 1) //Music
        {
            audioManager.muInstance = RuntimeManager.CreateInstance(audioManager.muRef[eventIndex]);
            if (debugMode && dbCreate)
            {
                Debug.Log("[" + gameObject.name + "] " + "muInstance is created with " + audioManager.muRef[eventIndex].Path);
            }
        }
        else if (SelectedEventType() == 2) //Ambiance
        {
            audioManager.ambInstance = RuntimeManager.CreateInstance(audioManager.ambRef[eventIndex]);
            if (debugMode && dbCreate)
            {
                Debug.Log("[" + gameObject.name + "] " + "ambInstance is created with " + audioManager.ambRef[eventIndex].Path);
            }
        }
        else if (SelectedEventType() == 3)  //Sfx
        {
            CheckFor3D();
            if (is3DEvent && positionStatic)
            {
                RuntimeManager.AttachInstanceToGameObject(SfxInstance, gameObject.GetComponent<Transform>(), true);
                if (debugMode && dbCreate)
                {
                    Debug.Log("[" + gameObject.name + "] " + "was attached with SfxInstance, and is positioned statically.");
                }
            }
            if (SelectedSfxType() == 1) //Character
            {
                SfxInstance = RuntimeManager.CreateInstance(audioManager.characterRef[eventIndex]);
                if (debugMode && dbCreate)
                {
                    Debug.Log("[" + gameObject.name + "] " + "SfxInstance is created with " + audioManager.characterRef[eventIndex].Path);
                }
            }
            else if (SelectedSfxType() == 2) //Enemy
            {
                SfxInstance = RuntimeManager.CreateInstance(audioManager.enemyRef[eventIndex]);
                if (debugMode && dbCreate)
                {
                    Debug.Log("[" + gameObject.name + "] " + "SfxInstance is created with " + audioManager.enemyRef[eventIndex].Path);
                }
            }
            else if (SelectedSfxType() == 3) //UI
            {
                SfxInstance = RuntimeManager.CreateInstance(audioManager.uiRef[eventIndex]);
                if (debugMode && dbCreate)
                {
                    Debug.Log("[" + gameObject.name + "] " + "SfxInstance is created with " + audioManager.uiRef[eventIndex].Path);
                }
            }
            else if (SelectedSfxType() == 4) //Interactable
            {
                SfxInstance = RuntimeManager.CreateInstance(audioManager.interactRef[eventIndex]);
                if (debugMode && dbCreate)
                {
                    Debug.Log("[" + gameObject.name + "] " + "SfxInstance is created with " + audioManager.interactRef[eventIndex].Path);
                }
            }
        }
    }

    private void CheckFor3D()
    {
        if (SelectedSfxType() == 1)
        {
            RuntimeManager.StudioSystem.getEvent(audioManager.characterRef[eventIndex].Path, out EventDescription desc);
            desc.is3D(out bool is3D);
            if (is3D)
            {
                is3DEvent = true;
            }
        }
        else if (SelectedSfxType() == 2)
        {
            RuntimeManager.StudioSystem.getEvent(audioManager.enemyRef[eventIndex].Path, out EventDescription desc);
            desc.is3D(out bool is3D);
            if (is3D)
            {
                is3DEvent = true;
            }
        }
        else if (SelectedSfxType() == 3)
        {
            RuntimeManager.StudioSystem.getEvent(audioManager.uiRef[eventIndex].Path, out EventDescription desc);
            desc.is3D(out bool is3D);
            if (is3D)
            {
                is3DEvent = true;
            }
        }

        else if (SelectedSfxType() == 4)
        {
            RuntimeManager.StudioSystem.getEvent(audioManager.interactRef[eventIndex].Path, out EventDescription desc);
            desc.is3D(out bool is3D);
            if (is3D)
            {
                is3DEvent = true;
            }
        }
        
    }
    
}
