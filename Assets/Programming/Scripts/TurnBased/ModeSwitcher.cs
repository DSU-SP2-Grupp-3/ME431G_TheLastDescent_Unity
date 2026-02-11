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

    private void EnterTurnBased()
    {
        Debug.Log("Enter turn based");
        OnEnterTurnBased?.Invoke(turnManager);
        roundClock.Get().EnterTurnBased();
        turnManager.Activate();
    }

    private void EnterRealTime()
    {
        Debug.Log("Enter real time");
        OnEnterRealTime?.Invoke(turnManager);
        roundClock.Get().EnterRealTime();
        turnManager.Deactivate();
    }

    public bool TryEnterTurnBased(bool automatic = false)
    {
        automaticTurnBasedEntrance = automatic;
        EnterTurnBased();
        return true;
    }

    public bool TryEnterRealTime(bool forced = false)
    {
        if (!forced && automaticTurnBasedEntrance) return false;
        else if (forced) automaticTurnBasedEntrance = false;
        EnterRealTime();
        return true;
    }
    

    public void Toggle()
    {
        if (mode == RoundClock.ProgressMode.RealTime) EnterTurnBased();
        if (mode == RoundClock.ProgressMode.TurnBased) EnterRealTime();
    }
}