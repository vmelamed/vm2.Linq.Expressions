namespace vm2.Linq.Expressions.Serialization.Json.Tests;

[CollectionDefinition("JSON")]
public partial class UnaryTests(JsonTestsFixture fixture, ITestOutputHelper output) : BaseTests(fixture, output)
{
    protected override string JsonTestFilesPath => Path.Combine(_fixture.TestFilesPath, "Unary");

    [Theory]
    [MemberData(nameof(UnaryTestData.Data), MemberType = typeof(UnaryTestData))]
    public async Task UnaryToJsonTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.ToJsonTestAsync(testFileLine, expressionString, fileName);

    [Theory]
    [MemberData(nameof(UnaryTestData.Data), MemberType = typeof(UnaryTestData))]
    public async Task UnaryFromJsonTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.FromJsonTestAsync(testFileLine, expressionString, fileName);

    protected override Expression Substitute(string id) => UnaryTestData.GetExpression(id);
}
