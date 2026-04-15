namespace vm2.Linq.Expressions.Serialization.Json;

/// <summary>
/// Class ExpressionTransform.
/// Implements the <see cref="IExpressionTransform{JsonNode}"/>: transforms a Linq expression to/from a JSON document object.
/// </summary>
/// <seealso cref="IExpressionTransform{XNode}" />
public class ExpressionJsonTransform(JsonOptions? options = null) : IExpressionTransform<JsonObject>
{
    readonly JsonOptions _options = options ?? new();
    JsonNodeOptions _nodeOptions = new() { PropertyNameCaseInsensitive = false };
    ToJsonTransformVisitor? _expressionVisitor;
    FromJsonTransformVisitor? _jsonVisitor;

    #region IExpressionTransform<JsonObject>
    /// <summary>
    /// Transforms the specified <see cref="Expression"/> to a <see cref="JsonNode"/> model.
    /// </summary>
    /// <param name="expression">The expression to be transformed.</param>
    /// <returns>The resultant top level document model document <see cref="JsonNode"/>.</returns>
    public JsonObject Transform(Expression expression)
    {
        _expressionVisitor ??= new ToJsonTransformVisitor(_options);
        _expressionVisitor.Visit(expression);

        return new JsonObject(_nodeOptions)
        {
            { Vocabulary.Schema, JsonOptions.Exs },
            _options.Comment(expression),
            { Vocabulary.Expression, new JsonObject(_nodeOptions) { _expressionVisitor.Result} }
        };
    }

    /// <summary>
    /// Transforms the specified document of type <see cref="JsonNode"/> to a LINQ <see cref="Expression"/>.
    /// </summary>
    /// <param name="document">The document document to be transformed.</param>
    /// <returns>The resultant <see cref="Expression"/>.</returns>
    public Expression Transform(JsonObject document)
    {
        _options.Validate(document);
        return DoTransform(document);
    }
    #endregion

    Expression DoTransform(JsonObject document)
    {
        if (_jsonVisitor is null)
            _jsonVisitor = new FromJsonTransformVisitor();
        else
            _jsonVisitor.ResetVisitState();
        if (!document.TryGetPropertyValue(Vocabulary.Expression, out var expression))
            throw new SerializationException($"The JSON document does not have property {Vocabulary.Expression} at the root.");
        return _jsonVisitor.Visit(new JElement(Vocabulary.Expression, expression));
    }

    /// <summary>
    /// Serializes the specified expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="stream">The stream to put the JSON document to.</param>
    /// <returns>Stream.</returns>
    public void Serialize(
        Expression expression,
        Stream stream)
    {
        using var writer = SerializeToWriter(expression, stream);
        writer.Flush();
    }

    /// <summary>
    /// Serializes the specified expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="stream">The stream to put the JSON document to.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Stream.</returns>
    public async Task SerializeAsync(
        Expression expression,
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        using var writer = SerializeToWriter(expression, stream);
        await writer.FlushAsync(cancellationToken);
    }

    Utf8JsonWriter SerializeToWriter(
        Expression expression,
        Stream stream)
    {
        var document = Transform(expression);
        var writer = new Utf8JsonWriter(
                                    stream,
                                    new JsonWriterOptions()
                                    {
                                        Indented       = _options.Indent,
                                        SkipValidation = false,
                                    });

        document.WriteTo(writer, _options.JsonSerializerOptions);
        return writer;
    }

    /// <summary>
    /// Serializes the specified expression.
    /// </summary>
    /// <param name="stream">The stream to get the JSON document from.</param>
    /// <returns>Stream.</returns>
    public Expression Deserialize(
        Stream stream)
    {
        var document = JsonNode.Parse(
                        stream,
                        new JsonNodeOptions()
                        {
                            PropertyNameCaseInsensitive = false
                        },
                        new JsonDocumentOptions()
                        {
                            AllowTrailingCommas = true,
                            CommentHandling     = JsonCommentHandling.Skip,
                            MaxDepth            = 1000
                        })
                        ??
                        throw new SerializationException("Could not load JSON object;");

        if (document.GetValueKind() != JsonValueKind.Object)
            throw new SerializationException($"The document does not contain a JSON object but {document.GetValueKind()}");

        return Transform(document.AsObject());
    }

    /// <summary>
    /// Serializes the specified expression.
    /// </summary>
    /// <param name="stream">The stream to get the JSON document from.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Stream.</returns>
    public async Task<Expression> DeserializeAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        var document = await JsonNode.ParseAsync(
                        stream,
                        new JsonNodeOptions()
                        {
                            PropertyNameCaseInsensitive = false
                        },
                        new JsonDocumentOptions()
                        {
                            AllowTrailingCommas = true,
                            CommentHandling     = JsonCommentHandling.Skip,
                            MaxDepth            = 1000
                        },
                        cancellationToken)
                        ??
                        throw new SerializationException("Could not load JSON object;");

        if (document.GetValueKind() != JsonValueKind.Object)
            throw new SerializationException($"The document does not contain a JSON object but {document.GetValueKind()}");

        return Transform(document.AsObject());
    }
}
