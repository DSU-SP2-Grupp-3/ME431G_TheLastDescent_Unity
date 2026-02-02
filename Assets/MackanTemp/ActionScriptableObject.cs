using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "InteractionSystem/Action")]
public class ActionScriptableObject : ScriptableObject, Iinteraction
{
    public FloatRefrence Cost;
    public FloatRefrence runTime { get => runTime; set => runTime = value; }
    public List<Iinteraction> interactions;
}
