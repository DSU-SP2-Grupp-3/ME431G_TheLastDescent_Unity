using UnityEngine;

[CreateAssetMenu(fileName = "NewHeatUpAbility", menuName = "Ability/HeatUp", order = 0)]
public class HeatUpAbility : ScriptableObject
{
    public float heatUpAmount;
    public float heatUpAPCost;
    public float heatUpResourceCost;

    public ResourceManager.ClickAbility GetAbility()
    {
        return default(ResourceManager.ClickAbility);
    }
}