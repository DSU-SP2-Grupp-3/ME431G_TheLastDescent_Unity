using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "DamageManager", menuName = "Scriptable Objects/DamageManager")]
public class DamageManager : ScriptableObject
{
    //Scriptable Object made to make it easier to send damage to world agents
    public UnityAction<int,GameObject> DealDamageEvent = delegate{};

    public void DealDamage(int damage, GameObject target)
    {
        //input the damage dealt, and the worldagent of the targeted target
        DealDamageEvent(damage, target);
    }

    

}
