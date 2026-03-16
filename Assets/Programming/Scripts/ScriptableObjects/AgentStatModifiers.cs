using UnityEngine;

[CreateAssetMenu(fileName = "NewAgentStatModifiers", menuName = "Stats/Agent Stats Modifiers")]
public class AgentStatModifiers : ScriptableObject
{
    // todo: take into account command queue

    public float movementCostModifier = 1;
    public float receivedDamageModifier = 1;
    public float tempertureLoss = 0;
}
