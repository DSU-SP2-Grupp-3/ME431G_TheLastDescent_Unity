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
        // AgentManager agentManager = aiAgent.agentManager.Get();
        
        BehaviourCommands commands = new();
        // DebugCommand dbgc = new DebugCommand(aiAgent, "active AI");
        // commands.AddCommand(dbgc);
        return commands;
        
        // playerPositions = agent.agentManager.Get().GetPlayerPositions();
        //
        // //sets the ais world navMeshAgent to a new path
        // NavMeshPath path = behaviourDefinition.FetchPath(agent.navMeshAgent, playerPositions);
        // path = TrimPathToMoveRange(path, agent.localStats.movement);
        //
        // //create and queue a movecommand using the path and the agent
        // MoveCommand aiMovement = new MoveCommand(path, agent); 
        // agent.OverwriteQueue(aiMovement);

    }
    
    

}