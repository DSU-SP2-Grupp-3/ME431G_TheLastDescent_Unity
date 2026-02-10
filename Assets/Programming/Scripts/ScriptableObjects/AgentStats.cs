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
}