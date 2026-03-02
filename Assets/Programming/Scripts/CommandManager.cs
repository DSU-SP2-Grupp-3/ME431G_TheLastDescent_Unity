using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CommandManager
{
    public static CommandPackage EmptyPackage()
    {
        return new CommandPackage();
    }

    public static CommandPackage GetMovePackage(WorldAgent agent, Vector3 position)
    {
        MoveCommand moveCommand = new MoveCommand(agent.GetLastMoveCommandToPosition(), position, agent);
        if (!moveCommand.possible || moveCommand.noMovement) return EmptyPackage();
        CommandPackage package = new CommandPackage(agent, moveCommand);
        package.SetHighlight(agent, false);
        package.SetType("move");
        package.SetCursor("Walk");
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

        if (!AllMoveCommandsPossible(result.invokingAgentCommands)) return EmptyPackage();

        TrimUnnecessaryMoveCommands(ref result.invokingAgentCommands);
        CommandPackage interactionPackage = new CommandPackage(agent, result.invokingAgentCommands);
        LookCommand lookCommand = new LookCommand(agent, result.interactableAgent);

        interactionPackage.AddCommand(lookCommand);
        interactionPackage.AddCommand(result.QueueInteractablesCommand(agent));
        interactionPackage.SetHighlight(agent, false);
        interactionPackage.SetHighlight(result.interactableAgent, true);
        interactionPackage.SetType("interaction");
        interactionPackage.SetCursor("Point");

        return interactionPackage;
    }

    public static CommandPackage GetSelectPlayerPackage(WorldAgent agent, ResourceManager.ClickAbility clickAbility)
    {
        CommandPackage package = new CommandPackage(agent);
        package.SetHighlight(agent, true);
        package.SetType("select");

        if (clickAbility != null)
        {
            package.SetCursor(clickAbility.validCursorPath);
            foreach (Command command in clickAbility.commands)
            {
                command.ChangeInvoker(agent);
                package.AddCommand(command);
            }
        }

        return package;
    }

    public static CommandPackage GetAttackEnemyPackage(WorldAgent attacker,
                                                       WorldAgent receiver,
                                                       DamageManager damageManager)
    {
        if (receiver.dead) return EmptyPackage();

        MoveInRangeCommand inRangeCommand = new MoveInRangeCommand(
            receiver.transform.position,
            attacker.weaponStats.attackRange,
            attacker
        );
        if (!inRangeCommand.possible) return EmptyPackage();
        AttackCommand attackCommand = new AttackCommand(
            attacker, receiver, damageManager,
            attacker.weaponStats.attackCost, "PlayerAttack"
        );
        LookCommand lookCommand = new LookCommand(attacker, receiver);

        Command[] commands = new Command[] { inRangeCommand, lookCommand, attackCommand };
        TrimUnnecessaryMoveCommands(ref commands);
        CommandPackage package = new CommandPackage(attacker, commands);

        package.SetHighlight(attacker, false);
        package.SetHighlight(receiver, false);
        package.SetType("attack");
        package.SetCursor("Crosshair");

        return package;
    }

    public static CommandPackage GetHealPackage(WorldAgent invoker, float amount, float cost)
    {
        HealCommand healCommand = new HealCommand(invoker, amount, cost);
        CommandPackage healPackage = new CommandPackage(invoker, healCommand);
        healPackage.SetHighlight(invoker, true);
        healPackage.SetType("heal");
        healPackage.SetCursor("Heal");

        return healPackage;
    }

    public static bool AllMoveCommandsPossible(Command[] commands)
    {
        return !commands
                .Where(c => c is IMoveCommand)
                .Select(c => (c as IMoveCommand).possible)
                .Where(possible => !possible)
                .Any();
    }

    public static void TrimUnnecessaryMoveCommands(ref Command[] commands)
    {
        Command[] trimmed = commands
                            .Where(c =>
                            {
                                if (c is IMoveCommand moveCommand)
                                {
                                    return !moveCommand.noMovement;
                                }
                                else return true;
                            })
                            .ToArray();
        commands = trimmed;
    }

    public class CommandPackage
    {
        public string type { get; private set; }
        public CursorInfo cursorInfo { get; private set; }
        public readonly WorldAgent agent;
        public readonly Dictionary<WorldAgent, bool> highlights;
        public readonly List<Command> commands;
        public readonly bool empty;
        public readonly bool clickOnAgentOnly;

        public CommandPackage()
        {
            this.agent = null;
            this.commands = new();
            highlights = new Dictionary<WorldAgent, bool>();
            empty = true;
        }

        public CommandPackage(WorldAgent agent)
        {
            this.agent = agent;
            this.commands = new();
            highlights = new Dictionary<WorldAgent, bool>();
            clickOnAgentOnly = true;
        }

        public CommandPackage(WorldAgent agent, Command[] commands)
        {
            this.agent = agent;
            this.commands = commands.ToList();
            highlights = new Dictionary<WorldAgent, bool>();
        }

        public CommandPackage(WorldAgent agent, Command command)
        {
            this.agent = agent;
            commands = new List<Command>() { command };
            highlights = new Dictionary<WorldAgent, bool>();
        }

        public void AddCommand(Command command)
        {
            commands.Add(command);
        }

        public void SetHighlight(WorldAgent agent, bool inRealTime)
        {
            if (agent) highlights.Add(agent, inRealTime);
        }

        public void SetType(string type)
        {
            this.type = type;
        }

        public void SetCursor(string resourcePath)
        {
            cursorInfo = Resources.Load<CursorInfo>($"Cursors/{resourcePath}");
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
            if (commands == null || commands.Count == 0) return false;
            float queueCost = 0f;
            foreach (Command command in commands)
            {
                queueCost += command.cost;
                if (agent.TotalCommandQueueCost() + queueCost > agent.localStats.initActionPoints) return false;
            }
            return true;
        }

        public float TotalPackageCommandCost()
        {
            float total = 0f;
            if (commands == null) return total;
            foreach (Command command in commands)
            {
                total += command.cost;
            }
            return total;
        }
    }
}