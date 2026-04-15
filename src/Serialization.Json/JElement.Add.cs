namespace vm2.Linq.Expressions.Serialization.Json;

public partial struct JElement
{
    /// <summary>
    /// Adds the <paramref key="key" /> and the <paramref name="value" /> to the current <see cref="Node" /> if its type is
    /// <see cref="JsonObject" /> or <c>null</c> (in which case the method creates a new JsonObject element).
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The object to add.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="InternalTransformErrorException">
    /// If the <see cref="Node"/> of this <see cref="JElement"/> is not <see cref="JsonObject"/>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// If a property with the same name as the <paramref name="key"/> already exists in the <see cref="Node"/> of this instance.
    /// If <paramref name="key"/> is an empty string.
    /// </exception>
    public JElement Add(string key, JsonNode? value = null)
    {
        Node ??= new JsonObject();

        if (Node is not JsonObject jObject)
            throw new InternalTransformErrorException($"Trying to add a string key and JsonNode to a Node of `{Node.GetValueKind()}` type of JSON element. The Node must be JsonObject type.");

        jObject.Add(key, value);

        return this;
    }

    /// <summary>
    /// Adds the <see cref="JElement"/> <paramref key="properties"/> to the current <see cref="Node"/> if its type is
    /// <see cref="JsonObject"/> or <c>null</c> (in which case the method creates a new JsonObject element).
    /// If any of the parameters are <c>null</c> the method quietly skips them.
    /// </summary>
    /// <param key="elements"></param>
    /// <returns>This instance.</returns>
    /// <exception cref="InternalTransformErrorException">
    /// If the <see cref="Node"/> of this <see cref="JElement"/> is not <see cref="JsonObject"/>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// If a property with the same name as the <see cref="Name"/> of any of the <paramref name="properties"/>
    /// already exists in the <see cref="Node"/> of this instance.
    /// </exception>
    public JElement Add(params JElement?[] properties)
        => Add(properties.AsEnumerable());

    /// <summary>
    /// Adds the <see cref="JElement"/> and <see cref="IEnumerable{JElement}"/> <paramref key="properties"/> to the
    /// current <see cref="Node"/> if its type is <see cref="JsonObject"/> or <c>null</c> (in which case the method
    /// creates a new JsonObject element).
    /// If any of the parameters are <c>null</c> the method quietly skips them. The <see cref="IEnumerable{JElement}"/>
    /// parameters are added iteratively in the current <see cref="Node"/>.
    /// </summary>
    /// <param key="properties"></param>
    /// <returns>This instance.</returns>
    /// <exception cref="InternalTransformErrorException">
    /// If the <see cref="Node"/> of this <see cref="JElement"/> is not <see cref="JsonObject"/>
    /// </exception>
    public JElement Add(params object?[] properties)
    {
        Node ??= new JsonObject();

        if (Node is not JsonObject jObject)
            throw new InternalTransformErrorException($"Trying to add JElement-s to a Node of `{Node.GetValueKind()}` type of JSON element. The Node must be JsonObject type.");

        foreach (var prop in properties)
            _ = prop switch {
                IEnumerable<JElement?> p => Add(p),
                JElement p => Add(p),
                null => this,
                _ => throw new InternalTransformErrorException($"Don't know how to add {prop.GetType().Name} to a JElement")
            };

        return this;
    }

    /// <summary>
    /// Adds a the elements from the parameter to the current <see cref="Node"/> if the type of the element is
    /// <see cref="JsonObject"/> or <c>null</c> (in which case the method creates a new JsonObject element). If any of the
    /// elements in the <paramref key="elements"/> are <c>null</c> the method quietly skips them.
    /// </summary>
    /// <param key="elements"></param>
    /// <returns>This instance.</returns>
    /// <exception cref="InternalTransformErrorException">
    /// If the <see cref="Node"/> of this <see cref="JElement"/> is not <see cref="JsonObject"/>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// If a property with the same name as the <see cref="Name"/> of any of the <paramref name="properties"/>
    /// already exists in the <see cref="Node"/> of this instance.
    /// </exception>
    public JElement Add(IEnumerable<JElement?> properties)
    {
        Node ??= new JsonObject();

        if (Node is not JsonObject jObject)
            throw new InternalTransformErrorException($"Trying to add JElement-s to a Node of `{Node.GetValueKind()}` type of JSON element. The Node must be JsonObject type.");

        foreach (var property in properties.Where(p => p is not null))
            jObject.Add(property!);

        return this;
    }

    /// <summary>
    /// Adds a the elements from the parameter to the current <see cref="Node"/> if the type of the element is
    /// <see cref="JsonObject"/> or <c>null</c> (in which case the method creates a new JsonObject element). If any of the
    /// elements in the <paramref key="elements"/> are <c>null</c> the method quietly skips them.
    /// </summary>
    /// <param key="elements"></param>
    /// <returns>This instance.</returns>
    /// <exception cref="InternalTransformErrorException">
    /// If the <see cref="Node"/> of this <see cref="JElement"/> is not <see cref="JsonObject"/>
    /// </exception>
    /// <exception cref="ArgumentException">
    /// If a property with the same name as the <see cref="Name"/> of any of the <paramref name="properties"/>
    /// already exists in the <see cref="Node"/> of this instance.
    /// </exception>
    public JElement Add(IEnumerable<JElement> properties)
    {
        Node ??= new JsonObject();

        if (Node is not JsonObject jObject)
            throw new InternalTransformErrorException($"Trying to add JElement-s to a Node of `{Node.GetValueKind()}` type of JSON element. The Node must be JsonObject type.");

        foreach (var property in properties)
            jObject.Add(property!);

        return this;
    }

    /// <summary>
    /// Adds the <paramref name="element"/> to the current <see cref="Node"/> if its type is
    /// <see cref="JsonArray"/> or <c>null</c> (in which case the method creates a new JsonArray).
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="InternalTransformErrorException">
    /// If the <see cref="Node"/> of this <see cref="JElement"/> is not <see cref="JsonArray"/>
    /// </exception>
    public JElement Add(JsonNode? element)
    {
        Node ??= new JsonArray();

        if (Node is not JsonArray jArray)
            throw new InternalTransformErrorException($"Trying to add a JsonNode to a Node of `{Node.GetValueKind()}` type of JSON element. The Node must be JsonArray type.");

        jArray.Add(element);

        return this;
    }

    /// <summary>
    /// Adds the <paramref name="elements"/> to the current <see cref="Node"/> if its type is
    /// <see cref="JsonArray"/> or <c>null</c> (in which case the method creates a new JsonArray).
    /// </summary>
    /// <param name="elements">The element.</param>
    /// <returns>This instance.</returns>
    /// <exception cref="InternalTransformErrorException">
    /// If the <see cref="Node"/> of this <see cref="JElement"/> is not <see cref="JsonArray"/>
    /// </exception>
    public JElement Add(params JsonNode?[] elements)
        => Add(elements.AsEnumerable());

    /// <summary>
    /// Adds a the elements from the parameter to the current <see cref="Node"/> if the type of the element is
    /// <see cref="JsonArray"/> or <c>null</c> (in which case the method creates a new JsonArray element). If any of the
    /// elements in the <paramref key="elements"/> are <c>null</c> the method quietly skips them.
    /// </summary>
    /// <param key="elements"></param>
    /// <returns>This instance.</returns>
    /// <exception cref="InternalTransformErrorException">
    /// If the <see cref="Node"/> of this <see cref="JElement"/> is not <see cref="JsonArray"/>
    /// </exception>
    public JElement Add(IEnumerable<JsonNode?> elements)
    {
        Node ??= new JsonArray();

        if (Node is not JsonArray jArray)
            throw new InternalTransformErrorException($"Trying to add JsonNode-s to a Node of `{Node.GetValueKind()}` type of JSON element. The Node must be JsonArray type.");

        foreach (var element in elements)
            jArray.Add(element);

        return this;
    }
}
