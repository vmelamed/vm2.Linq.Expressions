namespace vm2.Linq.Expressions.Serialization.Json;

/// <summary>
/// struct JElement is a key-value pair, similar to <see cref="XElement"/>. The <paramref name="key"/> is the name of
/// the element in the parent JsonObject.
/// </summary>
/// <remarks>
/// The <see cref="KeyValuePair{TKey, TValue}"/> is a struct, so we cannot inherit
/// from it but we have implicit conversions to and from it and instances of this class can be used anywhere where
/// <see cref="KeyValuePair{TKey, TValue}"/> is required.
/// </remarks>
[DebuggerDisplay("{Name}: {Node}")]
public partial struct JElement(string key = "", JsonNode? value = null)
{
    #region Constructors
    /// <summary>
    /// Gets or sets the key of this JElement. When this JElement is added to a <see cref="JsonObject"/> or
    /// <see cref="JsonDocument"/>, the <see cref="Name"/> will become the name of a property in that parent.
    /// </summary>
    public string Name { get; set; } = key;

    /// <summary>
    /// Gets or sets the value of this JElement. When this JElement is added to a <see cref="JsonObject"/> or
    /// <see cref="JsonDocument"/>, the <see cref="Node"/> will become the value of the property with name
    /// <see cref="Name"/> in that parent.
    /// </summary>
    /// <element>The element.</element>
    public JsonNode? Node { get; set; } = value;

    /// <summary>
    /// Gets the value of the element as JsonObject or throws exception..
    /// </summary>
    public readonly JsonObject JsObject => Node?.AsObject() ?? throw new ExpressionJsonSerializationException($"The value of the element {Name} is not JsonObject.");

    /// <summary>
    /// Gets the value of the element as JsonArray or throws exception..
    /// </summary>
    public readonly JsonArray JsArray => Node?.AsArray() ?? throw new ExpressionJsonSerializationException($"The value of the element {Name} is not JsonArray.");

    /// <summary>
    /// Gets the value of the element as JsonValue or throws exception..
    /// </summary>
    public readonly JsonValue JsValue => Node?.AsValue() ?? throw new ExpressionJsonSerializationException($"The value of the element {Name} is not JsonValue.");

    /// <summary>
    /// Gets the kind of the json value.
    /// </summary>
    public readonly JsonValueKind JsonValueKind => Node?.GetValueKind() ?? JsonValueKind.Null;

    /// <summary>
    /// Initializes a new instance of the <see cref="JElement" /> struct. Disambiguates the <see cref="JsonArray"/>
    /// parameter as a <see cref="JsonNode"/>, instead of <see cref="IEnumerable{JsonNode}"/>.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="jArray">The j array.</param>
    public JElement(string key, JsonArray jArray)
        : this(key, (JsonNode)jArray) { }

    /// <summary>
    /// Initializes a new instance with a <paramref key="key"/> and a new <see cref="JsonObject"/> in the <see cref="Node"/>
    /// with the given set of <see cref="JElement"/>-s.
    /// </summary>
    /// <param key="key">The name of the property in the parent JSON object that will contain this <see cref="JElement"/>.</param>
    /// <param key="properties">
    /// A set of <see cref="JElement"/>-s to be added to the new <see cref="JsonObject"/> to be set in the <see cref="Node"/>.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If there are two or more properties with the same key in the <paramref name="properties"/>.
    /// </exception>
    public JElement(string key, params JElement?[] properties)
        : this(key, new JsonObject(properties.Where(p => p is not null).Select(p => (KeyValuePair<string, JsonNode?>)p!))) { }

    /// <summary>
    /// Initializes a new instance with a <paramref key="key"/> and a new <see cref="JsonObject"/> in the <see cref="Node"/>
    /// with the given set of <see cref="JElement"/>-s.
    /// </summary>
    /// <param key="key">The name of the property in the parent JSON object that will contain this <see cref="JElement"/>.</param>
    /// <param key="properties">
    /// A set of <see cref="JElement"/>-s to be added to the new <see cref="JsonObject"/> to be set in the <see cref="Node"/>.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If there are two or more properties with the same key in the <paramref name="properties"/>.
    /// </exception>
    public JElement(string key, IEnumerable<JElement?> properties)
        : this(key, new JsonObject(properties.Where(p => p is not null).Select(p => (KeyValuePair<string, JsonNode?>)p!))) { }

    /// <summary>
    /// Initializes a new instance with a <paramref key="key"/> and a new <see cref="JsonObject"/> in the <see cref="Node"/>
    /// with the given set of <see cref="JElement"/>-s.
    /// </summary>
    /// <param key="key">The name of the property in the parent JSON object that will contain this <see cref="JElement"/>.</param>
    /// <param key="properties">
    /// A set of <see cref="JElement"/>-s to be added to the new <see cref="JsonObject"/> to be set in the <see cref="Node"/>.
    /// </param>
    /// <exception cref="ArgumentException">
    /// If there are two or more properties with the same key in the <paramref name="properties"/>.
    /// </exception>
    public JElement(string key, IEnumerable<JElement> properties)
        : this(key, new JsonObject(properties.Select(p => (KeyValuePair<string, JsonNode?>)p))) { }

    /// <summary>
    /// Initializes a new instance with a <paramref key="key"/> and a new <see cref="JsonArray"/> in the <see cref="Node"/>
    /// with the given set of <see cref="JsonNode"/>-s.
    /// </summary>
    /// <param key="key">The name of the property in the parent JSON object that will contain this <see cref="JElement"/>.</param>
    /// <param key="elements">
    /// A set of <see cref="JsonNode"/>-s to be added to the new <see cref="JsonArray"/> that will be set in the <see cref="Node"/>.
    /// </param>
    public JElement(string key, IEnumerable<JsonNode?> elements)
        : this(key, (JsonNode)new JsonArray([.. elements])) { }

    /// <summary>
    /// Initializes a new instance with a <paramref key="key"/> and a new <see cref="JsonArray"/> in the <see cref="Node"/>
    /// with the given set of <see cref="JsonNode"/>-s.
    /// </summary>
    /// <param key="key">The name of the property in the parent JSON object that will contain this <see cref="JElement"/>.</param>
    /// <param key="elements">
    /// A set of <see cref="JsonNode"/>-s to be added to the new <see cref="JsonArray"/> that will be set in the <see cref="Node"/>.
    /// </param>
    public JElement(string key, params JsonNode?[] elements)
        : this(key, (JsonNode)new JsonArray(elements)) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JElement" /> <see langword="struct"/> from a <see cref="KeyValuePair{TKey, TValue}"/>
    /// </summary>
    /// <param name="kvp">The key-value pair.</param>
    public JElement(KeyValuePair<string, JsonNode?> kvp)
            : this(kvp.Key, kvp.Value) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="JElement" /> <see langword="struct"/> from a <see cref="ValueTuple{TKey, TValue}"/>
    /// </summary>
    /// <param name="tuple">The tuple.</param>
    public JElement((string, JsonNode?) tuple)
            : this(tuple.Item1, tuple.Item2) { }
    #endregion

    /// <summary>
    /// Deeply clones this element.
    /// </summary>
    /// <returns>vm2.Linq.Expressions.Serialization.Json.JElement.</returns>
    public readonly JElement DeepClone()
            => new(Name, Node?.DeepClone());

    /// <summary>
    /// Performs an implicit conversion from <see cref="JElement"/> to <see cref="KeyValuePair{String, JsonNode}"/>.
    /// </summary>
    /// <param key="je">The je.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator KeyValuePair<string, JsonNode?>(JElement je) => new(je.Name, je.Node);

    /// <summary>
    /// Performs an implicit conversion <see cref="KeyValuePair{String, JsonNode}" /> from to <see cref="JElement" />.
    /// </summary>
    /// <param key="kvp">The KVP.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator JElement(KeyValuePair<string, JsonNode?> kvp) => new(kvp.Key, kvp.Value);

    /// <summary>
    /// Performs an implicit conversion from <see cref="JElement"/> to <see cref="ValueTuple{String, JsonNode}"/>.
    /// </summary>
    /// <param key="je">The je.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator ValueTuple<string, JsonNode?>(JElement je) => new(je.Name, je.Node);

    /// <summary>
    /// Performs an implicit conversion from <see cref="ValueTuple{String, JsonNode}" /> to <see cref="JElement" />.
    /// </summary>
    /// <param key="kvp">The KVP.</param>
    /// <returns>The result of the conversion.</returns>
    public static implicit operator JElement(ValueTuple<string, JsonNode?> kvp) => new(kvp.Item1, kvp.Item2);

    /// <summary>
    /// Deconstructs to the specified key and element.
    /// </summary>
    /// <param key="key">The key.</param>
    /// <param key="element">The element.</param>
    public readonly void Deconstruct(out string key, out JsonNode? value)
    {
        key = Name;
        value = Node;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="string" /> that represents this instance.</returns>
    public override readonly string ToString() => $"[{Name}, {Node}]";

    /// <summary>
    /// Throws a (de)serialization exception.
    /// </summary>
    /// <param name="message">The exception message will be appended with &quot; -- &apos;&lt;the element path&gt;&apos;.&quot;.</param>
    /// <exception cref="SerializationException"></exception>
    public readonly T ThrowSerializationException<T>(string message = "Invalid JSON")
        => throw new SerializationException($"{message} -- at '{GetPath()}'.");
}
