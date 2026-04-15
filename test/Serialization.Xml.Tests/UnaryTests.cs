namespace vm2.Linq.Expressions.Serialization.Xml.Tests;

[CollectionDefinition("XML")]
public partial class UnaryTests(XmlTestsFixture fixture, ITestOutputHelper output) : BaseTests(fixture, output)
{
    protected override string XmlTestFilesPath => Path.Combine(_fixture.TestFilesPath, "Unary");

    [Theory]
    [MemberData(nameof(UnaryTestData.Data), MemberType = typeof(UnaryTestData))]
    public async Task UnaryToXmlTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.ToXmlTestAsync(testFileLine, expressionString, fileName);

    [Theory]
    [MemberData(nameof(UnaryTestData.Data), MemberType = typeof(UnaryTestData))]
    public async Task UnaryFromXmlTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.FromXmlTestAsync(testFileLine, expressionString, fileName);

    protected override Expression Substitute(string id) => UnaryTestData.GetExpression(id);
}
