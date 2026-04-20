namespace vm2.Linq.Expressions.Serialization.Xml;

/// <summary>
/// Class XmlOptions holds options that control certain aspects of the transformations to/from LINQ expressions from/to
/// XML documents.
/// </summary>
public partial class XmlOptions : DocumentOptions
{
    /// <summary>
    /// The expression transformation XML schemaUri
    /// </summary>
    public const string Exs = "urn:schemas-vm-com:Linq.Expressions.Serialization.Xml";

    /// <summary>
    /// The W3C schemaUri definition.
    /// </summary>
    public const string Xsd = "http://www.w3.org/2001/XMLSchema";

    /// <summary>
    /// The W3C instance schemaUri definition.
    /// </summary>
    public const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";

    /// <summary>
    /// The Microsoft serialization schemaUri definition.
    /// </summary>
    public const string Ser = "http://schemas.microsoft.com/2003/10/Serialization/";

    /// <summary>
    /// The SOAP data contracts.
    /// </summary>
    public const string Dcs = "http://schemas.datacontract.org/2004/07/System";

    /// <summary>
    /// The schemas lock synchronizes the <see cref="Schemas"/> collection.
    /// </summary>
    static readonly ReaderWriterLockSlim _schemasLock = new(LockRecursionPolicy.SupportsRecursion);

    /// <summary>
    /// Gets the schemas.
    /// </summary>
    /// <value>The schemas.</value>
    static XmlSchemaSet Schemas { get; set; } = new();

    /// <summary>
    /// Resets the schemas.
    /// </summary>
    public static void ResetSchemas()
    {
        using var _ = _schemasLock.WriterLock();

        Schemas = new();
    }

    /// <summary>
    /// Sets the schemaUri path.
    /// </summary>
    /// <param name="schemaUri">The schema identifier (most likely <see cref="Exs"/> which is not URL).</param>
    /// <param name="url">The location of the schema file.</param>
    [ExcludeFromCodeCoverage]
    public static void SetSchemaLocation(string schemaUri, string? url)
    {
        using var _ = _schemasLock.WriterLock();

        if (Schemas.Contains(schemaUri))
            return;

        using var reader = XmlReader.Create(url ?? schemaUri, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
        Schemas.Add(schemaUri, reader);
    }

    /// <summary>
    /// Sets the schemaUri path.
    /// </summary>
    /// <param name="schemaUrisUrls">The schema URIs and their URLs.</param>
    /// <param name="reset">if set to <c>true</c> the method will first reset the schema collection.</param>
    public static void SetSchemasLocations(IEnumerable<KeyValuePair<string, string?>> schemaUrisUrls, bool reset = false)
    {
        using var _ = _schemasLock.WriterLock();

        if (reset)
            Schemas = new();

        foreach (var (schemaUri, url) in schemaUrisUrls)
        {
            if (Schemas.Contains(schemaUri))
                continue;

            using var reader = XmlReader.Create(url ?? schemaUri, new XmlReaderSettings() { DtdProcessing = DtdProcessing.Prohibit, XmlResolver = null });
            Schemas.Add(schemaUri, reader);
        }
    }

    /// <summary>
    /// Gets or sets the transformed document encoding.
    /// </summary>
    /// <value>The encoding.</value>
    public string CharacterEncoding
    {
        get;
        set
        {
            var encoding = value.ToUpperInvariant() switch {
                "ASCII" => "ascii",
                "UTF-8" => "utf-8",
                "UTF-16" => "utf-16",
                "UTF-32" => "utf-32",
                "ISO-8859-1" or "LATIN1" => "iso-8859-1",
                _ => throw new NotSupportedException($@"The encoding ""{value}"" is not supported. " +
                                                      @"The supported character encodings are: ""ascii"", ""utf-8"", ""utf-16"", ""utf-32"", and ""iso-8859-1"" (or ""Latin1"")."),
            };

            field = Change(field, encoding);
        }
    } = "utf-8";

    /// <summary>
    /// Gets or sets a value indicating whether to put in the output stream a byte order mark (BOM). Not recommended for UTF-8.
    /// </summary>
    public bool ByteOrderMark { get; set => field = Change(field, value); }

    /// <summary>
    /// Gets or sets a value indicating the endian-ness of the transformed document.
    /// </summary>
    public bool BigEndian { get; set => field = Change(field, value); }

    /// <summary>
    /// Gets or sets a value indicating whether to add an XML document declaration.
    /// </summary>
    public bool AddDocumentDeclaration { get; set => field = Change(field, value); } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to eliminate duplicate namespaces.
    /// </summary>
    public bool OmitDuplicateNamespaces { get; set => field = Change(field, value); } = true;

    /// <summary>
    /// Outputs all XML attribute on a new line - below their XML element and indented.
    /// </summary>
    public bool AttributesOnNewLine { get; set => field = Change(field, value); }

    /// <summary>
    /// Determines whether the expressions schemaUri <see cref="Exs"/> was added.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public override bool HasExpressionSchema
    {
        get
        {
            using var _ = _schemasLock.ReaderLock();

            return Schemas.Contains(Exs);
        }
    }

    /// <summary>
    /// Validates the specified document against the schema.
    /// </summary>
    /// <param name="document">The document.</param>
    public void Validate(XDocument document)
    {
        if (!MustValidate)
            return;

        List<XmlSchemaException> exceptions = [];

        using var _ = _schemasLock.ReaderLock();

        document.Validate(Schemas, (_, e) => exceptions.Add(e.Exception));

        if (exceptions.Count is not 0)
            throw new SchemaValidationErrorsException(
                        $"The validation of the XML document against the schema \"{Exs}\" failed:\n  " +
                        string.Join("\n  ", exceptions.Select(x => $"({x.LineNumber}, {x.LinePosition}) : {x.Message}")));
    }

    /// <summary>
    /// Validates the specified element against the schema.
    /// </summary>
    /// <param name="element">The element.</param>
    [ExcludeFromCodeCoverage]
    public void Validate(XElement element)
    {
        if (!MustValidate)
            return;

        var exceptions = new List<XmlSchemaException>();
        XmlSchemaObject? schema = null;

        using var _ = _schemasLock.ReaderLock();

        schema = Schemas.GlobalElements[new XmlQualifiedName(Vocabulary.Expression, Exs)];

        if (schema is null)
            throw new SerializationException($"Could not find schema for element {new XmlQualifiedName(Vocabulary.Expression, Exs)}.");

        element.Validate(schema, Schemas, (_, e) => exceptions.Add(e.Exception));

        if (exceptions.Count is not 0)
            throw new SchemaValidationErrorsException(
                        $"The validation of the XML element against the schema \"{Exs}\" failed:\n  " +
                        string.Join("\n  ", exceptions.Select(x => $"({x.LineNumber}, {x.LinePosition}) : {x.Message}")));
    }

    /// <summary>
    /// Gets the set  XML document encoding.
    /// </summary>
    /// <returns>The document encoding.</returns>
    public Encoding Encoding
        => CharacterEncoding switch {
            "ascii" => Encoding.ASCII,
            "utf-8" => new UTF8Encoding(ByteOrderMark, true),
            "utf-16" => new UnicodeEncoding(BigEndian, ByteOrderMark, true),
            "utf-32" => new UTF32Encoding(BigEndian, ByteOrderMark, true),
            "iso-8859-1" => Encoding.Latin1,
            _ => throw new NotSupportedException($@"The encoding ""{CharacterEncoding}"" is not supported. " +
                                    @"The supported character encodings are: ""ascii"", ""utf-8"", ""utf-16"", ""utf-32"", and ""iso-8859-1"" (or ""Latin1"")."),
        };

    /// <summary>
    /// Gets the document declaration from the current options if declarations are enabled.
    /// </summary>
    /// <returns>The document declaration System.Nullable&lt;XDeclaration&gt;.</returns>
    internal XDeclaration? DocumentDeclaration()
        => AddDocumentDeclaration ? new XDeclaration("1.0", CharacterEncoding, null) : null;

    /// <summary>
    /// Builds an XML comment object with the specified comment text if comments are enabled.
    /// </summary>
    /// <param name="comment">The comment.</param>
    /// <returns>The comment object System.Nullable&lt;XComment&gt;.</returns>
    internal XComment? Comment(string comment)
        => AddComments ? new XComment($" {comment} ") : null;

    /// <summary>
    /// Builds an XML comment object with the text of the expression if comments are enabled.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>System.Nullable&lt;XComment&gt;.</returns>
    internal XComment? Comment(Expression expression)
        => AddComments ? new XComment($" {expression} ") : null;

    /// <summary>
    /// Adds the comment to the specified XML container if comments are enabled.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="comment">The comment.</param>
    [ExcludeFromCodeCoverage]
    internal void AddComment(XContainer parent, string comment)
    {
        if (AddComments)
            parent.Add(new XComment($" {comment} "));
    }

    /// <summary>
    /// Adds the expression comment to the specified XML container if comments are enabled.
    /// </summary>
    /// <param name="parent">The parent.</param>
    /// <param name="expression">The expression.</param>
    [ExcludeFromCodeCoverage]
    internal void AddComment(XContainer parent, Expression expression)
    {
        if (AddComments)
            parent.Add(new XComment($" {expression} "));
    }

    /// <summary>
    /// Creates an <see cref="XComment" /> with the human readable name of the <paramref name="type" /> if comments are enabled.
    /// The type must be non-basic type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The comment as System.Nullable&lt;XComment&gt;.</returns>
    public XComment? TypeComment(Type type)
        => AddComments &&
           TypeNames != TypeNameConventions.AssemblyQualifiedName &&
           (!type.IsBasicType() && type != typeof(object) || type.IsEnum)
                ? Comment($" {Transform.TypeName(type, TypeNames)} ")
                : null;

    /// <summary>
    /// Gets the XML writer settings.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public XmlWriterSettings XmlWriterSettings
        => field is not null && !Changed
                ? field
                : field = new() {
                    Encoding = Encoding,
                    Indent = Indent,
                    IndentChars = new(' ', IndentSize),
                    NamespaceHandling = OmitDuplicateNamespaces ? NamespaceHandling.OmitDuplicates : NamespaceHandling.Default,
                    NewLineOnAttributes = AttributesOnNewLine,
                    OmitXmlDeclaration = !AddDocumentDeclaration,
                    WriteEndDocumentOnClose = true,
                };

}
