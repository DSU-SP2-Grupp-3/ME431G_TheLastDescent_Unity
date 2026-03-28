using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RoundClock))]
public sealed class ModeSwitcher : Service<ModeSwitcher>
{
    public event Action<TurnManager> OnEnterTurnBased;
    public UnityEvent OnTurnBasedEntered;
    public event Action<TurnManager> OnEnterRealTime;
    public UnityEvent OnRealTimeEntered;

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
        EnterRealTime();
    }
    
    public void EnterTurnBased()
    {
        Debug.Log("Enter turn based");
        roundClock.Get().Pause();
        OnEnterTurnBased?.Invoke(turnManager.Get());
        OnTurnBasedEntered?.Invoke();
        turnManager.Get().Activate();
    }

    public void EnterRealTime()
    {
        Debug.Log("Enter real time");
        roundClock.Get().Unpause();
        OnEnterRealTime?.Invoke(turnManager.Get());
        OnRealTimeEntered?.Invoke();
        turnManager.Get().Deactivate();
    }
}