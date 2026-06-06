// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Xml;

[ExcludeFromCodeCoverage]
public class ExpressionXmlTests
{
    static readonly Expression<Func<int, int, int>> _expr = (x, y) => x * y + 2;

    // ── FromStream ────────────────────────────────────────────────────────────

    [Fact]
    public void FromStream_WhenStreamIsNull_ShouldThrowArgumentNullException()
    {
        Stream? stream = null;

        var act = () => ExpressionXml.FromStream(stream!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("stream");
    }

    // ── FromStreamAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task FromStreamAsync_WhenStreamIsNull_ShouldThrowArgumentNullException_Async()
    {
        Stream? stream = null;

        var act = async () => await ExpressionXml.FromStreamAsync(stream!);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("stream");
    }

    // ── FromReader ────────────────────────────────────────────────────────────

    [Fact]
    public void FromReader_WhenReaderIsNull_ShouldThrowArgumentNullException()
    {
        XmlReader? reader = null;

        var act = () => ExpressionXml.FromReader(reader!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("reader");
    }

    [Fact]
    public void FromReader_WhenReaderContainsValidXml_ShouldRoundTrip()
    {
        var xml = _expr.ToXmlString();
        using var sr = new StringReader(xml);
        using var reader = XmlReader.Create(sr);

        var roundTrip = ExpressionXml.FromReader(reader);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    // ── FromReaderAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task FromReaderAsync_WhenReaderIsNull_ShouldThrowArgumentNullException_Async()
    {
        XmlReader? reader = null;

        var act = async () => await ExpressionXml.FromReaderAsync(reader!);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("reader");
    }

    [Fact]
    public async Task FromReaderAsync_WhenReaderContainsValidXml_ShouldRoundTrip_Async()
    {
        var xml = _expr.ToXmlString();
        using var sr = new StringReader(xml);
        using var reader = XmlReader.Create(sr, new XmlReaderSettings { Async = true });

        var roundTrip = await ExpressionXml.FromReaderAsync(reader, cancellationToken: TestContext.Current.CancellationToken);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    // ── FromFile ──────────────────────────────────────────────────────────────

    [Fact]
    public void FromFile_WhenFilePathIsNull_ShouldThrowArgumentNullException()
    {
        string? filePath = null;

        var act = () => ExpressionXml.FromFile(filePath!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("filePath");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void FromFile_WhenFilePathIsEmptyOrWhiteSpace_ShouldThrowArgumentException(string filePath)
    {
        var act = () => ExpressionXml.FromFile(filePath);

        act.Should().ThrowExactly<ArgumentException>().WithParameterName("filePath");
    }

    // ── FromFileAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task FromFileAsync_WhenFilePathIsNull_ShouldThrowArgumentNullException_Async()
    {
        string? filePath = null;

        var act = async () => await ExpressionXml.FromFileAsync(filePath!);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("filePath");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task FromFileAsync_WhenFilePathIsEmptyOrWhiteSpace_ShouldThrowArgumentException_Async(string filePath)
    {
        var act = async () => await ExpressionXml.FromFileAsync(filePath);

        (await act.Should().ThrowExactlyAsync<ArgumentException>()).WithParameterName("filePath");
    }

    // ── FromXDocument ─────────────────────────────────────────────────────────

    [Fact]
    public void FromXDocument_WhenDocumentIsNull_ShouldThrowArgumentNullException()
    {
        XDocument? document = null;

        var act = () => ExpressionXml.FromXDocument(document!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("document");
    }

    [Fact]
    public void FromXDocument_WhenDocumentIsValid_ShouldRoundTrip()
    {
        var doc = _expr.ToXmlDocument();

        var roundTrip = ExpressionXml.FromXDocument(doc);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    // ── FromXElement ──────────────────────────────────────────────────────────

    [Fact]
    public void FromXElement_WhenElementIsNull_ShouldThrowArgumentNullException()
    {
        XElement? element = null;

        var act = () => ExpressionXml.FromXElement(element!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("element");
    }

    [Fact]
    public void FromXElement_WhenElementIsValid_ShouldRoundTrip()
    {
        var element = _expr.ToXmlDocument().Root!;

        var roundTrip = ExpressionXml.FromXElement(element, new XmlOptions { ValidateInputDocuments = ValidateExpressionDocuments.Never });

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    // ── FromString ────────────────────────────────────────────────────────────

    [Fact]
    public void FromString_WhenXmlIsNull_ShouldThrowArgumentNullException()
    {
        string? xml = null;

        var act = () => ExpressionXml.FromString(xml!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("xml");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void FromString_WhenXmlIsEmptyOrWhiteSpace_ShouldThrowArgumentException(string xml)
    {
        var act = () => ExpressionXml.FromString(xml);

        act.Should().ThrowExactly<ArgumentException>().WithParameterName("xml");
    }

    [Fact]
    public void FromString_WhenXmlIsValid_ShouldRoundTrip()
    {
        var xml = _expr.ToXmlString();

        var roundTrip = ExpressionXml.FromString(xml);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }
}
