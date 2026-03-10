using UnityEngine;

public class ResourceMenu : MonoBehaviour
{
    [SerializeField]
    private ResourceManager resourceManager;

    private Locator<AgentManager> agentManager;

    [SerializeField]
    private HealAbility healAbilityInfo;
    [SerializeField]
    private HeatUpAbility heatUpAbilityInfo;
    [SerializeField]
    private ReviveAbility reviveAbilityInfo;

    private void Start()
    {
        agentManager = new();
    }

    public void ActivateHealAbility()
    {
        agentManager.Get().SetClickAbility(healAbilityInfo.GetAbility());
    }

    public void ActivateHeatUpAbility()
    {
        agentManager.Get().SetClickAbility(heatUpAbilityInfo.GetAbility());
    }

    public void ActivateReviveAbility()
    {
        agentManager.Get().SetClickAbility(reviveAbilityInfo.GetAbility());
    }
}