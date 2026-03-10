using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewReviveAbility", menuName = "Ability/Revive", order = 0)]
public class ReviveAbility : ScriptableObject, IClickAbility
{
    [Range(0f, 1f)]
    public float reviveAmount;
    public float reviveAPCost;
    public float reviveResourceCost;
    public DamageManager damageManager;
    public ResourceManager resourceManager;

    public ClickAbility GetAbility()
    {
        ClickAbility clickAbility = new ClickAbility(reviveAPCost, reviveResourceCost, "Heal", "NoHeal");

        // click reviver
        clickAbility.AddClickAction(
            (info, ability) =>
            {
                // set the reviver as the agent that will perform the commands
                info.GetAgent(out WorldAgent validAgent, LayerMask.NameToLayer("Player"));
                ability.AddData(validAgent);
                ability.queueingAgent = validAgent;
            },
            (info, ability) =>
            {
                if (info.GetAgent(out WorldAgent agent, LayerMask.NameToLayer("Player")))
                {
                    bool can = !agent.dead;
                    if (can) ability.AddAffectedAgent(agent);
                    return can;
                }
                else return false;
            },
            "Target reviver: alive party member"
        );

        // click revivee
        clickAbility.AddClickAction(
            (info, ability) =>
            {
                ability.commands.Clear();

                WorldAgent reviver = ability.GetData<WorldAgent>(0);
                info.GetAgent(out WorldAgent revivee, LayerMask.NameToLayer("Player"));

                MoveInRangeCommand inRangeCommand = new MoveInRangeCommand(
                    revivee.transform.position, 2f, reviver
                );
                ReviveCommand reviveCommand = new ReviveCommand(
                    reviver,
                    revivee,
                    damageManager,
                    resourceManager,
                    revivee.localStats.initHitPoints * reviveAmount,
                    reviveResourceCost,
                    reviveAPCost
                );

                ability.commands.Add(inRangeCommand);
                ability.commands.Add(reviveCommand);
            },
            (info, ability) =>
            {
                WorldAgent reviver = ability.GetData<WorldAgent>(0);
                ability.commands.Clear();
                ability.commands.Add(new CostCommand(reviver, reviveAPCost, reviveResourceCost));

                ability.AddAffectedAgent(reviver);

                if (info.GetAgent(out WorldAgent agent, LayerMask.NameToLayer("Player")))
                {
                    bool can = agent.dead;
                    if (can)
                    {
                        ability.AddAffectedAgent(agent);
                        MoveInRangeCommand inRangeCommand = new MoveInRangeCommand(
                            agent.transform.position, 2f, reviver
                        );
                        ability.commands.Add(inRangeCommand);
                    }
                    return can;
                }
                else return false;
            },
            "Target to be revived: dead party member"
        );

        return clickAbility;
    }
}