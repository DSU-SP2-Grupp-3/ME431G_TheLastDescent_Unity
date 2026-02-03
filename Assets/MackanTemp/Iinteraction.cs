using UnityEngine;

public interface IInteraction
{
    FloatRefrence runTime { get; }
    void RunAction(float temp);
}
