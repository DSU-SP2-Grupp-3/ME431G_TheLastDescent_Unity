using System;
using UnityEngine;

public class ReadyButton : Service<ReadyButton>
{
    public event Action ReadyButtonPressed;

    public void Ready()
    {
        ReadyButtonPressed?.Invoke();
    }
    
    private void Awake()
    {
        Register();
    }

    private void Start()
    {
        gameObject.SetActive(false);
        ModeSwitcher modeSwitcher = new Locator<ModeSwitcher>().Get();
        modeSwitcher.OnEnterTurnBased += (_) => gameObject.SetActive(true);
        modeSwitcher.OnEnterRealTime += (_) => gameObject.SetActive(false);
    }
}