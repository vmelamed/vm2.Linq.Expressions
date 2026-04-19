namespace vm2.Linq.Expressions.Serialization.Json.Tests;

using vm2.TestUtilities;


[CollectionDefinition("JSON")]
public abstract class BaseTests(
        JsonTestsFixture fixture,
        ITestOutputHelper output) : TestBase(output), IClassFixture<JsonTestsFixture>
{
    protected JsonTestsFixture _fixture = fixture;

    protected abstract string JsonTestFilesPath { get; }

    protected bool JsonTestFilesPathExists { get; set; }

    public virtual async Task ToJsonTestAsync(string testFileLine, string expressionString, string fileName)
    {
        if (!JsonTestFilesPathExists)
        {
            if (!Directory.Exists(JsonTestFilesPath))
                Directory.CreateDirectory(JsonTestFilesPath);

            JsonTestFilesPathExists = true;
        }

        var expression = Substitute(expressionString);
        var pathName = Path.Combine(JsonTestFilesPath, fileName+".json");
        var (expectedDoc, expectedStr) = await _fixture.GetJsonDocumentAsync(testFileLine, pathName, "EXPECTED", Out,
                                                            cancellationToken: TestContext.Current.CancellationToken);

        _fixture.TestExpressionToJson(testFileLine, expression, expectedDoc, expectedStr, pathName, Out);
        await _fixture.TestExpressionToJsonAsync(testFileLine, expression, expectedDoc, expectedStr, pathName, Out,
                                                            cancellationToken: TestContext.Current.CancellationToken);
    }

    public virtual async Task FromJsonTestAsync(string testFileLine, string expressionString, string fileName)
    {
        var expectedExpression = Substitute(expressionString);
        var pathName = Path.Combine(JsonTestFilesPath, fileName+".json");
        var (inputDoc, _) = await _fixture.GetJsonDocumentAsync(testFileLine, pathName, "INPUT", Out,
                                                            cancellationToken: TestContext.Current.CancellationToken);

        _fixture.TestJsonToExpression(testFileLine, inputDoc, expectedExpression);
    }

    protected virtual Expression Substitute(string id) => Expression.Constant(null);
}
