// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Linq.Expressions.Serialization.Xml;

/// <summary>
/// Extension methods that provide a simplified API for serializing and deserializing LINQ expression trees to and from XML.
/// </summary>
public static class ExpressionXmlExtensions
{
    // ── Expression → document ────────────────────────────────────

    /// <summary>
    /// Transforms the expression to an <see cref="XDocument"/>.
    /// </summary>
    /// <param name="expression">The expression to be transformed.</param>
    /// <param name="options">The options to control the transformation process.</param>
    /// <returns>The resultant top level document model <see cref="XDocument"/>.</returns>
    public static XDocument ToXmlDocument(
        [NotNull] this Expression expression,
        XmlOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(expression);

        return new ExpressionXmlTransform(options).Transform(expression);
    }

    /// <summary>
    /// Transforms the expression to an XML string.
    /// </summary>
    /// <param name="expression">The expression to be transformed.</param>
    /// <param name="options">The options to control the transformation process.</param>
    /// <returns>The resultant XML string.</returns>
    public static string ToXmlString(
        [NotNull] this Expression expression,
        XmlOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(expression);

        var transform = new ExpressionXmlTransform(options ??= new());
        var doc = transform.Transform(expression);

        using var sw = new StringWriter();
        using var xw = XmlWriter.Create(sw, options.XmlWriterSettings);
        doc.WriteTo(xw);
        xw.Flush();
        return sw.ToString();
    }

    // ── Expression → stream / writer / file ──────────────────────

    /// <summary>
    /// Serializes the expression to XML and writes it to the specified <paramref name="stream"/>.
    /// </summary>
    /// <param name="expression">The expression to be serialized.</param>
    /// <param name="stream">The stream to which the XML will be written.</param>
    /// <param name="options">The options to control the transformation process.</param>
    public static void ToXmlStream(
        [NotNull] this Expression expression,
        [NotNull] Stream stream,
        XmlOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(stream);

        new ExpressionXmlTransform(options).Serialize(expression, stream);
    }

    /// <summary>
    /// Serializes the expression to XML and writes it to the specified <paramref name="stream"/>.
    /// </summary>
    /// <param name="expression">The expression to be serialized.</param>
    /// <param name="stream">The stream to which the XML will be written.</param>
    /// <param name="options">The options to control the transformation process.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous serialization operation.</returns>
    public static async Task ToXmlStreamAsync(
        [NotNull] this Expression expression,
        [NotNull] Stream stream,
        XmlOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(stream);

        await new ExpressionXmlTransform(options).SerializeAsync(expression, stream, cancellationToken);
    }

    /// <summary>
    /// Serializes the expression to XML and writes it to the specified <see cref="XmlWriter"/>.
    /// </summary>
    /// <param name="expression">The expression to be serialized.</param>
    /// <param name="writer">The <see cref="XmlWriter"/> to which the XML will be written.</param>
    /// <param name="options">The options to control the transformation process.</param>
    public static void ToXmlWriter(
        [NotNull] this Expression expression,
        [NotNull] XmlWriter writer,
        XmlOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(writer);

        var doc = new ExpressionXmlTransform(options).Transform(expression);

        doc.WriteTo(writer);
        writer.Flush();
    }

    /// <summary>
    /// Serializes the expression to XML and writes it to the specified <see cref="XmlWriter"/>.
    /// </summary>
    /// <param name="expression">The expression to be serialized.</param>
    /// <param name="writer">The <see cref="XmlWriter"/> to which the XML will be written.</param>
    /// <param name="options">The options to control the transformation process.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task ToXmlWriterAsync(
        [NotNull] this Expression expression,
        [NotNull] XmlWriter writer,
        XmlOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(writer);

        var doc = new ExpressionXmlTransform(options).Transform(expression);
        await doc.WriteToAsync(writer, cancellationToken);
        await writer.FlushAsync();
    }

    /// <summary>
    /// Serializes the expression to XML and writes it to the specified file.
    /// </summary>
    /// <param name="expression">The expression to be serialized.</param>
    /// <param name="filePath">The path of the file to which the XML will be written.</param>
    /// <param name="options">The options to control the transformation process.</param>
    public static void ToXmlFile(
        [NotNull] this Expression expression,
        [NotNull] string filePath,
        XmlOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        new ExpressionXmlTransform(options).Serialize(expression, stream);
    }

    /// <summary>
    /// Serializes the expression to XML and writes it to the specified file.
    /// </summary>
    /// <param name="expression">The expression to be serialized.</param>
    /// <param name="filePath">The path of the file to which the XML will be written.</param>
    /// <param name="options">The options to control the transformation process.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    public static async Task ToXmlFileAsync(
        [NotNull] this Expression expression,
        [NotNull] string filePath,
        XmlOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await new ExpressionXmlTransform(options).SerializeAsync(expression, stream, cancellationToken);
    }
}
