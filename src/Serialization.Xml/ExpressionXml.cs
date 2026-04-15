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
    public static Expression FromStream(Stream stream, XmlOptions? options = null)
        => new ExpressionXmlTransform(options).Deserialize(stream);

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the XML content in the specified <paramref name="stream"/>.
    /// </summary>
    public static Task<Expression> FromStreamAsync(Stream stream, XmlOptions? options = null, CancellationToken cancellationToken = default)
        => new ExpressionXmlTransform(options).DeserializeAsync(stream, cancellationToken);

    // ── From reader ──────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the specified <see cref="XmlReader"/>.
    /// </summary>
    public static Expression FromReader(XmlReader reader, XmlOptions? options = null)
    {
        var document = XDocument.Load(reader, LoadOptions.None);
        return new ExpressionXmlTransform(options).Transform(document);
    }

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the specified <see cref="XmlReader"/>.
    /// </summary>
    public static async Task<Expression> FromReaderAsync(XmlReader reader, XmlOptions? options = null, CancellationToken cancellationToken = default)
    {
        var document = await XDocument.LoadAsync(reader, LoadOptions.None, cancellationToken);
        return new ExpressionXmlTransform(options).Transform(document);
    }

    // ── From file ────────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the XML file at the specified <paramref name="filePath"/>.
    /// </summary>
    public static Expression FromFile(string filePath, XmlOptions? options = null)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        return new ExpressionXmlTransform(options).Deserialize(stream);
    }

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the XML file at the specified <paramref name="filePath"/>.
    /// </summary>
    public static async Task<Expression> FromFileAsync(string filePath, XmlOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
        return await new ExpressionXmlTransform(options).DeserializeAsync(stream, cancellationToken);
    }

    // ── From string ──────────────────────────────────────────────

    /// <summary>
    /// Deserializes a LINQ <see cref="Expression"/> from the specified XML <paramref name="xml"/> string.
    /// </summary>
    public static Expression FromString(string xml, XmlOptions? options = null)
    {
        var document = XDocument.Parse(xml);
        return new ExpressionXmlTransform(options).Transform(document);
    }
}
