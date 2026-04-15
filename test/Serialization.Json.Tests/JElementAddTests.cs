namespace vm2.Linq.Expressions.Serialization.Json.Tests;

public class JElementAddTests
{
    #region Add(string key, JsonNode? value)
    [Fact]
    public void Add_KeyValue_ToNullNode_CreatesObject()
    {
        var je = new JElement("root");
        je.Add("key", JsonValue.Create(42));

        je.Node.Should().BeOfType<JsonObject>();
        je.Node!.AsObject()["key"]!.GetValue<int>().Should().Be(42);
    }

    [Fact]
    public void Add_KeyValue_ToExistingObject()
    {
        var je = new JElement("root", new JsonObject());
        je.Add("key", JsonValue.Create("hello"));

        je.Node!.AsObject()["key"]!.GetValue<string>().Should().Be("hello");
    }

    [Fact]
    public void Add_KeyValue_ToNonObject_Throws()
    {
        var je = new JElement("root", new JsonArray());
        var act = () => je.Add("key", JsonValue.Create(1));
        act.Should().Throw<InternalTransformErrorException>();
    }
    #endregion

    #region Add(params JElement?[])
    [Fact]
    public void Add_JElementParams_AddsAll()
    {
        var je = new JElement("root");
        je.Add(
            new JElement("a", JsonValue.Create(1)),
            new JElement("b", JsonValue.Create(2))
        );

        je.Node!.AsObject().Count.Should().Be(2);
    }
    #endregion

    #region Add(params object?[])
    [Fact]
    public void Add_ObjectParams_JElement()
    {
        var je = new JElement("root");
        var child = (object)new JElement("a", JsonValue.Create(1));
        je.Add(child);

        je.Node!.AsObject()["a"]!.GetValue<int>().Should().Be(1);
    }

    [Fact]
    public void Add_ObjectParams_NullIsSkipped()
    {
        var je = new JElement("root", new JsonObject());
        je.Add((object?)null);

        je.Node!.AsObject().Count.Should().Be(0);
    }

    [Fact]
    public void Add_ObjectParams_EnumerableOfJElement()
    {
        var je = new JElement("root");
        IEnumerable<JElement?> children = [
            new JElement("x", JsonValue.Create(1)),
            new JElement("y", JsonValue.Create(2)),
        ];
        je.Add((object)children);

        je.Node!.AsObject().Count.Should().Be(2);
    }

    [Fact]
    public void Add_ObjectParams_UnknownType_Throws()
    {
        var je = new JElement("root", new JsonObject());
        var act = () => je.Add((object)42);
        act.Should().Throw<InternalTransformErrorException>().WithMessage("*Int32*");
    }

    [Fact]
    public void Add_ObjectParams_ToNonObject_Throws()
    {
        var je = new JElement("root", new JsonArray());
        var act = () => je.Add((object)new JElement("a", JsonValue.Create(1)));
        act.Should().Throw<InternalTransformErrorException>();
    }
    #endregion

    #region Add(IEnumerable<JElement?>)
    [Fact]
    public void Add_NullableJElementEnumerable_SkipsNulls()
    {
        var je = new JElement("root");
        IEnumerable<JElement?> items = [
            new JElement("a", JsonValue.Create(1)),
            null,
            new JElement("b", JsonValue.Create(2)),
        ];
        je.Add(items);

        je.Node!.AsObject().Count.Should().Be(2);
    }

    [Fact]
    public void Add_NullableJElementEnumerable_ToNonObject_Throws()
    {
        var je = new JElement("root", new JsonArray());
        IEnumerable<JElement?> items = [new JElement("a", JsonValue.Create(1))];
        var act = () => je.Add(items);
        act.Should().Throw<InternalTransformErrorException>();
    }
    #endregion

    #region Add(IEnumerable<JElement>)
    [Fact]
    public void Add_JElementEnumerable_AddsAll()
    {
        var je = new JElement("root");
        IEnumerable<JElement> items = [
            new JElement("a", JsonValue.Create(1)),
            new JElement("b", JsonValue.Create(2)),
        ];
        je.Add(items);

        je.Node!.AsObject().Count.Should().Be(2);
    }

    [Fact]
    public void Add_JElementEnumerable_ToNonObject_Throws()
    {
        var je = new JElement("root", new JsonArray());
        IEnumerable<JElement> items = [new JElement("a", JsonValue.Create(1))];
        var act = () => je.Add(items);
        act.Should().Throw<InternalTransformErrorException>();
    }
    #endregion

    #region Add(JsonNode? element) - single to array
    [Fact]
    public void Add_SingleJsonNode_ToNullNode_CreatesArray()
    {
        var je = new JElement("arr");
        je.Add(JsonValue.Create(42));

        je.Node.Should().BeOfType<JsonArray>();
        je.Node!.AsArray()[0]!.GetValue<int>().Should().Be(42);
    }

    [Fact]
    public void Add_SingleJsonNode_ToExistingArray()
    {
        var je = new JElement("arr", new JsonArray(1));
        je.Add(JsonValue.Create(2));

        je.Node!.AsArray().Count.Should().Be(2);
    }

    [Fact]
    public void Add_SingleJsonNode_ToNonArray_Throws()
    {
        var je = new JElement("arr", new JsonObject());
        var act = () => je.Add(JsonValue.Create(1));
        act.Should().Throw<InternalTransformErrorException>();
    }
    #endregion

    #region Add(params JsonNode?[])
    [Fact]
    public void Add_JsonNodeParams_AddsAll()
    {
        var je = new JElement("arr", new JsonArray());
        je.Add(JsonValue.Create(1), JsonValue.Create(2), JsonValue.Create(3));

        je.Node!.AsArray().Count.Should().Be(3);
    }
    #endregion

    #region Add(IEnumerable<JsonNode?>) - the fixed method
    [Fact]
    public void Add_JsonNodeEnumerable_ToNullNode_CreatesArray()
    {
        var je = new JElement("arr");
        IEnumerable<JsonNode?> items = [JsonValue.Create(1), JsonValue.Create(2)];
        je.Add(items);

        je.Node.Should().BeOfType<JsonArray>();
        je.Node!.AsArray().Count.Should().Be(2);
    }

    [Fact]
    public void Add_JsonNodeEnumerable_ToExistingArray()
    {
        var je = new JElement("arr", new JsonArray());
        IEnumerable<JsonNode?> items = [JsonValue.Create(1), null, JsonValue.Create(3)];
        je.Add(items);

        je.Node!.AsArray().Count.Should().Be(3);
    }

    [Fact]
    public void Add_JsonNodeEnumerable_ToNonArray_Throws()
    {
        var je = new JElement("arr", new JsonObject());
        IEnumerable<JsonNode?> items = [JsonValue.Create(1)];
        var act = () => je.Add(items);
        act.Should().Throw<InternalTransformErrorException>();
    }
    #endregion
}
