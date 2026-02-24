using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandManager
{
    public static CommandPackage EmptyPackage()
    {
        return new CommandPackage();
    }

    public static CommandPackage GetMovePackage(WorldAgent agent, Vector3 position)
    {
        MoveCommand moveCommand = new MoveCommand(agent.GetLastMoveCommandToPosition(), position, agent);
        CommandPackage package = new CommandPackage(agent, moveCommand);
        package.SetType("move");
        return package;
    }

    public static CommandPackage GetInteractionPackage(WorldAgent agent, GameObject go)
    {
        Interactable.InteractionResult result;
        InteractionGroup group = go.GetComponentInParent<InteractionGroup>();
        if (group)
        {
            group.UnwrapInteractableCommands(agent, out result);
        }
        else if (go.TryGetComponent<Interactable>(out Interactable interactable))
        {
            interactable.UnwrapInteractableCommands(agent, out result);
        }
        else
        {
            // didn't hover over interactable
            return EmptyPackage();
        }
        
        CommandPackage interactionPackage = new CommandPackage(agent, result.invokingAgentCommands);

        LookCommand lookCommand = new LookCommand(agent, result.interactableAgent);
        
        interactionPackage.AddCommand(lookCommand);
        interactionPackage.AddCommand(result.QueueInteractablesCommand(agent));
        interactionPackage.SetType("interaction");
        
        return interactionPackage;
    }

    public static CommandPackage GetSelectPlayerPackage(WorldAgent agent)
    {
        CommandPackage package = new CommandPackage(agent);
        package.SetType("select");
        return package;
    }

    public static CommandPackage GetAttackEnemyPackage(WorldAgent attacker, WorldAgent receiver, DamageManager damageManager)
    {
        if (receiver.dead) return EmptyPackage();

        MoveInRangeCommand inRangeCommand = new MoveInRangeCommand(
            receiver.transform.position,
            attacker.weaponStats.attackRange,
            attacker
        );
        AttackCommand attackCommand = new AttackCommand(attacker, receiver, damageManager, "PlayerAttack");
        LookCommand lookCommand = new LookCommand(attacker, receiver);
        
        Command[] commands = new Command[] { inRangeCommand, lookCommand, attackCommand };
        CommandPackage package = new CommandPackage(attacker, commands);
        
        package.SetAdditionalHighlights(receiver);
        package.SetType("attack");
        
        return package;
    }

    public class CommandPackage
    {
        public string type { get; private set; }
        public readonly WorldAgent agent;
        public readonly HashSet<WorldAgent> highlights;
        public readonly List<Command> commands;
        public readonly bool empty;
        public readonly bool clickOnAgentOnly;

        public CommandPackage()
        {
            this.agent = null;
            this.commands = null;
            highlights = new HashSet<WorldAgent>();
            empty = true;
        }

        public CommandPackage(WorldAgent agent)
        {
            this.agent = agent;
            this.commands = null;
            highlights = new HashSet<WorldAgent>() { agent };
            clickOnAgentOnly = true;
        }
        
        public CommandPackage(WorldAgent agent, Command[] commands)
        {
            this.agent = agent;
            this.commands = commands.ToList();
            highlights = new HashSet<WorldAgent>() { agent };
        }

        public CommandPackage(WorldAgent agent, Command command)
        {
            this.agent = agent;
            commands = new List<Command>() { command };
            highlights = new HashSet<WorldAgent>() { agent };
        }

        public void AddCommand(Command command)
        {
            commands.Add(command);
        }

        public void SetAdditionalHighlights(WorldAgent agent)
        {
            highlights.Add(agent);
        }

        public void SetType(string type)
        {
            this.type = type;
        }
        
        public bool QueueCommands(RoundClock.ProgressMode mode)
        {
            switch (mode)
            {
                case RoundClock.ProgressMode.TurnBased:
                    if (!CanQueueCommands()) return false;
                    else agent.QueueCommands(commands.ToArray());
                    break;
                case RoundClock.ProgressMode.RealTime:
                    agent.OverwriteQueue(commands.ToArray());
                    break;
            }
            return true;
        }

        public bool CanQueueCommands()
        {
            // don't queue empty commands
            if (commands.Count == 0) return false;
            float queueCost = 0f;
            foreach (Command command in commands)
            {
                queueCost += command.cost;
                if (agent.TotalCommandQueueCost() + queueCost > agent.localStats.actionPoints) return false;
            }
            return true;
        }
    }
}