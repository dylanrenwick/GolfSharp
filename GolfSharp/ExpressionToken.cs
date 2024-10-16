namespace GolfSharp;

public abstract record class ExpressionToken;

public record class LiteralToken(object Value, Type Type) : ExpressionToken;
public record class CommandToken(string Command, IList<ExpressionToken> Args) : ExpressionToken;
public record class OperationToken(ExpressionToken Left, string Op, ExpressionToken Right) : ExpressionToken;
