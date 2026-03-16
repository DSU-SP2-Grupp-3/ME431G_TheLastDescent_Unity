using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class WorldAgent : MonoBehaviour
{
    private static bool enemyTakesSimulataneousTurns = false;

    public event Action<string, GameObject> AnimationEventTriggered;
    public event Action<WorldAgent> ForcedEnterTurnBased;
    public event Action<WorldAgent, Queue<Command>, Command> CommandQueueUpdated;
    public event Action OnDeath;
    public event Action OnRevive;
    public event Action OnActivate;
    public event Action<DebuffLevel> OnDebuffApplied;
    public event Action<DebuffLevel> OnDebuffRemoved;

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
    [Tooltip("If true, prevent queueing commands while the command queue is being executed")]
    public bool lockDuringQueueExecution;
    [Tooltip("If true this agent will revive if dead after exiting turn based and all enemies are dead")]
    public bool reviveAfterCombat;
    [Range(0f, 1f), Tooltip("The portion of hp restored when revived automatically after combat")]
    public float reviveHitPointPortion;

    public Team team;
    [Header("References")]
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    [Tooltip("Only required if the object will generate a path")]
    public Transform cameraFocusTransform;
    public Transform indicatorFocusTransform;

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

    [SerializeField, Tooltip("The levels off debuffs to be applied, make sure they are in descending order, " +
                             "meaning the first debuff received is the first element in the list")]
    private DebuffLevel[] debuffLevels;
    private int currentDebuffLevel;

    private DamageManager damageManager;

    public int actorID;
    /// True if this agent should enter into the turn order when turn based mode is activated
    public bool active { get; private set; }
    /// Dead agents are for most purposes non existant, do not partake in turn order and do not execute commands
    public bool dead { get; private set; }
    public Vector3 initialPosition { get; private set; }

    private Locator<ModeSwitcher> modeSwitcher;
    private Locator<AgentManager> agentManager;
    private Locator<TurnManager> turnManager;
    private Locator<Indicator> indicator;
    private Locator<RoundClock> roundClock;

    public AgentManager manager => agentManager.Get();

    private Queue<Command> commandQueue;
    private Command currentlyExecutingCommand;
    private Coroutine currentExecutingCommandCoroutine;
    private Stack<int> commandPacketSizes;

    public bool queueEmpty => commandQueue.Count == 0;
    public float queueResourceCost
    {
        get
        {
            float total = 0f;
            foreach (Command command in commandQueue)
            {
                total += command.resourceCost;
            }
            return total;
        }
    }
    private bool breakCommandQueue;

    private void Awake()
    {
        initialPosition = transform.position;

        commandQueue = new();
        commandPacketSizes = new();

        agentManager = new();
        modeSwitcher = new();
        turnManager = new();
        indicator = new();
        roundClock = new();

        if (team == Team.Player) active = true;
        if (stats) localStats = stats.Clone();
    }

    private void Start()
    {
        AgentManager am = agentManager.Get();
        am.RegisterAgent(this);
        damageManager = am.damageManager;

        if (localStats && team == Team.Player) localStats.temperature.Changed += UpdateDebuffLevel;

        //subscribe TakeDamage to the DamageManager of the PlayerManager
        damageManager.DealDamageEvent += TakeDamage;
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
        InterruptCommandQueue();
        if (reviveAfterCombat && dead)
        {
            float autoReviveAmount = localStats.initHitPoints * reviveHitPointPortion;
            ResourceManager rm = agentManager.Get().resourceManager;
            ReviveCommand reviveCommand = new ReviveCommand(this, this, damageManager, rm, autoReviveAmount, 0f, 0f);
            OverwriteQueue(reviveCommand, true);
        }
    }

    public void QueueCommand(Command command, bool bypassDead = false)
    {
        QueueCommands(new Command[] { command }, bypassDead);
    }

    public void QueueCommands(Command[] commands, bool bypassDead = false)
    {
        if (lockDuringQueueExecution && currentExecutingCommandCoroutine != null) return;
        if (!bypassDead && dead) return;
        commandPacketSizes.Push(commands.Length);
        foreach (Command command in commands)
        {
            Debug.Log(command.status);
            if (command.status == Command.Status.Invalid) continue;
            commandQueue.Enqueue(command);
            if (localStats) localStats.actionPoints.value -= command.apCost;
        }
        CommandQueueUpdated?.Invoke(this, commandQueue, null);
    }

    public void OverwriteQueue(Command command, bool bypassDead = false)
    {
        if (lockDuringQueueExecution && currentExecutingCommandCoroutine != null) return;
        InterruptCommandQueue();
        QueueCommand(command, bypassDead);
        StartCoroutine(ExecuteCommandQueue());
    }

    public void OverwriteQueue(Command[] commands, bool bypassDead = false)
    {
        if (lockDuringQueueExecution && currentExecutingCommandCoroutine != null) return;
        InterruptCommandQueue();
        QueueCommands(commands, bypassDead);
        StartCoroutine(ExecuteCommandQueue());
    }

    public IEnumerator OverwriteQueueIEnumerator(Command command)
    {
        InterruptCommandQueue();
        QueueCommand(command);
        yield return StartCoroutine(ExecuteCommandQueue());
    }

    public void ForceStartCommandQueueExecution()
    {
        StartCoroutine(ExecuteCommandQueue());
    }

    public void InterruptCommandQueue()
    {
        currentlyExecutingCommand?.Break();
        currentlyExecutingCommand = null;
        StopAllCoroutines();
        agentManager.Get().resourceManager.RemoveCommands(commandQueue);
        commandQueue.Clear();
        commandPacketSizes.Clear();
        CommandQueueUpdated?.Invoke(this, commandQueue, null);
    }

    public void UndoLastestCommand(ResourceManager resourceManager)
    {
        if (commandPacketSizes.TryPop(out int size))
        {
            resourceManager.RemoveCommands(commandQueue);
            Queue<Command> shortenedQueue = new();
            Command[] commandArray = commandQueue.ToArray();
            localStats.actionPoints.value = localStats.initActionPoints;
            for (int i = 0; i < commandArray.Length - size; i++)
            {
                Command command = commandArray[i];
                shortenedQueue.Enqueue(command);
                resourceManager.QueuePayResource(command);
                if (localStats) localStats.actionPoints.value -= command.apCost;
            }
            commandQueue = shortenedQueue;
            CommandQueueUpdated?.Invoke(this, commandQueue, null);
        }
    }

    public IEnumerator ExecuteCommandQueue()
    {
        if (localStats) localStats.actionPoints.value = localStats.initActionPoints;
        commandPacketSizes.Clear();
        while (commandQueue.TryDequeue(out Command command))
        {
            CommandQueueUpdated?.Invoke(this, commandQueue, command);
            currentlyExecutingCommand = command;
            currentExecutingCommandCoroutine = StartCoroutine(command.ExecuteCommand());
            yield return currentExecutingCommandCoroutine;

            breakCommandQueue = currentlyExecutingCommand.status == Command.Status.Failed;

            currentExecutingCommandCoroutine = null;
            currentlyExecutingCommand = null;
            CommandQueueUpdated?.Invoke(this, commandQueue, null);
            if (breakCommandQueue) break;
        }
        if (breakCommandQueue)
        {
            breakCommandQueue = false;
            InterruptCommandQueue();
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
        OnActivate?.Invoke();
    }

    public void Die()
    {
        Dehighlight();
        InterruptCommandQueue();
        dead = true;
        animator.SetTrigger("Die");
        navMeshAgent.enabled = false;
        OnDeath?.Invoke();
    }

    public void Revive()
    {
        if (!dead) return;
        dead = false;
        navMeshAgent.enabled = true;
        OnRevive?.Invoke();
    }

    public void Highlight()
    {
        if (indicatorFocusTransform) indicator.Get().GetIndicator(indicatorFocusTransform);
        else indicator.Get().GetIndicator(transform);
    }

    public void Dehighlight()
    {
        if (indicatorFocusTransform) indicator.Get().DisableIndicator(indicatorFocusTransform);
        else indicator.Get().DisableIndicator(transform);
    }

    private void OnDisable()
    {
        //unsubscribe TakeDamage to the DamageManager of the PlayerManager
        agentManager.Get().damageManager.DealDamageEvent -= TakeDamage;
    }

    private void TakeDamage(float damage, WorldAgent target)
    {
        if (dead) return;

        //currently functions, would be cool if we implemented resistances or elemental damage or something
        if (target != this) return;
        Debug.Log($"{name} receiving {damage} damage");

        bool zeroHitPointsRemaining = localStats.TakeDamage(damage);
        Debug.Log($"Remaining hit points: {localStats.hitPoints}");

        if (zeroHitPointsRemaining)
        {
            Die();
        }
    }

    private void UpdateDebuffLevel(float temperature)
    {
        int previousLevel = currentDebuffLevel;

        bool broke = false;
        for (int i = 0; i < debuffLevels.Length; i++)
        {
            if (debuffLevels[i].whileUnder <= temperature)
            {
                currentDebuffLevel = i;
                broke = true;
                break;
            }
        }
        if (!broke) currentDebuffLevel = debuffLevels.Length;

        int difference = currentDebuffLevel - previousLevel;

        if (difference > 0) // debuffs should be applied in forward order
        {
            for (int i = previousLevel; i < currentDebuffLevel; i++)
            {
                debuffLevels[i].debuff.Apply(this);
                OnDebuffApplied?.Invoke(debuffLevels[i]);
            }
        }
        else if (difference < 0) // debuffs should be removed in reverse order
        {
            for (int i = previousLevel - 1; i >= currentDebuffLevel; i--)
            {
                debuffLevels[i].debuff.Remove(this);
                OnDebuffRemoved?.Invoke(debuffLevels[i]);
            }
        }
        // if difference is zero debuff level has not changed
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
            totalCost += command.apCost;
        }
        return totalCost;
    }

    public void TriggerAnimationEvent(string id)
    {
        AnimationEventTriggered?.Invoke(id, gameObject);
    }

    public IEnumerable<Resource> ResourceObjectsInQueue()
    {
        return commandQueue.Where(c => c is GetResourceCommand).Select(r => (r as GetResourceCommand).resource);
    }

    [Serializable]
    public class DebuffLevel : IComparable<DebuffLevel>
    {
        [Range(0f, 1f), Tooltip("The temperature under which the debuff should apply")]
        public float whileUnder;
        [Tooltip("The debuff to apply")]
        public Debuff debuff;

        public int CompareTo(DebuffLevel level)
        {
            if (level == null) return 1;
            // invert normal ascending float comparison
            return whileUnder.CompareTo(level.whileUnder) * -1;
        }
    }
}