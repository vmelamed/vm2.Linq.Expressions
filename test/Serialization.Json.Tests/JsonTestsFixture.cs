namespace vm2.Linq.Expressions.Serialization.Json.Tests;

public class JsonTestsFixture : IDisposable, IAsyncDisposable
{
    public string TestFilesPath { get; init; }

    public string TestLoadPath { get; init; }

    public string SchemasPath { get; init; }

    public FileStreamOptions FileStreamOptions { get; } = new() {
        Mode = FileMode.Open,
        Access = FileAccess.Read,
        Share = FileShare.Read,
    };

    public JsonOptions Options { get; set; }

    public JsonTestsFixture()
    {
        var repoRoot = FindRepoRoot();

        TestFilesPath = Path.Combine(repoRoot, "test", "Serialization.TestData", "TestData", "Json");
        TestLoadPath  = Path.Combine(TestFilesPath, "LoadTestData");
        SchemasPath   = Path.Combine(repoRoot, "src", "Serialization.Json", "Schema");

        Options = new(Path.Combine(SchemasPath, "Linq.Expressions.Serialization.json")) {
            Indent = true,
            IndentSize = 4,
            AddComments = true,
            AllowTrailingCommas = true,
            ValidateInputDocuments = ValidateExpressionDocuments.Always,
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

    public void Validate(JsonNode doc) => Options.Validate(doc);

    public void Validate(string json) => Options.Validate(json);

    /// <summary>
    /// Get json document as an asynchronous operation.
    /// </summary>
    public async Task<(JsonNode?, string)> GetJsonDocumentAsync(
        string testFileLine,
        string pathName,
        string expectedOrInput,
        ITestOutputHelper? output = null,
        bool validate = false,
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

            output?.WriteLine($"{expectedOrInput}:\n{expectedStr}\n");

            streamExpected.Seek(0, SeekOrigin.Begin);

            var parse = async () => await JsonNode.ParseAsync(streamExpected, JsonOptions.JsonNodeOptions, JsonOptions.JsonDocumentOptions, cancellationToken);
            var expectedDoc = (await parse.Should().NotThrowAsync()).Which;

            expectedDoc.Should().NotBeNull();
            Debug.Assert(expectedDoc is not null);

            if (validate)
            {
                var isValid = () => Validate(expectedStr);

                isValid.Should().NotThrow($"the {expectedOrInput} document from {testFileLine} should be valid according to the schema");
            }

            return (expectedDoc, expectedStr);
        }
        catch (IOException x)
        {
            if (throwIo)
                throw;
            output?.WriteLine($"WARNING: error getting the {expectedOrInput} document from `{pathName}`:\n{x}\nProceeding with creating the file from the actual document...");
        }
#pragma warning disable CA1031 // Do not catch general exception types -- test helper: any failure should be reported via Assert.Fail
        catch (Exception x)
        {
            output?.WriteLine($"ERROR: Error getting the {expectedOrInput} document from `{pathName}`:\n{x}");
        }
#pragma warning restore CA1031
        return (null, "");
    }

    /// <summary>
    /// Tests the expression to json.
    /// </summary>
    public void TestExpressionToJson(
        string testFileLine,
        Expression expression,
        JsonNode? expectedDoc,
        string expectedStr,
        string? fileName,
        ITestOutputHelper? output = null,
        bool validate = true)
    {
        var transform = new ExpressionJsonTransform(Options);

        var actualDoc = transform.Transform(expression);
        using var streamActual = new MemoryStream();
        transform.Serialize(expression, streamActual);
        var actualStr = Encoding.UTF8.GetString(streamActual.ToArray());

        AssertJsonAsExpectedOrSave(testFileLine, expectedDoc, expectedStr, actualDoc, actualStr, fileName, false, output, validate);
    }

    /// <summary>
    /// Test expression to json as an asynchronous operation.
    /// </summary>
    public async Task TestExpressionToJsonAsync(
        string testFileLine,
        Expression expression,
        JsonNode? expectedDoc,
        string expectedStr,
        string? fileName,
        ITestOutputHelper? output = null,
        bool validate = false,
        CancellationToken cancellationToken = default)
    {
        var transform = new ExpressionJsonTransform(Options);

        var actualDoc = transform.Transform(expression);
        using var streamActual = new MemoryStream();
        await transform.SerializeAsync(expression, streamActual, cancellationToken);
        var actualStr = Encoding.UTF8.GetString(streamActual.ToArray());

        AssertJsonAsExpectedOrSave(testFileLine, expectedDoc, expectedStr, actualDoc, actualStr, fileName, true, output, validate);
    }

    void AssertJsonAsExpectedOrSave(
        string testFileLine,
        JsonNode? expectedDoc,
        string expectedStr,
        JsonObject actualDoc,
        string actualStr,
        string? fileName,
        bool async,
        ITestOutputHelper? output,
        bool validate)
    {
        output?.WriteLine($"ACTUAL ({(async ? "async" : "sync")}):\n{actualStr}\n");

        if (validate)
        {
            var isValid = () => Validate(actualStr);

            isValid.Should().NotThrow($"the ACTUAL document from {testFileLine} should be valid according to the schema `{JsonOptions.Exs}`.");
        }

        if (expectedDoc is null)
        {
            fileName = string.IsNullOrEmpty(fileName)
                            ? Path.GetFullPath(Path.Combine(TestFilesPath, DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss.fff") + ".json"))
                            : Path.GetFullPath(fileName);

            var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
            using var writer = new Utf8JsonWriter(stream, Options.JsonWriterOptions);

            actualDoc.WriteTo(writer);

            Assert.Fail($"The expected JSON does not appear to exist. Saved the actual JSON in the file `{fileName}`.");
        }

        expectedDoc.GetValueKind().Should().Be(JsonValueKind.Object, "The expected JSON document (JsonNode?) is not JsonObject.");

        actualStr.Should().Be(expectedStr, "the expected and the actual JSON texts should be the same");

        JsonNode
            .DeepEquals(actualDoc, expectedDoc)
            .Should()
            .BeTrue($"the expected and the actual top-level JsonObject objects (documents) from {testFileLine} should be deep-equal.");
    }

    /// <summary>
    /// Tests the json to expression transformation.
    /// </summary>
    public void TestJsonToExpression(
        string testFileLine,
        JsonNode? inputDoc,
        Expression expectedExpression)
    {
        inputDoc.Should().NotBeNull("The JSON document (JsonNode?) to transform is null");
        Debug.Assert(inputDoc is not null);
        inputDoc.GetValueKind().Should().Be(JsonValueKind.Object, "The input JSON document (JsonNode?) is not JsonObject.");

        var transform = new ExpressionJsonTransform(Options);
        var actualExpression = transform.Transform(inputDoc.AsObject());

        expectedExpression
            .DeepEquals(actualExpression, out var difference)
            .Should()
            .BeTrue($"the expression at {testFileLine} should be \"DeepEqual\" to `{expectedExpression}`\n({difference})");
    }

    public virtual void Dispose() => GC.SuppressFinalize(this);

    public virtual ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}
