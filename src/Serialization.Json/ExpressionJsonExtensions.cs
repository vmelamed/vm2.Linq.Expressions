namespace vm2.Linq.Expressions.Serialization.Json;

/// <summary>
/// Extension methods that provide a simplified API for serializing and deserializing LINQ expression trees to and from JSON.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ExpressionJsonExtensions
{
    // ── Expression → document ────────────────────────────────────

    /// <summary>
    /// Transforms the expression to a <see cref="JsonObject"/>.
    /// </summary>
    public static JsonObject ToJsonDocument(this Expression expression, JsonOptions? options = null)
        => new ExpressionJsonTransform(options).Transform(expression);

    /// <summary>
    /// Transforms the expression to a JSON string.
    /// </summary>
    public static string ToJsonString(this Expression expression, JsonOptions? options = null)
    {
        options ??= new();
        var document = new ExpressionJsonTransform(options).Transform(expression);
        return document.ToJsonString(options.JsonSerializerOptions);
    }

    // ── Expression → stream / writer / file ──────────────────────

    /// <summary>
    /// Serializes the expression to JSON and writes it to the specified <paramref name="stream"/>.
    /// </summary>
    public static void ToJsonStream(this Expression expression, Stream stream, JsonOptions? options = null)
        => new ExpressionJsonTransform(options).Serialize(expression, stream);

    /// <summary>
    /// Serializes the expression to JSON and writes it to the specified <paramref name="stream"/>.
    /// </summary>
    public static Task ToJsonStreamAsync(this Expression expression, Stream stream, JsonOptions? options = null, CancellationToken cancellationToken = default)
        => new ExpressionJsonTransform(options).SerializeAsync(expression, stream, cancellationToken);

    /// <summary>
    /// Serializes the expression to JSON and writes it to the specified <see cref="Utf8JsonWriter"/>.
    /// </summary>
    public static void ToJsonWriter(this Expression expression, Utf8JsonWriter writer, JsonOptions? options = null)
    {
        options ??= new();
        var document = new ExpressionJsonTransform(options).Transform(expression);
        document.WriteTo(writer, options.JsonSerializerOptions);
        writer.Flush();
    }

    /// <summary>
    /// Serializes the expression to JSON and writes it to the specified <see cref="Utf8JsonWriter"/>.
    /// </summary>
    public static async Task ToJsonWriterAsync(this Expression expression, Utf8JsonWriter writer, JsonOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= new();
        var document = new ExpressionJsonTransform(options).Transform(expression);
        document.WriteTo(writer, options.JsonSerializerOptions);
        await writer.FlushAsync(cancellationToken);
    }

    /// <summary>
    /// Serializes the expression to JSON and writes it to the specified file.
    /// </summary>
    public static void ToJsonFile(this Expression expression, string filePath, JsonOptions? options = null)
    {
        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        new ExpressionJsonTransform(options).Serialize(expression, stream);
    }

    /// <summary>
    /// Serializes the expression to JSON and writes it to the specified file.
    /// </summary>
    public static async Task ToJsonFileAsync(this Expression expression, string filePath, JsonOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await new ExpressionJsonTransform(options).SerializeAsync(expression, stream, cancellationToken);
    }

    // ── Document → Expression ────────────────────────────────────

    /// <summary>
    /// Transforms the <see cref="JsonObject"/> to a LINQ <see cref="Expression"/>.
    /// </summary>
    public static Expression ToExpression(this JsonObject document, JsonOptions? options = null)
        => new ExpressionJsonTransform(options).Transform(document);
}
