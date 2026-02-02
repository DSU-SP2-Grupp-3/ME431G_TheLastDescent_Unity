using UnityEngine;

public class Interaction : ScriptableObject, IInteraction
{
    public virtual FloatRefrence runTime { get => runTime; set => runTime = value; }
    public virtual void RunAction(float temp){}
}
