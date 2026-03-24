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

    public static CommandPackage OnlyCommands(IEnumerable<Command> commands)
    {
        CommandPackage empty = new CommandPackage();
        foreach (Command command in commands)
        {
            empty.AddCommand(command);
        }
        empty.SetType("commands");

        return empty;
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

        if (!AllMoveCommandsPossible(result.invokingAgentCommands) ||
            InvalidCommandInCollection(result.invokingAgentCommands))
        {
            CommandPackage badInteraction = EmptyPackage();
            badInteraction.SetHighlight(result.interactableAgent, true);
            badInteraction.SetCursor("InvalidPoint");
            return badInteraction;
        }

        TrimUnnecessaryMoveCommands(ref result.invokingAgentCommands);
        CommandPackage interactionPackage = new CommandPackage(agent, result.invokingAgentCommands);
        LookCommand lookCommand = new LookCommand(agent, result.interactableAgent);

        interactionPackage.AddCommand(lookCommand);
        interactionPackage.AddCommand(result.QueueInteractablesCommand(agent));
        interactionPackage.SetHighlight(agent, false);
        interactionPackage.SetHighlight(result.interactableAgent, true);
        interactionPackage.SetInfo(result.interactableAgent.displayName);
        interactionPackage.SetType("interaction");
        interactionPackage.SetCursor("Point");

        return interactionPackage;
    }

    public static CommandPackage GetSelectPlayerPackage(WorldAgent agent)
    {
        CommandPackage package = new CommandPackage(agent);
        package.SetInfo(agent.displayName);
        package.SetHighlight(agent, true);
        package.SetType("select");

        return package;
    }

    public static CommandPackage GetClickAbilityPackage(RaycastHit hit,
                                                        bool didHit,
                                                        WorldAgent agent,
                                                        ClickAbility clickAbility)
    {
        CommandPackage package = new CommandPackage(clickAbility.queueingAgent);
        package.SetType("click");

        if (clickAbility.CanClick(hit, agent))
        {
            package.SetCursor(clickAbility.validCursorPath);
            package.SetInfo(agent.displayName);
            package.MarkValid();
        }
        else
        {
            package.SetCursor(clickAbility.invalidCursorPath);
        }

        foreach (Command command in clickAbility.commands)
        {
            package.AddCommand(command);
        }

        foreach (WorldAgent affectedAgent in clickAbility.GetAffectedAgents())
        {
            package.SetHighlight(affectedAgent, true);
        }

        package.SetHint(clickAbility.GetHint());

        return package;
    }

    public static CommandPackage GetFinalizedClickAbilityPackage(ClickAbility clickAbility)
    {
        CommandPackage package = new CommandPackage(clickAbility.queueingAgent);
        package.SetType("click");

        foreach (Command command in clickAbility.commands)
        {
            package.AddCommand(command);
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
            attacker,
            receiver
        );
        if (!inRangeCommand.possible) return EmptyPackage();
        AttackCommand attackCommand = new AttackCommand(attacker, receiver, damageManager);
        LookCommand lookCommand = new LookCommand(attacker, receiver);

        Command[] commands = new Command[] { inRangeCommand, lookCommand, attackCommand };
        TrimUnnecessaryMoveCommands(ref commands);
        CommandPackage package = new CommandPackage(attacker, commands);

        package.SetHighlight(attacker, false);
        package.SetHighlight(receiver, false);
        package.SetType("attack");
        package.SetCursor("Crosshair");
        package.SetInfo($"{receiver.displayName} ({receiver.localStats.GetHitPointsStatusString()})");

        return package;
    }

    public static bool AllMoveCommandsPossible(Command[] commands)
    {
        return !commands
                .Where(c => c is IMoveCommand)
                .Select(c => (c as IMoveCommand).possible)
                .Where(possible => !possible)
                .Any();
    }

    public static bool InvalidCommandInCollection(IEnumerable<Command> commands)
    {
        return commands.Any(c => c.status == Command.Status.Invalid);
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
        /// <p> Info about the target of the package </p>
        /// Displayed above the ap and resource cost of the action
        public string info { get; private set; }
        /// Hint about what actions the player must perform
        public string hint { get; private set; }
        public string type { get; private set; }
        public bool valid { get; private set; }
        public bool unreachable { get; private set; }
        public CursorInfo cursorInfo { get; private set; }
        public readonly WorldAgent agent;
        public readonly Dictionary<WorldAgent, bool> highlights;
        public readonly List<Command> commands;
        public readonly bool empty;
        public readonly bool clickOnAgentOnly;

        public CommandPackage(bool empty = true)
        {
            this.agent = null;
            this.commands = new();
            highlights = new Dictionary<WorldAgent, bool>();
            this.empty = empty;
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

        public void SetHint(string hint)
        {
            this.hint = hint;
        }

        public void SetInfo(string info)
        {
            this.info = info;
        }

        public void SetCursor(string resourcePath)
        {
            cursorInfo = Resources.Load<CursorInfo>($"Cursors/{resourcePath}");
        }

        public void MarkValid()
        {
            valid = true;
        }

        public bool QueueCommands(RoundClock.ProgressMode mode, ResourceManager resourceManager)
        {
            switch (mode)
            {
                case RoundClock.ProgressMode.Manual:
                    if (!CanQueueCommands()) return false;
                    else
                    {
                        resourceManager.ProcessCommands(commands);
                        agent.QueueCommands(commands.Where(c => c.status != Command.Status.Invalid).ToArray());
                    }
                    break;
                case RoundClock.ProgressMode.Automatic:
                    resourceManager.ProcessCommands(commands);
                    agent.OverwriteQueue(commands.Where(c => c.status != Command.Status.Invalid).ToArray());
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
                queueCost += command.apCost;
                if (!agent && valid) return true;
                else if (!agent) return false;
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
                total += command.apCost;
            }
            return total;
        }
    }
}