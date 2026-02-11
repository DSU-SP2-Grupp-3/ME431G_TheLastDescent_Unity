using UnityEngine;

[CreateAssetMenu(fileName = "NewMeleeBehaviourType", menuName = "AI/Behaviour Type/Melee", order = 0)]
public class MeleeAttackBehaviour : AIBehaviourType
{
    [SerializeField]
    private WorldAgent.Team teamToAttack;

    public override AIBehaviourResults GetBehaviourResults()
    {
        return new AIBehaviourResults();
    }
}