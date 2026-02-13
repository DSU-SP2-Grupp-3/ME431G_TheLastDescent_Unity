using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentStats", menuName = "Stats/Agent Stats")]
public class AgentStats : ScriptableObject
{
    public event Action<float> HitPointsChanged;
    public event Action<float> ActionPointsChanged;

    [SerializeField]
    public float initHitPoints, initActionPoints, initMovement, initMovementCostModifier;
    private float _hitPoints;
    public float hitPoints
    {
        get => _hitPoints;
        set
        {
            _hitPoints = value;
            HitPointsChanged?.Invoke(_hitPoints);
        }
    }
    private float _actionPoints;

    public float actionPoints
    {
        get => _actionPoints;
        set
        {
            _actionPoints = value + _actionPoints > initActionPoints ? initHitPoints : value;
            ActionPointsChanged?.Invoke(_actionPoints);
        }
    }
    public float movement { get; set; }
    public float movementCostModifier { get; set; }

    public AgentStats Clone()
    {
        AgentStats clone = ScriptableObject.CreateInstance<AgentStats>();
        clone.hitPoints = initHitPoints;
        clone.actionPoints = initActionPoints;
        clone.movement = initMovement;
        clone.movementCostModifier = initMovementCostModifier;
        return clone;
    }

    /// <summary>
    /// Adjusts hitPoints according to damage taken, returns true if the resulting hitPoints are 0 or less.
    /// </summary>
    /// <param name="damage">The amount of damage dealt to this agent</param>
    /// <returns>True if damage dealt reduces current hit points below zero, otherwise false</returns>
    public bool TakeDamage(float damage)
    {
        hitPoints -= damage;
        if (hitPoints <= 0f)
        {
            return true;
        }
        return false;
    }
}