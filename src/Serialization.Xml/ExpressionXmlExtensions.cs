namespace vm2.Linq.Expressions.Serialization.Xml;

/// <summary>
/// Extension methods that provide a simplified API for serializing and deserializing LINQ expression trees to and from XML.
/// </summary>
[ExcludeFromCodeCoverage]
public static class ExpressionXmlExtensions
{
    // ── Expression → document ────────────────────────────────────

    /// <summary>
    /// Transforms the expression to an <see cref="XDocument"/>.
    /// </summary>
    public static XDocument ToXmlDocument(this Expression expression, XmlOptions? options = null)
        => new ExpressionXmlTransform(options).Transform(expression);

    /// <summary>
    /// Transforms the expression to an XML string.
    /// </summary>
    public static string ToXmlString(this Expression expression, XmlOptions? options = null)
    {
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
    public static void ToXmlStream(this Expression expression, Stream stream, XmlOptions? options = null)
        => new ExpressionXmlTransform(options).Serialize(expression, stream);

    /// <summary>
    /// Serializes the expression to XML and writes it to the specified <paramref name="stream"/>.
    /// </summary>
    public static Task ToXmlStreamAsync(this Expression expression, Stream stream, XmlOptions? options = null, CancellationToken cancellationToken = default)
        => new ExpressionXmlTransform(options).SerializeAsync(expression, stream, cancellationToken);

    /// <summary>
    /// Serializes the expression to XML and writes it to the specified <see cref="XmlWriter"/>.
    /// </summary>
    public static void ToXmlWriter(this Expression expression, XmlWriter writer, XmlOptions? options = null)
    {
        var doc = new ExpressionXmlTransform(options).Transform(expression);
        doc.WriteTo(writer);
        writer.Flush();
    }

    /// <summary>
    /// Serializes the expression to XML and writes it to the specified <see cref="XmlWriter"/>.
    /// </summary>
    public static async Task ToXmlWriterAsync(this Expression expression, XmlWriter writer, XmlOptions? options = null, CancellationToken cancellationToken = default)
    {
        var doc = new ExpressionXmlTransform(options).Transform(expression);
        await doc.WriteToAsync(writer, cancellationToken);
        await writer.FlushAsync();
    }

    /// <summary>
    /// Serializes the expression to XML and writes it to the specified file.
    /// </summary>
    public static void ToXmlFile(this Expression expression, string filePath, XmlOptions? options = null)
    {
        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        new ExpressionXmlTransform(options).Serialize(expression, stream);
    }

    /// <summary>
    /// Serializes the expression to XML and writes it to the specified file.
    /// </summary>
    public static async Task ToXmlFileAsync(this Expression expression, string filePath, XmlOptions? options = null, CancellationToken cancellationToken = default)
    {
        using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
        await new ExpressionXmlTransform(options).SerializeAsync(expression, stream, cancellationToken);
    }

    // ── Document → Expression ────────────────────────────────────

    /// <summary>
    /// Transforms the <see cref="XDocument"/> to a LINQ <see cref="Expression"/>.
    /// </summary>
    public static Expression ToExpression(this XDocument document, XmlOptions? options = null)
        => new ExpressionXmlTransform(options).Transform(document);

    /// <summary>
    /// Transforms the <see cref="XElement"/> to a LINQ <see cref="Expression"/>.
    /// </summary>
    public static Expression ToExpression(this XElement element, XmlOptions? options = null)
        => ((IExpressionTransform<XElement>)new ExpressionXmlTransform(options)).Transform(element);
}
