using System.Numerics;

namespace GolfSharp;

public interface IParentNode
{
    public void Add(ExpressionNode child);
    public void AddRange(IEnumerable<ExpressionNode> children);
}
public abstract record class ExpressionNode
{
    public virtual ExpressionType ResultType => ExpressionType.Unknown;
}

public record class BlockNode(List<ExpressionNode> Nodes) : ExpressionNode, IParentNode
{
    public override ExpressionType ResultType => Nodes.Count > 0 ? Nodes[0].ResultType : ExpressionType.Void;

    public void Add(ExpressionNode child) => Nodes.Add(child);
    public void AddRange(IEnumerable<ExpressionNode> children) => Nodes.AddRange(children);
}

public readonly struct ExpressionType : IEqualityOperators<ExpressionType, ExpressionType, bool>
{
    private static readonly string[] TypeNames = { "void", "unknown", "string", "bool", "float" };
    private static int ValFromName(string name) => Array.IndexOf(TypeNames, name.ToLower());
    private static string NameFromVal(int val) => TypeNames[val];

    public static readonly ExpressionType Void = new(ValFromName("void"));
    public static readonly ExpressionType Unknown = new(ValFromName("unknown"));
    public static readonly ExpressionType String = new(ValFromName("string"));
    public static readonly ExpressionType Bool = new(ValFromName("bool"));
    public static readonly ExpressionType Float = new(ValFromName("float"));

    public readonly bool IsArray;
    private readonly int _value;

    private ExpressionType(int value, bool isArray = false)
    {
        _value = value;
        IsArray = isArray;
    }

    public ExpressionType ArrayOf() => new(_value, true);
    public bool IsArrayOf(ExpressionType other)
        => other._value == _value && IsArray && !other.IsArray;

    public override string ToString()
    {
        return $"{NameFromVal(_value)}{(IsArray ? "[]" : "")}";
    }

    public static bool operator ==(ExpressionType left, ExpressionType right)
    {
        return left._value == right._value && left.IsArray == right.IsArray;
    }

    public static bool operator !=(ExpressionType left, ExpressionType right)
    {
        return !(left == right);
    }
}

