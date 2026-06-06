// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Json;

[ExcludeFromCodeCoverage]
public class ExpressionJsonExtensionsTests
{
    static readonly Expression<Func<int, int, int>> _expr = (x, y) => x * y + 2;

    // ── ToJsonDocument ─────────────────────────────────────────────────────────

    [Fact]
    public void ToJsonDocument_WhenExpressionIsNull_ShouldThrowArgumentNullException()
    {
        Expression? expression = null;

        var act = () => expression!.ToJsonDocument();

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("expression");
    }

    // ── ToJsonString ──────────────────────────────────────────────────────────

    [Fact]
    public void ToJsonString_WhenExpressionIsNull_ShouldThrowArgumentNullException()
    {
        Expression? expression = null;

        var act = () => expression!.ToJsonString();

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("expression");
    }

    // ── ToJsonStream ──────────────────────────────────────────────────────────

    [Fact]
    public void ToJsonStream_WhenExpressionIsNull_ShouldThrowArgumentNullException()
    {
        Expression? expression = null;
        using var stream = new MemoryStream();

        var act = () => expression!.ToJsonStream(stream);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("expression");
    }

    [Fact]
    public void ToJsonStream_WhenStreamIsNull_ShouldThrowArgumentNullException()
    {
        Stream? stream = null;

        var act = () => _expr.ToJsonStream(stream!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("stream");
    }

    // ── ToJsonStreamAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task ToJsonStreamAsync_WhenExpressionIsNull_ShouldThrowArgumentNullException_Async()
    {
        Expression? expression = null;
        using var stream = new MemoryStream();

        var act = async () => await expression!.ToJsonStreamAsync(stream);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("expression");
    }

    [Fact]
    public async Task ToJsonStreamAsync_WhenStreamIsNull_ShouldThrowArgumentNullException_Async()
    {
        Stream? stream = null;

        var act = async () => await _expr.ToJsonStreamAsync(stream!);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("stream");
    }

    // ── ToJsonWriter ──────────────────────────────────────────────────────────

    [Fact]
    public void ToJsonWriter_WhenExpressionIsNull_ShouldThrowArgumentNullException()
    {
        Expression? expression = null;
        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream);

        var act = () => expression!.ToJsonWriter(writer);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("expression");
    }

    [Fact]
    public void ToJsonWriter_WhenWriterIsNull_ShouldThrowArgumentNullException()
    {
        Utf8JsonWriter? writer = null;

        var act = () => _expr.ToJsonWriter(writer!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("writer");
    }

    // ── ToJsonWriterAsync ─────────────────────────────────────────────────────

    [Fact]
    public async Task ToJsonWriterAsync_WhenExpressionIsNull_ShouldThrowArgumentNullException_Async()
    {
        Expression? expression = null;
        using var stream = new MemoryStream();
        await using var writer = new Utf8JsonWriter(stream);

        var act = async () => await expression!.ToJsonWriterAsync(writer);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("expression");
    }

    [Fact]
    public async Task ToJsonWriterAsync_WhenWriterIsNull_ShouldThrowArgumentNullException_Async()
    {
        Utf8JsonWriter? writer = null;

        var act = async () => await _expr.ToJsonWriterAsync(writer!);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("writer");
    }

    // ── ToJsonFile ────────────────────────────────────────────────────────────

    [Fact]
    public void ToJsonFile_WhenExpressionIsNull_ShouldThrowArgumentNullException()
    {
        Expression? expression = null;

        var act = () => expression!.ToJsonFile("some.json");

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("expression");
    }

    [Fact]
    public void ToJsonFile_WhenFilePathIsNull_ShouldThrowArgumentNullException()
    {
        string? filePath = null;

        var act = () => _expr.ToJsonFile(filePath!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("filePath");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void ToJsonFile_WhenFilePathIsEmptyOrWhiteSpace_ShouldThrowArgumentException(string filePath)
    {
        var act = () => _expr.ToJsonFile(filePath);

        act.Should().ThrowExactly<ArgumentException>().WithParameterName("filePath");
    }

    // ── ToJsonFileAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task ToJsonFileAsync_WhenExpressionIsNull_ShouldThrowArgumentNullException_Async()
    {
        Expression? expression = null;

        var act = async () => await expression!.ToJsonFileAsync("some.json");

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("expression");
    }

    [Fact]
    public async Task ToJsonFileAsync_WhenFilePathIsNull_ShouldThrowArgumentNullException_Async()
    {
        string? filePath = null;

        var act = async () => await _expr.ToJsonFileAsync(filePath!);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("filePath");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ToJsonFileAsync_WhenFilePathIsEmptyOrWhiteSpace_ShouldThrowArgumentException_Async(string filePath)
    {
        var act = async () => await _expr.ToJsonFileAsync(filePath);

        (await act.Should().ThrowExactlyAsync<ArgumentException>()).WithParameterName("filePath");
    }

}
