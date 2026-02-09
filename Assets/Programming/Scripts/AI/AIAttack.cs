using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AIAttack", menuName = "Scriptable Objects/AIAttack")]
public class AIAttack : ScriptableObject, CommandProvider
{
    private void Attack(int Damage, float Probability)
    {
        //add damage type later
        //roll probability
        //if probability then deal damage to player
    }
    /// <inheritdoc />
    public Command[] ProvideCommands()
    {
        return Array.Empty<Command>();
    }
}