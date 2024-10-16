namespace GolfSharp;

public abstract record class LiteralNode : ExpressionNode;
public record class StringNode(string Value) : LiteralNode
{
    public override ExpressionType ResultType => ExpressionType.String;
}
public record class BoolNode(bool Value) : LiteralNode
{
    public override ExpressionType ResultType => base.ResultType;
}
public record class FloatNode(float Value) : LiteralNode
{
    public override ExpressionType ResultType => base.ResultType;
}
public record class ArrayNode(ExpressionNode[] Values) : LiteralNode
{
    public override ExpressionType ResultType => Values.Length > 0 ? Values[0].ResultType.ArrayOf() : ExpressionType.Unknown;
}

