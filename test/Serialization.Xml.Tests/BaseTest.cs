namespace vm2.Linq.Expressions.Serialization.Xml.Tests;

[CollectionDefinition("XML")]
public abstract class BaseTests(
        XmlTestsFixture fixture,
        ITestOutputHelper output) : IClassFixture<XmlTestsFixture>
{
    public ITestOutputHelper Out { get; } = output;

    protected XmlTestsFixture _fixture = fixture;

    protected abstract string XmlTestFilesPath { get; }

    protected bool XmlTestFilesPathExists { get; set; }

    public virtual async Task ToXmlTestAsync(string testFileLine, string expressionString, string fileName)
    {
        if (!XmlTestFilesPathExists)
        {
            if (!Directory.Exists(XmlTestFilesPath))
                Directory.CreateDirectory(XmlTestFilesPath);

            XmlTestFilesPathExists = true;
        }

        var expression = Substitute(expressionString);
        var pathName = Path.Combine(XmlTestFilesPath, fileName+".xml");
        var (expectedDoc, expectedStr) = await _fixture.GetXmlDocumentAsync(testFileLine, pathName, "EXPECTED", Out,
                                                            cancellationToken: TestContext.Current.CancellationToken);

        _fixture.TestExpressionToXml(testFileLine, expression, expectedDoc, expectedStr, pathName, Out);
        await _fixture.TestExpressionToXmlAsync(testFileLine, expression, expectedDoc, expectedStr, pathName, Out,
                                                            cancellationToken: TestContext.Current.CancellationToken);
    }

    public virtual async Task FromXmlTestAsync(string testFileLine, string expressionString, string fileName)
    {
        var expectedExpression = Substitute(expressionString);
        var pathName = Path.Combine(XmlTestFilesPath, fileName+".xml");
        var (inputDoc, _) = await _fixture.GetXmlDocumentAsync(testFileLine, pathName, "INPUT", Out, true,
                                                            cancellationToken: TestContext.Current.CancellationToken);

        inputDoc.Should().NotBeNull($"the input XDocument from {testFileLine} should not be null");

        _fixture.TestXmlToExpression(testFileLine, inputDoc!, expectedExpression);
    }

    protected virtual Expression Substitute(string id) => Expression.Constant(null);
}
