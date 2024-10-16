using System.Text;

namespace GolfSharp;

public class CodeGen
{
    private readonly StringBuilder _buffer = new();

    public string Generate(ExpressionNode node)
    {
        _buffer.Clear();
        GenerateHeader();
        GenerateExpression(node);
        GenerateTrailer();
        return _buffer.ToString();
    }

    private void GenerateHeader()
    {
        _buffer.Append("""
            class GolfSharp_Generated
            {
                public static Dictionary<string, object> _vars = new();

                public static void Main(string[] args)
                {
                    Run();
                }

                private static void Run()
                {
            """);
    }
    private void GenerateTrailer()
    {
        _buffer.Append(';');
        _buffer.Append("""
                }
            }
            """);
    }

    private void GenerateExpression(ExpressionNode node)
    {
        switch(node)
        {
            case LiteralNode lit:
                GenerateLiteral(lit);
                break;
            case ResolvedCommandNode cmd:
                GenerateCommand(cmd);
                break;
            case BlockNode block:
                _buffer.Append("()=>{");
                foreach (var child in block.Nodes)
                {
                    GenerateExpression(child);
                    _buffer.Append(';');
                }
                _buffer.Append("}()");
                break;
            default:
                throw new InvalidOperationException($"Unknown expression type: {node.GetType()}");
        };
    }

    private void GenerateLiteral(LiteralNode node)
    {
        switch(node)
        {
            case StringNode str:
                _buffer.Append($"\"{str.Value}\"");
                break;
            case BoolNode bln:
                _buffer.Append(bln.Value ? "true" : "false");
                break;
            case FloatNode flt:
                _buffer.Append(flt.Value);
                break;
            default:
                throw new InvalidOperationException($"Unknown literal type: {node.GetType()}");
        };
    }

    private void GenerateCommand(ResolvedCommandNode cmdNode)
    {
        Command cmd = cmdNode.Command;
        switch(cmd)
        {
            case AliasCommand alias:
                _buffer.Append(alias.TrueName);
                _buffer.Append('(');
                for (int i = 0; i < cmdNode.Args.Count; i++)
                {
                    GenerateExpression(cmdNode.Args[i]);
                    if (i < cmdNode.Args.Count - 1)
                        _buffer.Append(',');
                }
                _buffer.Append(')');
                break;
            default:
                throw new InvalidOperationException($"Unknown command type: {cmd.GetType()}");
        }
    }
}
