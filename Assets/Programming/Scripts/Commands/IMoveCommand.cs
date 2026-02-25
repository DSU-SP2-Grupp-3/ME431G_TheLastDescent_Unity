using UnityEngine;

public interface IMoveCommand
{
    public Vector3 ToPosition();
    public bool possible { get; }
    public bool noMovement { get; }
}