using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;


public class AudioManager : Service<AudioManager>
{
    [SerializeField]
    private List<EventScriptable> audioBanks = new();
    private Dictionary<GUID, EventPlayer> playingAudio;
    private readonly Queue<EventPlayer> removalQueue = new();
    private void Awake()
    {
        playingAudio = new();

        Register();
    }
    public void PlayAudioEvent(string name)
    {
        if (TryGet(name, out EventScriptable eventScriptable))
        {
            PlayAudioEvent(eventScriptable);
            return;
        }
        throw new Exception($"Null Reference error. name: {name} does not exist in the audiobank");
    }
    //Plays aduio from a monobehavior player.
    public void PlayAudioEvent(EventMono eventMono)
    {
        if (eventMono == null) throw new Exception("Null Reference error. Audio event does not exist");
        PlayAudio(eventMono.eventReference);

    }
    //Plays aduio from a Scriptable Object
    public void PlayAudioEvent(EventScriptable eventScriptable)
    {
        if (eventScriptable == null) throw new Exception("Null Reference error. Audio event does not exist");
        PlayAudio(eventScriptable.eventReference);
    }
    public void PlayAudio(EventReference eventReference)
    {
        playingAudio.TryGetValue(eventReference.Guid, out EventPlayer player);
        if (player != null)
        {
            player.PlayEvent();
            return;
        }
        CreatePlayer(eventReference, out EventPlayer eventPlayer);
        eventPlayer.PlayEvent();
    }

    public void StopAudioEvent(EventScriptable eventScriptable)
    {

    }
    //-Ma. We do NOT use FMOD's callbacks. They cause crashes, at random, because they are not on the main thread.
    //
    private void Update()
    {
        var finished = new List<EventReference>();
        foreach (var kvp in playingAudio)
        {
            var state = kvp.Value.eventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
            if (playbackState == PLAYBACK_STATE.STOPPED)
            {
                kvp.Value.eventInstance.release();
                finished.Add(kvp.Value.eventReference);
            }
        }
        foreach(EventReference eventReference in finished)
        {
            RemovePlayer(eventReference, out _);
        }

    }

    public void RemovePlayer(EventReference eventReference, out EventPlayer eventPlayer)
    {
        if (!playingAudio.TryGetValue(eventReference.Guid, out eventPlayer)) { eventPlayer = null; return; }
        playingAudio.Remove(eventReference.Guid);

    }
    public void CreatePlayer(EventReference eventReference, out EventPlayer eventPlayer)
    {
        eventPlayer = new EventPlayer(eventReference);
        playingAudio.Add(eventReference.Guid, eventPlayer);
    }



    public EventReference Get(string eventName)
    {
        return audioBanks.FirstOrDefault(p => p.eventName == eventName).eventReference;
    }

    //-Ma. Useless
    public bool TryGet(string eventName, out EventScriptable result)
    {
        result = audioBanks.FirstOrDefault(p => p.eventName == eventName);
        return result != null;
    }
}
