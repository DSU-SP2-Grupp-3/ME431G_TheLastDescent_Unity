using UnityEngine;

[CreateAssetMenu(fileName = "AgentStats", menuName = "Stats/Agent Stats")]
public class AgentStats : ScriptableObject
{
    [SerializeField]
    private float initHitPoints, initActionPoints, initMovement, initMovementCostModifier;
    public float hitPoints { get; private set; }
    public float actionPoints { get; private set; }
    public float movement { get; private set; }
    public float movementCostModifier { get; private set; }

    public AgentStats Clone()
    {
        AgentStats clone = ScriptableObject.CreateInstance<AgentStats>();
        clone.hitPoints = initHitPoints;
        clone.actionPoints = initActionPoints;
        clone.movement = initMovement;
        clone.movementCostModifier = initMovementCostModifier;
        return clone;
    }
}