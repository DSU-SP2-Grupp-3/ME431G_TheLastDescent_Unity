using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AgentStats", menuName = "Stats/Agent Stats")]
public class AgentStats : ScriptableObject
{
    [SerializeField]
    private float initHitPoints, initActionPoints, initMovement, initMovementCostModifier;
    public float hitPoints { get; set; }
    public float actionPoints { get; set; }
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