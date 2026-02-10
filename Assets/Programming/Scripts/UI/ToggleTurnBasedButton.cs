using System;
using TMPro;
using UnityEngine;

public class ToggleTurnBasedButton : Service<ToggleTurnBasedButton>
{
    public event Action<bool> OnToggleTurnBased;
    
    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private string inTurnBasedText, inRealTimeText;
    [SerializeField]
    private bool inTurnBased;

    private void Awake()
    {
        Register();
    }

    public void Toggle()
    {
        inTurnBased = !inTurnBased;
        OnToggleTurnBased?.Invoke(inTurnBased);
        
        if (inTurnBased)
        {
            text.text = inTurnBasedText;
        }
        else
        {
            text.text = inRealTimeText;
        }
        
    }

}