using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentStats", menuName = "Stats/Agent Stats")]
public class AgentStats : ScriptableObject
{
    [SerializeField]
    public float
        initHitPoints,
        initActionPoints,
        initMovement,
        initMovementCostModifier,
        initTemperatureLoss = 0.01f;

    public Watcher<float> hitPoints;
    public Watcher<float> actionPoints;
    public Watcher<float> temperature;

    public float movement { get; set; }
    public float movementCostModifier { get; set; }

    public AgentStats Clone()
    {
        AgentStats clone = ScriptableObject.CreateInstance<AgentStats>();

        clone.initHitPoints = initHitPoints;
        clone.initActionPoints = initActionPoints;
        clone.initMovement = initMovement;
        clone.initMovementCostModifier = initMovementCostModifier;
        clone.initTemperatureLoss = initTemperatureLoss;

        clone.hitPoints = new Watcher<float>(initHitPoints, Clamp(0f, initHitPoints));
        clone.actionPoints = new Watcher<float>(initActionPoints, Clamp(0f, initActionPoints));
        clone.temperature = new Watcher<float>(1f, Clamp(0f, 1f));
        clone.movement = initMovement;
        clone.movementCostModifier = initMovementCostModifier;

        if (new Locator<RoundClock>().TryGet(out RoundClock roundClock))
        {
            roundClock.RoundProgressed += clone.LoseTemperature;
        }
        else
        {
            RoundClock.OnRegister += (rc) => rc.RoundProgressed += clone.LoseTemperature;
        }

        return clone;
    }

    private Func<float, float> Clamp(float min, float max)
    {
        return (f) => Mathf.Clamp(f, min, max);
    }

    /// <summary>
    /// Adjusts hitPoints according to damage taken, returns true if the resulting hitPoints are 0 or less.
    /// </summary>
    /// <param name="damage">The amount of damage dealt to this agent</param>
    /// <returns>True if damage dealt reduces current hit points below zero, otherwise false</returns>
    public bool TakeDamage(float damage)
    {
        hitPoints.value -= damage;
        if (hitPoints <= 0f)
        {
            return true;
        }
        return false;
    }

    private void LoseTemperature(int round)
    {
        temperature.value -= initTemperatureLoss;
    }
}