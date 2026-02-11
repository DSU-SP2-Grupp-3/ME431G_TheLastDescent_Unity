using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewMeleeBehaviourType", menuName = "AI/Behaviour Type/Melee", order = 0)]
public class MeleeAttackBehaviour : AIBehaviourType
{
    [SerializeField]
    private WorldAgent.Team teamToAttack;

    
    public override BehaviourResults GetIdleBehaviourResults(BehaviourDefinition.Stats stats, WorldAgent aiAgent)
    {
        BehaviourResults results = new();
        if (RandomPoint(aiAgent.initialPosition, stats.wanderingRadius, out Vector3 result))
        {
            MoveCommand moveCommand = new MoveCommand(result, aiAgent);
            results.AddCommand(moveCommand);
        }
        else
        {
            MoveCommand moveCommand = new MoveCommand(aiAgent.initialPosition, aiAgent);
            results.AddCommand(moveCommand);
        }
        
        // DebugCommand dbgc = new DebugCommand(aiAgent, "idle AI");
        return results;
    }

    public override BehaviourResults GetActiveBehaviourResults(BehaviourDefinition.Stats stats, WorldAgent aiAgent)
    {
        BehaviourResults results = new();
        DebugCommand dbgc = new DebugCommand(aiAgent, "active AI");
        results.AddCommand(dbgc);
        return results;
    }
    
    // https://docs.unity3d.com/6000.0/Documentation/ScriptReference/AI.NavMesh.SamplePosition.html
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

}