namespace vm2.Linq.Expressions.Serialization.Xml.Tests;

[CollectionDefinition("XML")]
public partial class StatementTests(XmlTestsFixture fixture, ITestOutputHelper output) : BaseTests(fixture, output)
{
    protected override string XmlTestFilesPath => Path.Combine(_fixture.TestFilesPath, "Statements");

    [Theory]
    [MemberData(nameof(StatementTestData.Data), MemberType = typeof(StatementTestData))]
    [MemberData(nameof(StatementTestDataNs.Data), MemberType = typeof(StatementTestDataNs))]
    public async Task StatementToXmlTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.ToXmlTestAsync(testFileLine, expressionString, fileName);

    [Theory]
    [MemberData(nameof(StatementTestData.Data), MemberType = typeof(StatementTestData))]
    [MemberData(nameof(StatementTestDataNs.Data), MemberType = typeof(StatementTestDataNs))]
    public async Task StatementFromXmlTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.FromXmlTestAsync(testFileLine, expressionString, fileName);

    protected override Expression Substitute(string id) => StatementTestData.GetExpression(id) ?? StatementTestDataNs.GetExpression(id);
}
