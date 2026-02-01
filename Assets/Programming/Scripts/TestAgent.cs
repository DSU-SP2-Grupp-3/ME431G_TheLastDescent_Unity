using UnityEngine;

public class TestAgent : MonoBehaviour
{
    
    void Start()
    {
        RoundClock.instance.RoundProgressed += OnNewRound;
    }

    void OnNewRound(int round)
    {
        Debug.Log($"Current round: {round}");
    }
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            RoundClock.instance.EnterTurnBased();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RoundClock.instance.EnterRealTime();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RoundClock.instance.ProgressToNextRound();
        }
    }
}
