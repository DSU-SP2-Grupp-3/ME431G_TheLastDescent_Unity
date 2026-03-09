using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewReviveAbility", menuName = "Ability/Revive", order = 0)]
public class ReviveAbility : ScriptableObject, IClickAbility
{
    public float reviveAmount;
    public float reviveAPCost;
    public float reviveResourceCost;
    public DamageManager damageManager;
    public ResourceManager resourceManager;

    public ClickAbility GetAbility()
    {
        ClickAbility clickAbility = new ClickAbility(reviveAPCost, reviveResourceCost, "Heal", "NoHeal");



        return clickAbility;
    }
}