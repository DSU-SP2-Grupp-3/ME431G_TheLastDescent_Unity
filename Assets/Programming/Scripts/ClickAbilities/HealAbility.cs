using UnityEngine;

[CreateAssetMenu(fileName = "NewHealAbility", menuName = "Ability/Heal", order = 0)]
public class HealAbility : ScriptableObject, IClickAbility
{
    public float healAmount;
    public float healAPCost;
    public float healResourceCost;
    public DamageManager damageManager;
    public ResourceManager resourceManager;

    public ResourceManager.ClickAbility GetAbility()
    {
        HealCommand healCommand = new HealCommand(
            null,
            damageManager,
            resourceManager,
            healAmount,
            healAPCost,
            healResourceCost
        );

        return new ResourceManager.ClickAbility(healCommand, "Heal", "NoHeal");
    }
}