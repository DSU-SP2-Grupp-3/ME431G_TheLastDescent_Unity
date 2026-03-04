using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RoundClock))]
public sealed class ModeSwitcher : Service<ModeSwitcher>
{
    public event Action<TurnManager> OnEnterTurnBased;
    public UnityEvent OnTurnBasedEntered;
    public UnityEvent OnTurnBasedEnteredForced;
    public event Action<TurnManager> OnEnterRealTime;
    public UnityEvent OnRealTimeEntered;
    public UnityEvent OnRealTimeEnteredForced;

    private Locator<RoundClock> roundClock;

    private Locator<TurnManager> turnManager;

    public RoundClock.ProgressMode mode => roundClock.Get().currentMode;

    private bool automaticTurnBasedEntrance;

    private void Awake()
    {
        Register();
        roundClock = new();
        turnManager = new();
    }

    private void Start()
    {
        TryEnterRealTime();
    }

    public bool TryEnterTurnBased(bool automatic = false)
    {
        automaticTurnBasedEntrance = automatic;
        EnterTurnBased();
        return true;
    }

    private void EnterTurnBased()
    {
        Debug.Log("Enter turn based");
        roundClock.Get().EnterTurnBased();
        OnEnterTurnBased?.Invoke(turnManager.Get());
        OnTurnBasedEntered?.Invoke();
        if (automaticTurnBasedEntrance) OnTurnBasedEnteredForced?.Invoke();
        turnManager.Get().Activate();
    }

    public bool TryEnterRealTime(bool forced = false)
    {
        if (!forced && automaticTurnBasedEntrance)
        {
            Debug.Log("Cannot enter real time");
            return false;
        }
        else if (forced && !automaticTurnBasedEntrance)
        {
            Debug.Log("Entered turn based manually, don't automatically exit");
            return false;
        }
        if (forced && automaticTurnBasedEntrance)
        {
            // turn based forced by enemies, and all enemies have died
            OnRealTimeEnteredForced?.Invoke();
        }
        EnterRealTime();
        return true;
    }

    private void EnterRealTime()
    {
        Debug.Log("Enter real time");
        roundClock.Get().EnterRealTime();
        OnEnterRealTime?.Invoke(turnManager.Get());
        OnRealTimeEntered?.Invoke();
        turnManager.Get().Deactivate();
    }
}