using UnityEngine;

public abstract class Debuff : ScriptableObject
{
    public abstract string hint { get; }
    public abstract void Apply(WorldAgent agent);
    public abstract void Remove(WorldAgent agent);
}
