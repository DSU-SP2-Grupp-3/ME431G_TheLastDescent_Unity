using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class AIBehaviourType : ScriptableObject
{
    public abstract BehaviourResults GetIdleBehaviourResults(BehaviourDefinition.Stats stats, WorldAgent aiAgent);
    public abstract BehaviourResults GetActiveBehaviourResults(BehaviourDefinition.Stats stats, WorldAgent aiAgent);
    
    public class BehaviourResults
    {
        private List<Command> behaviourCommands;

        public BehaviourResults()
        {
            behaviourCommands = new();
        }

        public void AddCommand(Command command)
        {
            behaviourCommands.Add(command);
        }

        public Command[] GetCommands() => behaviourCommands.ToArray();
    }
}

