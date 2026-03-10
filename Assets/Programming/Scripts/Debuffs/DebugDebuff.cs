using UnityEngine;

[CreateAssetMenu(fileName = "NewDebugDebuff", menuName = "Debuffs/Debug")]
public class DebugDebuff : Debuff
{
    [SerializeField]
    private string debugHint;

    public override string hint => debugHint;
    public override void Apply(WorldAgent agent)
    {
        Debug.Log($"Apply debuff: {hint} (on {agent.name})");
    }
    public override void Remove(WorldAgent agent)
    {
        Debug.Log($"Remove debuff: {hint} (from {agent.name})");
    }
}
