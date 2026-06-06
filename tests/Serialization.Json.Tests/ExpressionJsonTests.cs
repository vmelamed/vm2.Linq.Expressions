// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Json;

[ExcludeFromCodeCoverage]
public class ExpressionJsonTests
{
    static readonly Expression<Func<int, int, int>> _expr = (x, y) => x * y + 2;

    // ── FromString ────────────────────────────────────────────────────────────

    [Fact]
    public void FromString_WhenJsonIsNull_ShouldThrowArgumentNullException()
    {
        string? json = null;

        var act = () => ExpressionJson.FromString(json!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("json");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void FromString_WhenJsonIsEmptyOrWhiteSpace_ShouldThrowArgumentException(string json)
    {
        var act = () => ExpressionJson.FromString(json);

        act.Should().ThrowExactly<ArgumentException>().WithParameterName("json");
    }

    [Fact]
    public void FromString_WhenJsonIsNotAnObject_ShouldThrowSerializationException()
    {
        var act = () => ExpressionJson.FromString("[1, 2, 3]");

        act.Should().ThrowExactly<SerializationException>();
    }

    // ── FromJsonObject ────────────────────────────────────────────────────────

    [Fact]
    public void FromJsonObject_WhenJsonObjectIsNull_ShouldThrowArgumentNullException()
    {
        JsonObject? jsonObject = null;

        var act = () => ExpressionJson.FromJsonObject(jsonObject!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("jsonObject");
    }

    [Fact]
    public void FromJsonObject_WhenJsonObjectIsValid_ShouldRoundTrip()
    {
        var doc = _expr.ToJsonDocument();

        var roundTrip = ExpressionJson.FromJsonObject(doc);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    // ── FromStream ────────────────────────────────────────────────────────────

    [Fact]
    public void FromStream_WhenStreamIsNull_ShouldThrowArgumentNullException()
    {
        Stream? stream = null;

        var act = () => ExpressionJson.FromStream(stream!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("stream");
    }

    // ── FromStreamAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task FromStreamAsync_WhenStreamIsNull_ShouldThrowArgumentNullException_Async()
    {
        Stream? stream = null;

        var act = async () => await ExpressionJson.FromStreamAsync(stream!);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("stream");
    }

    // ── FromReader ────────────────────────────────────────────────────────────

    [Fact]
    public void FromReader_WhenReaderIsNull_ShouldThrowArgumentNullException()
    {
        TextReader? reader = null;

        var act = () => ExpressionJson.FromReader(reader!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("reader");
    }

    [Fact]
    public void FromReader_WhenReaderContainsValidJson_ShouldRoundTrip()
    {
        var json = _expr.ToJsonString();
        using var reader = new StringReader(json);

        var roundTrip = ExpressionJson.FromReader(reader);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    // ── FromReaderAsync ───────────────────────────────────────────────────────

    [Fact]
    public async Task FromReaderAsync_WhenReaderIsNull_ShouldThrowArgumentNullException_Async()
    {
        TextReader? reader = null;

        var act = async () => await ExpressionJson.FromReaderAsync(reader!);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("reader");
    }

    [Fact]
    public async Task FromReaderAsync_WhenReaderContainsValidJson_ShouldRoundTrip_Async()
    {
        var json = _expr.ToJsonString();
        using var reader = new StringReader(json);

        var roundTrip = await ExpressionJson.FromReaderAsync(reader, cancellationToken: TestContext.Current.CancellationToken);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    // ── FromFile ──────────────────────────────────────────────────────────────

    [Fact]
    public void FromFile_WhenFilePathIsNull_ShouldThrowArgumentNullException()
    {
        string? filePath = null;

        var act = () => ExpressionJson.FromFile(filePath!);

        act.Should().ThrowExactly<ArgumentNullException>().WithParameterName("filePath");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void FromFile_WhenFilePathIsEmptyOrWhiteSpace_ShouldThrowArgumentException(string filePath)
    {
        var act = () => ExpressionJson.FromFile(filePath);

        act.Should().ThrowExactly<ArgumentException>().WithParameterName("filePath");
    }

    // ── FromFileAsync ─────────────────────────────────────────────────────────

    [Fact]
    public async Task FromFileAsync_WhenFilePathIsNull_ShouldThrowArgumentNullException_Async()
    {
        string? filePath = null;

        var act = async () => await ExpressionJson.FromFileAsync(filePath!);

        (await act.Should().ThrowExactlyAsync<ArgumentNullException>()).WithParameterName("filePath");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task FromFileAsync_WhenFilePathIsEmptyOrWhiteSpace_ShouldThrowArgumentException_Async(string filePath)
    {
        var act = async () => await ExpressionJson.FromFileAsync(filePath);

        (await act.Should().ThrowExactlyAsync<ArgumentException>()).WithParameterName("filePath");
    }
}
