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
    private Dictionary<GUID, EventPlayer> PersistentPlayers;
    private List<EventPlayer> OneShotPlayers = new();
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
        name = name.ToLower().Trim();
        if (TryGet(name, out EventScriptable eventScriptable))
        {
            PlayAudio(eventScriptable);
            return;
        }
        throw new Exception($"Null Reference error. name: {name} does not exist in the audiobank");
    }
    public void PlayAudioEvent(string name, GameObject gameObject)
    {
        name = name.ToLower().Trim();
        if (TryGet(name, out EventScriptable eventScriptable))
        {
            CreatePlayer(eventScriptable, out EventPlayer player);
            player.AttachToGameObject(gameObject);
            player.PlayEvent();
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
        PlayAudio(eventScriptable);
    }


    private void PlayAudio(EventScriptable eventScriptable)
    {
        if (eventScriptable.type == EventScriptable.Override.multi)
        {
            CreatePlayer(eventScriptable, out EventPlayer tempEvent);
            tempEvent.PlayEvent();
            return;
        }
        PersistentPlayers.TryGetValue(eventScriptable.eventReference.Guid, out EventPlayer player);
        if (player != null)
        {
            player.PlayEvent();
            return;
        }
        CreatePlayer(eventScriptable, out EventPlayer eventPlayer);
        eventPlayer.PlayEvent();
    }
    #endregion PlayEvents


    public void StopAudioEvent(EventScriptable eventScriptable)
    {
        RemovePlayer(eventScriptable.eventReference);
    }
    public void StopAudioEvent(string name)
    {
        name = name.ToLower().Trim();
        if (TryGet(name, out EventScriptable result)) StopAudioEvent(result);
    }


    public void RunInstanceModification(EventScriptable eventScriptable, string paramName, float value)
    {
        if(PersistentPlayers.TryGetValue(eventScriptable.eventReference.Guid, out EventPlayer player))
        player.RunInstanceModification(paramName, value);
    }
    public void RunInstanceModification(EventMono eventMono, string paramName, float value)
    {
        eventMono.eventPlayer.RunInstanceModification(paramName, value);
    }
    public void RunInstanceModification(string name, string paramName, float value)
    {
        name = name.ToLower().Trim();
        if (TryGet(name, out EventScriptable result)) RunInstanceModification(result, paramName, value);
    }



    //-Ma. We do NOT use FMOD's callbacks. They cause crashes, at random, because they are not on the main thread.
    private void Update()
    {
        var finished = new List<EventPlayer>();
        var persistentFinished = new List<GUID>();
        foreach (var player in OneShotPlayers)
        {
            if (player.IsFinished())
            {
                finished.Add(player);
            }
        }
        foreach (var kvp in PersistentPlayers)
        {
            if (kvp.Value.IsFinished())
            {
                persistentFinished.Add(kvp.Key);
            }
        }
        foreach (var player in finished)
        {
            RemovePlayer(player);
        }

        foreach (var guid in persistentFinished)
        {
            RemovePlayer(new EventReference { Guid = guid });
        }

    }


    #region PlayerHandler

    public bool IsPlaying(EventReference eventReference)
    {
        if (TryGet(eventReference, out EventPlayer player))
        {
            return player.IsFinished();
        }
        return true;

    }

    public void RemovePlayer(EventPlayer eventPlayer)
    {
        eventPlayer.eventInstance.release();
        OneShotPlayers.Remove(eventPlayer);
    }


    public void RemovePlayer(EventReference eventReference)
    {
        if (!PersistentPlayers.TryGetValue(eventReference.Guid, out EventPlayer eventPlayer)) { return; }
        eventPlayer.eventInstance.release();
        PersistentPlayers.Remove(eventReference.Guid);
    }


    public void CreatePlayer(EventScriptable eventScriptable, out EventPlayer eventPlayer)
    {
        eventPlayer = new EventPlayer(eventScriptable.eventReference);
        if (!eventPlayer.isOneshot() || eventScriptable.type == EventScriptable.Override.persistent) PersistentPlayers.Add(eventScriptable.eventReference.Guid, eventPlayer);
        else OneShotPlayers.Add(eventPlayer);
    }
    #endregion PlayerHandler


    public EventScriptable Get(string eventName)
    {
        string eventNameC = eventName.ToLower().Trim();
        EventScriptable result = Resources.Load<EventScriptable>(eventName);
        return result;
    }


    public bool TryGet(string eventName, out EventScriptable result)
    {
        string eventNameC = eventName.ToLower().Trim();
        result = Resources.Load<EventScriptable>(eventName);
        return result != null;
    }
    public bool TryGet(EventReference eventReference, out EventPlayer result)
    {
        bool boolean = PersistentPlayers.TryGetValue(eventReference.Guid, out EventPlayer player);
        result = player;
        return boolean;
    }
}
