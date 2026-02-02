using System;
using UnityEngine;

public class RoundClock : Service<RoundClock>
{
    /// Invokes when a new round is progressed to, passing the new round as the first parameter
    public event Action<int> RoundProgressed;
    
    [SerializeField, Tooltip("How many realtime seconds it takes for a round to pass in realtime mode")]
    private float realSecondsPerRound;

    private int currentRound;
    private float nextRoundTime;
    
    /// Determines the mode by which the round should progress.
    /// Automatically in real time, or manually in turn based.
    private ProgressMode mode;

    void Awake()
    {
        Register();
    }
    
    void Start()
    {
        currentRound = 0;
        nextRoundTime = Time.time + realSecondsPerRound;
    }

    void Update()
    {
        if (mode == ProgressMode.RealTime)
        {
            if (nextRoundTime <= Time.time)
            {
                ProgressToNextRound();
            }        
        }
    }

    /// Progress the round clock to the next round, will be called automatically in real time mode,
    /// must be called manually in turned based mode. 
    public void ProgressToNextRound()
    {
        currentRound++;
        nextRoundTime = Time.time + realSecondsPerRound;
        RoundProgressed.Invoke(currentRound);
    }

    public void EnterTurnBased()
    {
        mode = ProgressMode.TurnBased;
    }

    public void EnterRealTime()
    {
        mode = ProgressMode.RealTime;
        nextRoundTime = Time.time + realSecondsPerRound;
    }
    
    public enum ProgressMode 
    {
        RealTime,
        TurnBased,
    }
}
