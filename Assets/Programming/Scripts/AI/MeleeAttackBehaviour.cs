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
        IEnumerable<WorldAgent> nearestCandidates = AI.GetNearestAgent(aiAgent.transform.position, targets);

        MoveInRangeCommand inRangeCommand = null;
        WorldAgent closest = null;
        bool candidateExists = false;
        foreach (WorldAgent candidate in nearestCandidates)
        {
            closest = candidate;
            inRangeCommand = new MoveInRangeCommand(
                closest.transform.position,
                aiAgent.weaponStats.attackRange,
                aiAgent, closest
            );
            if (inRangeCommand.possible)
            {
                candidateExists = true;
                break;
            }
        }

        // if no alive and reachable candidate exists, perform idle commands
        if (!candidateExists) return GetIdleBehaviourCommands(aiAgent, parameters);

        bool trimmed = inRangeCommand.Trim(aiAgent.localStats.movement);

        commands.AddCommand(inRangeCommand);

        LookCommand lookCommand = new LookCommand(aiAgent, closest);
        commands.AddCommand(lookCommand);

        // if the path was not trimmed that means that the enemy is in range to attack
        if (!trimmed)
        {
            AttackCommand attackPlayerCommand = new AttackCommand(aiAgent, closest, agentManager.damageManager);
            commands.AddCommand(attackPlayerCommand);
        }

        return commands;
    }
}