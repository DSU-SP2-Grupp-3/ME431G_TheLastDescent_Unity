using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(RoundClock))]
public sealed class ModeSwitcher : Service<ModeSwitcher>
{
    public event Action<TurnManager> OnEnterTurnBased;
    public event Action<TurnManager> OnEnterRealTime;

    private Locator<RoundClock> roundClock;

    private TurnManager turnManager;

    public RoundClock.ProgressMode mode => roundClock.Get().currentMode;

    private bool automaticTurnBasedEntrance;
    
    private void Awake()
    {
        Register(); 
        roundClock = new();
        turnManager = GetComponent<TurnManager>();
    }

    public bool TryEnterTurnBased(bool automatic = false)
    {
        automaticTurnBasedEntrance = automatic;
        EnterTurnBased();
        return true;
    }
    
    private void EnterTurnBased()
    {
        // todo: handle if combat is entered while already in turn based
        Debug.Log("Enter turn based");
        OnEnterTurnBased?.Invoke(turnManager);
        roundClock.Get().EnterTurnBased();
        turnManager.Activate();
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
        OnEnterRealTime?.Invoke(turnManager);
        roundClock.Get().EnterRealTime();
        turnManager.Deactivate();
    }
}