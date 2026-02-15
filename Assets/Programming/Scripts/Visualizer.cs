using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Visualizer : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRendererPrefab;

    private Dictionary<WorldAgent, VisualizeTools> agentVisualizeTools;
    private Dictionary<WorldAgent, Command> currentlyExecutingCommands;

    [SerializeField]
    private AgentManager agentManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        agentManager.AgentRegistered += OnAgentRegistered;
        agentVisualizeTools = new();
        currentlyExecutingCommands = new();
    }

    private void Update()
    {
        // todo: tweak here so we can choose what agents to visualize and when
        
        foreach (Command command in currentlyExecutingCommands.Values)
        {
            command.VisualizeExecution(this);
        }
    }

    private void OnAgentRegistered(WorldAgent registeredAgent)
    {
        if (registeredAgent.team == WorldAgent.Team.Player)
        {
            LineRenderer queueLineRenderer = Instantiate(lineRendererPrefab, transform);
            LineRenderer executionLineRenderer = Instantiate(lineRendererPrefab, transform);
            VisualizeTools tools = new(queueLineRenderer, executionLineRenderer);
            agentVisualizeTools.Add(registeredAgent, tools);
            registeredAgent.CommandQueueUpdated += OnCommandQueueUpdated;
        }
    }

    private void OnCommandQueueUpdated(WorldAgent agent, Queue<Command> commandQueue, Command currentlyExecuting)
    {
        agentVisualizeTools[agent].Reset();
        foreach (Command command in commandQueue)
        {
            command.VisualizeInQueue(this);
        }

        if (currentlyExecuting != null)
        {
            currentlyExecutingCommands.TryAdd(agent, currentlyExecuting);
        }
        else
        {
            currentlyExecutingCommands.Remove(agent);
            agentVisualizeTools[agent].StoppedExecuting();
        }
    }
    
    public void AppendQueuedPath(NavMeshPath inputPath, WorldAgent agent)
    {
        LineRenderer agentLineRenderer = agentVisualizeTools[agent].queueLineRenderer;

        if (agentLineRenderer.positionCount == 0) // first path in command queue
        {
            agentLineRenderer.positionCount = inputPath.corners.Length;
            for (int i = 0; i < agentLineRenderer.positionCount; i++)
            {
                agentLineRenderer.SetPosition(i, inputPath.corners[i]);
            }
        }
        else // append path to line renderer
        {
            int startIndex = agentLineRenderer.positionCount - 1;
            agentLineRenderer.positionCount += inputPath.corners.Length - 1;
            for (int i = 0; i < inputPath.corners.Length; i++)
            {
                agentLineRenderer.SetPosition(i + startIndex, inputPath.corners[i]);
            }
        }
    }

    public void DrawExecutingPath(NavMeshPath executingPath, WorldAgent agent)
    {
        // ideally the drawn path only constists of one line renderer but it was too hard to make work right now -se
        // todo: connect the line rendering later
        
        LineRenderer executionLineRenderer = agentVisualizeTools[agent].executionLineRenderer;
        executionLineRenderer.positionCount = executingPath.corners.Length;
        executionLineRenderer.SetPositions(executingPath.corners);
    }

    private class VisualizeTools
    {
        public LineRenderer queueLineRenderer;
        public LineRenderer executionLineRenderer;

        public VisualizeTools(LineRenderer queueLineRenderer, LineRenderer executionLineRenderer)
        {
            this.queueLineRenderer = queueLineRenderer;
            this.executionLineRenderer = executionLineRenderer;
        }

        public void StoppedExecuting()
        {
            executionLineRenderer.positionCount = 0;
        }

        public void Reset()
        {
            executionLineRenderer.positionCount = 0;
            queueLineRenderer.positionCount = 0;
        }
        
    }

}
