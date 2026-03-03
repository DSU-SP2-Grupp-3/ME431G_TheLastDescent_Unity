using UnityEngine;

[CreateAssetMenu(fileName = "NewHealAbility", menuName = "Ability/Heal", order = 0)]
public class HealAbility : ScriptableObject
{
    public float healAmount;
    public float healAPCost;
    public float healResourceCost;
    public DamageManager damageManager;

    public ResourceManager.ClickAbility GetAbility()
    {
        HealCommand healCommand = new HealCommand(null, damageManager, healAmount, healAPCost, healResourceCost);
        ResourceManager.ClickAbility ability = new ResourceManager.ClickAbility(
            healCommand,
            healResourceCost,
            "Heal", "NoHeal"
        );

        return ability;
    }
}