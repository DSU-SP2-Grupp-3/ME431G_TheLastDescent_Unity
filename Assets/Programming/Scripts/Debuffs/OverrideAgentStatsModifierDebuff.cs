using UnityEngine;

[CreateAssetMenu(fileName = "NewChangeAgentStatsDebuff", menuName = "Debuffs/Override Agent Stats Modifiers")]
public class OverrideAgentStatsModifierDebuff : Debuff
{
    [SerializeField]
    private string debuffHint;
    [SerializeField, Tooltip("The modifiers to be used while the debuff is applied")]
    private AgentStatModifiers applied;
    private AgentStatModifiers original;
    
    public override string hint => debuffHint;
    public override void Apply(WorldAgent agent)
    {
        original = agent.localStats.GetModifiersObject();
        agent.localStats.OverrideModifiers(applied);
    }
    public override void Remove(WorldAgent agent)
    {
        agent.localStats.OverrideModifiers(original);
    }
}
