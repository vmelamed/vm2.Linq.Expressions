// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Linq.Expressions.Serialization.Json;

/// <summary>
/// Extension methods that provide a simplified API for serializing and deserializing LINQ expression trees to and from JSON.
/// </summary>
public static class ExpressionJsonExtensions
{
    // ── Expression → JSON document ────────────────────────────────────

    /// <summary>
    /// Transforms the expression to a JSON document model of type <see cref="JsonObject"/>.
    /// </summary>
    /// <param name="expression">The expression to be transformed.</param>
    /// <param name="options">The options to control the transformation process.</param>
    /// <returns>The resultant top level document model document <see cref="JsonObject"/>.</returns>
    public static JsonObject ToJsonDocument(
        this Expression expression,
        JsonOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(expression);

        return new ExpressionJsonTransform(options).Transform(expression);
    }

    // ── Expression → string ────────────────────────────────────

    /// <summary>
    /// Transforms the expression to a JSON string.
    /// </summary>
    /// <param name="expression">The expression to be transformed.</param>
    /// <param name="options">The options to control the transformation process.</param>
    /// <returns>The resultant JSON string.</returns>
    public static string ToJsonString(
        this Expression expression,
        JsonOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(expression);

        options ??= new();

        var document = new ExpressionJsonTransform(options).Transform(expression);

        return document.ToJsonString(options.JsonSerializerOptions);
    }

    // ── Expression → stream ──────────────────────

    /// <summary>
    /// Serializes the expression to JSON and writes it to the specified <paramref name="stream"/>.
    /// </summary>
    /// <param name="expression">The expression to be serialized.</param>
    /// <param name="stream">The stream to which the JSON will be written.</param>
    /// <param name="options">The options to control the transformation process.</param>
    public static void ToJsonStream(
        this Expression expression,
        Stream stream,
        JsonOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(stream);

        new ExpressionJsonTransform(options).Serialize(expression, stream);
    }

    /// <summary>
    /// Serializes the expression to JSON and writes it to the specified <paramref name="stream"/>.
    /// </summary>
    /// <param name="expression">The expression to be serialized.</param>
    /// <param name="stream">The stream to which the JSON will be written.</param>
    /// <param name="options">The options to control the transformation process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static Task ToJsonStreamAsync(
        this Expression expression,
        Stream stream,
        JsonOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(stream);

        return new ExpressionJsonTransform(options).SerializeAsync(expression, stream, cancellationToken);
    }

    // ── Expression → writer ──────────────────────

    /// <summary>
    /// Serializes the expression to JSON and writes it to the specified <see cref="Utf8JsonWriter"/>.
    /// </summary>
    /// <param name="expression">The expression to be serialized.</param>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to which the JSON will be written.</param>
    /// <param name="options">The options to control the transformation process.</param>
    public static void ToJsonWriter(
        this Expression expression,
        Utf8JsonWriter writer,
        JsonOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(writer);

        options ??= new();

        var document = new ExpressionJsonTransform(options).Transform(expression);

        document.WriteTo(writer, options.JsonSerializerOptions);
        writer.Flush();
    }

    /// <summary>
    /// Serializes the expression to JSON and writes it to the specified <see cref="Utf8JsonWriter"/>.
    /// </summary>
    /// <param name="expression">The expression to be serialized.</param>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to which the JSON will be written.</param>
    /// <param name="options">The options to control the transformation process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task ToJsonWriterAsync(
        this Expression expression,
        Utf8JsonWriter writer,
        JsonOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(writer);

        options ??= new();

        var document = new ExpressionJsonTransform(options).Transform(expression);

        document.WriteTo(writer, options.JsonSerializerOptions);
        await writer.FlushAsync(cancellationToken);
    }

    // ── Expression → file ──────────────────────

    /// <summary>
    /// Serializes the expression to JSON and writes it to the specified file.
    /// </summary>
    /// <param name="expression">The expression to be serialized.</param>
    /// <param name="filePath">The path of the file to which the JSON will be written.</param>
    /// <param name="options">The options to control the transformation process.</param>
    public static void ToJsonFile(
        this Expression expression,
        string filePath,
        JsonOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

        new ExpressionJsonTransform(options).Serialize(expression, stream);
    }

    /// <summary>
    /// Serializes the expression to JSON and writes it to the specified file.
    /// </summary>
    /// <param name="expression">The expression to be serialized.</param>
    /// <param name="filePath">The path of the file to which the JSON will be written.</param>
    /// <param name="options">The options to control the transformation process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task ToJsonFileAsync(
        this Expression expression,
        string filePath,
        JsonOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);

        await new ExpressionJsonTransform(options).SerializeAsync(expression, stream, cancellationToken);
    }
}
