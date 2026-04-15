namespace vm2.Linq.Expressions.Serialization.Json.Tests;

public class ExpressionJsonSerializationExceptionTests
{
    [Fact]
    public void DefaultMessage_WhenNoMessageProvided()
    {
        var ex = new ExpressionJsonSerializationException();
        ex.Message.Should().Contain("error occurred during expression JSON serialization");
        ex.InnerException.Should().BeNull();
    }

    [Fact]
    public void CustomMessage()
    {
        var ex = new ExpressionJsonSerializationException("custom message");
        ex.Message.Should().Be("custom message");
    }

    [Fact]
    public void WithInnerException()
    {
        var inner = new InvalidOperationException("inner");
        var ex = new ExpressionJsonSerializationException("outer", inner);
        ex.Message.Should().Be("outer");
        ex.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public void IsInvalidOperationException()
    {
        var ex = new ExpressionJsonSerializationException();
        ex.Should().BeAssignableTo<InvalidOperationException>();
    }
}
