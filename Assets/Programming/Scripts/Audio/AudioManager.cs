using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;


public class AudioManager : Service<AudioManager>
{
    //-Ma. Simple enough start to the audio system,
    //Offers some variations of accessing how audio is played.
    //What is missing is a proper refrencing system for paramters, and other FMOD Utilites, although this will be added shortly.

    //-Ma. It should also be noted that music should be handled diffrently from basic audio files.
    //-Ma. the massive audio files prohibit us from
    [SerializeField]
    private List<EventScriptable> audioPlayers;
    private void Awake()
    {
        Register();
        LoadBanks();
    }
    private void LoadBanks()
    {
        foreach(EventScriptable es in audioPlayers)
        {
            es.CreateInstance();
        }
    }
    public void PlayAudioEvent(string name)
    {
        EventInstance player = audioPlayers.FirstOrDefault(p => p.name == name).eventInstance;
        player.start();
    }
    //Plays a monobehavior player.
    public void PlayAudioEvent(EventPlayer eventPlayer)
    {
        if(eventPlayer == null) throw new Exception("Null Refrence error. Audio event does not exist");
        eventPlayer.eventInstance.start();

    }
    public void PlayAudioEvent(EventScriptable eventPlayer)
    {
        if(eventPlayer == null) throw new Exception("Null Refrence error. Audio event does not exist");
        eventPlayer.eventInstance.start();

    }
    //Plays from banks with the provided eventRefrence. 
    // If none exists, Instansiates a runtime instance and puts it in the refrence list..
    public void PlayAudioEvent(EventReference eventReference)
    {
        EventScriptable player = audioPlayers.FirstOrDefault(p => p.fmodEvent.Guid == eventReference.Guid);
        if(player != null)
        {
            player.eventInstance.start();
            return;
        }
        player = ScriptableObject.CreateInstance<EventScriptable>();
        player.eventInstance = RuntimeManager.CreateInstance(eventReference);
        player.eventInstance.start();
    }
    public EventReference Get(string eventName)
    {
        return audioPlayers.FirstOrDefault(p => p.eventName == eventName).fmodEvent;
    }
    public EventReference Get(EventReference eventReference)
    {
        return audioPlayers.FirstOrDefault(p => p.fmodEvent.Guid == eventReference.Guid).fmodEvent;
    }
    public bool TryGet(string eventName, out EventReference result)
    {
        EventScriptable player = audioPlayers.FirstOrDefault(p => p.eventName == eventName);
        bool p = player != null;
        if (!p)
        {
            result = player.fmodEvent;
            return p;
        }
        result = player.fmodEvent;
        return player != null;
    }
}
