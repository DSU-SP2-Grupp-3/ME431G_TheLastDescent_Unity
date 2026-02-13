using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "Stats/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    [SerializeField]
    private float minDamage, maxDamage, range;

    public float attackRange => range;
    public float GetDamage() => Random.Range(minDamage, maxDamage);
}