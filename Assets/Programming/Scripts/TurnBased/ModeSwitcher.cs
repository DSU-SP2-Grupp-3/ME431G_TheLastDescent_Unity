using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(RoundClock))]
public sealed class ModeSwitcher : Service<ModeSwitcher>
{
    public event Action<TurnManager> OnEnterTurnBased;
    public event Action<TurnManager> OnEnterRealTime;

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

    public bool TryEnterTurnBased(bool automatic = false)
    {
        automaticTurnBasedEntrance = automatic;
        EnterTurnBased();
        return true;
    }
    
    private void EnterTurnBased()
    {
        Debug.Log("Enter turn based");
        OnEnterTurnBased?.Invoke(turnManager.Get());
        roundClock.Get().EnterTurnBased();
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
        EnterRealTime();
        return true;
    }

    private void EnterRealTime()
    {
        Debug.Log("Enter real time");
        OnEnterRealTime?.Invoke(turnManager.Get());
        roundClock.Get().EnterRealTime();
        turnManager.Get().Deactivate();
    }
}