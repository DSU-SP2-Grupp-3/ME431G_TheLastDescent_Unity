using System;
using TMPro;
using UnityEngine;

public class ToggleTurnBasedButton : Service<ToggleTurnBasedButton>
{
    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private string inTurnBasedText, inRealTimeText;
    [SerializeField]
    private bool inTurnBased;

    private Locator<ModeSwitcher> modeSwitcher;

    private void Awake()
    {
        Register();
    }

    private void Start()
    {
        modeSwitcher = new();
        modeSwitcher.Get().OnEnterRealTime += (_) =>
        {
            inTurnBased = false;
            text.text = inRealTimeText;
        };
        modeSwitcher.Get().OnEnterTurnBased += (_) =>
        {
            inTurnBased = true;
            text.text = inTurnBasedText;
        };
    }

    public void Toggle()
    {
        switch (inTurnBased)
        {
            case true:
                modeSwitcher.Get().TryEnterRealTime();
                break;
            case false:
                modeSwitcher.Get().TryEnterTurnBased();
                break;
        }
    }

}