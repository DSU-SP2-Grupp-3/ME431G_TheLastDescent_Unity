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

    public static CommandPackage GetMoveCommand(WorldAgent agent, Vector3 position)
    {
        MoveCommand moveCommand = new MoveCommand(agent.GetLastMoveCommandToPosition(), position, agent);
        return new CommandPackage(agent, moveCommand).SetType("move");
    }

    public static CommandPackage GetInteractionCommands(WorldAgent agent, GameObject go)
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
        interactionPackage.AppendCommand(result.QueueInteractablesCommand(agent));
        
        return interactionPackage.SetType("interaction");
    }

    public static CommandPackage SelectPlayerPackage(WorldAgent agent)
    {
        return new CommandPackage(agent).SetType("select");
    }

    public static CommandPackage AttackEnemyPackage(WorldAgent attacker, WorldAgent receiver, DamageManager damageManager)
    {
        if (receiver.dead) return EmptyPackage();

        MoveInRangeCommand inRangeCommand = new MoveInRangeCommand(
            receiver.transform.position,
            attacker.weaponStats.attackRange,
            attacker
        );
        AttackCommand attackCommand = new AttackCommand(attacker, receiver, damageManager, "PlayerAttack");
        Command[] commands = new Command[] { inRangeCommand, attackCommand };
        CommandPackage package = new CommandPackage(attacker, commands);
        
        return package.SetAdditionalHighlights(receiver).SetType("attack");
    }

    public class CommandPackage
    {
        public string type { get; private set; }
        public readonly WorldAgent agent;
        public readonly HashSet<WorldAgent> highlights;
        public readonly Command[] commands;
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
            highlights = new HashSet<WorldAgent>();
            highlights.Add(agent);
            clickOnAgentOnly = true;
        }
        
        public CommandPackage(WorldAgent agent, Command[] commands)
        {
            this.agent = agent;
            this.commands = commands;
            highlights = new HashSet<WorldAgent>();
            highlights.Add(agent);
        }

        public CommandPackage(WorldAgent agent, Command command)
        {
            this.agent = agent;
            this.commands = new Command[] { command };
            highlights = new HashSet<WorldAgent>();
            highlights.Add(agent);
        }

        public CommandPackage AppendCommand(Command command)
        {
            commands.Append(command);
            return this;
        }

        public CommandPackage SetAdditionalHighlights(WorldAgent agent)
        {
            highlights.Add(agent);
            return this;
        }

        public CommandPackage SetType(string type)
        {
            this.type = type;
            return this;
        }
        
        public bool QueueCommands(RoundClock.ProgressMode mode)
        {
            switch (mode)
            {
                case RoundClock.ProgressMode.TurnBased:
                    if (!CanQueueCommands()) return false;
                    else agent.QueueCommands(commands);
                    break;
                case RoundClock.ProgressMode.RealTime:
                    agent.OverwriteQueue(commands);
                    break;
            }
            return true;
        }

        public bool CanQueueCommands()
        {
            // don't queue empty commands
            if (commands.Length == 0) return false;
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