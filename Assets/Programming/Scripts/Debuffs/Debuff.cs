using UnityEngine;

public abstract class Debuff : ScriptableObject
{
    [TextArea(1, 3)]
    public abstract string hint { get; }
    public abstract void Apply(WorldAgent agent);
    public abstract void Remove(WorldAgent agent);
}
