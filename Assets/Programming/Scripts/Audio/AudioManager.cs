using System;
using System.Collections.Generic;
using System.Linq;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

//-Ma. You will see a lot of repeated code here due to Unity Events.
//General rules to follow.
//Scriptable refreneces will not hold any reference to any instance. They are used as banks for sounds. It is possible to link, if the need should arise.
//Scriptable references 
public class AudioManager : Service<AudioManager>
{
    [SerializeField]
    private List<EventScriptable> audioBanks = new();
    private Dictionary<GUID, EventPlayer> PersistentPlayers;
    private List<EventPlayer> OneShotPlayers;
    private readonly Queue<EventPlayer> removalQueue = new();
    private void Awake()
    {
        PersistentPlayers = new();

        Register();
    }
    #region PlayEvents
    /// <summary>
    /// Plays audio from the bank reference.
    /// </summary>
    /// <param name="eventMono"></param>
    /// <exception cref="Exception">Exception upon null parameter value</exception>
    public void PlayAudioEvent(string name)
    {
        if (TryGet(name, out EventScriptable eventScriptable))
        {
            PlayAudio(eventScriptable.eventReference);
            return;
        }
        throw new Exception($"Null Reference error. name: {name} does not exist in the audiobank");
    }


    /// <summary>
    /// Plays audio from a monobehavior player.
    /// </summary>
    /// <param name="eventMono"></param>
    /// <exception cref="Exception">Exception upon null parameter value</exception>
    public void PlayAudioEvent(EventMono eventMono)
    {
        if (eventMono == null) throw new Exception("Null Reference error. Audio event does not exist");
        if (eventMono.eventPlayer == null) throw new Exception($"Null Reference error. EventPlayer is missing in: {eventMono}");
        eventMono.eventPlayer.PlayEvent();
    }


    /// <summary>
    /// Plays audio from banks or creates a new player. Uses Scriptables as a template for generating new sounds.
    /// </summary>
    /// <param name="eventScriptable"></param>
    /// <exception cref="Exception">Exception upon null parameter value</exception>
    public void PlayAudioEvent(EventScriptable eventScriptable)
    {
        if (eventScriptable == null) throw new Exception("Null Reference error. Audio event does not exist");
        PlayAudio(eventScriptable.eventReference);
    }

    private void PlayAudio(EventReference eventReference)
    {
        PersistentPlayers.TryGetValue(eventReference.Guid, out EventPlayer player);
        if (player != null)
        {
            player.PlayEvent();
            return;
        }
        CreatePlayer(eventReference, out EventPlayer eventPlayer);
        eventPlayer.PlayEvent();
    }
    #endregion PlayEvents

    public void StopAudioEvent(EventScriptable eventScriptable)
    {
        RemovePlayer(eventScriptable.eventReference);
    }
    public void RunInstanceModification(EventScriptable eventScriptable, string paramName, float value)
    {
        PersistentPlayers.TryGetValue(eventScriptable.eventReference.Guid, out EventPlayer player);
        player.RunInstanceModification(paramName, value);
    }
    public void RunInstanceModification(EventMono eventMono, string paramName, float value)
    {
        eventMono.RunInstanceModification(paramName, value);
    }
    public void RunInstanceModification(string name, string paramName, float value)
    {
        
    }

    //-Ma. We do NOT use FMOD's callbacks. They cause crashes, at random, because they are not on the main thread.
    private void Update()
    {
        var finished = new List<EventPlayer>();
        foreach (var player in OneShotPlayers)
        {
            if (player.IsFinished())
            {
                player.eventInstance.release();
                finished.Add(player);
            }
        }
        foreach(EventPlayer eventPlayer in finished)
        {
            RemovePlayer(eventPlayer);
        }

    }

    #region PlayerHandler
    public void RemovePlayer(EventPlayer eventPlayer)
    {
        if (!OneShotPlayers.Remove(eventPlayer)) OneShotPlayers.Remove(eventPlayer);

    }
    public void RemovePlayer(EventReference eventReference)
    {
        if (!PersistentPlayers.TryGetValue(eventReference.Guid, out _)) {return; }
        PersistentPlayers.Remove(eventReference.Guid);

    }
    public void CreatePlayer(EventReference eventReference, out EventPlayer eventPlayer)
    {
        eventPlayer = new EventPlayer(eventReference);
        if(eventPlayer.isOneshot()) OneShotPlayers.Add(eventPlayer);
        else PersistentPlayers.Add(eventReference.Guid, eventPlayer);
    }
    #endregion PlayerHandler


    public EventReference Get(string eventName)
    {
        return audioBanks.FirstOrDefault(p => p.eventName == eventName).eventReference;
    }

    public bool TryGet(string eventName, out EventScriptable result)
    {
        result = audioBanks.FirstOrDefault(p => p.eventName == eventName);
        return result != null;
    }
}
