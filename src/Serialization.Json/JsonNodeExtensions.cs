namespace vm2.Linq.Expressions.Serialization.Json;

/// <summary>
/// Class JsonObject extensions.
/// </summary>
public static class JsonNodeExtensions
{
    #region Add methods
    /// <summary>
    /// Adds the specified elements (key, JsonNode) to the object.
    /// </summary>
    /// <param name="jsObj">The JSON object.</param>
    /// <param name="element">The elements.</param>
    /// <returns>
    /// The <paramref name="jsObj"/> and a boolean which will be <c>true</c> if the operation was successful, or
    /// <c>false</c> if the <paramref name="jsObj"/> already has a property with the elements' <see cref="JElement.Name"/>.
    /// </returns>
    public static bool TryAdd(
        this JsonObject jsObj,
        JElement? element)
    {
        if (element is null)
            return true;

        if (element.Value.Name is "")
            throw new ArgumentException("The key of the elements is an empty string.");

        return jsObj.TryAdd(element.Value.Name, element.Value.Node);
    }

    /// <summary>
    /// Adds the specified elements (key, JsonNode) to the object.
    /// </summary>
    /// <param name="jsObj">The JSON object.</param>
    /// <param name="element">The elements.</param>
    /// <returns>The <see cref="JsonObject"/>.</returns>
    /// <exception cref="ArgumentException">
    /// If a property with the key of the <paramref name="element"/> already exists in the <paramref name="jsObj"/>.
    /// </exception>
    public static JsonObject Add(
        this JsonObject jsObj,
        JElement? element)
    {
        if (element is null)
            return jsObj;

        if (element.Value.Name is "")
            throw new ArgumentException("The key of the elements is an empty string.");

        jsObj.Add(element.Value.Name, element.Value.Node);
        return jsObj;
    }

    /// <summary>
    /// Adds the specified elements (key, JsonNode) to the object.
    /// </summary>
    /// <param name="jsObj">The JSON object.</param>
    /// <param name="elements">The elements.</param>
    /// <returns>
    /// The <paramref name="jsObj"/> and a boolean which will be <c>true</c> if the operation was successful, or
    /// <c>false</c> if the <paramref name="jsObj"/> already has a property with the elements' <see cref="JElement.Name"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If there is a property with empty key.
    /// </exception>
    public static bool TryAdd(
        this JsonObject jsObj,
        params JElement?[] elements)
        => jsObj.TryAdd(elements.AsEnumerable());

    /// <summary>
    /// Adds the specified elements (key, JsonNode) to the object.
    /// </summary>
    /// <param name="jsObj">The JSON object.</param>
    /// <param name="elements">The elements to add.</param>
    /// <returns>The <see cref="JsonObject"/>.</returns>
    /// <exception cref="ArgumentException">
    /// If a property with the key of any of the <paramref name="elements"/> already exists in the <paramref name="jsObj"/>.
    /// If there is a property with empty key.
    /// </exception>
    public static JsonObject Add(
        this JsonObject jsObj,
        params JElement?[] elements)
        => jsObj.Add(elements.AsEnumerable());

    /// <summary>
    /// Adds the <see cref="JElement"/> and <see cref="IEnumerable{JElement}"/> <paramref key="elements"/> to the
    /// current <paramref name="jsObj"/>.
    /// If any of the parameters are <c>null</c> the method quietly skips them. The <see cref="IEnumerable{JElement}"/>
    /// parameters are added iteratively in the current <paramref name="jsObj"/>.
    /// </summary>
    /// <param name="jsObj">The JSON object.</param>
    /// <param name="elements">The elements.</param>
    /// <returns>
    /// The <paramref name="jsObj"/> and a boolean which will be <c>true</c> if the operation was successful, or
    /// <c>false</c> if the <paramref name="jsObj"/> already has a property with the elements' <see cref="JElement.Name"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If there is a property with empty key.
    /// </exception>
    public static bool TryAdd(
        this JsonObject jsObj,
        params object?[] elements)
    {
        bool ret = true;

        foreach (var prop in elements)
            ret &= prop switch {
                IEnumerable<JElement?> jes => jsObj.TryAdd(jes),
                JElement je => jsObj.TryAdd(je),
                null => true,
                _ => throw new InternalTransformErrorException($"Don't know how to add {prop.GetType().Name} to a JElement")
            };

        return ret;
    }

    /// <summary>
    /// Adds the <see cref="JElement"/> and <see cref="IEnumerable{JElement}"/> <paramref key="elements"/> to the
    /// current <paramref name="jsObj"/>.
    /// If any of the parameters are <c>null</c> the method quietly skips them. The <see cref="IEnumerable{JElement}"/>
    /// parameters are added iteratively in the current <paramref name="jsObj"/>.
    /// </summary>
    /// <param name="jsObj">The JSON object.</param>
    /// <param name="elements">The elements to add.</param>
    /// <returns>The <see cref="JsonObject"/>.</returns>
    /// <exception cref="ArgumentException">
    /// If a property with the key of any of the <paramref name="elements"/> already exists in the <paramref name="jsObj"/>.
    /// If there is a property with empty key.
    /// </exception>
    public static JsonObject Add(
        this JsonObject jsObj,
        params object?[] elements)
    {
        foreach (var prop in elements)
            _ = prop switch {
                IEnumerable<JElement?> jes => jsObj.Add(jes),
                JElement je => jsObj.Add(je, null),
                null => jsObj,
                _ => throw new InternalTransformErrorException($"Don't know how to add {prop.GetType().Name} to a JElement")
            };

        return jsObj;
    }

    /// <summary>
    /// Adds the specified elements (key, JsonNode) to the object.
    /// </summary>
    /// <param name="jsObj">The JSON object.</param>
    /// <param name="elements">The elements.</param>
    /// <returns>
    /// The <paramref name="jsObj"/> and a boolean which will be <c>true</c> if the operation was successful, or
    /// <c>false</c> if the <paramref name="jsObj"/> already has a property with the elements' <see cref="JElement.Name"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// If there is a property with empty key.
    /// </exception>
    public static bool TryAdd(
        this JsonObject jsObj,
        IEnumerable<JElement?> elements)
    {
        var ret = true;

        foreach (var element in elements)
            if (element is not null)
                ret &= jsObj.TryAdd(element.Value.Name, element.Value.Node);

        return ret;
    }

    /// <summary>
    /// Adds the specified elements (key, JsonNode) to the object.
    /// </summary>
    /// <param name="jsObj">The JSON object.</param>
    /// <param name="elements">The elements to add.</param>
    /// <returns>The <see cref="JsonObject"/>.</returns>
    /// <exception cref="ArgumentException">
    /// If a property with the key of any of the <paramref name="elements"/> already exists in the <paramref name="jsObj"/>.
    /// If there is a property with empty key.
    /// </exception>
    public static JsonObject Add(
        this JsonObject jsObj,
        IEnumerable<JElement?> elements)
    {
        foreach (var element in elements)
            if (element is not null)
                jsObj.Add(element.Value.Name, element.Value.Node);

        return jsObj;
    }
    #endregion

    #region Get methods
    // <summary>
    //   Returns the value of a property with the specified name.
    // </summary>
    // <param name="propertyName">The name of the property to return.</param>
    // <param name="jsonNode">The JSON value of the property with the specified name.</param>
    // <returns>
    //   <see langword="true"/> if a property with the specified name was found; otherwise, <see langword="false"/>.
    // </returns>
    // public bool TryGetPropertyValue(string propertyName, out JsonNode? jsonNode) => provided by the JsonObject class

    /// <summary>
    /// Gets the node of a property with the specified name <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>JsonNode?.</returns>
    public static JsonNode? GetPropertyValue(
        this JsonObject jsObj,
        string propertyName)
        => jsObj.TryGetPropertyValue(propertyName, out var node)
                ? node
                : jsObj.ThrowSerializationException<JsonNode?>($"Could not find a property by name '{propertyName}'");

    /// <summary>
    /// Gets the value of type <typeparamref name="T"/> of a property with the specified name <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="value">The output value.</param>
    /// <returns><c>true</c> if a property name and type was found, <c>false</c> otherwise.</returns>
    public static bool TryGetPropertyValue<T>(
        this JsonObject jsObj,
        string propertyName,
        out T? value)
    {
        value = default;
        return jsObj.TryGetPropertyValue(propertyName, out var node)
                && node is not null
                && node.AsValue().TryGetValue<T>(out value);
    }

    /// <summary>
    /// Gets the value of type <typeparamref name="T"/> of a property with the specified name <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>JsonNode?.</returns>
    public static T? GetPropertyValue<T>(
        this JsonObject jsObj,
        string propertyName)
        => jsObj.TryGetPropertyValue<T>(propertyName, out var value)
                ? value
                : jsObj.ThrowSerializationException<T?>($"Could not find a property by name '{propertyName}'");

    /// <summary>
    /// Tries to find a property with one of the names in the sequence <paramref name="names"/>.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="names">The names of the sought properties.</param>
    /// <param name="propertyName">Name of the found property.</param>
    /// <param name="node">The found node.</param>
    /// <returns><c>true</c> if a property was found, <c>false</c> otherwise.</returns>
    public static bool TryGetOneOf(
        this JsonObject jsObj,
        IEnumerable<string> names,
        out string? propertyName,
        out JsonNode? node)
    {
        var kvp = jsObj.FirstOrDefault(kvp => names.Contains(kvp.Key));

        propertyName = kvp.Key;
        node = kvp.Value;

        return propertyName is not null;
    }

    /// <summary>
    /// Gets a property with a name one of the names in <paramref name="names"/>.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="names">The names of the sought properties.</param>
    /// <returns><c>true</c> if a property was found, <c>false</c> otherwise.</returns>
    public static (string, JsonNode?) GetOneOf(
        this JsonObject jsObj,
        IEnumerable<string> names)
        => jsObj.TryGetOneOf(names, out var propertyName, out var node)
                ? (propertyName!, node)
                : jsObj.ThrowSerializationException<(string, JsonNode?)>($"Could not find any of the properties '{string.Join("', '", names)}'");

    /// <summary>
    /// Determines whether the js object was marked to have a <see langword="null"/> value.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <returns><c>true</c> if the .NET node of the specified js object is <see langword="null"/>; otherwise, <c>false</c>.</returns>
    public static bool IsNil(this JsonObject jsObj)
        => TryGetValue(jsObj, out var value)
            && (value is null
                || value.GetValueKind() == JsonValueKind.Null);

    /// <summary>
    /// Tries to get the node of the JSON property 'Node'.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="node">The node.</param>
    /// <param name="propertyValueName">The name of the 'Node' property.</param>
    /// <returns><c>true</c> if property exists, <c>false</c> otherwise.</returns>
    public static bool TryGetValue(
        this JsonObject jsObj,
        out JsonNode? node,
        string propertyValueName = Vocabulary.Value)
        => jsObj.TryGetPropertyValue(propertyValueName, out node);

    /// <summary>
    /// Gets the node of the JSON property 'Node'.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="propertyValueName">The name of the 'Node' property.</param>
    public static JsonNode? GetValue(
        this JsonObject jsObj,
        string propertyValueName = Vocabulary.Value)
        => jsObj.TryGetPropertyValue(propertyValueName, out var value)
                ? value
                : jsObj.ThrowSerializationException<JsonNode?>($"Could not find the property '{propertyValueName}'");

    /// <summary>
    /// Tries to get the node of the JSON property 'Length'.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="length">The obtained node.</param>
    /// <param name="propertyLengthName">The name of the property length.</param>
    /// <returns><c>true</c> if property exists, <c>false</c> otherwise.</returns>
    public static bool TryGetLength(
        this JsonObject jsObj,
        out int length,
        string propertyLengthName = Vocabulary.Length)
    {
        if (jsObj.TryGetPropertyValue(propertyLengthName, out var len)
            && len is not null)
        {
            length = len.AsValue().GetValue<int>();
            return true;
        }

        length = default;
        return false;
    }

    /// <summary>
    /// Gets the node of the JSON property 'Length'.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="propertyLengthName">The name of the property length.</param>
    public static int GetLength(
        this JsonObject jsObj,
        string propertyLengthName = Vocabulary.Length)
        => jsObj.TryGetLength(out var length, propertyLengthName)
                ? length
                : jsObj.ThrowSerializationException<int>($"Could not find the property '{propertyLengthName}'");

    /// <summary>
    /// Tries to get the type of the object, either from a property with a basic type name (e.g. 'int') or
    /// from a property with name 'Type'.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="type">The type.</param>
    /// <param name="propertyTypeName">Name of the property type.</param>
    /// <returns><c>true</c> if successfully determined the type of this JsonObject, <c>false</c> otherwise.</returns>
    public static bool TryGetType(
        this JsonObject jsObj,
        out Type? type,
        string propertyTypeName = Vocabulary.Type)
    {
        type = jsObj
                .Where(kvm => Vocabulary.BasicTypeNames.Contains(kvm.Key))
                .Select(kv => Vocabulary.NamesToTypes[kv.Key])
                .FirstOrDefault()
                ;

        if (type is not null)
            return true;

        return jsObj.TryGetPropertyValue(propertyTypeName, out var typeNode)
                && typeNode is not null
                && typeNode.GetValueKind() is JsonValueKind.String
                && (Vocabulary.NamesToTypes.TryGetValue(typeNode.GetValue<string>(), out type)
                    || (type = Type.GetType(typeNode.GetValue<string>())) is not null);
    }

    /// <summary>
    /// Gets the type of the object, either from a property with a basic type name (e.g. 'int') or
    /// from a property with name 'Type'.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="propertyTypeName">Name of the property type.</param>
    /// <returns>Type.</returns>
    public static Type GetType(
        this JsonObject jsObj,
        string propertyTypeName = Vocabulary.Type)
        => jsObj.TryGetType(out var type, propertyTypeName) && type is not null
                ? type
                : jsObj.ThrowSerializationException<Type>($"Could not find a property '{propertyTypeName}' that defines the type of the object");

    /// <summary>
    /// Tries to get a JsonObject node from property <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="propertyName">Name of the property holding the JsonObject.</param>
    /// <param name="child">The found array.</param>
    /// <returns><c>true</c> if JsonObject array node was found, <c>false</c> otherwise.</returns>
    public static bool TryGetObject(
        this JsonObject jsObj,
        string propertyName,
        out JsonObject? child)
    {
        if (jsObj.TryGetPropertyValue(propertyName, out var node)
            && node is JsonObject or null)
        {
            child = node as JsonObject;
            return true;
        }
        child = null;
        return false;
    }

    /// <summary>
    /// Gets a JsonObject node from property <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="propertyName">Name of the property holding the JsonObject.</param>
    /// <returns>JsonObject</returns>
    public static JsonObject GetObject(
        this JsonObject jsObj,
        string propertyName)
        => jsObj.TryGetObject(propertyName, out var child) && child is not null
                ? child
                : jsObj.ThrowSerializationException<JsonObject>($"Could not find a JsonObject property '{propertyName}'");

    /// <summary>
    /// Tries to get a JsonArray node from property <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="propertyName">Name of the array.</param>
    /// <param name="array">The found array.</param>
    /// <returns><c>true</c> if JsonArray node was found, <c>false</c> otherwise.</returns>
    public static bool TryGetArray(
        this JsonObject jsObj,
        string propertyName,
        out JsonArray? array)
    {
        if (jsObj.TryGetPropertyValue(propertyName, out var node)
            && node is JsonArray or null)
        {
            array = node as JsonArray;
            return true;
        }

        array = default;
        return false;
    }

    /// <summary>
    /// Gets a JsonArray node from property <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="propertyName">Name of the property holding the array.</param>
    /// <returns>JsonObject</returns>
    public static JsonArray GetArray(
        this JsonObject jsObj,
        string propertyName)
        => jsObj.TryGetArray(propertyName, out var array) && array is not null
                ? array
                : jsObj.ThrowSerializationException<JsonArray>($"Could not find a JsonArray property '{propertyName}'");

    /// <summary>
    /// Tries to get the first JsonObject node.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <param name="propertyName">Name of the property where the found array is.</param>
    /// <param name="child">The found array.</param>
    /// <returns><c>true</c> if JsonObject array node was found, <c>false</c> otherwise.</returns>
    public static bool TryGetFirstObject(
        this JsonObject jsObj,
        out string? propertyName,
        out JsonObject? child)
    {
        var kop = jsObj.FirstOrDefault(kvm => kvm.Value is JsonObject);

        propertyName = kop.Key;
        child = kop.Value as JsonObject;
        return propertyName is not null;
    }

    /// <summary>
    /// Gets the first JsonObject node.
    /// </summary>
    /// <param name="jsObj">The extended JsonObject.</param>
    /// <returns>JsonObject</returns>
    public static (string, JsonObject) GetFirstObject(
        this JsonObject jsObj)
        => jsObj.TryGetFirstObject(out var name, out var obj) &&
           name is not null
            && obj is not null
                ? (name, obj)
                : jsObj.ThrowSerializationException<(string, JsonObject)>($"Could not find a property of type 'JsonObject'");
    #endregion

    /// <summary>
    /// Throws a (de)serialization exception at the specified node.
    /// </summary>
    /// <param name="node">The node where the problem was encountered.</param>
    /// <param name="message">The exception message will be appended with &quot; -- &apos;&lt;the node path&gt;&apos;.&quot;.</param>
    /// <exception cref="SerializationException"></exception>
    public static T ThrowSerializationException<T>(this JsonNode node, string message = "Invalid JSON")
        => throw new SerializationException($"{message} -- at '{node.GetPath()}'.");

    /// <summary>
    /// Casts the <paramref name="node"/> to object or throws an exception.
    /// </summary>
    /// <param name="node">The node to cast.</param>
    /// <param name="message">The message of the exception.</param>
    /// <returns>JsonObject.</returns>
    /// <exception cref="SerializationException"></exception>
    public static JsonObject ToObject(
        this JsonNode? node,
        string message = "Expected child JsonObject")
        => node is JsonObject jsObject
                ? jsObject
                : node?.ThrowSerializationException<JsonObject>(message) ?? throw new SerializationException(message);
}
