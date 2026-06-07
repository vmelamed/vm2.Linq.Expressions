// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Linq.Expressions.Serialization.Xml;

/// <summary>
/// Static methods for deserializing LINQ expression trees from XML sources (streams, files, strings, readers).
/// </summary>
public static class ExpressionXml
{
    // ── From stream ──────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the XML content in the specified <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream containing the XML content to be deserialized.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <returns>The deserialized expression</returns>
    public static Expression FromStream(
        [NotNull] Stream stream,
        XmlOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return new ExpressionXmlTransform(options).Deserialize(stream);
    }

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the XML content in the specified <paramref name="stream"/>.
    /// </summary>
    /// <param name="stream">The stream containing the XML content to be deserialized.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized expression</returns>
    public static Task<Expression> FromStreamAsync(
        [NotNull] Stream stream,
        XmlOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        return new ExpressionXmlTransform(options).DeserializeAsync(stream, cancellationToken);
    }

    // ── From reader ──────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the specified <see cref="XmlReader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="XmlReader"/> to read the XML content from.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <returns>The deserialized expression</returns>
    public static Expression FromReader(
        [NotNull] XmlReader reader,
        XmlOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var document = XDocument.Load(reader, LoadOptions.None);

        return new ExpressionXmlTransform(options).Transform(document);
    }

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the specified <see cref="XmlReader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="XmlReader"/> to read the XML content from.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized expression</returns>
    public static async Task<Expression> FromReaderAsync(
        [NotNull] XmlReader reader,
        XmlOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var document = await XDocument.LoadAsync(reader, LoadOptions.None, cancellationToken);

        return new ExpressionXmlTransform(options).Transform(document);
    }

    // ── From file ────────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the XML file at the specified <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">The path to the XML file to be deserialized.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <returns>The deserialized expression</returns>
    public static Expression FromFile(
        [NotNull] string filePath,
        XmlOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        return new ExpressionXmlTransform(options).Deserialize(stream);
    }

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the XML file at the specified <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">The path to the XML file to be deserialized.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The deserialized expression</returns>
    public static async Task<Expression> FromFileAsync(
        [NotNull] string filePath,
        XmlOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);

        return await new ExpressionXmlTransform(options).DeserializeAsync(stream, cancellationToken);
    }

    // ── From XML object ────────────────────────────────────

    /// <summary>
    /// Transforms the <see cref="XDocument"/> to a LINQ <see cref="Expression"/>.
    /// </summary>
    /// <param name="document">The <see cref="XDocument"/> to be transformed.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <returns>The deserialized expression</returns>
    public static Expression FromXDocument(
        [NotNull] XDocument document,
        XmlOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(document);

        return new ExpressionXmlTransform(options).Transform(document);
    }

    /// <summary>
    /// Transforms the <see cref="XElement"/> to a LINQ <see cref="Expression"/>.
    /// </summary>
    /// <param name="element">The <see cref="XElement"/> to be transformed.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <returns>The deserialized expression</returns>
    public static Expression FromXElement(
        [NotNull] XElement element,
        XmlOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(element);

        return ((IExpressionTransform<XElement>)new ExpressionXmlTransform(options)).Transform(element);
    }

    // ── From string ──────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the specified XML <paramref name="xml"/> string.
    /// </summary>
    /// <param name="xml">The XML string to be deserialized.</param>
    /// <param name="options">The options to control the deserialization process.</param>
    /// <returns>The deserialized expression</returns>
    public static Expression FromString(
        [NotNull] string xml,
        XmlOptions? options = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xml);

        var document = XDocument.Parse(xml);
        return new ExpressionXmlTransform(options).Transform(document);
    }
}
