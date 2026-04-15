namespace vm2.Linq.Expressions.Serialization.Json.Tests;

[CollectionDefinition("JSON")]
public partial class StatementTests(JsonTestsFixture fixture, ITestOutputHelper output) : BaseTests(fixture, output)
{
    protected override string JsonTestFilesPath => Path.Combine(_fixture.TestFilesPath, "Statements");

    [Theory]
    [MemberData(nameof(StatementTestData.Data), MemberType = typeof(StatementTestData))]
#if !JSON_SCHEMA
    [MemberData(nameof(StatementTestDataNs.Data), MemberType = typeof(StatementTestDataNs))]
#endif
    public async Task StatementToJsonTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.ToJsonTestAsync(testFileLine, expressionString, fileName);

    [Theory]
    [MemberData(nameof(StatementTestData.Data), MemberType = typeof(StatementTestData))]
#if !JSON_SCHEMA
    [MemberData(nameof(StatementTestDataNs.Data), MemberType = typeof(StatementTestDataNs))]
#endif
    public async Task StatementFromJsonTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.FromJsonTestAsync(testFileLine, expressionString, fileName);

    protected override Expression Substitute(string id) => StatementTestData.GetExpression(id) ?? StatementTestDataNs.GetExpression(id);
}
