using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldAgent : MonoBehaviour
{
    private static bool enemyTakesSimulataneousTurns = false;

    public event Action<string> AnimationEventTriggered;
    
    public enum Team
    {
        Player,
        Ally,
        Enemy,
        Neutral,
        Interactable
    }

    public Team team;
    [Header("References")]
    public Animator animator;
    public NavMeshAgent navMeshAgent;
    [Tooltip("Only required if the object will generate a path")]
    public LineRenderer lineRenderer;
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
    [NonSerialized] public Locator<AgentManager> agentManager;
    
    private Queue<Command> commandQueue;
    private Command currentlyExecutingCommand;
    private Coroutine currentExecutingCommandCoroutine;
    public bool queueEmpty => commandQueue.Count == 0;
    
    private void Awake()
    {
        initialPosition = transform.position;
        agentManager = new Locator<AgentManager>();
        if (stats) localStats = stats.Clone();
        commandQueue = new();
        modeSwitcher = new();
        if (team == Team.Player) active = true;
    }

    private void Start()
    {
        AgentManager am = agentManager.Get();
        am.RegisterAgent(this);
        
        //subscribe TakeDamage to the DamageManager of the PlayerManager
        am.damageManager.DealDamageEvent += TakeDamage;
        modeSwitcher.Get().OnEnterTurnBased += RegisterInTurnManager;
        StartCoroutine(ExecuteCommandQueue());
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

    public void QueueCommand(Command command)
    {
        if (dead) return;
        commandQueue.Enqueue(command);
    }

    public void QueueCommands(Command[] commands)
    {
        if (dead) return;
        foreach (Command command in commands)
        {
            commandQueue.Enqueue(command);
        }
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
    }

    public IEnumerator ExecuteCommandQueue()
    {
        while (commandQueue.TryDequeue(out Command command))
        {
            currentlyExecutingCommand = command;
            currentExecutingCommandCoroutine = StartCoroutine(command.Execute());
            yield return currentExecutingCommandCoroutine;
            currentExecutingCommandCoroutine = null;
            currentlyExecutingCommand = null;
        }
    }
    
    public void Activate()
    {
        active = true;
        if (team == Team.Enemy)
        {
            // todo: call some enemy group component to activate all enemies in an area whenever one of the enemies is activated
            // todo: check if already in turnbased and enter the combat in that case
            if (modeSwitcher.Get().mode == RoundClock.ProgressMode.TurnBased) return; // just don't do anything unless in realtime for now
            modeSwitcher.Get().TryEnterTurnBased(true);
        }
    }

    public void Die()
    {
        Debug.Log($"Agent {name} has died");
        InterruptCommandQueue();
        dead = true;
        animator.SetTrigger("Die");
        agentManager.Get().damageManager.DealDamageEvent -= TakeDamage;
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

    private void Update()
    {
        if (navMeshAgent && lineRenderer)
        {
            if (navMeshAgent.isStopped)
            {
                lineRenderer.positionCount = 0;
            }
            else
            {
                DrawPath(navMeshAgent.path);
            }
        }
    }

    public void DrawPath(NavMeshPath path)
    {
        //needs to be improved
        if (!lineRenderer) return;
        lineRenderer.positionCount = path.corners.Length;
        lineRenderer.SetPosition(0, transform.position);
        for (int i = 1; i < path.corners.Length; i++)
        {
            lineRenderer.SetPosition(i, path.corners[i]);
        }
    }

    public void TriggerAnimationEvent(string id)
    {
        AnimationEventTriggered?.Invoke(id);
    }
}