namespace vm2.Linq.Expressions.Serialization.Xml;

/// <summary>
/// Class ExpressionTransform.
/// Implements the <see cref="IExpressionTransform{XNode}"/>: transforms a Linq expression to an XML Node object.
/// </summary>
/// <seealso cref="IExpressionTransform{XNode}" />
public class ExpressionXmlTransform(XmlOptions? options = null) : IExpressionTransform<XDocument>, IExpressionTransform<XElement>
{
    XmlOptions _options = options ?? new();
    ToXmlTransformVisitor? _expressionVisitor;
    FromXmlTransformVisitor? _xmlVisitor;

    #region IExpressionTransform<XElement>
    /// <summary>
    /// Transformer the specified expression to a document model node type `XNode` (XML).
    /// </summary>
    /// <param name="expression">The expression to be transformed.</param>
    /// <returns>The resultant top level document model node `XNode`.</returns>
    XElement IExpressionTransform<XElement>.Transform(Expression expression)
    {
        _expressionVisitor ??= new ToXmlTransformVisitor(_options);
        _expressionVisitor.Visit(expression);

        return new XElement(
                        ElementNames.Expression,
                        new XAttribute("xmlns", Namespaces.Exs),
                        new XAttribute(XNamespace.Xmlns + "i", Namespaces.Xsi),
                        _expressionVisitor.Result);
    }

    /// <summary>
    /// Transformer the specified document model node of type `TDocument` to a LINQ expression.
    /// </summary>
    /// <param name="element">The document node to be transformed.</param>
    /// <returns>The resultant expression.</returns>
    Expression IExpressionTransform<XElement>.Transform(XElement element)
    {
        _options.Validate(element);
        return DoTransform(element);
    }
    #endregion

    #region IExpressionTransform<XDocument>
    /// <summary>
    /// Transformer the specified expression to a document model node type `XNode` (XML).
    /// </summary>
    /// <param name="expression">The expression to be transformed.</param>
    /// <returns>The resultant top level document model node `XNode`.</returns>
    public XDocument Transform(Expression expression)
        => new(
            _options.DocumentDeclaration(),
            _options.Comment(expression),
            ((IExpressionTransform<XElement>)this).Transform(expression));

    /// <summary>
    /// Transformer the specified document model node of type `TDocument` to a LINQ expression.
    /// </summary>
    /// <param name="document">The document node to be transformed.</param>
    /// <returns>The resultant expression.</returns>
    public Expression Transform(XDocument document)
    {
        _options.Validate(document);
        var me = ((IExpressionTransform<XElement>)this);
        var root = document.Root ?? new XElement(ElementNames.Expression, ElementNames.Object, new XAttribute(AttributeNames.Nil, true));

        if (root.Name.LocalName != Vocabulary.Expression)
            throw new SerializationException($"Expected document root element with name `{Vocabulary.Expression}`.");

        return DoTransform(root);
    }
    #endregion

    Expression DoTransform(XElement element)
    {
        if (_xmlVisitor is null)
            _xmlVisitor = new FromXmlTransformVisitor();
        else
            _xmlVisitor.ResetVisitState();
        return _xmlVisitor.Visit(element);
    }

    /// <summary>
    /// Serializes the specified expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="stream">The stream to put the XML document to.</param>
    /// <returns>Stream.</returns>
    public void Serialize(
        Expression expression,
        Stream stream)
    {
        var doc = Transform(expression);
        using var writer = new StreamWriter(stream, _options.Encoding);
        using var xmlWriter = XmlWriter.Create(writer, new() {
            Encoding = _options.Encoding,
            Indent = _options.Indent,
            IndentChars = new(' ', _options.IndentSize),
            NamespaceHandling = _options.OmitDuplicateNamespaces ? NamespaceHandling.OmitDuplicates : NamespaceHandling.Default,
            NewLineOnAttributes = _options.AttributesOnNewLine,
            OmitXmlDeclaration = !_options.AddDocumentDeclaration,
            WriteEndDocumentOnClose = true,
        });

        doc.WriteTo(xmlWriter);
        xmlWriter.Flush();
        writer.Flush();
        stream.Flush();
    }

    /// <summary>
    /// Serialize as an asynchronous operation.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="stream">The stream.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>A Task&lt;Stream&gt; representing the asynchronous operation.</returns>
    public async Task<Stream> SerializeAsync(
        Expression expression,
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        var doc = Transform(expression);
        var encoding = _options.Encoding;
        var settings = new XmlWriterSettings() {
            Async = true,
            Encoding = encoding,
            Indent = _options.Indent,
            IndentChars = new(' ', _options.IndentSize),
            NamespaceHandling = _options.OmitDuplicateNamespaces ? NamespaceHandling.OmitDuplicates : NamespaceHandling.Default,
            NewLineOnAttributes = _options.AttributesOnNewLine,
            OmitXmlDeclaration = !_options.AddDocumentDeclaration,
            WriteEndDocumentOnClose = true,
        };
        using var writer = new StreamWriter(stream, encoding);
        using var xmlWriter = XmlWriter.Create(writer, settings);

        await doc.WriteToAsync(xmlWriter, cancellationToken);
        await xmlWriter.FlushAsync();
        await writer.FlushAsync(cancellationToken);
        await stream.FlushAsync(cancellationToken);

        return stream;
    }

    /// <summary>
    /// Serializes the specified expression.
    /// </summary>
    /// <param name="stream">The stream to get the XML document from.</param>
    /// <returns>Stream.</returns>
    public Expression Deserialize(
        Stream stream)
    {
        using var reader = new StreamReader(stream, _options.Encoding);
        var readerSettings = new XmlReaderSettings()
        {
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
            ValidationFlags = XmlSchemaValidationFlags.None,
        };

        using var xmlReader = XmlReader.Create(reader, readerSettings);
        var document = XDocument.Load(
                            xmlReader,
                            _options.MustValidate
                                ? LoadOptions.SetLineInfo
                                : LoadOptions.None);

        return Transform(document);
    }

    /// <summary>
    /// Deserializes an expression from the specified document.
    /// </summary>
    /// <param name="stream">The stream to get the XML document from.</param>
    /// <param name="cancellationToken">The cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
    /// <returns>Stream.</returns>
    public async Task<Expression> DeserializeAsync(
        Stream stream,
        CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream, _options.Encoding);
        var readerSettings = new XmlReaderSettings() {
            Async = true,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
            IgnoreWhitespace = true,
        };

        using var xmlReader = XmlReader.Create(reader, readerSettings);
        var document = await XDocument.LoadAsync(
                                xmlReader,
                                _options.MustValidate
                                    ? LoadOptions.SetLineInfo
                                    : LoadOptions.None, cancellationToken);

        return Transform(document);
    }
}
