namespace GolfSharp;

public static class Program
{
    private static readonly Dictionary<string, Command> _commands = new()
    {
        ["writeLine"] = new AliasCommand("System.Console.WriteLine", ExpressionType.Void, [ExpressionType.String]),
        ["range"] = new AliasCommand("System.Linq.Enumerable.Range", ExpressionType.Float.ArrayOf(), [ExpressionType.Float, ExpressionType.Float]),
        ["readLine"] = new AliasCommand("System.Console.ReadLine", ExpressionType.String, []),

        ["convert_toString"] = new AliasCommand("System.Convert.ToString", ExpressionType.String, [ExpressionType.Unknown]),
    };
    private static readonly Dictionary<string, string> _commandAliases = new()
    {
        ["w"] = "writeLine",
        ["#"] = "range",
        ["i"] = "readLine",
    };


    public static void Main(string[] args)
    {
        Tokenizer tokenizer = new();
        var result = tokenizer.Parse("w\"Hello, World!\"");
        Parser parser = new(_commands, _commandAliases);
        var node = parser.Parse(result);
        CodeGen gen = new();
        var code = gen.Generate(node);
        Console.WriteLine(code);
    }
}
