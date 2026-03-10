using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClickAbility
{
    public WorldAgent queueingAgent;
    public readonly string validCursorPath;
    public readonly string invalidCursorPath;
    public readonly List<Command> commands;
    private List<Action<ClickInfo, ClickAbility>> clickFuncs;
    private List<Func<ClickInfo, ClickAbility, bool>> clickPreviews;
    private List<string> hints;
    private int clickIndex = 0;
    private List<object> data;
    private List<WorldAgent> affectedAgents;
    private ClickInfo currentInfo;
    public bool valid { get; set; }

    public ClickAbility(float apCost, float resourceCost, string validCursorPath, string invalidCursorPath)
    {
        clickFuncs = new();
        clickPreviews = new();
        hints = new();
        data = new();
        affectedAgents = new();
        commands = new();
        commands.Add(new CostCommand(null, apCost, resourceCost));
        this.validCursorPath = validCursorPath;
        this.invalidCursorPath = invalidCursorPath;
    }

    public void AddClickAction(
        Action<ClickInfo, ClickAbility> clickFunc,
        Func<ClickInfo, ClickAbility, bool> preview,
        string hint
    )
    {
        clickFuncs.Add(clickFunc);
        clickPreviews.Add(preview);
        hints.Add(hint);
    }

    public void SetCurrentHover(RaycastHit hit, WorldAgent agent)
    {
        currentInfo = new ClickInfo(agent, hit);
        affectedAgents.Clear();
    }

    /// <summary>
    /// Calls the next click function for the click ability. True if the last ability has been called
    /// </summary>
    public bool Click()
    {
        if (clickFuncs.Count == 0)
        {
            Debug.LogError("Click ability must have at least one click func");
            return false;
        }
        if (!valid) return false;
        clickFuncs[clickIndex].Invoke(currentInfo, this);
        clickIndex++;
        if (clickIndex >= clickFuncs.Count) return true;
        else return false;
    }

    /// <summary>
    /// Returns true if the next click function can be called
    /// </summary>
    public bool CanClick(RaycastHit hit, WorldAgent agent)
    {
        SetCurrentHover(hit, agent);
        valid = clickPreviews[clickIndex].Invoke(currentInfo, this);
        return valid;
    }

    public string GetHint()
    {
        return hints[clickIndex];
    }

    public int AddData(object newData)
    {
        int index = data.Count;
        data.Add(newData);
        return index;
    }

    public T GetData<T>(int index)
    {
        object dataEntry = data[index];
        return (T)dataEntry;
    }

    public void AddAffectedAgent(WorldAgent affected)
    {
        affectedAgents.Add(affected);
    }

    public List<WorldAgent> GetAffectedAgents() => affectedAgents;

    public struct ClickInfo
    {
        public WorldAgent agent;
        public RaycastHit hit;

        public ClickInfo(WorldAgent agent, RaycastHit hit)
        {
            this.agent = agent;
            this.hit = hit;
        }

        public bool GetAgent(out WorldAgent worldAgent, int layer)
        {
            if (agent)
            {
                worldAgent = agent;
                return true;
            }
            else if (hit.collider && hit.collider.gameObject.layer == layer)
            {
                worldAgent = hit.collider.GetComponentInParent<WorldAgent>();
                return true;
            }
            else if (hit.collider && layer == -1)
            {
                worldAgent = hit.collider.GetComponentInParent<WorldAgent>();
                if (worldAgent) return true;
            }
            worldAgent = null;
            return false;
        }
    }
}