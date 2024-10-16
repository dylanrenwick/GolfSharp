namespace GolfSharp;

public static class Program
{
    private static readonly Dictionary<string, Command> _commandMappings = new()
    {
        ["w"] = new AliasCommand("Console.WriteLine", ExpressionType.Void, [ExpressionType.String]),
        ["#"] = new AliasCommand("Enumerable.Range", ExpressionType.Float.ArrayOf(), [ExpressionType.Float, ExpressionType.Float]),

    };

    public static void Main(string[] args)
    {
        Tokenizer tokenizer = new();
        var result = tokenizer.Parse("w\"Hello, World!\"");
        Parser parser = new(_commandMappings);
        var node = parser.Parse(result);
        CodeGen gen = new();
        var code = gen.Generate(node);
        Console.WriteLine(code);
    }
}
