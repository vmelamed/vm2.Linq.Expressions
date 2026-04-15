namespace vm2.Linq.Expressions.Serialization.Xml.Tests;

public class XmlTestsFixture : IDisposable, IAsyncDisposable
{
    public string TestFilesPath { get; init; }

    public string TestLoadPath { get; init; }

    public string SchemasPath { get; init; }

    internal FileStreamOptions FileStreamOptions { get; } = new() {
        Mode = FileMode.Open,
        Access = FileAccess.Read,
        Share = FileShare.Read,
    };

    internal XmlOptions Options { get; }

    internal const LoadOptions XmlLoadOptions = LoadOptions.SetLineInfo;

    public XmlTestsFixture()
    {
        var repoRoot = FindRepoRoot();

        TestFilesPath = Path.Combine(repoRoot, "test", "Serialization.TestData", "TestData", "Xml");
        TestLoadPath  = Path.Combine(TestFilesPath, "LoadTestData");
        SchemasPath   = Path.Combine(repoRoot, "src", "Serialization.Xml", "Schema");

        XmlOptions.SetSchemasLocations(
            new Dictionary<string, string?> {
                [XmlOptions.Ser] = Path.Combine(SchemasPath, "Microsoft.Serialization.xsd"),
                [XmlOptions.Dcs] = Path.Combine(SchemasPath, "DataContract.xsd"),
                [XmlOptions.Exs] = Path.Combine(SchemasPath, "Linq.Expressions.Serialization.xsd"),
            }, true);

        Options = new() {
            ByteOrderMark = true,
            AddDocumentDeclaration = true,
            OmitDuplicateNamespaces = false,
            Indent = true,
            IndentSize = 4,
            AttributesOnNewLine = true,
            AddComments = true,
        };
    }

    static string FindRepoRoot()
    {
        var dir = AppContext.BaseDirectory;

        while (dir is not null)
        {
            if (File.Exists(Path.Combine(dir, "vm2.Linq.Expressions.slnx")))
                return dir;

            dir = Path.GetDirectoryName(dir);
        }

        throw new InvalidOperationException("Could not find the repository root (looked for vm2.Linq.Expressions.slnx).");
    }

    public void Validate(XDocument doc) => Options.Validate(doc);

    public async Task<(XDocument?, string)> GetXmlDocumentAsync(
        string testFileLine,
        string pathName,
        string expectedOrInput,
        ITestOutputHelper? output = null,
        bool throwIo = false,
        CancellationToken cancellationToken = default)
    {
        try
        {
            pathName = Path.GetFullPath(pathName);
            using var streamExpected = new FileStream(pathName, FileStreamOptions);
            var length = (int)streamExpected.Length;
            Memory<byte> buf = new byte[length];
            var read = await streamExpected.ReadAsync(buf, cancellationToken);
            read.Should().Be(length, "should be able to read the whole file");
            var expectedStr = Encoding.UTF8.GetString(buf.Span);

            output?.WriteLine($"{expectedOrInput}:\n{0}\n", expectedStr);

            streamExpected.Seek(0, SeekOrigin.Begin);

            var expectedDoc = await XDocument.LoadAsync(streamExpected, XmlLoadOptions, cancellationToken);
            var validate = () => Validate(expectedDoc);

            validate.Should().NotThrow($"the {expectedOrInput} document from {testFileLine} should be valid according to the schema");
            return (expectedDoc, expectedStr);
        }
        catch (IOException x)
        {
            if (throwIo)
                throw;
            output?.WriteLine($"Error getting the {expectedOrInput} document from `{pathName}`:\n{x}\nProceeding with creating the file from the actual document...");
        }
#pragma warning disable CA1031 // Do not catch general exception types -- test helper: any failure should be reported via Assert.Fail
        catch (Exception x)
        {
            Assert.Fail($"Error getting the {expectedOrInput} document from `{pathName}`:\n{x}");
        }
#pragma warning restore CA1031
        return (null, "");
    }

    public void TestExpressionToXml(
        string testFileLine,
        Expression expression,
        XDocument? expectedDoc,
        string expectedStr,
        string? fileName,
        ITestOutputHelper? output = null)
    {
        var transform = new ExpressionXmlTransform(Options);
        var actualDoc = transform.Transform(expression);
        using var streamActual = new MemoryStream();
        transform.Serialize(expression, streamActual);
        var actualStr = Encoding.UTF8.GetString(streamActual.ToArray());

        AssertXmlAsExpectedOrSave(testFileLine, expectedDoc, expectedStr, actualDoc, actualStr, fileName, false, output);
    }

    public async Task TestExpressionToXmlAsync(
        string testFileLine,
        Expression expression,
        XDocument? expectedDoc,
        string expectedStr,
        string? fileName,
        ITestOutputHelper? output = null,
        CancellationToken cancellationToken = default)
    {
        var transform = new ExpressionXmlTransform(Options);
        var actualDoc = transform.Transform(expression);
        using var streamActual = new MemoryStream();
        await transform.SerializeAsync(expression, streamActual, cancellationToken);
        var actualStr = Encoding.UTF8.GetString(streamActual.ToArray());

        AssertXmlAsExpectedOrSave(testFileLine, expectedDoc, expectedStr, actualDoc, actualStr, fileName, true, output);
    }

    public void TestXmlToExpression(
        string testFileLine,
        XDocument inputDoc,
        Expression expectedExpression)
    {
        var transform = new ExpressionXmlTransform(Options);
        var actualExpression = transform.Transform(inputDoc);

        expectedExpression.DeepEquals(actualExpression, out var difference).Should().BeTrue($"the expression at {testFileLine} should be \"DeepEqual\" to `{expectedExpression}`\n({difference})");
    }

    void AssertXmlAsExpectedOrSave(
        string testFileLine,
        XDocument? expectedDoc,
        string expectedStr,
        XDocument actualDoc,
        string actualStr,
        string? fileName,
        bool async,
        ITestOutputHelper? output = null)
    {
        output?.WriteLine($"ACTUAL {(async ? "async" : "sync")}:\n{actualStr}\n");

        var validate = () => Validate(actualDoc);

        validate.Should().NotThrow($"the ACTUAL document from {testFileLine} should be valid according to the schema `{XmlOptions.Exs}`.");

        if (expectedDoc is null)
        {
            fileName = string.IsNullOrEmpty(fileName)
                            ? Path.GetFullPath(Path.Combine(TestFilesPath, DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss.fff") + ".xml"))
                            : Path.GetFullPath(fileName);

            var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            using var writer = new StreamWriter(stream, Options.Encoding);
            using var xmlWriter = XmlWriter.Create(writer, Options.XmlWriterSettings);

            actualDoc.WriteTo(xmlWriter);

            Assert.Fail($"The expected XML does not appear to exist. Saved the actual XML in the file `{fileName}`.");
        }

        actualStr.Should().Be(expectedStr, "the expected and the actual XML texts should be the same");

        var ignoreComments = false;
        var comparer = new XNodeDeepEquals(ignoreComments);
        var myEquals = comparer.AreEqual(actualDoc, expectedDoc);

        if (!myEquals || ignoreComments && comparer.LastResult != "")
            output?.WriteLine(comparer.LastResult);

        var deepEquals = XNode.DeepEquals(actualDoc, expectedDoc);

        if (!deepEquals)
            output?.WriteLine("XNode.DeepEquals returned false!");

        (myEquals || deepEquals).Should().BeTrue($"the expected and the actual XDocument objects from {testFileLine} should be deep-equal");
    }

    public virtual void Dispose() => GC.SuppressFinalize(this);

    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
