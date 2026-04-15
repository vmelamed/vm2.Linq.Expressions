namespace vm2.Linq.Expressions.Serialization.Xml.Tests;

[CollectionDefinition("XML")]
public partial class LambdaTests(XmlTestsFixture fixture, ITestOutputHelper output) : BaseTests(fixture, output)
{
    protected override string XmlTestFilesPath => Path.Combine(_fixture.TestFilesPath, "Lambdas");

    [Theory]
    [MemberData(nameof(LambdaTestData.Data), MemberType = typeof(LambdaTestData))]
    public async Task LambdaToXmlTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.ToXmlTestAsync(testFileLine, expressionString, fileName);

    [Theory]
    [MemberData(nameof(LambdaTestData.Data), MemberType = typeof(LambdaTestData))]
    public async Task LambdaFromXmlTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.FromXmlTestAsync(testFileLine, expressionString, fileName);

    protected override Expression Substitute(string id) => LambdaTestData.GetExpression(id);
}
