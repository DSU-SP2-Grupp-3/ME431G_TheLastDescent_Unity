using UnityEngine;

public class InterAct : ScriptableObject
{
    public virtual FloatRefrence runTime { get => runTime; set => runTime = value; }
    public virtual void RunAction(float temp){}
}
