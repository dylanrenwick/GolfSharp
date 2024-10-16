namespace GolfSharp;

public class Parser
{
    private readonly Dictionary<string, Command> _commands;
    private readonly Dictionary<string, string> _commandAliases;

    public Parser(Dictionary<string, Command> commands, Dictionary<string, string> commandAliases)
    {
        _commands = commands;
        _commandAliases = commandAliases;
    }

    public ExpressionNode Parse(ExpressionToken rootToken)
    {
        ExpressionNode firstPass = NodeFromToken(rootToken);
        return ResolveCommands(firstPass);
    }

    private ExpressionNode NodeFromToken(ExpressionToken token)
    {
        return token switch
        {
            LiteralToken lit => LiteralNodeFromToken(lit),
            CommandToken cmd => CommandNodeFromToken(cmd),
            OperationToken op => CommandNodeFromToken(op),
            BlockToken block => BlockNodeFromToken(block),
            _ => throw new InvalidOperationException($"Unknown token type: {token.GetType()}")
        };
    }

    private LiteralNode LiteralNodeFromToken(LiteralToken token)
    {
        if (token.Type == typeof(string))
            return new StringNode((string)token.Value);
        if (token.Type == typeof(bool))
            return new BoolNode((bool)token.Value);
        if (token.Type == typeof(float))
            return new FloatNode((float)token.Value);
        throw new InvalidOperationException($"Unknown literal type: {token.Type}");
    }

    private BlockNode BlockNodeFromToken(BlockToken token)
    {
        return new BlockNode(token.Nodes.Select(NodeFromToken).ToList());
    }

    private CommandNode CommandNodeFromToken(CommandToken token)
    {
        return new UnresolvedCommandNode(token.Command, token.Args.Select(NodeFromToken).ToList());
    }

    private CommandNode CommandNodeFromToken(OperationToken token)
    {
        return new UnresolvedCommandNode(token.Op, [NodeFromToken(token.Left), NodeFromToken(token.Right)]);
    }

    private ExpressionNode ResolveCommands(ExpressionNode node, IParentNode? parent = null)
    {
        if (node is UnresolvedCommandNode cmd)
        {
            var resolvedCmd = ResolveCommand(cmd, out var extraNodes);
            if (extraNodes.Any())
            {
                if (parent is not null)
                    parent.AddRange(extraNodes);
                else
                    throw new InvalidOperationException("Found orphaned args");
            }
            return resolvedCmd;
        }
        else if (node is BlockNode block)
        {
            List<ExpressionNode> nodes = [];
            for (int i = 0; i < block.Nodes.Count; i++)
            {
                nodes.Add(ResolveCommands(block.Nodes[i], block));
            }
            return new BlockNode(nodes);
        }

        return node;
    }

    private ResolvedCommandNode ResolveCommand(UnresolvedCommandNode cmd, out IEnumerable<ExpressionNode> extraNodes)
    {
        if (!_commands.TryGetValue(cmd.CommandName, out Command? command))
        {
            if (!_commandAliases.TryGetValue(cmd.CommandName, out string? alias))
                throw new InvalidOperationException($"Unknown command: {cmd.CommandName}");
            if (!_commands.TryGetValue(alias, out command))
                throw new InvalidOperationException($"Unknown command: {cmd.CommandName}");
        }

        List<ExpressionNode> args = [];
        for (int i = 0; i < cmd.Args.Count; i++)
        {
            args.Add(ResolveCommands(cmd.Args[i], cmd));
        }

        (args, extraNodes) = ReconcileArgTypes(args, command.ArgTypes);

        if (args.Count < command.ArgCount)
            throw new InvalidOperationException($"Not enough args for command {cmd.CommandName} expected {command.ArgCount} got {cmd.Args.Count}");

        return new ResolvedCommandNode(cmd.CommandName, args, command);
    }

    private (List<ExpressionNode>, List<ExpressionNode>) ReconcileArgTypes(List<ExpressionNode> args, ExpressionType[] expectedTypes)
    {
        List<ExpressionNode> selectedArgs = [];
        List<ExpressionNode> extraArgs = [];

        for (int inCount = 0, outCount = 0; inCount < args.Count; inCount++)
        {
            if (outCount >= expectedTypes.Length)
            {
                extraArgs.Add(args[inCount]);
                continue;
            }

            var expected = expectedTypes[inCount];
            var actual = args[inCount].ResultType;

            if (expected == actual || expected == ExpressionType.Unknown)
            {
                selectedArgs.Add(args[inCount]);
                outCount++;
            }
            else if (expected.IsArrayOf(actual))
            {
                List<ExpressionNode> arrArgs = [];
                while (inCount < args.Count && expected.IsArrayOf(args[inCount].ResultType))
                {
                    arrArgs.Add(args[inCount]);
                    inCount++;
                }
                selectedArgs.Add(new ArrayNode(arrArgs.ToArray()));
                outCount++;
            }
            else
            {
                string? conversionCommand = FindConversion(actual, expected);
                if (conversionCommand is null)
                    throw new InvalidOperationException($"Can't cast from type {actual} to {expected}");
                else
                {
                    selectedArgs.Add(WrapInSystemCommand(args[inCount], conversionCommand));
                    outCount++;
                }
            }
        }

        return (selectedArgs, extraArgs);
    }

    private string? FindConversion(ExpressionType @in, ExpressionType @out)
    {
        if (@out == ExpressionType.String)
            return "convert_toString";
        if (@out == ExpressionType.Float)
        {
            if (@in == ExpressionType.Bool)
                return "convert_BooltoFloat";
            if (@in == ExpressionType.String)
                return "convert_StringtoFloat";
        }
        if (@out == ExpressionType.Bool)
        {
            if (@in == ExpressionType.Float)
                return "convert_FloattoBool";
            if (@in == ExpressionType.String)
                return "convert_StringtoBool";
        }

        return null;
    }

    private Command GetSystemCommand(string name)
    {
        if (!_commands.TryGetValue(name, out Command? command))
            throw new InvalidOperationException($"Could not find system command {name}, system commands should always be defined");
        return command;
    }
    private ResolvedCommandNode WrapInSystemCommand(ExpressionNode node, string commandName)
        => WrapInCommand(node, GetSystemCommand(commandName), commandName);
    private ResolvedCommandNode WrapInCommand(ExpressionNode node, Command command, string commandName)
    {
        return new ResolvedCommandNode(commandName, [node], command);
    }
}

