using System;
using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;


public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music Events")] 
    public EventReference[] muRef;
    
    [Header("Ambiance Events")]
    public EventReference[] ambianceRef;
    
    [Header("Oneshot Events")]
    
    public EventReference[] oneRef;
    
    [HideInInspector] public EventInstance muInstance; //Music
    [HideInInspector] public EventInstance amInstance; //Ambiance
    
    [Header("Debug Mode")]
    public bool debug = false;
    public bool inCombat = false;
    [Range(0, 10)] public float combatTime = 5f;
    
    
   void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
        
    } 
   public void PlayEvent(int i)
    {
        muInstance = RuntimeManager.CreateInstance(muRef[i]);
        muInstance.getPlaybackState(out PLAYBACK_STATE state);
        if (state == PLAYBACK_STATE.STOPPED || state == PLAYBACK_STATE.STOPPING)
        {
            muInstance.start();
        }

    }

    public void StopEvent()
    {
        muInstance.stop(STOP_MODE.ALLOWFADEOUT);
    }

    public void SetEventParam(float i, string namePar)
    {
        muInstance.setParameterByName(namePar, i, false);
    }

    public void Combat()
    {
        if (!inCombat)
        {
            RuntimeManager.StudioSystem.setParameterByName("Combat", 1);
            StartCoroutine(CombatTimer());
            inCombat = true;
        }
        else if (inCombat)
        {
            StopAllCoroutines();
            StartCoroutine(CombatTimer());
        }
    }

    private IEnumerator CombatTimer()
    {
        yield return new WaitForSeconds(combatTime);
        RuntimeManager.StudioSystem.setParameterByName("Combat", 0);
        inCombat = false;
    }
    
}
