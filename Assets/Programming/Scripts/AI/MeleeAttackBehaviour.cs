using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "NewMeleeAttackDefinition", menuName = "AI/Behaviour Defintion/Melee Attack", order = 0)]
public class MeleeAttackBehaviour : BehaviourDefinition
{
    [SerializeField]
    private WorldAgent.Team teamToAttack;

    public override BehaviourCommands GetIdleBehaviourCommands(WorldAgent aiAgent, AI.AIParameters parameters)
    {
        BehaviourCommands commands = new();
        if (RandomPoint(aiAgent.initialPosition, parameters.wanderingRadius, out Vector3 result))
        {
            MoveCommand moveCommand = new MoveCommand(result, aiAgent);
            commands.AddCommand(moveCommand);
        }
        else
        {
            MoveCommand moveCommand = new MoveCommand(aiAgent.initialPosition, aiAgent);
            commands.AddCommand(moveCommand);
        }

        // DebugCommand dbgc = new DebugCommand(aiAgent, "idle AI");
        return commands;
    }

    public override BehaviourCommands GetActiveBehaviourCommands(WorldAgent aiAgent, AI.AIParameters parameters)
    {
        AgentManager agentManager = aiAgent.manager;
        BehaviourCommands commands = new();

        List<WorldAgent> targets = agentManager.GetFilteredAgents((w => w.team == teamToAttack)).ToList();
        WorldAgent closestTarget = AI.GetNearestAgent(aiAgent.transform.position, targets);
        
        //sets the ais world navMeshAgent to a new path

        MoveInRangeCommand inRangeCommand = new MoveInRangeCommand(
            closestTarget.transform.position,
            aiAgent.weaponStats.attackRange,
            aiAgent, closestTarget
        );
        
        bool attackCanReach = !inRangeCommand.Trim(aiAgent.localStats.movement);

        commands.AddCommand(inRangeCommand);
        
        LookCommand lookCommand = new LookCommand(aiAgent, closestTarget);
        commands.AddCommand(lookCommand);
        
        if (attackCanReach)
        {
            // if the path does not need to be trimmed then we attack the player as well
            AttackCommand attackPlayerCommand = new AttackCommand(aiAgent, closestTarget, agentManager.damageManager);
            commands.AddCommand(attackPlayerCommand);
        }

        return commands;
    }
}