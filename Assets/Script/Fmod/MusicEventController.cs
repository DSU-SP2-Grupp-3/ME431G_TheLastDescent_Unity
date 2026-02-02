using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class MusicEventController : MonoBehaviour
{
    
    [Serializable]
    public struct EvParams
    {
        public string parName;
        public float parValue;
    }

    private int refIndex;
    
    [SerializeField] private FmodEvents eventName;
    [SerializeField] private  EvAction action;
    private GameObject aM;
    private AudioManager audioManager;
    
    [Serializable] private enum FmodEvents
    {
        Test,
        Test2,
        Test3,
        Test4,
    }

    [Serializable] private enum EvAction
    {
        None,
        Play,
        Stop,
        Set 
    }
    
    public EvParams[] evParamsArray;
    
    [Header("Settings")]
    public bool startWithParam; //Kör eventet med parametrar.
    public bool initiateOnStart; //Kör eventet då Start tillkallas.
    [SerializeField] private bool allowFadeOut = true;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aM = GameObject.FindGameObjectWithTag("AudioManager");
        audioManager = aM.GetComponent<AudioManager>();
        SetIndex();
        
        //Kollar om mu ref är tom.
        if (audioManager.muRef == null || audioManager.muRef.Length == 0)
        {
            Debug.Log("[" + gameObject.name + "] " + "The Array muRef is empty or not defined.");
        }
        else
        {
            audioManager.muInstance = RuntimeManager.CreateInstance(audioManager.muRef[refIndex]);

            if (initiateOnStart)
            {
                Initiate();
            }
        }
    }
    
    

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Vilken typ av action som ska utföras på det aktiva eventet.
    public void Initiate()
    {
        switch (action)
        {
            case EvAction.None:
                None();
                break;
            case EvAction.Play:
                if (startWithParam)
                {
                    SetEventParams();
                }
                PlayEvent();
                break;
            case EvAction.Stop:
                StopEvent();
                break;
            case EvAction.Set:
                SetEventParams();
                break;
        }
    }
    
    //Sätter värdet på refIndex, som bestämmer vilken EventReference ur listan muRef som tillkallas vid "CreateInstance" i Start. 
    private void SetIndex()
    {
        switch (eventName)
        {
            case FmodEvents.Test:
                refIndex = 0;
                Debug.Log("[" + gameObject.name + "] " + "Event index set to " + 0);
                break;
        }
    }
    
    //Debug för om eventet aldrig tilldelades en action.
    private void None()
    {
        audioManager.muInstance.getDescription(out EventDescription desc);
        desc.getPath(out string path);
        Debug.Log("[" + gameObject.name + "] " + path + " was initiated without an action.");
    }
    
    //Kollar om det aktiva tillståndet av eventet och spelar det.
    private void PlayEvent()
    {
        audioManager.muInstance.getPlaybackState(out PLAYBACK_STATE state);
        audioManager.muInstance.getDescription(out EventDescription desc);
        desc.getPath(out string path);
        if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
        {
            
            Debug.Log("[" + gameObject.name + "] " + "Playing: " + path);
            audioManager.muInstance.start();
        }
        else
        {
            Debug.Log("[" + gameObject.name + "] " + path + " is already playing!");
        }
    }
    
    //Kollar det aktiva tillståndet av eventet och stoppar det.
    private void StopEvent()
    {
        
        audioManager.muInstance.getPlaybackState(out PLAYBACK_STATE state);
        audioManager.muInstance.getDescription(out EventDescription desc);
        desc.getPath(out string path);
        if (state == PLAYBACK_STATE.PLAYING)
        {
            Debug.Log("[" + gameObject.name + "] " + "Stopping: " + path);
            audioManager.muInstance.stop(allowFadeOut ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
            audioManager.muInstance.release();
        }
        else
        {
            Debug.Log("[" + gameObject.name + "] " + path + " is already stopped!");
        }
    }
    
    //Sätter alla parametrarna ur parameter-arrayen för det aktiva fmod-eventet i muInstance.
    private void SetEventParams()
    {
        audioManager.muInstance.getDescription(out EventDescription desc);
        desc.getParameterDescriptionCount(out int count);
        
        //Kollar om evParamsArray har EvParams structs i sig
        if (evParamsArray == null || evParamsArray.Length == 0)
        {
            Debug.LogError("[" + gameObject.name + "] " + "Parameter array empty or not defined!");
        }
        else
        {   
            //Loopar igenom arrayen 
            foreach (var x in evParamsArray)
            {
                bool matchfound = false;
                bool valOver = true;
                
                for (int i = 0; i < count; i++)
                {
                    //Kollar om parametervärdena name och value överensstämmer med det aktiva eventet.
                    desc.getParameterDescriptionByIndex(i, out PARAMETER_DESCRIPTION paraDesc);
                    if (paraDesc.name == x.parName)
                    {
                        matchfound = true;
                    }

                    if (x.parValue <= paraDesc.maximum)
                    {
                        valOver = false;
                    }
                    
                }
                
                if (matchfound && valOver == false)
                {
                    //Vid korrekt namngiven parameter och värdet är inom parameterns maximala värde.
                    audioManager.muInstance.setParameterByName(x.parName, x.parValue);
                    Debug.Log("[" + gameObject.name + "] " + x.parName + " is set to " + x.parValue);
                }
                else if (matchfound && valOver) 
                {
                    //Om parametervärdet överskrids
                    Debug.LogError("[" + gameObject.name + "] " + x.parName+ " found, but its maximum value was outside of allowed range.");
                }
                else
                {
                    //Om parameternamnet ej hittas i eventet
                    Debug.LogError("[" + gameObject.name + "] " + "Could not set parameter!" + x.parName + " not found.");
                    
                }
                
                
            }
            
        }
        
        
    }
    
    
    
}
