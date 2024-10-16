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
        _parentContext = null;
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

    private CommandNode CommandNodeFromToken(CommandToken token)
    {
        return new UnresolvedCommandNode(token.Command, token.Args.Select(NodeFromToken).ToList());
    }

    private CommandNode CommandNodeFromToken(OperationToken token)
    {
        return new UnresolvedCommandNode(token.Op, [NodeFromToken(token.Left), NodeFromToken(token.Right)]);
    }

    private ExpressionNode ResolveCommands(ExpressionNode node)
    {
        if (node is UnresolvedCommandNode cmd)
        {
            _parentContext = cmd;
            return ResolveCommand(cmd, out var extraNodes);
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

        if (args.Count < command.ArgCount)
            throw new InvalidOperationException($"Not enouth args for command {cmd.CommandName} expected {command.ArgCount} got {args.Count}");

        extraNodes = args.Skip(command.ArgCount).ToList();
        args = args.Take(command.ArgCount).ToList();
        for (int i = 0; i < command.ArgCount; i++)
        {
            var expected = command.ArgTypes[i];
            var actual = args[i].ResultType;
            if (expected != actual)
                args[i] = ReconcileType(args[i], expected);
        }

        return new ResolvedCommandNode(cmd.CommandName, args, command);
    }

    private ExpressionNode ReconcileType(ExpressionNode node, ExpressionType expectedType)
    {
        if (node.ResultType == expectedType)
            return node;
        throw new InvalidCastException($"Could not convert type {node.ResultType} to {expectedType}");
    }
}

