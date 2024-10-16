namespace GolfSharp.Test;

using GolfSharp;

public class UnitTest1
{
    [Fact]
    public void HelloWorld()
    {
        Tokenizer tokenizer = new();
        var result = tokenizer.Parse("w\"Hello, World!\"");

        Assert.IsType<CommandToken>(result);
        var command = (CommandToken)result;
        Assert.Equal("w", command.Command);
        Assert.Single(command.Args);
        var arg = command.Args[0];
        Assert.IsType<LiteralToken>(arg);
        var literal = (LiteralToken)arg;
        Assert.Equal("Hello, World!", literal.Value);
        Assert.Equal(typeof(string), literal.Type);
    }

    [Fact]
    public void SimpleRange()
    {
        Tokenizer tokenizer = new();
        var result = tokenizer.Parse("#1,10");

        Assert.IsType<CommandToken>(result);
        var command = (CommandToken)result;
        Assert.Equal("#", command.Command);
        Assert.Equal(2, command.Args.Count);

        var arg1 = command.Args[0];
        Assert.IsType<LiteralToken>(arg1);
        var literal1 = (LiteralToken)arg1;
        Assert.Equal(1f, literal1.Value);
        Assert.Equal(typeof(float), literal1.Type);

        var arg2 = command.Args[1];
        Assert.IsType<LiteralToken>(arg2);
        var literal2 = (LiteralToken)arg2;
        Assert.Equal(10f, literal2.Value);
        Assert.Equal(typeof(float), literal2.Type);
    }
}