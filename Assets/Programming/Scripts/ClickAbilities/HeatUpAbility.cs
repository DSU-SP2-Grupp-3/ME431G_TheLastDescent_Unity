using UnityEngine;

[CreateAssetMenu(fileName = "NewHeatUpAbility", menuName = "Ability/HeatUp", order = 0)]
public class HeatUpAbility : ScriptableObject, IClickAbility
{
    [Range(0f, 1f)]
    public float heatUpAmount;
    public float heatUpAPCost;
    public float heatUpResourceCost;
    public ResourceManager resourceManager;

    public ClickAbility GetAbility()
    {
        ClickAbility clickAbility = new ClickAbility(heatUpAPCost, heatUpResourceCost, "Heal", "NoHeal");

        // click heater upper
        clickAbility.AddClickAction(
            (info, ability) =>
            {
                info.GetAgent(out WorldAgent validAgent, LayerMask.NameToLayer("Player"));
                HeatUpCommand heatUpCommand = new HeatUpCommand(
                    validAgent,
                    heatUpAmount,
                    resourceManager,
                    heatUpAPCost,
                    heatUpResourceCost
                );
                ability.commands.Clear();
                ability.commands.Add(heatUpCommand);
                ability.queueingAgent = validAgent;
            },
            (info, ability) =>
            {
                if (info.GetAgent(out WorldAgent agent, LayerMask.NameToLayer("Player")))
                {
                    bool can = !agent.dead && agent.localStats.temperature < 1f;
                    if (can) ability.AddAffectedAgent(agent);
                    return can;
                }
                else return false;
            },
            "Target: alive and cold player"
        );

        return clickAbility;
    }
}