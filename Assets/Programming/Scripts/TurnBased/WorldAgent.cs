using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldAgent : MonoBehaviour
{
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

    /// <summary>
    /// True if this agent should enter into the turn order when turn based mode is activated
    /// </summary>
    private bool active;
    private Locator<ModeSwitcher> modeSwitcher; 
    [NonSerialized] public Locator<PlayerManager> agentManager;
    
    private Queue<Command> commandQueue;
    private Command currentlyExecutingCommand;
    private Coroutine currentExecutingCommandCoroutine;

    private void Awake()
    {
        agentManager = new Locator<PlayerManager>();
        if (stats) localStats = stats.Clone();
        commandQueue = new();
        modeSwitcher = new();
        if (team == Team.Player) active = true;
    }

    private void Start()
    {
        //subscribe TakeDamage to the DamageManager of the PlayerManager
        agentManager.Get().damageManager.DealDamageEvent += TakeDamage;
        modeSwitcher.Get().OnEnterTurnBased += RegisterInTurnManager;
        StartCoroutine(ExecuteCommandQueue());
    }

    private void RegisterInTurnManager(TurnManager turnManager)
    {
        if (active && team != Team.Interactable)
        {
            turnManager.RegisterAgentInTeam(team, this);
            InterruptCommandQueue();
        }
    }

    public void QueueCommand(Command command)
    {
        commandQueue.Enqueue(command);
    }

    public void QueueCommands(Command[] commands)
    {
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

    // visualise command queue /se
    // can afford new command /se
    
    private void OnDisable()
    {
        //unsubscribe TakeDamage to the DamageManager of the PlayerManager
        agentManager.Get().damageManager.DealDamageEvent += TakeDamage;

    }

    private void TakeDamage(int damage, GameObject target)
    {
        //currently functions, would be cool if we implemented resistances or elemental damage or something
        if (target == gameObject)
        {
            localStats.hitPoints -= damage;
        }
    }

    private void Update()
    {
        if (navMeshAgent != null)
        {
            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathPartial)
            {
                DrawPath(navMeshAgent.path);
            } 
        }
    }

    public void DrawPath(NavMeshPath path)
    {
        //needs to be improved
        lineRenderer.positionCount = path.corners.Length;
        lineRenderer.SetPosition(0, transform.position);
        for (int i = 1; i < path.corners.Length; i++)
        {
            lineRenderer.SetPosition(i, path.corners[i]);
        }
    }
}