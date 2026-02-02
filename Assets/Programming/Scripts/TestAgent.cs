using UnityEngine;

public class TestAgent : MonoBehaviour
{
    private Locator<RoundClock> roundClock;
    
    void Start()
    {
        roundClock = new Locator<RoundClock>();   
        roundClock.Get().RoundProgressed += OnNewRound;
    }

    void OnNewRound(int round)
    {
        Debug.Log($"Current round: {round}");
    }
    
    
    void Update()
    {
        RoundClock rc = roundClock.Get();
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            rc.EnterTurnBased();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            rc.EnterRealTime();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rc.ProgressToNextRound();
        }
    }
}
