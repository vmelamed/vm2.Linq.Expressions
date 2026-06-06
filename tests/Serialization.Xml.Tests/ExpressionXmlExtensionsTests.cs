// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Xml;

[ExcludeFromCodeCoverage]
public class ExpressionXmlExtensionsTests
{
    static readonly Expression<Func<int, int, int>> _expr = (x, y) => x * y + 2;

    // ── ToXmlDocument ─────────────────────────────────────────────────────────

    [Fact]
    public void ToXmlDocument_WhenExpressionIsNull_ShouldThrowArgumentNullException()
    {
        Expression? expression = null;

        var act = () => expression!.ToXmlDocument();

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("expression");
    }

    // ── ToXmlString ───────────────────────────────────────────────────────────

    [Fact]
    public void ToXmlString_WhenExpressionIsNull_ShouldThrowArgumentNullException()
    {
        Expression? expression = null;

        var act = () => expression!.ToXmlString();

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("expression");
    }

    // ── ToXmlStream ───────────────────────────────────────────────────────────

    [Fact]
    public void ToXmlStream_WhenExpressionIsNull_ShouldThrowArgumentNullException()
    {
        Expression? expression = null;
        using var stream = new MemoryStream();

        var act = () => expression!.ToXmlStream(stream);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("expression");
    }

    [Fact]
    public void ToXmlStream_WhenStreamIsNull_ShouldThrowArgumentNullException()
    {
        Stream? stream = null;

        var act = () => _expr.ToXmlStream(stream!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("stream");
    }

    // ── ToXmlStreamAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task ToXmlStreamAsync_WhenExpressionIsNull_ShouldThrowArgumentNullException_Async()
    {
        Expression? expression = null;
        using var stream = new MemoryStream();

        var act = async () => await expression!.ToXmlStreamAsync(stream);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("expression");
    }

    [Fact]
    public async Task ToXmlStreamAsync_WhenStreamIsNull_ShouldThrowArgumentNullException_Async()
    {
        Stream? stream = null;

        var act = async () => await _expr.ToXmlStreamAsync(stream!);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("stream");
    }

    // ── ToXmlWriter ───────────────────────────────────────────────────────────

    [Fact]
    public void ToXmlWriter_WhenExpressionIsNull_ShouldThrowArgumentNullException()
    {
        Expression? expression = null;
        using var sw = new StringWriter();
        using var writer = XmlWriter.Create(sw);

        var act = () => expression!.ToXmlWriter(writer);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("expression");
    }

    [Fact]
    public void ToXmlWriter_WhenWriterIsNull_ShouldThrowArgumentNullException()
    {
        XmlWriter? writer = null;

        var act = () => _expr.ToXmlWriter(writer!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("writer");
    }

    // ── ToXmlWriterAsync ──────────────────────────────────────────────────────

    [Fact]
    public async Task ToXmlWriterAsync_WhenExpressionIsNull_ShouldThrowArgumentNullException_Async()
    {
        Expression? expression = null;
        using var sw = new StringWriter();
        await using var writer = XmlWriter.Create(sw, new XmlWriterSettings { Async = true });

        var act = async () => await expression!.ToXmlWriterAsync(writer);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("expression");
    }

    [Fact]
    public async Task ToXmlWriterAsync_WhenWriterIsNull_ShouldThrowArgumentNullException_Async()
    {
        XmlWriter? writer = null;

        var act = async () => await _expr.ToXmlWriterAsync(writer!);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("writer");
    }

    // ── ToXmlFile ─────────────────────────────────────────────────────────────

    [Fact]
    public void ToXmlFile_WhenExpressionIsNull_ShouldThrowArgumentNullException()
    {
        Expression? expression = null;

        var act = () => expression!.ToXmlFile("some.xml");

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("expression");
    }

    [Fact]
    public void ToXmlFile_WhenFilePathIsNull_ShouldThrowArgumentNullException()
    {
        string? filePath = null;

        var act = () => _expr.ToXmlFile(filePath!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("filePath");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ToXmlFile_WhenFilePathIsEmptyOrWhiteSpace_ShouldThrowArgumentException(string filePath)
    {
        var act = () => _expr.ToXmlFile(filePath);

        act.Should().ThrowExactly<ArgumentException>().WithParameterName("filePath");
    }

    // ── ToXmlFileAsync ────────────────────────────────────────────────────────

    [Fact]
    public async Task ToXmlFileAsync_WhenExpressionIsNull_ShouldThrowArgumentNullException_Async()
    {
        Expression? expression = null;

        var act = async () => await expression!.ToXmlFileAsync("some.xml");

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("expression");
    }

    [Fact]
    public async Task ToXmlFileAsync_WhenFilePathIsNull_ShouldThrowArgumentNullException_Async()
    {
        string? filePath = null;

        var act = async () => await _expr.ToXmlFileAsync(filePath!);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("filePath");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ToXmlFileAsync_WhenFilePathIsEmptyOrWhiteSpace_ShouldThrowArgumentException_Async(string filePath)
    {
        var act = async () => await _expr.ToXmlFileAsync(filePath);

        (await act.Should().ThrowExactlyAsync<ArgumentException>()).WithParameterName("filePath");
    }
}
