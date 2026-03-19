using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "Stats/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    [SerializeField]
    private float minDamage, maxDamage, range, cost = 1f;
    [SerializeField]
    private EventScriptable attackSfxEvent;

    public float attackRange => range;
    public float attackCost => cost;
    public float GetDamage() => Random.Range(minDamage, maxDamage);
    public EventScriptable attackEvent => attackSfxEvent;
}