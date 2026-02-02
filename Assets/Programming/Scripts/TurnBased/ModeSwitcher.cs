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

    private void Awake()
    {
        roundClock = new();
        turnManager = GetComponent<TurnManager>();
        Register(); 
    }

    public void EnterTurnBased()
    {
        OnEnterTurnBased.Invoke(turnManager);
        roundClock.Get().EnterTurnBased();
        turnManager.Activate();
    }

    public void EnterRealTime()
    {
        OnEnterRealTime.Invoke(turnManager);
        roundClock.Get().EnterRealTime();
        turnManager.Deactivate();
    }
}