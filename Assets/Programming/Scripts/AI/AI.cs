using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviour
{
    [SerializeField]
    private WorldAgent agent;
    [SerializeField]
    private BehaviourDefinition behaviourDefinition;

    [SerializeField]
    private AIParameters parameters;

    private Locator<RoundClock> roundClock;
    private Locator<AgentManager> agentManager;
    List<NavMeshAgent> playerNavMeshes;

    private void Start()
    {
        roundClock = new();
        playerNavMeshes = new();
        agentManager = new();
        roundClock.Get().RoundProgressed += RoundUpdate;
        agent.OnActivate += LookAtNearestTarget;
    }

    private void RoundUpdate(int round)
    {
        // todo: perhaps don't do anything at all when very far away from the players, to avoid unnecessary calculations
        if (!agent.active && !agent.dead) // perform idle behaviour
        {
            Command[] idleCommands = behaviourDefinition.GetIdleBehaviourCommands(agent, parameters).GetCommands();
            agent.OverwriteQueue(idleCommands);
        }
    }

    private void Update()
    {
        if (!agent.active && CheckIfShouldBeActive())
        {
            Debug.Log("activate");
            agent.Activate();
        }
    }

    private bool CheckIfShouldBeActive()
    {
        IEnumerable<NavMeshAgent> playerNavMeshes = agentManager.Get().GetPlayerAgents().Select(p => p.navMeshAgent);
        foreach (NavMeshAgent playerNavMesh in playerNavMeshes)
        {
            bool unobstructed = !agent.navMeshAgent.Raycast(playerNavMesh.transform.position, out NavMeshHit hit);
            float distance = (transform.position - playerNavMesh.transform.position).magnitude;
            if (unobstructed && distance < parameters.activationDistance)
            {
                return true;
            }
        }

        return false;
    }

    public void GetActiveCommands()
    {
        if (agent.dead) return;
        Debug.Assert(agent.queueEmpty);
        Command[] commands = behaviourDefinition.GetActiveBehaviourCommands(agent, parameters).GetCommands();
        agent.QueueCommands(commands);
    }

    public void LookAtNearestTarget()
    {
        List<WorldAgent> targets = agentManager.Get().GetPlayerAgents();
        WorldAgent closestTarget = GetNearestAgent(agent.transform.position, targets).First();

        LookCommand lookCommand = new LookCommand(agent, closestTarget);
        agent.OverwriteQueue(lookCommand);
    }

    public static IEnumerable<WorldAgent> GetNearestAgent(Vector3 fromPosition, List<WorldAgent> candidates)
    {
        candidates.Sort((a1, a2) =>
        {
            float distance1 = Vector3.Distance(fromPosition, a1.transform.position);
            float distance2 = Vector3.Distance(fromPosition, a2.transform.position);
            return distance1.CompareTo(distance2);
        });

        return candidates.Where(c => !c.dead);
    }

    [Serializable]
    public struct AIParameters
    {
        public float wanderingRadius;
        public float activationDistance;
    }
}