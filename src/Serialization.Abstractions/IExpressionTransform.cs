namespace vm2.Linq.Expressions.Serialization;

/// <summary>
/// Defines the bidirectional transform between a <typeparamref name="TDocument"/> and a LINQ <see cref="Expression"/>.
/// </summary>
/// <typeparam name="TDocument">The document type (e.g. XDocument, JsonNode).</typeparam>
public interface IExpressionTransform<TDocument>
{
    /// <summary>
    /// Transforms the given <see cref="Expression"/> into a <typeparamref name="TDocument"/>.
    /// </summary>
    /// <param name="expression">The expression to transform.</param>
    /// <returns>The resulting document.</returns>
    TDocument Transform(Expression expression);

    /// <summary>
    /// Transforms the given <typeparamref name="TDocument"/> into an <see cref="Expression"/>.
    /// </summary>
    /// <param name="document">The document to transform.</param>
    /// <returns>The resulting expression.</returns>
    Expression Transform(TDocument document);
}
