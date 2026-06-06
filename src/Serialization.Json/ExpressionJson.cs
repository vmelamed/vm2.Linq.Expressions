// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Linq.Expressions.Serialization.Json;

/// <summary>
/// Static methods for deserializing LINQ expression trees from JSON sources (streams, files, strings).
/// </summary>
public static class ExpressionJson
{
    // ── From string ──────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the specified JSON <paramref name="json"/> string.
    /// </summary>
    public static Expression FromString(
        [NotNull] string json,
        JsonOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json, nameof(json));

        var node = JsonNode.Parse(json, JsonOptions.JsonNodeOptions, JsonOptions.JsonDocumentOptions)
                        ?? throw new SerializationException("Could not parse the JSON string.");

        if (node.GetValueKind() != JsonValueKind.Object)
            throw new SerializationException($"The JSON string does not contain a JSON object but {node.GetValueKind()}.");

        return new ExpressionJsonTransform(options).Transform(node.AsObject());
    }

    // ── From JSON object ─────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the specified JSON object <paramref name="jsonObject"/>.
    /// </summary>
    /// <param name="jsonObject">The JSON object to be transformed.</param>
    /// <param name="options">The options to control the transformation process.</param>
    /// <returns>The deserialized expression</returns>
    public static Expression FromJsonObject(
        [NotNull] JsonObject jsonObject,
        JsonOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(jsonObject, nameof(jsonObject));

        return new ExpressionJsonTransform(options).Transform(jsonObject);
    }

    // ── From stream ──────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the JSON content in the specified <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream containing the JSON content to be deserialized.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <returns>The deserialized expression</returns>
    public static Expression FromStream(
        [NotNull] Stream stream,
        JsonOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        return new ExpressionJsonTransform(options).Deserialize(stream);
    }

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the JSON content in the specified <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream containing the JSON content to be deserialized.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the deserialization to complete.</param>
    /// <returns>A task that represents the asynchronous deserialization operation. The task result contains the deserialized expression.</returns>
    public static Task<Expression> FromStreamAsync(
        [NotNull] Stream stream,
        JsonOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        return new ExpressionJsonTransform(options).DeserializeAsync(stream, cancellationToken);
    }

    // ── From reader ──────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the JSON content read from the specified <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">The text reader to read the JSON content from.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <returns>The deserialized expression</returns>
    public static Expression FromReader(
        [NotNull] TextReader reader,
        JsonOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(reader, nameof(reader));

        var json = reader.ReadToEnd();

        return FromString(json, options);
    }

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the JSON content read from the specified <paramref name="reader"/>.
    /// </summary>
    /// <param name="reader">The text reader to read the JSON content from.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the deserialization to complete.</param>
    /// <returns>The deserialized expression</returns>
    public static async Task<Expression> FromReaderAsync(
        [NotNull] TextReader reader,
        JsonOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader, nameof(reader));

        var json = await reader.ReadToEndAsync(cancellationToken);

        return FromString(json, options);
    }

    // ── From file ────────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the JSON file at the specified <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">The path to the JSON file to be deserialized.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <returns>The deserialized expression</returns>
    public static Expression FromFile(
        [NotNull] string filePath,
        JsonOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        return new ExpressionJsonTransform(options).Deserialize(stream);
    }

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the JSON file at the specified <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">The path to the JSON file to be deserialized.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the deserialization to complete.</param>
    /// <returns>A task that represents the asynchronous deserialization operation. The task result contains the deserialized expression.</returns>
    public static async Task<Expression> FromFileAsync(
        [NotNull] string filePath,
        JsonOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath, nameof(filePath));

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

        return await new ExpressionJsonTransform(options).DeserializeAsync(stream, cancellationToken);
    }
}
