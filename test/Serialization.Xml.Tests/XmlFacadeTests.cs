namespace vm2.Linq.Expressions.Serialization.Xml.Tests;

public class XmlFacadeTests
{
    static readonly Expression<Func<int, int, int>> _expr = (x, y) => x * y + 2;

    sealed class NonClosingMemoryStream : MemoryStream
    {
        protected override void Dispose(bool disposing)
        {
            // Keep the underlying memory stream readable after facade methods dispose nested writers.
            Flush();
        }
    }

    [Fact]
    public void ToXmlDocument_And_ToExpression_RoundTrip()
    {
        var doc = _expr.ToXmlDocument();

        var roundTrip = doc.ToExpression();

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToXmlString_And_FromString_RoundTrip()
    {
        var xml = _expr.ToXmlString();

        var roundTrip = ExpressionXml.FromString(xml);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToXmlStream_And_FromStream_RoundTrip()
    {
        using var stream = new NonClosingMemoryStream();
        _expr.ToXmlStream(stream);

        stream.Position = 0;
        var roundTrip = ExpressionXml.FromStream(stream);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToXmlWriter_And_FromReader_RoundTrip()
    {
        using var sw = new StringWriter();
        using (var writer = XmlWriter.Create(sw))
            _expr.ToXmlWriter(writer);

        using var sr = new StringReader(sw.ToString());
        using var reader = XmlReader.Create(sr);
        var roundTrip = ExpressionXml.FromReader(reader);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToXmlFile_And_FromFile_RoundTrip()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"xml-facade-{Guid.NewGuid():N}.xml");

        try
        {
            _expr.ToXmlFile(filePath);
            var roundTrip = ExpressionXml.FromFile(filePath);

            _expr.DeepEquals(roundTrip).Should().BeTrue();
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public async Task AsyncFacadeMethods_RoundTrip()
    {
        var cancellationToken = TestContext.Current.CancellationToken;

        using var stream = new NonClosingMemoryStream();
        await _expr.ToXmlStreamAsync(stream, cancellationToken: cancellationToken);

        stream.Position = 0;
        var fromStream = await ExpressionXml.FromStreamAsync(stream, cancellationToken: cancellationToken);
        _expr.DeepEquals(fromStream).Should().BeTrue();

        var filePath = Path.Combine(Path.GetTempPath(), $"xml-facade-async-{Guid.NewGuid():N}.xml");

        try
        {
            await _expr.ToXmlFileAsync(filePath, cancellationToken: cancellationToken);
            var fromFile = await ExpressionXml.FromFileAsync(filePath, cancellationToken: cancellationToken);
            _expr.DeepEquals(fromFile).Should().BeTrue();
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        using var sw = new StringWriter();
        var settings = new XmlWriterSettings { Async = true, OmitXmlDeclaration = false };
        using (var writer = XmlWriter.Create(sw, settings))
            await _expr.ToXmlWriterAsync(writer, cancellationToken: cancellationToken);

        using var sr = new StringReader(sw.ToString());
        using var reader = XmlReader.Create(sr, new XmlReaderSettings { Async = true });
        var fromReader = await ExpressionXml.FromReaderAsync(reader, cancellationToken: cancellationToken);
        _expr.DeepEquals(fromReader).Should().BeTrue();
    }

    [Fact]
    public void ToXmlString_RespectsOptions()
    {
        var options = new XmlOptions
        {
            AddComments = true,
            Indent = true,
        };

        var xml = _expr.ToXmlString(options);

        xml.Should().Contain("<!--");
    }
}
