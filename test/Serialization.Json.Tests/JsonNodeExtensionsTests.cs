namespace vm2.Linq.Expressions.Serialization.Json.Tests;

public class JsonNodeExtensionsTests
{
    #region TryAdd / Add single JElement
    [Fact]
    public void TryAdd_NullElement_ReturnsTrue()
    {
        var obj = new JsonObject();
        obj.TryAdd((JElement?)null).Should().BeTrue();
        obj.Count.Should().Be(0);
    }

    [Fact]
    public void TryAdd_EmptyKey_Throws()
    {
        var obj = new JsonObject();
        var act = () => obj.TryAdd(new JElement("", JsonValue.Create(1)));
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TryAdd_NewKey_ReturnsTrue()
    {
        var obj = new JsonObject();
        obj.TryAdd(new JElement("key", JsonValue.Create(42))).Should().BeTrue();
        obj["key"]!.GetValue<int>().Should().Be(42);
    }

    [Fact]
    public void TryAdd_DuplicateKey_ReturnsFalse()
    {
        var obj = new JsonObject { ["key"] = 1 };
        obj.TryAdd(new JElement("key", JsonValue.Create(2))).Should().BeFalse();
        obj["key"]!.GetValue<int>().Should().Be(1);
    }

    [Fact]
    public void Add_NullElement_ReturnsObject()
    {
        var obj = new JsonObject();
        var result = obj.Add((JElement?)null);
        result.Should().BeSameAs(obj);
        obj.Count.Should().Be(0);
    }

    [Fact]
    public void Add_EmptyKey_Throws()
    {
        var obj = new JsonObject();
        JElement? element = new JElement("", JsonValue.Create(1));
        var act = () => JsonNodeExtensions.Add(obj, element);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Add_NewElement_AddsToObject()
    {
        var obj = new JsonObject();
        JsonNodeExtensions.Add(obj, new JElement("key", JsonValue.Create(42)));

        obj["key"]!.GetValue<int>().Should().Be(42);
    }

    [Fact]
    public void Add_DuplicateKey_Throws()
    {
        var obj = new JsonObject { ["key"] = 1 };
        var act = () => obj.Add(new JElement("key", JsonValue.Create(2)));
        act.Should().Throw<ArgumentException>();
    }
    #endregion

    #region TryAdd / Add params JElement?[]
    [Fact]
    public void TryAdd_Params_AddsAll()
    {
        var obj = new JsonObject();
        var result = obj.TryAdd(
            new JElement("a", JsonValue.Create(1)),
            new JElement("b", JsonValue.Create(2))
        );
        result.Should().BeTrue();
        obj.Count.Should().Be(2);
    }

    [Fact]
    public void Add_Params_AddsAll()
    {
        var obj = new JsonObject();
        var result = obj.Add(
            new JElement("a", JsonValue.Create(1)),
            new JElement("b", JsonValue.Create(2))
        );
        result.Should().BeSameAs(obj);
        obj.Count.Should().Be(2);
    }
    #endregion

    #region TryAdd / Add params object?[]
    [Fact]
    public void TryAdd_ObjectParams_JElement()
    {
        var obj = new JsonObject();
        obj.TryAdd((object)new JElement("key", JsonValue.Create(1))).Should().BeTrue();
        obj["key"]!.GetValue<int>().Should().Be(1);
    }

    [Fact]
    public void TryAdd_ObjectParams_EnumerableJElement()
    {
        var obj = new JsonObject();
        IEnumerable<JElement?> items = [
            new JElement("a", JsonValue.Create(1)),
            new JElement("b", JsonValue.Create(2)),
        ];
        obj.TryAdd((object)items).Should().BeTrue();
        obj.Count.Should().Be(2);
    }

    [Fact]
    public void TryAdd_ObjectParams_Null_ReturnsTrue()
    {
        var obj = new JsonObject();
        obj.TryAdd(new object?[] { null }).Should().BeTrue();
    }

    [Fact]
    public void TryAdd_ObjectParams_UnknownType_Throws()
    {
        var obj = new JsonObject();
        var act = () => obj.TryAdd((object)42);
        act.Should().Throw<InternalTransformErrorException>();
    }

    [Fact]
    public void TryAdd_ObjectParams_DuplicateKey_ReturnsFalse()
    {
        var obj = new JsonObject { ["a"] = 1 };
        obj.TryAdd(
            (object)new JElement("a", JsonValue.Create(2)),
            (object)new JElement("b", JsonValue.Create(3))
        ).Should().BeFalse();
    }

    [Fact]
    public void Add_ObjectParams_JElement_Adds()
    {
        var obj = new JsonObject();
        var result = obj.Add(
            (object)new JElement("a", JsonValue.Create(1)),
            (object?)null
        );
        result.Should().BeSameAs(obj);
        obj.Count.Should().Be(1);
    }

    [Fact]
    public void Add_ObjectParams_EnumerableJElement_Adds()
    {
        var obj = new JsonObject();
        IEnumerable<JElement?> items = [
            new JElement("a", JsonValue.Create(1)),
        ];
        obj.Add((object)items);
        obj.Count.Should().Be(1);
    }

    [Fact]
    public void Add_ObjectParams_UnknownType_Throws()
    {
        var obj = new JsonObject();
        var act = () => obj.Add((object)42);
        act.Should().Throw<InternalTransformErrorException>();
    }
    #endregion

    #region TryAdd / Add IEnumerable<JElement?>
    [Fact]
    public void TryAdd_Enumerable_SkipsNulls()
    {
        var obj = new JsonObject();
        IEnumerable<JElement?> items = [
            new JElement("a", JsonValue.Create(1)),
            null,
            new JElement("b", JsonValue.Create(2)),
        ];
        obj.TryAdd(items).Should().BeTrue();
        obj.Count.Should().Be(2);
    }

    [Fact]
    public void Add_Enumerable_SkipsNulls()
    {
        var obj = new JsonObject();
        IEnumerable<JElement?> items = [
            new JElement("a", JsonValue.Create(1)),
            null,
        ];
        var result = obj.Add(items);
        result.Should().BeSameAs(obj);
        obj.Count.Should().Be(1);
    }
    #endregion

    #region GetPropertyValue (by name)
    [Fact]
    public void GetPropertyValue_Exists_ReturnsNode()
    {
        var obj = new JsonObject { ["key"] = 42 };
        obj.GetPropertyValue("key")!.GetValue<int>().Should().Be(42);
    }

    [Fact]
    public void GetPropertyValue_Missing_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var act = () => obj.GetPropertyValue("missing");
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region TryGetPropertyValue<T> / GetPropertyValue<T>
    [Fact]
    public void TryGetPropertyValue_Typed_Found()
    {
        var obj = new JsonObject { ["key"] = 42 };
        obj.TryGetPropertyValue<int>("key", out var val).Should().BeTrue();
        val.Should().Be(42);
    }

    [Fact]
    public void TryGetPropertyValue_Typed_Missing()
    {
        var obj = new JsonObject { ["other"] = 42 };
        obj.TryGetPropertyValue<int>("missing", out _).Should().BeFalse();
    }

    [Fact]
    public void TryGetPropertyValue_Typed_NullNode()
    {
        var obj = new JsonObject { ["key"] = null };
        obj.TryGetPropertyValue<int>("key", out _).Should().BeFalse();
    }

    [Fact]
    public void GetPropertyValue_Typed_Returns()
    {
        var obj = new JsonObject { ["key"] = "hello" };
        obj.GetPropertyValue<string>("key").Should().Be("hello");
    }

    [Fact]
    public void GetPropertyValue_Typed_Missing_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var act = () => obj.GetPropertyValue<string>("missing");
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region TryGetOneOf / GetOneOf
    [Fact]
    public void TryGetOneOf_Finds()
    {
        var obj = new JsonObject { ["add"] = new JsonObject(), ["other"] = 1 };
        obj.TryGetOneOf(["add", "subtract"], out var name, out var node).Should().BeTrue();
        name.Should().Be("add");
        node.Should().BeOfType<JsonObject>();
    }

    [Fact]
    public void TryGetOneOf_NotFound()
    {
        var obj = new JsonObject { ["other"] = 1 };
        obj.TryGetOneOf(["add", "subtract"], out _, out _).Should().BeFalse();
    }

    [Fact]
    public void GetOneOf_Returns()
    {
        var obj = new JsonObject { ["add"] = new JsonObject() };
        var (name, node) = obj.GetOneOf(["add", "subtract"]);
        name.Should().Be("add");
    }

    [Fact]
    public void GetOneOf_NotFound_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var act = () => obj.GetOneOf(["add"]);
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region IsNil
    [Fact]
    public void IsNil_NullValue_ReturnsTrue()
    {
        var obj = new JsonObject { [Vocabulary.Value] = null };
        obj.IsNil().Should().BeTrue();
    }

    [Fact]
    public void IsNil_NoValueProperty_ReturnsFalse()
    {
        var obj = new JsonObject { ["other"] = 1 };
        obj.IsNil().Should().BeFalse();
    }

    [Fact]
    public void IsNil_NonNullValue_ReturnsFalse()
    {
        var obj = new JsonObject { [Vocabulary.Value] = 42 };
        obj.IsNil().Should().BeFalse();
    }
    #endregion

    #region TryGetValue / GetValue
    [Fact]
    public void TryGetValue_Exists_ReturnsTrue()
    {
        var obj = new JsonObject { [Vocabulary.Value] = 42 };
        obj.TryGetValue(out var node).Should().BeTrue();
        node!.GetValue<int>().Should().Be(42);
    }

    [Fact]
    public void TryGetValue_Missing_ReturnsFalse()
    {
        var obj = new JsonObject { ["other"] = 1 };
        obj.TryGetValue(out _).Should().BeFalse();
    }

    [Fact]
    public void TryGetValue_CustomPropertyName()
    {
        var obj = new JsonObject { ["custom"] = 42 };
        obj.TryGetValue(out var node, "custom").Should().BeTrue();
        node!.GetValue<int>().Should().Be(42);
    }

    [Fact]
    public void GetValue_Exists()
    {
        var obj = new JsonObject { [Vocabulary.Value] = "hello" };
        obj.GetValue()!.GetValue<string>().Should().Be("hello");
    }

    [Fact]
    public void GetValue_Missing_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var act = () => obj.GetValue();
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region TryGetLength / GetLength
    [Fact]
    public void TryGetLength_Exists_ReturnsTrue()
    {
        var obj = new JsonObject { [Vocabulary.Length] = 42 };
        obj.TryGetLength(out var len).Should().BeTrue();
        len.Should().Be(42);
    }

    [Fact]
    public void TryGetLength_Missing_ReturnsFalse()
    {
        var obj = new JsonObject { ["other"] = 1 };
        obj.TryGetLength(out _).Should().BeFalse();
    }

    [Fact]
    public void TryGetLength_NullValue_ReturnsFalse()
    {
        var obj = new JsonObject { [Vocabulary.Length] = null };
        obj.TryGetLength(out _).Should().BeFalse();
    }

    [Fact]
    public void GetLength_Exists()
    {
        var obj = new JsonObject { [Vocabulary.Length] = 5 };
        obj.GetLength().Should().Be(5);
    }

    [Fact]
    public void GetLength_Missing_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var act = () => obj.GetLength();
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region TryGetType / GetType
    [Fact]
    public void TryGetType_FromBasicTypeName_ReturnsTrue()
    {
        var obj = new JsonObject { [Vocabulary.Int] = 42 };
        obj.TryGetType(out var type).Should().BeTrue();
        type.Should().Be(typeof(int));
    }

    [Fact]
    public void TryGetType_FromTypeProperty_FullName_ReturnsTrue()
    {
        var obj = new JsonObject { [Vocabulary.Type] = "System.Int32" };
        obj.TryGetType(out var type).Should().BeTrue();
        type.Should().Be(typeof(int));
    }

    [Fact]
    public void TryGetType_FromTypeProperty_ShortName_ReturnsTrue()
    {
        var obj = new JsonObject { [Vocabulary.Type] = "int" };
        obj.TryGetType(out var type).Should().BeTrue();
        type.Should().Be(typeof(int));
    }

    [Fact]
    public void TryGetType_Missing_ReturnsFalse()
    {
        var obj = new JsonObject { ["other"] = 1 };
        obj.TryGetType(out _).Should().BeFalse();
    }

    [Fact]
    public void TryGetType_NullTypeProperty_ReturnsFalse()
    {
        var obj = new JsonObject { [Vocabulary.Type] = null };
        obj.TryGetType(out _).Should().BeFalse();
    }

    [Fact]
    public void TryGetType_NonStringType_ReturnsFalse()
    {
        var obj = new JsonObject { [Vocabulary.Type] = 42 };
        obj.TryGetType(out _).Should().BeFalse();
    }

    [Fact]
    public void GetType_Returns()
    {
        var obj = new JsonObject { [Vocabulary.Int] = 42 };
        obj.GetType(Vocabulary.Type).Should().Be(typeof(int));
    }

    [Fact]
    public void GetType_Missing_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var act = () => obj.GetType(Vocabulary.Type);
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region TryGetObject / GetObject
    [Fact]
    public void TryGetObject_Found()
    {
        var child = new JsonObject { ["inner"] = 1 };
        var obj = new JsonObject { ["child"] = child };
        obj.TryGetObject("child", out var result).Should().BeTrue();
        result.Should().BeSameAs(child);
    }

    [Fact]
    public void TryGetObject_NullValue_ReturnsTrue()
    {
        var obj = new JsonObject { ["child"] = null };
        obj.TryGetObject("child", out var result).Should().BeTrue();
        result.Should().BeNull();
    }

    [Fact]
    public void TryGetObject_NotObject_ReturnsFalse()
    {
        var obj = new JsonObject { ["child"] = 42 };
        obj.TryGetObject("child", out _).Should().BeFalse();
    }

    [Fact]
    public void TryGetObject_Missing_ReturnsFalse()
    {
        var obj = new JsonObject { ["other"] = 1 };
        obj.TryGetObject("child", out _).Should().BeFalse();
    }

    [Fact]
    public void GetObject_Returns()
    {
        var child = new JsonObject { ["inner"] = 1 };
        var obj = new JsonObject { ["child"] = child };
        obj.GetObject("child").Should().BeSameAs(child);
    }

    [Fact]
    public void GetObject_Missing_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var act = () => obj.GetObject("child");
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region TryGetArray / GetArray
    [Fact]
    public void TryGetArray_Found()
    {
        var arr = new JsonArray(1, 2, 3);
        var obj = new JsonObject { ["items"] = arr };
        obj.TryGetArray("items", out var result).Should().BeTrue();
        result!.Count.Should().Be(3);
    }

    [Fact]
    public void TryGetArray_NullValue_ReturnsTrue()
    {
        var obj = new JsonObject { ["items"] = null };
        obj.TryGetArray("items", out var result).Should().BeTrue();
        result.Should().BeNull();
    }

    [Fact]
    public void TryGetArray_NotArray_ReturnsFalse()
    {
        var obj = new JsonObject { ["items"] = 42 };
        obj.TryGetArray("items", out _).Should().BeFalse();
    }

    [Fact]
    public void TryGetArray_Missing_ReturnsFalse()
    {
        var obj = new JsonObject { ["other"] = 1 };
        obj.TryGetArray("items", out _).Should().BeFalse();
    }

    [Fact]
    public void GetArray_Returns()
    {
        var arr = new JsonArray(1, 2, 3);
        var obj = new JsonObject { ["items"] = arr };
        obj.GetArray("items").Count.Should().Be(3);
    }

    [Fact]
    public void GetArray_Missing_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var act = () => obj.GetArray("items");
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region TryGetFirstObject / GetFirstObject
    [Fact]
    public void TryGetFirstObject_Found()
    {
        var child = new JsonObject { ["inner"] = 1 };
        var obj = new JsonObject { ["first"] = child };
        obj.TryGetFirstObject(out var name, out var result).Should().BeTrue();
        name.Should().Be("first");
        result.Should().BeSameAs(child);
    }

    [Fact]
    public void TryGetFirstObject_NoObjects_ReturnsFalse()
    {
        var obj = new JsonObject { ["value"] = 42 };
        obj.TryGetFirstObject(out _, out _).Should().BeFalse();
    }

    [Fact]
    public void GetFirstObject_Returns()
    {
        var child = new JsonObject { ["inner"] = 1 };
        var obj = new JsonObject { ["first"] = child };
        var (name, result) = obj.GetFirstObject();
        name.Should().Be("first");
        result.Should().BeSameAs(child);
    }

    [Fact]
    public void GetFirstObject_NoObjects_Throws()
    {
        var obj = new JsonObject { ["value"] = 42 };
        var act = () => obj.GetFirstObject();
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region ThrowSerializationException
    [Fact]
    public void ThrowSerializationException_IncludesPath()
    {
        var obj = new JsonObject { ["child"] = new JsonObject() };
        var child = obj["child"]!;
        var act = () => child.ThrowSerializationException<int>("test error");
        act.Should().Throw<SerializationException>().WithMessage("test error*");
    }
    #endregion

    #region ToObject
    [Fact]
    public void ToObject_JsonObject_ReturnsIt()
    {
        var obj = new JsonObject();
        JsonNode node = obj;
        node.ToObject().Should().BeSameAs(obj);
    }

    [Fact]
    public void ToObject_NonObject_Throws()
    {
        JsonNode node = JsonValue.Create(42)!;
        var act = () => node.ToObject();
        act.Should().Throw<SerializationException>();
    }

    [Fact]
    public void ToObject_Null_Throws()
    {
        JsonNode? node = null;
        var act = () => node.ToObject();
        act.Should().Throw<SerializationException>();
    }

    [Fact]
    public void ToObject_CustomMessage()
    {
        JsonNode? node = null;
        var act = () => node.ToObject("custom message");
        act.Should().Throw<SerializationException>().WithMessage("custom message*");
    }
    #endregion
}
