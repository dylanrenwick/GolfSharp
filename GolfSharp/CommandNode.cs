namespace GolfSharp;

public abstract record class CommandNode(string CommandName, List<ExpressionNode> Args) : ExpressionNode;
public record class UnresolvedCommandNode : CommandNode
{
    public UnresolvedCommandNode(string commandName, List<ExpressionNode> args)
        : base(commandName, args) { }
}

public record class ResolvedCommandNode : CommandNode
{
    public Command Command { get; init; }
    public override ExpressionType ResultType => Command.ResultType;

    public ResolvedCommandNode(string commandName, List<ExpressionNode> args, Command command)
        : base(commandName, args)
    {
        Command = command;        
    }
}

