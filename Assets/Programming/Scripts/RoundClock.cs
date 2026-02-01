using System;
using UnityEngine;

public class RoundClock : MonoBehaviour
{
    // todo: use service locator instead of static field
    public static RoundClock instance;
    
    /// Invokes when a new round is progressed to, passing the new round as the first parameter
    public event Action<int> RoundProgressed;
    
    [SerializeField]
    private float realSecondsPerRound;

    private int currentRound;
    private float nextRoundTime;
    
    /// Determines which mode by which the round should progress.
    /// Automatically in real time, or manually in turn based.
    private ProgressMode mode;

    void Awake()
    {
        instance = this;
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
