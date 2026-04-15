namespace vm2.Linq.Expressions.Serialization.Json;

/// <summary>
/// Static methods for deserializing LINQ expression trees from JSON sources (streams, files, strings).
/// </summary>
public static class ExpressionJson
{
    // ── From stream ──────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the JSON content in the specified <paramref name="stream"/>.
    /// </summary>
    public static Expression FromStream(Stream stream, JsonOptions? options = null)
        => new ExpressionJsonTransform(options).Deserialize(stream);

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the JSON content in the specified <paramref name="stream"/>.
    /// </summary>
    public static Task<Expression> FromStreamAsync(Stream stream, JsonOptions? options = null, CancellationToken cancellationToken = default)
        => new ExpressionJsonTransform(options).DeserializeAsync(stream, cancellationToken);

    // ── From file ────────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the JSON file at the specified <paramref name="filePath"/>.
    /// </summary>
    public static Expression FromFile(string filePath, JsonOptions? options = null)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new ExpressionJsonTransform(options).Deserialize(stream);
    }

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the JSON file at the specified <paramref name="filePath"/>.
    /// </summary>
    public static async Task<Expression> FromFileAsync(string filePath, JsonOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        return await new ExpressionJsonTransform(options).DeserializeAsync(stream, cancellationToken);
    }

    // ── From string ──────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the specified JSON <paramref name="json"/> string.
    /// </summary>
    public static Expression FromString(string json, JsonOptions? options = null)
    {
        var node = JsonNode.Parse(
                        json,
                        new JsonNodeOptions { PropertyNameCaseInsensitive = false },
                        new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip, MaxDepth = 1000 })
                    ?? throw new SerializationException("Could not parse the JSON string.");

        if (node.GetValueKind() != JsonValueKind.Object)
            throw new SerializationException($"The JSON string does not contain a JSON object but {node.GetValueKind()}.");

        return new ExpressionJsonTransform(options).Transform(node.AsObject());
    }
}
