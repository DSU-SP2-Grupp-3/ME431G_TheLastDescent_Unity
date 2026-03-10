using UnityEngine;

[CreateAssetMenu(fileName = "NewHeatUpAbility", menuName = "Ability/HeatUp", order = 0)]
public class HeatUpAbility : ScriptableObject, IClickAbility
{
    [Range(0f, 1f)]
    public float heatUpAmount;
    public float heatUpAPCost;
    public float heatUpResourceCost;
    public ResourceManager resourceManager;

    public ResourceManager.ClickAbility GetAbility()
    {
        HeatUpCommand heatUpCommand = new HeatUpCommand(
            null,
            heatUpAmount,
            resourceManager,
            heatUpAPCost,
            heatUpResourceCost
        );

        return new ResourceManager.ClickAbility(heatUpCommand, "Heal", "NoHeal");
    }
}