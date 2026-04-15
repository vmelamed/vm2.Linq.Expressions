namespace vm2.Linq.Expressions.Serialization.Json.Tests;

public class JElementTests
{
    #region Construction and properties
    [Fact]
    public void DefaultConstructor_SetsNullNameNullNode()
    {
        var je = new JElement();

        je.Name.Should().BeNull();
        je.Node.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithKeyAndValue()
    {
        JsonNode value = JsonValue.Create(42)!;
        var je = new JElement("key", value);

        je.Name.Should().Be("key");
        je.Node.Should().BeSameAs(value);
    }

    [Fact]
    public void Constructor_WithKeyAndJsonArray()
    {
        var array = new JsonArray(1, 2, 3);
        var je = new JElement("arr", array);

        je.Name.Should().Be("arr");
        je.Node.Should().BeOfType<JsonArray>();
    }

    [Fact]
    public void Constructor_WithKeyAndJElementParams()
    {
        var child1 = new JElement("a", JsonValue.Create(1));
        var child2 = new JElement("b", JsonValue.Create(2));
        var je = new JElement("parent", child1, child2);

        je.Name.Should().Be("parent");
        je.Node.Should().BeOfType<JsonObject>();
        var obj = je.Node!.AsObject();
        obj["a"]!.GetValue<int>().Should().Be(1);
        obj["b"]!.GetValue<int>().Should().Be(2);
    }

    [Fact]
    public void Constructor_WithKeyAndJElementEnumerable()
    {
        IEnumerable<JElement?> children = [
            new JElement("x", JsonValue.Create("hello")),
            null,
            new JElement("y", JsonValue.Create("world")),
        ];
        var je = new JElement("parent", children);

        je.Node.Should().BeOfType<JsonObject>();
        var obj = je.Node!.AsObject();
        obj.Count.Should().Be(2);
    }

    [Fact]
    public void Constructor_WithKeyAndNonNullableJElementEnumerable()
    {
        IEnumerable<JElement> children = [
            new JElement("a", JsonValue.Create(1)),
            new JElement("b", JsonValue.Create(2)),
        ];
        var je = new JElement("parent", children);

        je.Node.Should().BeOfType<JsonObject>();
        je.Node!.AsObject().Count.Should().Be(2);
    }

    [Fact]
    public void Constructor_WithKeyAndJsonNodeEnumerable()
    {
        IEnumerable<JsonNode?> nodes = [JsonValue.Create(1), JsonValue.Create(2), null];
        var je = new JElement("arr", nodes);

        je.Node.Should().BeOfType<JsonArray>();
        je.Node!.AsArray().Count.Should().Be(3);
    }

    [Fact]
    public void Constructor_WithKeyAndJsonNodeParams()
    {
        var je = new JElement("arr", JsonValue.Create(1), JsonValue.Create(2));

        je.Node.Should().BeOfType<JsonArray>();
        je.Node!.AsArray().Count.Should().Be(2);
    }

    [Fact]
    public void Constructor_WithKeyValuePair()
    {
        var kvp = new KeyValuePair<string, JsonNode?>("test", JsonValue.Create(42));
        var je = new JElement(kvp);

        je.Name.Should().Be("test");
        je.Node!.GetValue<int>().Should().Be(42);
    }

    [Fact]
    public void Constructor_WithTuple()
    {
        var je = new JElement(("test", JsonValue.Create(42)));

        je.Name.Should().Be("test");
        je.Node!.GetValue<int>().Should().Be(42);
    }
    #endregion

    #region Accessors
    [Fact]
    public void JsObject_ReturnsJsonObject()
    {
        var je = new JElement("obj", new JsonObject());
        je.JsObject.Should().BeOfType<JsonObject>();
    }

    [Fact]
    public void JsObject_ThrowsWhenNull()
    {
        var je = new JElement("obj");
        var act = () => je.JsObject;
        act.Should().Throw<ExpressionJsonSerializationException>();
    }

    [Fact]
    public void JsArray_ReturnsJsonArray()
    {
        var array = new JsonArray(1, 2, 3);
        var je = new JElement("arr", array);
        je.JsArray.Count.Should().Be(3);
    }

    [Fact]
    public void JsArray_ThrowsWhenNull()
    {
        var je = new JElement("arr");
        var act = () => je.JsArray;
        act.Should().Throw<ExpressionJsonSerializationException>();
    }

    [Fact]
    public void JsValue_ReturnsJsonValue()
    {
        var je = new JElement("val", JsonValue.Create(42));
        je.JsValue.Should().BeAssignableTo<JsonValue>();
    }

    [Fact]
    public void JsValue_ThrowsWhenNull()
    {
        var je = new JElement("val");
        var act = () => je.JsValue;
        act.Should().Throw<ExpressionJsonSerializationException>();
    }

    [Fact]
    public void JsonValueKind_ReturnsCorrectKind()
    {
        new JElement("n", JsonValue.Create(42)).JsonValueKind.Should().Be(JsonValueKind.Number);
        new JElement("s", JsonValue.Create("hi")).JsonValueKind.Should().Be(JsonValueKind.String);
        new JElement("null").JsonValueKind.Should().Be(JsonValueKind.Null);
    }
    #endregion

    #region Implicit conversions
    [Fact]
    public void ImplicitConversion_ToKeyValuePair()
    {
        var je = new JElement("key", JsonValue.Create(1));
        KeyValuePair<string, JsonNode?> kvp = je;

        kvp.Key.Should().Be("key");
        kvp.Value!.GetValue<int>().Should().Be(1);
    }

    [Fact]
    public void ImplicitConversion_FromKeyValuePair()
    {
        var kvp = new KeyValuePair<string, JsonNode?>("key", JsonValue.Create(1));
        JElement je = kvp;

        je.Name.Should().Be("key");
        je.Node!.GetValue<int>().Should().Be(1);
    }

    [Fact]
    public void ImplicitConversion_ToTuple()
    {
        var je = new JElement("key", JsonValue.Create(1));
        ValueTuple<string, JsonNode?> tuple = je;

        tuple.Item1.Should().Be("key");
        tuple.Item2!.GetValue<int>().Should().Be(1);
    }

    [Fact]
    public void ImplicitConversion_FromTuple()
    {
        JElement je = ("key", JsonValue.Create(1));

        je.Name.Should().Be("key");
        je.Node!.GetValue<int>().Should().Be(1);
    }
    #endregion

    #region Deconstruct and ToString
    [Fact]
    public void Deconstruct()
    {
        var je = new JElement("key", JsonValue.Create(42));
        var (k, v) = je;

        k.Should().Be("key");
        v!.GetValue<int>().Should().Be(42);
    }

    [Fact]
    public void ToString_ReturnsExpectedFormat()
    {
        var je = new JElement("key", JsonValue.Create(42));
        je.ToString().Should().Be("[key, 42]");
    }
    #endregion

    #region DeepClone
    [Fact]
    public void DeepClone_CreatesIndependentCopy()
    {
        var obj = new JsonObject { ["x"] = 1 };
        var je = new JElement("key", obj);
        var clone = je.DeepClone();

        clone.Name.Should().Be("key");
        obj["x"] = 999;
        clone.Node!.AsObject()["x"]!.GetValue<int>().Should().Be(1);
    }
    #endregion

    #region ThrowSerializationException
    [Fact]
    public void ThrowSerializationException_ThrowsWithPath()
    {
        var obj = new JsonObject { ["child"] = new JsonObject() };
        var je = new JElement("root", obj);

        var act = () => je.ThrowSerializationException<int>("test error");
        act.Should().Throw<SerializationException>().WithMessage("*test error*");
    }
    #endregion

    #region GetValueKind and GetPath
    [Fact]
    public void GetValueKind_NullNode_ReturnsUndefined()
    {
        var je = new JElement("test");
        je.GetValueKind().Should().Be(JsonValueKind.Undefined);
    }

    [Fact]
    public void GetValueKind_WithNode_ReturnsKind()
    {
        var je = new JElement("test", JsonValue.Create(42));
        je.GetValueKind().Should().Be(JsonValueKind.Number);
    }

    [Fact]
    public void GetPath_NullNode_ReturnsName()
    {
        var je = new JElement("myName");
        je.GetPath().Should().Be("myName");
    }
    #endregion

    #region IsNil
    [Fact]
    public void IsNil_NullNode_ReturnsTrue()
    {
        new JElement("x").IsNil().Should().BeTrue();
    }

    [Fact]
    public void IsNil_JsonObjectWithNullValue_ReturnsTrue()
    {
        var obj = new JsonObject { [Vocabulary.Value] = null };
        new JElement("x", obj).IsNil().Should().BeTrue();
    }

    [Fact]
    public void IsNil_NullNodeValue_ReturnsTrue()
    {
        new JElement("x").IsNil().Should().BeTrue();
    }

    [Fact]
    public void IsNil_JsonArray_ReturnsFalse()
    {
        new JElement("x", new JsonArray()).IsNil().Should().BeFalse();
    }

    [Fact]
    public void IsNil_NonNullValue_ReturnsFalse()
    {
        new JElement("x", JsonValue.Create(42)).IsNil().Should().BeFalse();
    }
    #endregion

    #region TryGetValue / GetValue
    [Fact]
    public void TryGetValue_NullNode_ReturnsTrueDefaultValue()
    {
        var je = new JElement("x");
        je.TryGetValue<int>(out var value).Should().BeTrue();
        value.Should().Be(default);
    }

    [Fact]
    public void TryGetValue_JsonValue_ReturnsTrue()
    {
        var je = new JElement("x", JsonValue.Create(42));
        je.TryGetValue<int>(out var value).Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void TryGetValue_JsonObject_ReturnsFalse()
    {
        var je = new JElement("x", new JsonObject());
        je.TryGetValue<int>(out _).Should().BeFalse();
    }

    [Fact]
    public void GetValue_ReturnsValue()
    {
        var je = new JElement("x", JsonValue.Create("hello"));
        je.GetValue<string>().Should().Be("hello");
    }
    #endregion

    #region TryGetPropertyValue / GetPropertyValue
    [Fact]
    public void TryGetPropertyValue_WithJsonObject_ReturnsTrue()
    {
        var obj = new JsonObject { ["value"] = 42 };
        var je = new JElement("x", obj);
        je.TryGetPropertyValue(out var node).Should().BeTrue();
        node!.GetValue<int>().Should().Be(42);
    }

    [Fact]
    public void TryGetPropertyValue_NonJsonObject_ReturnsFalse()
    {
        var je = new JElement("x", JsonValue.Create(42));
        je.TryGetPropertyValue(out _).Should().BeFalse();
    }

    [Fact]
    public void GetPropertyValue_ReturnsNode()
    {
        var obj = new JsonObject { ["value"] = "test" };
        var je = new JElement("x", obj);
        je.GetPropertyValue()!.GetValue<string>().Should().Be("test");
    }

    [Fact]
    public void GetPropertyValue_MissingProperty_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var je = new JElement("x", obj);
        var act = () => je.GetPropertyValue("missing");
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region Typed TryGetPropertyValue / GetPropertyValue<T>
    [Fact]
    public void TryGetPropertyValue_Typed_ReturnsCorrectValue()
    {
        var obj = new JsonObject { ["value"] = 42 };
        var je = new JElement("x", obj);
        je.TryGetPropertyValue<int>(out var val).Should().BeTrue();
        val.Should().Be(42);
    }

    [Fact]
    public void TryGetPropertyValue_Enum_ParsesByName()
    {
        var obj = new JsonObject { ["value"] = "Add" };
        var je = new JElement("x", obj);
        je.TryGetPropertyValue<ExpressionType>(out var val).Should().BeTrue();
        val.Should().Be(ExpressionType.Add);
    }

    [Fact]
    public void TryGetPropertyValue_Typed_NonJsonObject_ReturnsFalse()
    {
        var je = new JElement("x", JsonValue.Create(42));
        je.TryGetPropertyValue<int>(out _).Should().BeFalse();
    }

    [Fact]
    public void TryGetPropertyValue_Typed_PropertyNotJsonValue_ReturnsFalse()
    {
        var obj = new JsonObject { ["value"] = new JsonObject() };
        var je = new JElement("x", obj);
        je.TryGetPropertyValue<int>(out _).Should().BeFalse();
    }

    [Fact]
    public void GetPropertyValue_Typed()
    {
        var obj = new JsonObject { ["name"] = "test" };
        var je = new JElement("x", obj);
        je.GetPropertyValue<string>("name").Should().Be("test");
    }
    #endregion

    #region TryGetName / GetName / TryGetId / GetId
    [Fact]
    public void TryGetName_ReturnsName()
    {
        var obj = new JsonObject { ["name"] = "myName" };
        var je = new JElement("x", obj);
        je.TryGetName(out var name).Should().BeTrue();
        name.Should().Be("myName");
    }

    [Fact]
    public void GetName_ReturnsName()
    {
        var obj = new JsonObject { ["name"] = "myName" };
        var je = new JElement("x", obj);
        je.GetName().Should().Be("myName");
    }

    [Fact]
    public void TryGetId_ReturnsId()
    {
        var obj = new JsonObject { ["id"] = "myId" };
        var je = new JElement("x", obj);
        je.TryGetId(out var id).Should().BeTrue();
        id.Should().Be("myId");
    }

    [Fact]
    public void GetId_ReturnsId()
    {
        var obj = new JsonObject { ["id"] = "myId" };
        var je = new JElement("x", obj);
        je.GetId().Should().Be("myId");
    }
    #endregion

    #region TryGetLength / GetLength
    [Fact]
    public void TryGetLength_ReturnsLength()
    {
        var obj = new JsonObject { ["length"] = 5 };
        var je = new JElement("x", obj);
        je.TryGetLength(out var len).Should().BeTrue();
        len.Should().Be(5);
    }

    [Fact]
    public void GetLength_ReturnsLength()
    {
        var obj = new JsonObject { ["length"] = 5 };
        var je = new JElement("x", obj);
        je.GetLength().Should().Be(5);
    }
    #endregion

    #region TryGetExpressionType / GetExpressionType
    [Fact]
    public void TryGetExpressionType_ValidName_ReturnsTrue()
    {
        var je = new JElement("Add", JsonValue.Create(0));
        je.TryGetExpressionType(out var et).Should().BeTrue();
        et.Should().Be(ExpressionType.Add);
    }

    [Fact]
    public void TryGetExpressionType_InvalidName_ReturnsFalse()
    {
        var je = new JElement("notAnExpressionType", JsonValue.Create(0));
        je.TryGetExpressionType(out _).Should().BeFalse();
    }

    [Fact]
    public void GetExpressionType_ValidName_Returns()
    {
        var je = new JElement("Add", JsonValue.Create(0));
        je.GetExpressionType().Should().Be(ExpressionType.Add);
    }

    [Fact]
    public void GetExpressionType_InvalidName_Throws()
    {
        var je = new JElement("invalid", JsonValue.Create(0));
        var act = () => je.GetExpressionType();
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region TryGetType / GetType / TryGetTypeFromProperty / GetTypeFromProperty
    [Fact]
    public void TryGetType_FromName_ReturnsTrue()
    {
        var je = new JElement("int", JsonValue.Create(0));
        je.TryGetType(out var type).Should().BeTrue();
        type.Should().Be(typeof(int));
    }

    [Fact]
    public void TryGetTypeFromProperty_ReturnsTrue()
    {
        var obj = new JsonObject { ["type"] = "System.Int32" };
        var je = new JElement("x", obj);
        je.TryGetTypeFromProperty(out var type).Should().BeTrue();
        type.Should().Be(typeof(int));
    }

    [Fact]
    public void TryGetTypeFromProperty_NullNode_ReturnsFalse()
    {
        var je = new JElement("x");
        je.TryGetTypeFromProperty(out _).Should().BeFalse();
    }

    [Fact]
    public void GetTypeFromProperty_ReturnsType()
    {
        var obj = new JsonObject { ["type"] = "System.String" };
        var je = new JElement("x", obj);
        je.GetTypeFromProperty().Should().Be(typeof(string));
    }

    [Fact]
    public void GetTypeFromProperty_Missing_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var je = new JElement("x", obj);
        var act = () => je.GetTypeFromProperty();
        act.Should().Throw<SerializationException>();
    }

    [Fact]
    public void GetType_FallsThrough_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var je = new JElement("unknownType", obj);
        var act = () => je.GetType();
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region TryGetOneOf / GetOneOf
    [Fact]
    public void TryGetOneOf_FindsExisting()
    {
        var obj = new JsonObject { ["add"] = new JsonObject(), ["other"] = 1 };
        var je = new JElement("x", obj);
        je.TryGetOneOf(["add", "subtract"], out var element).Should().BeTrue();
        element!.Value.Name.Should().Be("add");
    }

    [Fact]
    public void TryGetOneOf_NoneFound_ReturnsFalse()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var je = new JElement("x", obj);
        je.TryGetOneOf(["add", "subtract"], out _).Should().BeFalse();
    }

    [Fact]
    public void TryGetOneOf_NullNode_ReturnsFalse()
    {
        var je = new JElement("x");
        je.TryGetOneOf(["add"], out _).Should().BeFalse();
    }

    [Fact]
    public void GetOneOf_Returns()
    {
        var obj = new JsonObject { ["add"] = new JsonObject() };
        var je = new JElement("x", obj);
        var result = je.GetOneOf(["add", "subtract"]);
        result.Name.Should().Be("add");
    }

    [Fact]
    public void GetOneOf_NoneFound_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var je = new JElement("x", obj);
        var act = () => je.GetOneOf(["add"]);
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region TryGetElement / GetElement
    [Fact]
    public void TryGetElement_FindsChild()
    {
        var child = new JsonObject { ["inner"] = 1 };
        var obj = new JsonObject { ["value"] = child };
        var je = new JElement("x", obj);
        je.TryGetElement(out var element).Should().BeTrue();
        element!.Value.Name.Should().Be("value");
    }

    [Fact]
    public void TryGetElement_NonJsonObject_ReturnsFalse()
    {
        var je = new JElement("x", JsonValue.Create(42));
        je.TryGetElement(out _).Should().BeFalse();
    }

    [Fact]
    public void GetElement_ReturnsChild()
    {
        var child = new JsonObject { ["inner"] = 1 };
        var obj = new JsonObject { ["value"] = child };
        var je = new JElement("x", obj);
        je.GetElement().Name.Should().Be("value");
    }

    [Fact]
    public void GetElement_Missing_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var je = new JElement("x", obj);
        var act = () => je.GetElement("missing");
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region TryGetArray / GetArray
    [Fact]
    public void TryGetArray_FindsArray()
    {
        var arr = new JsonArray(1, 2, 3);
        var obj = new JsonObject { ["value"] = arr };
        var je = new JElement("x", obj);
        je.TryGetArray(out var array).Should().BeTrue();
        array!.Count.Should().Be(3);
    }

    [Fact]
    public void TryGetArray_NotArray_ReturnsFalse()
    {
        var obj = new JsonObject { ["value"] = "notArray" };
        var je = new JElement("x", obj);
        je.TryGetArray(out _).Should().BeFalse();
    }

    [Fact]
    public void TryGetArray_NonJsonObject_ReturnsFalse()
    {
        var je = new JElement("x", JsonValue.Create(42));
        je.TryGetArray(out _).Should().BeFalse();
    }

    [Fact]
    public void GetArray_ReturnsArray()
    {
        var arr = new JsonArray(1, 2, 3);
        var obj = new JsonObject { ["value"] = arr };
        var je = new JElement("x", obj);
        je.GetArray().Count.Should().Be(3);
    }

    [Fact]
    public void GetArray_Missing_Throws()
    {
        var obj = new JsonObject { ["other"] = 1 };
        var je = new JElement("x", obj);
        var act = () => je.GetArray("missing");
        act.Should().Throw<SerializationException>();
    }
    #endregion

    #region TryGetFirstElement / GetFirstElement
    [Fact]
    public void TryGetFirstElement_FindsFirst()
    {
        var obj = new JsonObject { ["child"] = new JsonObject { ["inner"] = 1 } };
        var je = new JElement("x", obj);
        je.TryGetFirstElement(out var element).Should().BeTrue();
        element!.Value.Name.Should().Be("child");
    }

    [Fact]
    public void TryGetFirstElement_NonObject_ReturnsFalse()
    {
        var je = new JElement("x", JsonValue.Create(42));
        je.TryGetFirstElement(out _).Should().BeFalse();
    }

    [Fact]
    public void GetFirstElement_Returns()
    {
        var obj = new JsonObject { ["child"] = new JsonObject() };
        var je = new JElement("x", obj);
        je.GetFirstElement().Name.Should().Be("child");
    }
    #endregion
}
