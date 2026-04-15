namespace vm2.Linq.Expressions.Serialization.Json;

delegate JElement TransformConstant(object? value, Type type);

/// <summary>
/// Class ToJsonDataTransform transforms data (Expression constants) to JSON.
/// </summary>
partial class ToJsonDataTransform(JsonOptions options)
{
    /// <summary>Transforms the node.</summary>
    /// <param name="node">The node.</param>
    /// <returns>JElement.</returns>
    public JElement TransformNode(ConstantExpression node) => GetTransform(node.Type)(node.Value, node.Type);

    /// <summary>
    /// Gets the best matching transform function for the type encapsulated in the  for the specified <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>
    /// A delegate that can transform a nodeValue of the specified <paramref name="type"/> into an JSON obj (<see cref="JElement"/>).
    /// </returns>
    /// <exception cref="SerializationException"></exception>
    public TransformConstant GetTransform(Type type)
    {
        // get the transform from the table, or
        if (_constantTransforms.TryGetValue(type, out var transform))
            return transform;

        // if it is an enum - return the EnumTransform
        if (type.IsEnum)
            return EnumTransform;

        // if it is a nodeValue - get nodeValue transform or
        if (type.IsNullable())
            return NullableTransform;

        // if it is an anonymous - get anonymous transform or
        if (type.IsAnonymous())
            return AnonymousTransform;

        if (type.IsByteSequence())
            return ByteSequenceTransform;

        if (type.IsDictionary())
            return DictionaryTransform;

        if (type.IsSequence())
            return SequenceTransform;

        if (type.IsTuple())
            return TupleTransform;

        // get general object transform
        return ObjectTransform;
    }

    #region Transforms
    /// <summary>
    /// Transforms enum values.
    /// </summary>
    /// <param name="nodeValue">The node v.</param>
    /// <param name="nodeType">GetType of the node v.</param>
    JElement EnumTransform(
        object? nodeValue,
        Type nodeType)
    {
        Debug.Assert(nodeValue is not null);

        var strValue = nodeValue.ToString();
        var valueType = new JElement(Vocabulary.Type, Transform.TypeName(nodeType));
        var valueElement = new JElement(Vocabulary.Value);

        if (strValue is not null && nodeType.IsDefined(typeof(FlagsAttribute)))
            valueElement.Node = new JsonArray(
                                        strValue
                                            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                                            .Select(v => JsonValue.Create(v))
                                            .ToArray());
        else
            valueElement.Node = JsonValue.Create(strValue);

        var underlyingType = nodeType.GetEnumUnderlyingType();

        return new JElement(
                        Vocabulary.Enum,
                            valueType,
                            valueElement,
                            underlyingType != typeof(int) ? new JElement(Vocabulary.BaseType, Transform.TypeName(underlyingType)) : null,
                            new JElement(Vocabulary.BaseValue, (int)nodeValue)
                    );
    }

    /// <summary>
    /// Transforms a nodeValue nodeValue.
    /// </summary>
    /// <param name="nodeValue">The node v.</param>
    /// <param name="nodeType">GetType of the node v.</param>
    JElement NullableTransform(
        object? nodeValue,
        Type nodeType)
    {
        var underlyingType  = nodeType.GetGenericArguments()[0];

        if (nodeValue is null || nodeType.GetProperty("HasValue")?.GetValue(nodeValue) is false)
            return new JElement(
                            Vocabulary.Nullable,
                                new JElement(Vocabulary.Type, Transform.TypeName(underlyingType)),
                                new JElement(Vocabulary.Value));    // null value

        var value = nodeType.GetProperty(nameof(Nullable<>.Value))?.GetValue(nodeValue)
                        ?? throw new InternalTransformErrorException("'Nullable<T>.HasValue' is true but 'Nullable<T>.Node' is null.");

        return new JElement(
                        Vocabulary.Nullable,
                            options.TypeComment(underlyingType),
                            GetTransform(underlyingType)(value, underlyingType));
    }

    /// <summary>
    /// Transforms sequences of bytes.
    /// </summary>
    /// <param name="nodeValue">The node v.</param>
    /// <param name="nodeType">GetType of the node v.</param>
    /// <exception cref="InternalTransformErrorException"></exception>
    JElement ByteSequenceTransform(
        object? nodeValue,
        Type nodeType)
    {
        var sequenceElement = new JElement(
                                    Vocabulary.ByteSequence,
                                        new JElement(Vocabulary.Type, Transform.TypeName(nodeType)),
                                        nodeValue is null ? new JElement(Vocabulary.Value) : null
                                );
        ReadOnlySpan<byte> bytes;

        if (nodeType == typeof(byte[]))
        {
            if (nodeValue is null)
                return sequenceElement;

            bytes = ((byte[])nodeValue).AsSpan();
        }
        else
        {
            if (nodeValue is null)
                throw new InternalTransformErrorException($"Unexpected non-array byte sequenceElement with null value of type '{nodeType.FullName}'.");

            if (nodeValue is ImmutableArray<byte> iab)
                bytes = iab.AsSpan();
            else
            if (nodeValue is Memory<byte> mb)
                bytes = mb.Span;
            else
            if (nodeValue is ReadOnlyMemory<byte> rmb)
                bytes = rmb.Span;
            else
            if (nodeValue is ArraySegment<byte> asb)
                bytes = asb.AsSpan();
            else
                throw new InternalTransformErrorException($"Unknown byte sequenceElement '{nodeType.FullName}'.");
        }

        sequenceElement.Add(
            new JElement(Vocabulary.Value, Convert.ToBase64String(bytes)),
            new JElement(Vocabulary.Length, bytes.Length));

        return sequenceElement;
    }

    /// <summary>
    /// Transforms an anonymous object.
    /// </summary>
    /// <param name="nodeValue">The node v.</param>
    /// <param name="nodeType">GetType of the node v.</param>
    JElement AnonymousTransform(
        object? nodeValue,
        Type nodeType)
        => new(
            Vocabulary.Anonymous,
                new JElement(
                        Vocabulary.Type,
                        Transform.TypeName(nodeType)),
                new JElement(
                        Vocabulary.Value,
                        nodeType
                            .GetProperties()
                            .Select(pi => new JElement(
                                                    pi.Name,
                                                    GetTransform(pi.PropertyType)(
                                                        pi.GetValue(nodeValue, null),
                                                        pi.PropertyType)))));

    JElement TupleTransform(
        object? nodeValue,
        Type nodeType)
    {
        if (nodeType.IsValueType && nodeValue is null)
            throw new InternalTransformErrorException("The propValue of a 'ValueTuple' is null.");

        var value = nodeValue is not null ? new JsonObject() : null;
        var tupleElement = new JElement(
                                    Vocabulary.Tuple,
                                        new JElement(Vocabulary.Type, Transform.TypeName(nodeType)),
                                        new JElement(Vocabulary.Value, value));

        if (nodeValue is null)
            return tupleElement;

        Debug.Assert(value is not null);

        var tuple = nodeValue as ITuple ?? throw new InternalTransformErrorException("Expected tuple propValue to implement ITuple");
        var types = nodeType.GetGenericArguments();

        for (var i = 0; i < tuple.Length; i++)
        {
            var propValue = tuple[i];
            var declaredType = types[i];

            value.Add(
                new JElement(
                        $"Item{i + 1}",
                        GetTransform(declaredType)(propValue, propValue?.GetType() ?? declaredType)
                        ));
        }

        return tupleElement;
    }

    /// <summary>
    /// Transforms sequences of objects.
    /// </summary>
    /// <param name="nodeValue">The node v.</param>
    /// <param name="nodeType">GetType of the node v.</param>
    JElement SequenceTransform(
        object? nodeValue,
        Type nodeType)
    {
        var elementType = (nodeType.IsGenericType
                                ? nodeType.GetGenericArguments()[0]
                                : nodeType.GetElementType()) ?? throw new InternalTransformErrorException("Could not find the type of a sequenceElement elements.");

        if (nodeValue is null)
            return new JElement(
                            Vocabulary.Sequence,
                                new JElement(Vocabulary.Type, Transform.TypeName(nodeType)),
                                options.TypeComment(elementType),
                                new JElement(Vocabulary.Value)
                        );

        var piLength = nodeType.GetProperty("Count") ?? nodeType.GetProperty("GetLength");
        var length = (int?)piLength?.GetValue(nodeValue);
        var enumerable = nodeValue as IEnumerable;

        if (enumerable is null)
        {
            Debug.Assert(nodeType.IsMemory());

            enumerable = nodeType.GetMethod("ToArray")?.Invoke(nodeValue, null) as IEnumerable;

            if (enumerable is null)
                throw new InternalTransformErrorException($"Could not find the enumerable for {nodeType.FullName}.");
        }

        return new JElement(
                        Vocabulary.Sequence,
                            new JElement(Vocabulary.Type, Transform.TypeName(nodeType)),
                            options.TypeComment(elementType),
                            new JElement(
                                    Vocabulary.Value,
                                    (JsonNode)new JsonArray(
                                        enumerable
                                            .Cast<object?>()
                                            .Select(e => new JsonObject() { GetTransform(elementType)(e, elementType) })
                                            .ToArray())),
                            length is not null ? new JElement(Vocabulary.Length, length.Value) : null);
    }

    /// <summary>
    /// Transforms dictionaries.
    /// </summary>
    /// <param name="nodeValue">The node v.</param>
    /// <param name="nodeType">GetType of the node v.</param>
    JElement DictionaryTransform(
        object? nodeValue,
        Type nodeType)
    {
        if (nodeValue is null)
            return new JElement(
                            Vocabulary.Dictionary,
                                new JElement(Vocabulary.Type, Transform.TypeName(nodeType)),
                                new JElement(Vocabulary.Value));

        if (nodeValue is not IDictionary dict)
            throw new InternalTransformErrorException("The v of type 'Dictionary' doesn't implement IDictionary.");

        Type kType, vType;

        var dictInterface = nodeType.GetInterfaces()
                                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));

        if (dictInterface is not null)
        {
            var kvTypes = dictInterface.GetGenericArguments();
            kType = kvTypes[0];
            vType = kvTypes[1];
        }
        else
        if (nodeType.IsGenericType)
        {
            var kvTypes = nodeType.GetGenericArguments();

            if (kvTypes.Length is not 2)
                throw new InternalTransformErrorException("The elements of 'Dictionary' do not have key-type and obj-type.");

            kType = kvTypes[0];
            vType = kvTypes[1];
        }
        else
        {
            kType = typeof(object);
            vType = typeof(object);
        }

        var dictElements = new JsonArray();
        var dictionary = new JElement(
                                Vocabulary.Dictionary,
                                    new JElement(Vocabulary.Type, Transform.TypeName(nodeType)),
                                    new JElement(Vocabulary.Value, (JsonNode)dictElements));

        foreach (DictionaryEntry kv in dict)
            dictElements.Add(
                new JsonObject() {
                        new JElement(
                                Vocabulary.Key,
                                    GetTransform(kType)(kv.Key, kv.Key.GetType())),
                        new JElement(
                                Vocabulary.Value,
                                    GetTransform(vType)(kv.Value, kv.Value?.GetType() ?? vType))
                });

        return dictionary.Add(Vocabulary.Length, dict.Count);
    }

    /// <summary>
    /// Transforms objects using <see cref="DataContractSerializer" /> which also works with classes marked with
    /// <see cref="SerializableAttribute"/> types too.
    /// </summary>
    /// <param name="nodeValue">The node v.</param>
    /// <param name="nodeType">GetType of the node v.</param>
    JElement ObjectTransform(
        object? nodeValue,
        Type nodeType)
    {
        var concreteType = nodeValue?.GetType();
        var obj = new JElement(
                        Vocabulary.Object,
                            new JElement(Vocabulary.Type, Transform.TypeName(nodeType)),
                            concreteType is not null && concreteType != nodeType
                                ? new JElement(Vocabulary.ConcreteType, Transform.TypeName(concreteType))
                                : null);

        if (nodeValue is null)
            return obj.Add(Vocabulary.Value, null);

        Debug.Assert(concreteType is not null);

        var actualTransform = GetTransform(concreteType);

        if (actualTransform != ObjectTransform)
            return actualTransform(nodeValue, concreteType);

        if (concreteType == typeof(object))
            return obj.Add(Vocabulary.Value, new JsonObject());

        return obj.Add(Vocabulary.Value, JsonSerializer.SerializeToNode(nodeValue, options.JsonSerializerOptions));
    }
    #endregion
}
