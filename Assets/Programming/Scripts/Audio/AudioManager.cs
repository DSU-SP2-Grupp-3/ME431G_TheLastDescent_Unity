using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

//-Ma. You will see a lot of repeated code here due to Unity Events.
//General rules to follow.
//Scriptable refreneces will not hold any reference to any instance. They are used as banks for sounds. It is possible to link, if the need should arise.
//Scriptable references 
public class AudioManager : Service<AudioManager>
{
    [SerializeField]
    private static Dictionary<EventReference, EventPlayer> PersistentPlayers = new();
    private static Dictionary<string, EventScriptable> bank = new();
    private static List<EventPlayer> OneShotPlayers = new();
    private static EventPlayer MusicPlayer;
    private void Awake()
    {
        Register();

        var all = Resources.LoadAll<EventScriptable>("");

        foreach (var e in all)
        {
            bank[e.name.ToLower().Trim()] = e;
        }
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
        if (eventScriptable.isMusic)
        {
            PlayMusic(eventScriptable);
            return;
        }
        if (eventScriptable.type == EventScriptable.Override.multi)
        {
            CreatePlayer(eventScriptable, out EventPlayer tempEvent);
            tempEvent.PlayEvent();
            return;
        }
        if (TryGet(eventScriptable.eventReference, out EventPlayer player))
        {
            player.PlayEvent();
            return;
        }
        CreatePlayer(eventScriptable, out EventPlayer eventPlayer);
        eventPlayer.PlayEvent();
    }
    #endregion PlayEvents

    public void PlayMusic(EventScriptable eventScriptable)
    {
        StopMusicPlayer();

        CreatePlayer(eventScriptable, out EventPlayer musicPlayer);

        MusicPlayer = musicPlayer;
        musicPlayer.PlayEvent();
    }
    public void StopAudioEvent(EventScriptable eventScriptable)
    {
        if (eventScriptable.isMusic)
        {
            StopMusicPlayer();
            return;
        }
        RemovePlayer(eventScriptable.eventReference);
    }
    public void StopAudioEvent(string name)
    {
        name = name.ToLower().Trim();
        if (TryGet(name, out EventScriptable result)) StopAudioEvent(result);
    }
    public void StopMusicPlayer()
    {
        if (MusicPlayer == null) return;

        MusicPlayer.Stop();
        MusicPlayer.eventInstance.release();

        PersistentPlayers.Remove(MusicPlayer.eventReference);
        MusicPlayer = null;
    }
    private void StopAllPersistentPlayers()
    {
        foreach (var kvp in PersistentPlayers)
        {
            var player = kvp.Value;
            player.Stop(); // consider fadeout if needed
            player.eventInstance.release();
        }

        PersistentPlayers.Clear();
    }
    public void RunInstanceModification(EventScriptable eventScriptable, string paramName, float value)
    {
        if (eventScriptable.isMusic)
        {
            if (MusicPlayer != null)
            {
                MusicPlayer.RunInstanceModification(paramName, value);
            }
            return;
        }
        if (PersistentPlayers.TryGetValue(eventScriptable.eventReference, out EventPlayer player))
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
        foreach (var player in OneShotPlayers)
        {
            if (player.IsFinished())
            {
                finished.Add(player);
            }
        }
        foreach (var player in finished)
        {
            RemovePlayer(player);
        }
    }


    #region PlayerHandler

    public bool IsPlaying(EventReference eventReference)
    {
        if (TryGet(eventReference, out EventPlayer player))
        {
            return player.IsPlaying();
        }
        return false;

    }

    public void RemovePlayer(EventPlayer eventPlayer)
    {
        eventPlayer.Stop();
        eventPlayer.eventInstance.release();
        OneShotPlayers.Remove(eventPlayer);
    }


    public void RemovePlayer(EventReference eventReference)
    {
        if (!TryGet(eventReference, out EventPlayer player)) { return; }
        player.Stop();
        player.eventInstance.release();
        PersistentPlayers.Remove(eventReference);
    }


    public void CreatePlayer(EventScriptable eventScriptable, out EventPlayer eventPlayer)
    {
        eventPlayer = new EventPlayer(eventScriptable.eventReference);
        if (!eventPlayer.isOneshot() || eventScriptable.type == EventScriptable.Override.persistent)
        {
            PersistentPlayers.Add(eventScriptable.eventReference, eventPlayer);
        }
        else OneShotPlayers.Add(eventPlayer);
    }
    #endregion PlayerHandler
    public EventScriptable Get(string eventName)
    {
        string eventNameC = eventName.ToLower().Trim();
        EventScriptable result = Resources.Load<EventScriptable>(eventName);
        return result;
    }


    public bool TryGet(string name, out EventScriptable result)
    {
        return bank.TryGetValue(name.ToLower().Trim(), out result);
    }
    public bool TryGet(EventReference eventReference, out EventPlayer result)
    {
        bool boolean = PersistentPlayers.TryGetValue(eventReference, out EventPlayer player);
        result = player;
        return boolean;
    }
}
