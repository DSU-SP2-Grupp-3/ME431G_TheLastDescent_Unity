using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class WorldAgent : MonoBehaviour
{
    private static bool enemyTakesSimulataneousTurns = false;

    public event Action<string> AnimationEventTriggered;
    public event Action<WorldAgent> ForcedEnterTurnBased;
    public event Action<WorldAgent, Queue<Command>, Command> CommandQueueUpdated;

    public enum Team
    {
        Player,
        Ally,
        Enemy,
        Neutral,
        Interactable
    }

    [Tooltip("True if this is the agent (player) should be the default selection when loading the scene")]
    public bool defaultSelected;

    public Team team;
    [Header("References")]
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    [Tooltip("Only required if the object will generate a path")]
    public Transform cameraFocusTransform;

    [SerializeField]
    private AgentStats stats;
    public AgentStats localStats { get; set; } //set could be privated but is not for now
    [SerializeField]
    private WeaponStats equippedWeapon;
    public WeaponStats weaponStats
    {
        get
        {
            if (equippedWeapon) return equippedWeapon;
            else
            {
                Debug.LogError($"Accessing weapon on agent {gameObject.name} with no weapon", this);
                return ScriptableObject.CreateInstance<WeaponStats>();
            }
        }
    }

    /// True if this agent should enter into the turn order when turn based mode is activated
    public bool active { get; private set; }
    /// Dead agents are for most purposes non existant, do not partake in turn order and do not execute commands
    public bool dead { get; private set; }
    public Vector3 initialPosition { get; private set; }

    private Locator<ModeSwitcher> modeSwitcher;
    private Locator<AgentManager> agentManager;
    private Locator<TurnManager> turnManager;

    public AgentManager manager => agentManager.Get();

    private Queue<Command> commandQueue;
    private Command currentlyExecutingCommand;
    private Coroutine currentExecutingCommandCoroutine;
    public bool queueEmpty => commandQueue.Count == 0;

    private void Awake()
    {
        initialPosition = transform.position;

        commandQueue = new();

        agentManager = new();
        modeSwitcher = new();
        turnManager = new();

        if (stats) localStats = stats.Clone();
        if (team == Team.Player) active = true;
    }

    private void Start()
    {
        AgentManager am = agentManager.Get();
        am.RegisterAgent(this);

        //subscribe TakeDamage to the DamageManager of the PlayerManager
        am.damageManager.DealDamageEvent += TakeDamage;
        modeSwitcher.Get().OnEnterTurnBased += RegisterInTurnManager;
        modeSwitcher.Get().OnEnterRealTime += ExitTurnBased;
    }

    private void RegisterInTurnManager(TurnManager turnManager)
    {
        if (active)
        {
            switch (team)
            {
                case Team.Player:
                    turnManager.RegisterAgentInGroup(team, this);
                    InterruptCommandQueue();
                    break;
                case Team.Enemy:
                    if (enemyTakesSimulataneousTurns)
                    {
                        turnManager.RegisterAgentInGroup(team, this);
                        InterruptCommandQueue();
                    }
                    else
                    {
                        turnManager.RegisterAgentAsOneManTeam(this);
                        InterruptCommandQueue();
                    }
                    break;
                case Team.Interactable:
                    // do nothing, interactables are not part of the turn order
                    break;
                default:
                    Debug.LogWarning($"Undefined turn behaviour for team: {team}");
                    break;
            }
        }
    }

    private void ExitTurnBased(TurnManager _)
    {
        // todo: thought this would fix funky animation behaviour where stop moving trigger is permanently on
        // todo: it did not but the queue should still be interrupted when entering real time I think /se
        InterruptCommandQueue();
    }

    public void QueueCommand(Command command)
    {
        if (dead) return;
        commandQueue.Enqueue(command);
        CommandQueueUpdated?.Invoke(this, commandQueue, null);
    }

    public void QueueCommands(Command[] commands)
    {
        if (dead) return;
        foreach (Command command in commands)
        {
            commandQueue.Enqueue(command);
        }
        CommandQueueUpdated?.Invoke(this, commandQueue, null);
    }

    public void OverwriteQueue(Command command)
    {
        InterruptCommandQueue();
        QueueCommand(command);
        StartCoroutine(ExecuteCommandQueue());
    }

    public void OverwriteQueue(Command[] commands)
    {
        InterruptCommandQueue();
        QueueCommands(commands);
        StartCoroutine(ExecuteCommandQueue());
    }

    public void ForceStartCommandQueueExecution()
    {
        StartCoroutine(ExecuteCommandQueue());
    }

    private void InterruptCommandQueue()
    {
        currentlyExecutingCommand?.Break();
        currentlyExecutingCommand = null;
        StopAllCoroutines();
        commandQueue.Clear();
        CommandQueueUpdated?.Invoke(this, commandQueue, null);
    }

    public IEnumerator ExecuteCommandQueue()
    {
        while (commandQueue.TryDequeue(out Command command))
        {
            CommandQueueUpdated?.Invoke(this, commandQueue, command);
            currentlyExecutingCommand = command;
            currentExecutingCommandCoroutine = StartCoroutine(command.Execute());
            yield return currentExecutingCommandCoroutine;
            currentExecutingCommandCoroutine = null;
            currentlyExecutingCommand = null;
            CommandQueueUpdated?.Invoke(this, commandQueue, null);
        }
    }

    public void Activate()
    {
        active = true;
        if (team == Team.Enemy)
        {
            Debug.Log($"Enemy: {name} activated!");
            if (modeSwitcher.Get().mode == RoundClock.ProgressMode.TurnBased)
            {
                RegisterInTurnManager(turnManager.Get());
            }
            else
            {
                if (modeSwitcher.Get().TryEnterTurnBased(true))
                {
                    ForcedEnterTurnBased?.Invoke(this);
                }
            }
        }
    }

    public void Die()
    {
        // todo: emit event here so agent manager can check if all players are dead
        Debug.Log($"Agent {name} has died");
        InterruptCommandQueue();
        dead = true;
        animator.SetTrigger("Die");
        agentManager.Get().damageManager.DealDamageEvent -= TakeDamage;
        navMeshAgent.enabled = false;
    }

    // visualise command queue /se
    // can afford new command /se

    private void OnDisable()
    {
        //unsubscribe TakeDamage to the DamageManager of the PlayerManager
        agentManager.Get().damageManager.DealDamageEvent -= TakeDamage;
    }

    private void TakeDamage(float damage, WorldAgent target)
    {
        //currently functions, would be cool if we implemented resistances or elemental damage or something
        if (target != this) return;
        Debug.Log($"{name} receiving {damage} damage");

        bool dead = localStats.TakeDamage(damage);
        Debug.Log($"Remaining hit points: {localStats.hitPoints}");

        if (dead)
        {
            Die();
        }
    }

    public Vector3 GetLastMoveCommandToPosition()
    {
        IEnumerable<IMoveCommand> moveCommandsInQueue = commandQueue
                                                        .Where(c => c is IMoveCommand)
                                                        .Select(c => c as IMoveCommand);

        // if currently executing a move command then it should be first in the queue
        // it won't appear in the commandQueue tho since it has been dequeued, so we add it manually
        if (currentlyExecutingCommand is IMoveCommand moveCommand)
        {
            moveCommandsInQueue.Prepend(moveCommand);
        }

        if (moveCommandsInQueue.Any()) return moveCommandsInQueue.Last().ToPosition();
        else return transform.position;
    }

    public float TotalCommandQueueCost()
    {
        float totalCost = 0f;
        foreach (Command command in commandQueue)
        {
            totalCost += command.cost;
        }
        return totalCost;
    }

    public void TriggerAnimationEvent(string id)
    {
        AnimationEventTriggered?.Invoke(id);
    }
}