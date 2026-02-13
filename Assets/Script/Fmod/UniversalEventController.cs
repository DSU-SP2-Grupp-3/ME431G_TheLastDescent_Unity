using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
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

    public enum MusicEventSelect
    {
        Test,
        Test2,
        Test3,
        FyraTest
    }

    public enum CharEventSelect
    {
        Test1,
        Test2,
    }

    public enum EnemyEventSelect
    {
        Test1,
        Test2,
        Tjockis,
    }

    public enum UIEventSelect
    {
        Test1,
        Test2,
    }

    public enum InterEventSelect
    {
        Test1,
        Test2,
    }

    public enum AmbienceEventSelect
    {
        Ambience1,
        Ambience2,
        Amby,
    }

    //Parametrar
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
    private Locator<AudioManager> aMLocator = new();
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
    [SerializeField] private MusicEventSelect musicEventSelect;
    [SerializeField] private CharEventSelect charEventSelect;
    [SerializeField] private EnemyEventSelect enemyEventSelect;
    [SerializeField] private UIEventSelect uiEventSelect;
    [SerializeField] private InterEventSelect interEventSelect;
    [SerializeField] private AmbienceEventSelect ambienceEventSelect;

    //Array
    [SerializeField] private EventParameter[] parameters;

    //Intergers
    private int eventIndex;
    #endregion
    //-Ma. Vi kommer behöva göra om detta till att ladda in markerade ljud i början av varje scen 
    //eftersom scriptable objects inte kan koppla sig till en audio manager innan scenen existerar.
    //ScriptableObjects är en potentiel lag maskin i dags läget.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (aMLocator.TryGet(out AudioManager audioManager)) { this.audioManager = audioManager; }
        else { new Exception("No audioManager registered in scene"); return; }

        CreateInstance();
        if (playOnStart)
        {
            Initiate();
            if (debugMode)
            {
                Debug.Log("[" + name + "] " + " was initiated on Start().");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (is3DEvent && !positionStatic)
        {
            //RuntimeManager.AttachInstanceToGameObject(SfxInstance, GetComponent<Transform>(), false);
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
                    Debug.Log("[" + name + "] " + " was initiated with StartWithParameters");
                }
            }
            PlayEvent();
            if (debugMode && dbInit)
            {
                Debug.Log("[" + name + "] " + " Initiated SFX Event");
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
                        Debug.Log("[" + name + "] " + " was initiated with StartWithParameters.");
                    }
                }
                PlayEvent();
                if (debugMode && dbInit)
                {
                    Debug.Log("[" + name + "] " + " was initiated with Play.");
                }
            }
            else if (SelectedActionType() == 2) //Stop
            {
                StopEvent();
                if (debugMode && dbInit)
                {
                    Debug.Log("[" + name + "] " + " was initiated with Stop.");
                }
            }
            else if (SelectedActionType() == 3) //Set Parameter
            {
                SetEventParameters();
                if (debugMode && dbInit)
                {
                    Debug.Log("[" + name + "] " + " was initiated with SetParameter.");
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
            Debug.Log("[" + name + "] " + "Event Type set to " + eventTypeSelect);
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
            Debug.Log("[" + name + "] " + "Action Type set to " + actionTypeSelect);
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
            Debug.Log("[" + name + "] " + "Sfx Type set to " + sfxTypeSelect);
        }
        return sfxType;
    }

    private void SetEventParameters()
    {
        if (parameters == null || parameters.Length == 0)
        {
            if (debugMode && dbSEP) { }
            Debug.LogError("[" + name + "] " + "Parameter array empty or null!");
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
                        if (debugMode && dbSEP) { Debug.Log("[" + name + "] " + x.name + " is set to " + x.value); }

                    }
                    else if (matchFound && valOver)
                    {
                        //Om parametervärdet överskrids
                        if (debugMode && dbSEP) { Debug.LogError("[" + name + "] " + x.name + " found, but its maximum value was outside of allowed range."); }

                    }
                    else
                    {
                        //Om parameternamnet ej hittas i eventet
                        if (debugMode && dbSEP) { Debug.LogError("[" + name + "] " + "Could not set parameter!" + x.name + " not found."); }


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
                        if (debugMode && dbSEP) { Debug.Log("[" + name + "] " + x.name + " is set to " + x.value); }
                    }
                    else if (matchFound && valOver)
                    {
                        //Om parametervärdet överskrids
                        if (debugMode && dbSEP) { Debug.LogError("[" + name + "] " + x.name + " found, but its maximum value was outside of allowed range."); }
                    }
                    else
                    {
                        //Om parameternamnet ej hittas i eventet
                        if (debugMode && dbSEP) { Debug.LogError("[" + name + "] " + "Could not set parameter!" + x.name + " not found."); }

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
                        if (debugMode && dbSEP) { Debug.Log("[" + name + "] " + x.name + " is set to " + x.value); }
                    }
                    else if (matchFound && valOver)
                    {
                        //Om parametervärdet överskrids
                        if (debugMode && dbSEP) { Debug.LogError("[" + name + "] " + x.name + " found, but its maximum value was outside of allowed range."); }
                    }
                    else
                    {
                        //Om parameternamnet ej hittas i eventet
                        if (debugMode && dbSEP) { Debug.LogError("[" + name + "] " + "Could not set parameter!" + x.name + " not found."); }

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
                if (audioManager.muRef[i].Path.Contains(musicEventSelect.ToString()))
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
                if (audioManager.ambRef[i].Path.Contains(ambienceEventSelect.ToString()))
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
                    if (audioManager.characterRef[i].Path.Contains(charEventSelect.ToString()))
                    {
                        eventIndex = i;
                    }
                }
            }
            else if (SelectedSfxType() == 2) //Enemy sfx
            {
                for (int i = 0; i < audioManager.enemyRef.Length; i++)
                {
                    if (audioManager.enemyRef[i].Path.Contains(enemyEventSelect.ToString()))
                    {
                        eventIndex = i;
                    }
                }
            }
            else if (SelectedSfxType() == 3) //UI sfx
            {
                for (int i = 0; i < audioManager.uiRef.Length; i++)
                {
                    if (audioManager.uiRef[i].Path.Contains(uiEventSelect.ToString()))
                    {
                        eventIndex = i;
                    }
                }
            }
            else if (SelectedSfxType() == 4) //Interactable sfx
            {
                for (int i = 0; i < audioManager.interactRef.Length; i++)
                {
                    if (audioManager.interactRef[i].Path.Contains(interEventSelect.ToString()))
                    {
                        eventIndex = i;
                    }
                }
            }
        }

        if (debugMode && dbSEI)
        {
            Debug.Log("[" + name + "] " + "Event Index set to " + eventIndex);
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

                if (debugMode && dbSET) { Debug.Log("[" + name + "] " + "Playing: " + path); }

                audioManager.muInstance.start();
            }
            else
            {
                if (debugMode && dbSET) { Debug.Log("[" + name + "] " + path + " is already playing!"); }
            }
        }
        else if (SelectedEventType() == 2) //Ambiance
        {
            audioManager.ambInstance.getPlaybackState(out PLAYBACK_STATE state);
            audioManager.ambInstance.getDescription(out EventDescription desc);
            desc.getPath(out string path);
            if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
            {
                if (debugMode && dbSET) { Debug.Log("[" + name + "] " + "Playing: " + path); }
                audioManager.ambInstance.start();
            }
            else
            {
                if (debugMode && dbSET) { Debug.Log("[" + name + "] " + path + " is already playing!"); }
            }
        }
        else if (SelectedEventType() == 3) //Sfx
        {
            SfxInstance.getPlaybackState(out PLAYBACK_STATE state);
            SfxInstance.getDescription(out EventDescription desc);
            desc.getPath(out string path);
            if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
            {
                if (debugMode && dbSET) { Debug.Log("[" + name + "] " + "Playing: " + path); }
                SfxInstance.start();
                if (toggleRelease)
                {
                    if (debugMode && dbSET) { Debug.Log("[" + name + "] " + "Release toggled ON for: " + path); }
                    SfxInstance.release();
                }
            }
            else
            {
                if (debugMode && dbSET) { Debug.Log("[" + name + "] " + path + " is already playing!"); }
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
                if (debugMode && dbStop) { Debug.Log("[" + name + "] " + "Stopping: " + path); }
                audioManager.muInstance.stop(allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
                audioManager.muInstance.release();
            }
            else
            {
                if (debugMode && dbStop) { Debug.Log("[" + name + "] " + path + " is already stopped!"); }
            }
        }
        else if (SelectedEventType() == 2) //Ambiance
        {
            audioManager.ambInstance.getPlaybackState(out PLAYBACK_STATE state);
            audioManager.ambInstance.getDescription(out EventDescription desc);
            desc.getPath(out string path);
            if (state == PLAYBACK_STATE.PLAYING)
            {
                if (debugMode && dbStop) { Debug.Log("[" + name + "] " + "Stopping: " + path); }
                audioManager.ambInstance.stop(allowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
                audioManager.ambInstance.release();
            }
            else
            {
                if (debugMode && dbStop) { Debug.Log("[" + name + "] " + path + " is already stopped!"); }
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
                Debug.Log("[" + name + "] " + "muInstance is created with " + audioManager.muRef[eventIndex].Path);
            }
        }
        else if (SelectedEventType() == 2) //Ambiance
        {
            audioManager.ambInstance = RuntimeManager.CreateInstance(audioManager.ambRef[eventIndex]);
            if (debugMode && dbCreate)
            {
                Debug.Log("[" + name + "] " + "ambInstance is created with " + audioManager.ambRef[eventIndex].Path);
            }
        }
        else if (SelectedEventType() == 3)  //Sfx
        {
            CheckFor3D();
            if (is3DEvent && positionStatic)
            {
                //RuntimeManager.AttachInstanceToGameObject(SfxInstance, GetComponent<Transform>(), true);
                if (debugMode && dbCreate)
                {
                    Debug.Log("[" + name + "] " + "was attached with SfxInstance, and is positioned statically.");
                }
            }
            if (SelectedSfxType() == 1) //Character
            {
                SfxInstance = RuntimeManager.CreateInstance(audioManager.characterRef[eventIndex]);
                if (debugMode && dbCreate)
                {
                    Debug.Log("[" + name + "] " + "SfxInstance is created with " + audioManager.characterRef[eventIndex].Path);
                }
            }
            else if (SelectedSfxType() == 2) //Enemy
            {
                SfxInstance = RuntimeManager.CreateInstance(audioManager.enemyRef[eventIndex]);
                if (debugMode && dbCreate)
                {
                    Debug.Log("[" + name + "] " + "SfxInstance is created with " + audioManager.enemyRef[eventIndex].Path);
                }
            }
            else if (SelectedSfxType() == 3) //UI
            {
                SfxInstance = RuntimeManager.CreateInstance(audioManager.uiRef[eventIndex]);
                if (debugMode && dbCreate)
                {
                    Debug.Log("[" + name + "] " + "SfxInstance is created with " + audioManager.uiRef[eventIndex].Path);
                }
            }
            else if (SelectedSfxType() == 4) //Interactable
            {
                SfxInstance = RuntimeManager.CreateInstance(audioManager.interactRef[eventIndex]);
                if (debugMode && dbCreate)
                {
                    Debug.Log("[" + name + "] " + "SfxInstance is created with " + audioManager.interactRef[eventIndex].Path);
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
            Debug.Log("Bajs");
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
