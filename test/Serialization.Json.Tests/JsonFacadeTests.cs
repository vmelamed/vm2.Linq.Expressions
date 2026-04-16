namespace vm2.Linq.Expressions.Serialization.Json.Tests;

using vm2.Linq.Expressions.Serialization;
using vm2.Linq.Expressions.Serialization.Json;

public class JsonFacadeTests
{
    static readonly Expression<Func<int, int, int>> _expr = (x, y) => x * y + 2;

    [Fact]
    public void ToJsonDocument_And_ToExpression_RoundTrip()
    {
        var doc = _expr.ToJsonDocument();

        var roundTrip = doc.ToExpression();

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToJsonString_And_FromString_RoundTrip()
    {
        var json = _expr.ToJsonString();

        var roundTrip = ExpressionJson.FromString(json);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToJsonStream_And_FromStream_RoundTrip()
    {
        using var stream = new MemoryStream();
        _expr.ToJsonStream(stream);

        stream.Position = 0;
        var roundTrip = ExpressionJson.FromStream(stream);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToJsonWriter_And_FromString_RoundTrip()
    {
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream))
            _expr.ToJsonWriter(writer);

        var json = Encoding.UTF8.GetString(stream.ToArray());
        var roundTrip = ExpressionJson.FromString(json);

        _expr.DeepEquals(roundTrip).Should().BeTrue();
    }

    [Fact]
    public void ToJsonFile_And_FromFile_RoundTrip()
    {
        var filePath = Path.Combine(Path.GetTempPath(), $"json-facade-{Guid.NewGuid():N}.json");

        try
        {
            _expr.ToJsonFile(filePath);
            var roundTrip = ExpressionJson.FromFile(filePath);

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

        using var stream = new MemoryStream();
        await _expr.ToJsonStreamAsync(stream, cancellationToken: cancellationToken);

        stream.Position = 0;
        var fromStream = await ExpressionJson.FromStreamAsync(stream, cancellationToken: cancellationToken);
        _expr.DeepEquals(fromStream).Should().BeTrue();

        var filePath = Path.Combine(Path.GetTempPath(), $"json-facade-async-{Guid.NewGuid():N}.json");

        try
        {
            await _expr.ToJsonFileAsync(filePath, cancellationToken: cancellationToken);
            var fromFile = await ExpressionJson.FromFileAsync(filePath, cancellationToken: cancellationToken);
            _expr.DeepEquals(fromFile).Should().BeTrue();
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        using var stream2 = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream2))
            await _expr.ToJsonWriterAsync(writer, cancellationToken: cancellationToken);

        var json = Encoding.UTF8.GetString(stream2.ToArray());
        var fromWriter = ExpressionJson.FromString(json);
        _expr.DeepEquals(fromWriter).Should().BeTrue();
    }

    [Fact]
    public void ToJsonString_RespectsOptions()
    {
        var options = new JsonOptions
        {
            Indent = true,
        };

        var json = _expr.ToJsonString(options);

        json.Should().Contain("\n");
    }
}
