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

    private void Awake()
    {
        Register(); 
        roundClock = new();
        turnManager = GetComponent<TurnManager>();
    }

    private void Start()
    {
        Locator<ToggleTurnBasedButton> toggleTurnBasedButton = new();
        toggleTurnBasedButton.Get().OnToggleTurnBased += toggledOn =>
        {
            if (toggledOn) EnterTurnBased();
            else EnterRealTime();
        };
    }

    public void EnterTurnBased()
    {
        OnEnterTurnBased?.Invoke(turnManager);
        roundClock.Get().EnterTurnBased();
        turnManager.Activate();
    }

    public void EnterRealTime()
    {
        OnEnterRealTime?.Invoke(turnManager);
        roundClock.Get().EnterRealTime();
        turnManager.Deactivate();
    }

    public void Toggle()
    {
        if (mode == RoundClock.ProgressMode.RealTime) EnterTurnBased();
        if (mode == RoundClock.ProgressMode.TurnBased) EnterRealTime();
    }
    
    public void Nothing() {}
}