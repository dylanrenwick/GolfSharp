namespace GolfSharp;

public abstract record class Command(ExpressionType ResultType, ExpressionType[] ArgTypes)
{
    public int ArgCount => ArgTypes.Length;
}
public record class AliasCommand : Command
{
    public string TrueName { get; init; }

    public AliasCommand(string trueName, ExpressionType resultType, ExpressionType[] argTypes)
        : base(resultType, argTypes)
    {
        TrueName = trueName;
    }
}

