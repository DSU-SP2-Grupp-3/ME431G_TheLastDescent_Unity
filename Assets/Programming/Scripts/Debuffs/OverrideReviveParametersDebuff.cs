using UnityEngine;

[CreateAssetMenu(fileName = "NewOverrideReviveParametersDebuff", menuName = "Debuffs/Override Revive Parameters")]
public class OverrideReviveParametersDebuff : Debuff
{
    [SerializeField]
    private string debuffHint;

    [SerializeField]
    private bool reviveAfterCombatOverride;
    [SerializeField, Range(0f, 1f)]
    private float revivePortionOverride;

    private bool reviveAfterCombatOriginal;
    private float revivePortionOriginal;

    public override string hint => debuffHint;
    public override void Apply(WorldAgent agent)
    {
        reviveAfterCombatOriginal = agent.reviveAfterCombat;
        revivePortionOriginal = agent.reviveHitPointPortion;
        agent.reviveAfterCombat = reviveAfterCombatOverride;
        agent.reviveHitPointPortion = revivePortionOverride;
    }
    public override void Remove(WorldAgent agent)
    {
        agent.reviveAfterCombat = reviveAfterCombatOriginal;
        agent.reviveHitPointPortion = revivePortionOriginal;
    }
}
