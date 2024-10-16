namespace GolfSharp;

public abstract record class CommandNode(string CommandName, List<ExpressionNode> Args) : ExpressionNode;
public record class UnresolvedCommandNode : CommandNode, IParentNode
{
    public UnresolvedCommandNode(string commandName, List<ExpressionNode> args)
        : base(commandName, args) { }

    public void Add(ExpressionNode child) => Args.Add(child);
    public void AddRange(IEnumerable<ExpressionNode> children) => Args.AddRange(children);
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

