using UnityEngine;

public abstract class CommandWrapper : MonoBehaviour
{
    public abstract Command UnwrapCommand(WorldAgent agent);
}