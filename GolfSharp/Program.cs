namespace GolfSharp;

public static class Program
{
    private static readonly Dictionary<string, Command> _commands = new()
    {
        ["writeLine"] = new AliasCommand("System.Console.WriteLine", ExpressionType.Void, [ExpressionType.String]),
        ["readLine"] = new AliasCommand("System.Console.ReadLine", ExpressionType.String, []),
        ["range"] = new AliasCommand("System.Linq.Enumerable.Range", ExpressionType.Float.ArrayOf(), [ExpressionType.Float, ExpressionType.Float]),

        ["add"] = new AliasCommand("((a,b)=>a+b)", ExpressionType.Float, [ExpressionType.Float, ExpressionType.Float]),
        ["sub"] = new AliasCommand("((a,b)=>a-b)", ExpressionType.Float, [ExpressionType.Float, ExpressionType.Float]),
        ["mul"] = new AliasCommand("((a,b)=>a*b)", ExpressionType.Float, [ExpressionType.Float, ExpressionType.Float]),
        ["div"] = new AliasCommand("((a,b)=>a/b)", ExpressionType.Float, [ExpressionType.Float, ExpressionType.Float]),
        ["mod"] = new AliasCommand("((a,b)=>a%b)", ExpressionType.Float, [ExpressionType.Float, ExpressionType.Float]),

        ["convert_toString"] = new AliasCommand("System.Convert.ToString", ExpressionType.String, [ExpressionType.Unknown]),
        ["convert_FloattoBool"] = new AliasCommand("(n=>n!=0)", ExpressionType.Bool, [ExpressionType.Float]),
        ["convert_StringtoBool"] = new AliasCommand("System.String.IsNullOrEmpty", ExpressionType.Bool, [ExpressionType.String]),
        ["convert_StringtoFloat"] = new AliasCommand("System.Single.Parse", ExpressionType.Float, [ExpressionType.String]),
        ["convert_BooltoFloat"] = new AliasCommand("(n=>n?1:0)", ExpressionType.Float, [ExpressionType.Bool]),
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
        var result = tokenizer.Parse("{w\"Hello, World!\"");
        Parser parser = new(_commands, _commandAliases);
        var node = parser.Parse(result);
        CodeGen gen = new();
        var code = gen.Generate(node);
        Console.WriteLine(code);
    }
}
