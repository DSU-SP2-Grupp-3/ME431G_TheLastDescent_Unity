using System;
using UnityEngine;

[CreateAssetMenu(fileName = "AIAttack", menuName = "Scriptable Objects/AIAttack")]
public class AIAttack : ScriptableObject, CommandProvider
{
    protected virtual void Attack(int Damage, float Probability)
    {
        //add damage type later
        //roll probability
        //if probability then deal damage to player
    }

    public Command[] ProvideCommands()
    {
        // return correct commands
        return Array.Empty<Command>();
    }
}
