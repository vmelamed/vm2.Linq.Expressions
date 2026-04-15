namespace vm2.Linq.Expressions.Serialization.Json;

public partial struct JElement
{
    /// <summary>
    /// Gets the JSON kind of the name in <see cref="Node"/>.
    /// </summary>
    /// <returns>JsonValueKind.</returns>
    public readonly JsonValueKind GetValueKind()
        => Node?.GetValueKind() ?? JsonValueKind.Undefined;

    /// <summary>
    /// Gets the JSON kind of the name in <see cref="Node"/>.
    /// </summary>
    /// <returns>JsonValueKind.</returns>
    public readonly string GetPath()
        => Node?.GetPath() ?? Name;

    /// <summary>
    /// Determines whether this element's <see cref="Node"/> is <see langword="null"/>, or
    /// if it has a property called 'name' and its name is <see langword="null"/>.
    /// </summary>
    /// <returns><c>true</c> if this instance represents a nil element; otherwise, <c>false</c>.</returns>
    public readonly bool IsNil()
        => Node switch {
            null => true,
            JsonObject jsObj => jsObj.IsNil(),
            JsonValue jsVal => jsVal.GetValueKind() == JsonValueKind.Null,
            _ => false,
        };

    /// <summary>
    /// Tries to convert the <see cref="Node"/> of this element to a simple (non-object or array) name.
    /// </summary>
    /// <typeparam name="T">The type of the name.</typeparam>
    /// <param name="value">The name.</param>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetValue<T>(out T? value)
    {
        value = default;
        return Node switch {
            null => true,
            JsonValue jsVal => jsVal.TryGetValue(out value),
            _ => false,
        };
    }

    /// <summary>
    /// Converts the <see cref="Node"/> of this element to a simple (non-object or array) name.
    /// </summary>
    /// <returns><see cref="JsonNode"/>?</returns>
    public readonly T? GetValue<T>()
        => TryGetValue<T>(out var value)
                ? value
                : ThrowSerializationException<T?>($"Could not get the value of the element");

    /// <summary>
    /// Tries to get the name of the property with name <paramref name="propertyValueName"/>.
    /// </summary>
    /// <param name="node">The property name.</param>
    /// <param name="propertyValueName">Name of the property.</param>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetPropertyValue(
        out JsonNode? node,
        string propertyValueName = Vocabulary.Value)
    {
        node = default;
        return Node is JsonObject jsObj && jsObj.TryGetValue(out node, propertyValueName) is true;
    }

    /// <summary>
    /// Gets the name of property <paramref name="propertyValueName"/>.
    /// </summary>
    /// <param name="propertyValueName">Name of the property.</param>
    /// <returns><see cref="JsonNode"/>?</returns>
    public readonly JsonNode? GetPropertyValue(
        string propertyValueName = Vocabulary.Value)
        => TryGetPropertyValue(out var node, propertyValueName)
            ? node
            : ThrowSerializationException<JsonNode?>($"Could not get the value of a property '{propertyValueName}'");

    /// <summary>
    /// Tries to get the strongly typed name of the property with name <paramref name="propertyValueName"/>.
    /// </summary>
    /// <param name="value">The property name.</param>
    /// <param name="propertyValueName">Name of the property.</param>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetPropertyValue<T>(
        out T? value,
        string propertyValueName = Vocabulary.Value)
    {
        value = default;

        if (Node is not JsonObject jsObj
            || jsObj.TryGetValue(out var node, propertyValueName) is false
            || node is not JsonValue jsVal)
            return false;

        if (typeof(T).IsEnum
            && jsVal.TryGetValue<string>(out var str) is true
            && Enum.TryParse(typeof(T), str, true, out var obj))
        {
            value = (T)obj;
            return true;
        }

        return jsVal.TryGetValue<T>(out value);
    }

    /// <summary>
    /// Gets the strongly typed name of the property with name <paramref name="propertyValueName"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="propertyValueName">Name of the property.</param>
    /// <returns>The name of the property.</returns>
    public readonly T GetPropertyValue<T>(
        string propertyValueName = Vocabulary.Value)
        => TryGetPropertyValue<T>(out var value, propertyValueName)
            && value is not null
                ? value
                : ThrowSerializationException<T>($"Could not get '{nameof(T)}' value of property '{propertyValueName}'");

    /// <summary>
    /// Tries to get the strongly typed name of the property with name <paramref name="propertyNameName"/>.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <param name="propertyNameName">Name of the property Name.</param>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetName(
        out string? name,
        string propertyNameName = Vocabulary.Name)
        => TryGetPropertyValue(out name, propertyNameName);

    /// <summary>
    /// Gets the strongly typed name of the property with name <paramref name="propertyNameName"/>.
    /// </summary>
    /// <param name="propertyNameName">Name of the property.</param>
    /// <returns>The value of the property.</returns>
    public readonly string GetName(
        string propertyNameName = Vocabulary.Name)
        => GetPropertyValue<string>(propertyNameName);

    /// <summary>
    /// Tries to get the strongly typed id of the property with id <paramref id="propertyIdName"/>.
    /// </summary>
    /// <param id="id">The property id.</param>
    /// <param id="propertyIdName">Name of the property Id.</param>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetId(
        out string? id,
        string propertyIdName = Vocabulary.Id)
        => TryGetPropertyValue(out id, propertyIdName);

    /// <summary>
    /// Gets the strongly typed id of the property with id <paramref id="propertyIdName"/>.
    /// </summary>
    /// <param id="propertyIdName">Id of the property.</param>
    /// <returns>The value of the property.</returns>
    public readonly string GetId(
        string propertyIdName = Vocabulary.Id)
        => GetPropertyValue<string>(propertyIdName);

    /// <summary>
    /// Tries to get the integer name from a property <paramref name="propertyLengthName"/> representing the length of the object.
    /// </summary>
    /// <param name="length">The length.</param>
    /// <param name="propertyLengthName">Name of the property length.</param>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetLength(
        out int length,
        string propertyLengthName = Vocabulary.Length)
        => TryGetPropertyValue(out length, propertyLengthName);

    /// <summary>
    /// Gets the length of the sub-elements in the element from element <see cref="Vocabulary.Length"/>
    /// </summary>
    /// <param name="propertyLengthName">The name of the property containing the length.</param>
    /// <returns>The <see cref="int"/> length.</returns>
    public readonly int GetLength(string propertyLengthName = Vocabulary.Length)
        => GetPropertyValue<int>(propertyLengthName);

    /// <summary>
    /// Tries to translate this element's name to the enum <see cref="ExpressionType" />.
    /// </summary>
    /// <param name="expressionType">Type of the expression.</param>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetExpressionType(out ExpressionType expressionType)
        => Enum.TryParse(Name, true, out expressionType);

    /// <summary>
    /// Translates this element's name to the enum ExpressionType.
    /// </summary>
    /// <returns>The <see cref="ExpressionType"/> represented by the element.</returns>
    public readonly ExpressionType GetExpressionType()
        => TryGetExpressionType(out var expressionType)
            ? expressionType
            : ThrowSerializationException<ExpressionType>($"The name of the element '{Name}' is not from the 'enum ExpressionType'");

    /// <summary>
    /// Tries to get the .NET type of this element from its <see cref="Node"/>'s property <paramref name="propertyTypeName"/>.
    /// </summary>
    /// <param name="type">The element's type.</param>
    /// <param name="propertyTypeName">The name of the property representing the type.</param>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetTypeFromProperty(
        out Type? type,
        string propertyTypeName = Vocabulary.Type)
    {
        type = default;
        return Node is JsonObject jsObj
                && jsObj.TryGetType(out type, propertyTypeName);
    }

    /// <summary>
    /// Gets the .NET type of the element only from its property (default "type"). If not found - throws Exception.
    /// </summary>
    /// <param name="propertyTypeName">The name of the property representing the type.</param>
    /// <returns><see cref="Type"/>.</returns>
    public readonly Type GetTypeFromProperty(string propertyTypeName = Vocabulary.Type)
        => TryGetTypeFromProperty(out var type, propertyTypeName) && type is not null
                ? type
                : ThrowSerializationException<Type>($"Could not get the .NET type from property '{propertyTypeName}'");

    /// <summary>
    /// Tries to get the .NET type of the element
    /// <list type="number">
    /// <item>from the name of the element or</item>
    /// <item>from its attribute "type".</item>
    /// </list>
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="propertyTypeName">The name of the property representing the type.</param>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetType(
        out Type? type,
        string propertyTypeName = Vocabulary.Type)
    {
        type = default;
        return Vocabulary.NamesToTypes.TryGetValue(Name, out type)
                || TryGetTypeFromProperty(out type, propertyTypeName);
    }

    /// <summary>
    /// Tries to get the .NET type of the element
    /// <list type="number">
    /// <item>from the name of the element or</item>
    /// <item>from its attribute "type".</item>
    /// </list>
    /// </summary>
    /// <param name="propertyTypeName">The name of the property representing the type.</param>
    /// <returns><see cref="Type"/>.</returns>
    public readonly Type GetType(string propertyTypeName = Vocabulary.Type)
        => TryGetType(out var type, propertyTypeName) && type is not null
                ? type
                : ThrowSerializationException<Type>($"Could not get the .NET type from an element - neither from the its name '{Name}' nor from its property '{propertyTypeName}'");

    /// <summary>
    /// Tries to construct a JElement from the name and the name of a property with one of the names in <paramref name="names"/>.
    /// </summary>
    /// <param name="names">The names of properties to search for.</param>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetOneOf(
        IEnumerable<string> names,
        out JElement? element)
    {
        if (Node is JsonObject jsObj
            && jsObj.TryGetOneOf(names, out var propertyName, out var node) is true
            && propertyName is not null)
        {
            element = new(propertyName, node);
            return true;
        }

        element = default;
        return false;
    }

    /// <summary>
    /// Constructs a JElement from the name and the name of a property with one of the names in <paramref name="names"/>.
    /// </summary>
    /// <param name="names">The names.</param>
    /// <returns>JElement.</returns>
    public readonly JElement GetOneOf(IEnumerable<string> names)
        => TryGetOneOf(names, out var element) && element is not null
                ? element.Value
                : ThrowSerializationException<JElement>($"Could not find a property with any of the names '{string.Join("', '", names)}'");

    /// <summary>
    /// Tries to construct a <see cref="JElement" /> from this <see cref="Node"/>'s property <paramref name="childPropertyName" /> and
    /// its JsonObject name.
    /// </summary>
    /// <param name="childPropertyName">Name of the element property.</param>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetElement(
        out JElement? element,
        string childPropertyName = Vocabulary.Value)
    {
        if (Node is JsonObject jsObj
            && jsObj.TryGetObject(childPropertyName, out var obj))
        {
            element = new(childPropertyName, obj);
            return true;
        }

        element = default;
        return false;
    }

    /// <summary>
    /// Constructs a <see cref="JElement" /> from this <see cref="Node"/>'s property <paramref name="childPropertyName" /> and
    /// its JsonObject name.
    /// </summary>
    /// <param name="childPropertyName">Name of the element property.</param>
    public readonly JElement GetElement(string childPropertyName = Vocabulary.Value)
        => TryGetElement(out var element, childPropertyName)
            && element is not null
                ? element.Value
                : ThrowSerializationException<JElement>($"Could not get a child 'JsonObject' with name '{childPropertyName}'");

    /// <summary>
    /// Tries to construct a <see cref="JElement" /> from this <see cref="Node"/>'s property <paramref name="childPropertyName" /> and
    /// its JsonObject name.
    /// </summary>
    /// <param name="childPropertyName">Name of the element property.</param>
    /// <param name="array">The element.</param>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetArray(
        out JsonArray? array,
        string childPropertyName = Vocabulary.Value)
    {
        if (Node is JsonObject jsObj
            && jsObj.TryGetArray(childPropertyName, out array))
            return true;

        array = default;
        return false;
    }

    /// <summary>
    /// Constructs a <see cref="JElement" /> from this <see cref="Node"/>'s property <paramref name="childPropertyName" /> and
    /// its JsonObject name.
    /// </summary>
    /// <param name="childPropertyName">Name of the element property.</param>
    public readonly JsonArray GetArray(string childPropertyName = Vocabulary.Value)
        => TryGetArray(out var array, childPropertyName)
            && array is not null
            ? array
            : throw new SerializationException($"Could not get a child 'JsonArray' with name '{childPropertyName}'");

    /// <summary>
    /// Tries to construct a JElement from the name and name of the first property where the property name is a JsonObject.
    /// </summary>
    /// <returns><c>true</c> if successful, <c>false</c> otherwise.</returns>
    public readonly bool TryGetFirstElement(out JElement? element)
    {
        if (Node is JsonObject jsObj)
        {
            var kvp = jsObj.FirstOrDefault(kvp => !string.IsNullOrWhiteSpace(kvp.Key) && kvp.Value is JsonObject);

            element = new JElement(kvp);
            return true;
        }

        element = null;
        return false;
    }

    /// <summary>
    /// Tries to construct a JElement from the name and name of the first property where the property name is a JsonObject.
    /// </summary>
    /// <returns>System.Nullable&lt;JElement&gt;.</returns>
    public readonly JElement GetFirstElement()
        => TryGetFirstElement(out var element) && element is not null
                ? element.Value
                : ThrowSerializationException<JElement>($"Could not find a single child 'JsonObject'");
}
