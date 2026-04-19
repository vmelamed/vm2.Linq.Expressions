namespace vm2.Linq.Expressions.Serialization.Exceptions;

/// <summary>
/// Thrown when the expression visitor encounters an unexpected or not-implemented expression node.
/// </summary>
/// <param name="messageFormat">The message format string (expects one string argument).</param>
/// <param name="param">The parameter substituted into the format.</param>
/// <param name="innerException">The inner exception.</param>
[ExcludeFromCodeCoverage]
public class NotImplementedExpressionException(
    string messageFormat = NotImplementedExpressionException.DefaultMessageFormat,
    string? param = null,
    Exception? innerException = null) : Exception(string.Format(messageFormat, param ?? ""), innerException)
{
    const string DefaultMessageFormat =
        "Expression node {0} was encountered while visiting the expression. " +
        "This visitor does not process expression nodes of types: `DebugInfoExpression`, " +
        "`DynamicExpression`, `RuntimeVariablesExpression` and does not override the " +
        "visitor method `ExpressionVisitor.VisitExtension`.";
}
