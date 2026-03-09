using UnityEngine;

[CreateAssetMenu(fileName = "NewHealAbility", menuName = "Ability/Heal", order = 0)]
public class HealAbility : ScriptableObject, IClickAbility
{
    public float healAmount;
    public float healAPCost;
    public float healResourceCost;
    public DamageManager damageManager;
    public ResourceManager resourceManager;

    public ClickAbility GetAbility()
    {
        ClickAbility clickAbility = new ClickAbility(healAPCost, healResourceCost, "Heal", "NoHeal");

        // click healer
        clickAbility.AddClickAction(
            (info, ability) =>
            {
                info.GetAgent(out WorldAgent validAgent, LayerMask.NameToLayer("Player"));
                HealCommand healCommand = new HealCommand(
                    validAgent,
                    damageManager,
                    resourceManager,
                    healAmount,
                    healAPCost,
                    healResourceCost
                );
                ability.commands.Clear();
                ability.commands.Add(healCommand);
            },
            (info, ability) =>
            {
                if (info.GetAgent(out WorldAgent agent, LayerMask.NameToLayer("Player")))
                {
                    bool can = !agent.dead && agent.localStats.hitPoints < agent.localStats.initHitPoints;
                    if (can) ability.AddAffectedAgent(agent);
                    return can;
                }
                else return false;
            },
            "Target: alive and damaged player"
        );

        return clickAbility;
    }
}