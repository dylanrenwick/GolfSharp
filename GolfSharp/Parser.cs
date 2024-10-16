namespace GolfSharp;

public class Parser
{
    private readonly Dictionary<string, Command> _commandMappings;

    public Parser(Dictionary<string, Command> commandMappings)
    {
        _commandMappings = commandMappings;
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
        return node switch
        {
            UnresolvedCommandNode cmd => ResolveCommand(cmd),
            _ => node,
        };
    }

    private ResolvedCommandNode ResolveCommand(UnresolvedCommandNode cmd)
    {
        var args = cmd.Args.Select(ResolveCommands).ToList();
        if (!_commandMappings.TryGetValue(cmd.CommandName, out Command? command))
            throw new InvalidOperationException($"Unknown command: {cmd.CommandName}");
        return new ResolvedCommandNode(cmd.CommandName, args, command);
    }
}

