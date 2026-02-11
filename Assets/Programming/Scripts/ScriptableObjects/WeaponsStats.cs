using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStast", menuName = "Stats/Weapon Stats")]
public class WeaponsStats : ScriptableObject
{
    [SerializeField]
    private float minDamage, maxDamage, range;

    public float GetDamage() => Random.Range(minDamage, maxDamage);
}