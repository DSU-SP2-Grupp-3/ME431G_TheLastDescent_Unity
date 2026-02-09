using UnityEngine;

[CreateAssetMenu(fileName = "NewBehaviourDefintion", menuName = "AI/Behaviour Definition", order = 0)]
public class BehaviourDefinition : ScriptableObject
{
    [SerializeField]
    private float attackRange, hitPoints, movement;
    [SerializeField]
    private DamageInterval damage;

    [SerializeField]
    private AIBehaviourType behaviourType;
}

public struct DamageInterval
{
    public float minDamage;
    public float maxDamage;
}
