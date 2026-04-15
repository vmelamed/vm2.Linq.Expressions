namespace vm2.Linq.Expressions.Serialization.Json;
static partial class FromJsonDataTransform
{
    /// <summary>
    /// Transforms the element to a <see cref="ConstantExpression"/> object.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>ConstantExpression.</returns>
    internal static ConstantExpression ConstantTransform(JElement element)
    {
        var (value, type) = ValueTransform(element.GetOneOf(ConstantTypes));

        return Expression.Constant(value, type);
    }

    delegate object? Transformation(JElement element, ref Type type);

    static (object?, Type) ValueTransform(JElement element)
    {
        var type = element.GetType();

        if (type == typeof(void))
            throw new SerializationException($"Got 'void' type of constant data in the element at '{element.GetPath()}'");

        if (!_constantTransformations.TryGetValue(element.Name, out var transform))
            throw new SerializationException($"Error deserializing and converting to a strong type the value at '{element.GetPath()}'.");

        return (transform(element, ref type), type);
    }

    static object? TransformEnum(
        JElement element,
        ref Type type)
    {
        try
        {
            type = element.GetTypeFromProperty();

            if (element.TryGetPropertyValue<string>(out var value)
                && !string.IsNullOrWhiteSpace(value))
                return Enum.Parse(type, value);

            if (element.TryGetArray(out var arrayFlags)
                && arrayFlags is not null
                && arrayFlags.All(n => n?.GetValueKind() == JsonValueKind.String))
                return Enum.Parse(type, string.Join(", ", arrayFlags.Select(n => n?.GetValue<string>())));

            throw new SerializationException($"Could not convert the valueElement at '{element.GetPath()}' to '{type.FullName}'");
        }
        catch (ArgumentException ex)
        {
            throw new SerializationException($"Cannot transform '{element.GetPath()}' to '{type.FullName}' valueElement.", ex);
        }
        catch (OverflowException ex)
        {
            throw new SerializationException($"Cannot transform '{element.GetPath()}' to '{type.FullName}' valueElement.", ex);
        }
    }

    static object? TransformNullable(
        JElement element,
        ref Type type)
    {
        object? value = null;
        Type? underlyingType = null;

        if (element.IsNil())
        {
            underlyingType = element.GetTypeFromProperty();

            if (underlyingType == typeof(void))
                throw new SerializationException($"Constant expression of 'Nullable<>' type specified with 'void' type: '{element.Name}'.");

            type = typeof(Nullable<>).MakeGenericType(underlyingType);
            return value;
        }

        var vElement = element.GetOneOf(ConstantTypes);

        (value, underlyingType) = ValueTransform(vElement);

        if (underlyingType == typeof(void))
            throw new SerializationException($"Constant expression of 'Nullable<>' type specified with 'void' valueElement: '{element.Name}'.");

        type = typeof(Nullable<>).MakeGenericType(underlyingType);

        var ctor = type.GetConstructor([underlyingType])
                            ?? throw new InternalTransformErrorException($"Could not get the constructor for Nullable<{underlyingType.Name}>");

        return ctor.Invoke([value]);
    }

    static object? TransformObject(
        JElement element,
        ref Type type)
    {
        // get the expression type
        type = element.GetTypeFromProperty();

        // get the concrete type but do not change the expression type
        if (!element.TryGetTypeFromProperty(out var concreteType, Vocabulary.ConcreteType) || concreteType is null)
            concreteType = type;   // the element type IS the concrete type too

        if (concreteType is null)
            throw new SerializationException($"Unknown type at '{element.GetPath()}'.");

        var valueElement = element.GetElement(Vocabulary.Value);

        if (valueElement.IsNil())
            return null;

        if (concreteType == typeof(object))
            return new();

        return JsonSerializer.Deserialize(valueElement.Node, concreteType);
    }

    static object? TransformAnonymous(
        JElement element,
        ref Type type)
    {
        if (element.IsNil())
            return null;

        type = element.GetTypeFromProperty();

        var constructor = type.GetConstructors()[0];
        var constructorParameters = constructor.GetParameters();
        var valueElement = element.GetElement(Vocabulary.Value);

        if (valueElement.Node is null || valueElement.Node.GetValueKind() != JsonValueKind.Object)
            throw new SerializationException($"Invalid value at '{element.GetPath()}'.");

        var value = valueElement.Node.AsObject();

        if (value.Count != constructorParameters.Length)
            throw new SerializationException($"The number of properties and the number of initialization parameters do not match for anonymous type at '{element.GetPath()}'.");

        var parameters = new object?[constructorParameters.Length];

        for (var i = 0; i < constructorParameters.Length; i++)
        {
            var paramName = constructorParameters[i].Name ?? "";
            var propElement = valueElement.GetElement(paramName).GetOneOf(ConstantTypes);
            parameters[i] = ValueTransform(propElement).Item1;
        }

        return constructor.Invoke(parameters);
    }

    static object? TransformTuple(
        JElement element,
        ref Type type)
    {
        if (element.IsNil())
            return null;

        return Activator.CreateInstance(
                    type,
                    [.. element
                            .GetElement()
                            .Node!
                            .AsObject()
                            .Where(kvp => kvp.Value is JsonObject)
                            .Select(kvp => ValueTransform(kvp.Value?.AsObject()?.GetOneOf(ConstantTypes) ?? throw new SerializationException(kvp.Value?.AsObject().GetPath())).Item1)
                    ]);
    }

    static IEnumerable TransformArray(
        Type elementType,
        int length,
        IEnumerable elements)
    {
        var array = Array.CreateInstance(elementType, length);
        var itr = elements.GetEnumerator();

        itr.MoveNext();
        for (int i = 0; i < length; i++, itr.MoveNext())
            array.SetValue(itr.Current, i);

        return array;
    }

    static object? TransformByteSequence(
        JElement element,
        ref Type type)
    {
        if (element.IsNil())
            return null;

        var bytes = Convert.FromBase64String(element.GetPropertyValue<string>()
                        ?? throw new SerializationException($"Could not find the Base64 string representation of the value at '{element.GetPath()}'."));

        if (element.TryGetLength(out var length) && length != bytes.Length)
            throw new SerializationException($"The actual length of byte sequence is different from the one specified at '{element.GetPath()}'.");

        if (type == typeof(byte[]))
            return bytes;
        if (type == typeof(ImmutableArray<byte>))
            return bytes.ToImmutableArray();
        if (type == typeof(ArraySegment<byte>))
            return new ArraySegment<byte>(bytes);
        if (type == typeof(Memory<byte>) || type == typeof(Span<byte>))
            return new Memory<byte>(bytes);
        if (type == typeof(ReadOnlyMemory<byte>) || type == typeof(ReadOnlySpan<byte>))
            return new ReadOnlyMemory<byte>(bytes);

        throw new SerializationException($"Unexpected type of element '{element.Name}'.");
    }

    static object? TransformCollection(
        JElement element,
        ref Type type)
    {
        if (element.IsNil())
            return null;

        var jsArray = element.GetArray(Vocabulary.Value);
        int length = jsArray.Count;

        if (element.TryGetLength(out var len) && len != length)
            throw new SerializationException($"The actual length of a collection is different from the one specified at '{element.GetPath()}'.");

        Type elementType = type.IsArray
                                ? type.GetElementType() ?? throw new SerializationException($"Could not get the type of the arrayFlags elements at '{element.GetPath()}'.")
                                : type.IsGenericType
                                    ? type.GetGenericArguments()[0]
                                    : throw new SerializationException($"Could not get the type of the arrayFlags elements at '{element.GetPath()}'.");

        if (elementType == typeof(void))
            throw new SerializationException($"Constant expression's type specified as type 'void' at '{element.GetPath()}'.");

        var elements = jsArray
                        .Select(
                            (e, i) =>
                            {
                                if (e is null)
                                    return null;

                                var (elem, t) = ValueTransform(e.AsObject()?.GetOneOf(ConstantTypes)
                                                                    ?? throw new SerializationException($"Expected a JsonObject for each arrayFlags item at '{e.GetPath()}'."));

                                if (!elementType.IsAssignableFrom(t))
                                    throw new SerializationException($"The actual type of the element at '{e.GetPath()}' is not compatible with the arrayFlags element type '{elementType.FullName}'");

                                return elem;
                            });

        if (type.IsArray)
            return TransformArray(elementType, length, elements);

        if (!type.IsGenericType)
            throw new SerializationException($"The collection in `{element.Name}` must be either arrayFlags or a generic collection.");

        // TODO: this is pretty wonky but I don't know how to detect the internal "SmallValueTypeComparableFrozenSet`1" or "SmallFrozenSet`1"
        var genericType = type.Name.EndsWith("FrozenSet`1")
                                ? typeof(FrozenSet<>)
                                : type.GetGenericTypeDefinition();

        if (_sequenceBuilders.TryGetValue(genericType, out var seqTransform))
            return seqTransform(genericType, elementType, length, elements);

        throw new SerializationException($"Don't know how to deserialize '{type.FullName}'.");
    }

    readonly struct DictBuildData(IDictionary dictionary, Type[] keyValueTypes, Func<IDictionary, object?> convert)
    {
        /// <summary>
        /// Gets or sets the initial dictionary to build.
        /// </summary>
        public IDictionary Dictionary { get; } = dictionary;

        /// <summary>
        /// Gets or sets the types of the key and the value in the dictionary.
        /// </summary>
        public Type[] KeyValueTypes { get; } = keyValueTypes;

        /// <summary>
        /// Converts the <see cref="Dictionary"/> to its final dictionary type, incl. <see cref="Hashtable"/>
        /// </summary>
        public Func<IDictionary, object?> ConvertToTargetType { get; } = convert;

        public void Deconstruct(out IDictionary dictionary, out Type[] keyValueTypes, out Func<IDictionary, object?> convert)
        {
            dictionary = Dictionary;
            keyValueTypes = KeyValueTypes;
            convert = ConvertToTargetType;
        }
    }

    delegate DictBuildData PrepForDict(Type[] kvTypes);

    static DictBuildData PrepForDictionary(Type[] kvTypes)
    {
        var dict = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(kvTypes[0], kvTypes[1])) as IDictionary
                                    ?? throw new InternalTransformErrorException($"Could not create object of type 'Dictionary<{kvTypes[0].Name},{kvTypes[1].Name}>'.");
        return new(dict, kvTypes, d => d);
    }

    static DictBuildData PrepForReadOnlyDictionary(Type[] kvTypes)
    {
        var dict = Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(kvTypes[0], kvTypes[1])) as IDictionary
                                    ?? throw new InternalTransformErrorException($"Could not create object of type 'Dictionary<{kvTypes[0].Name},{kvTypes[1].Name}>'.");
        var ctor = typeof(ReadOnlyDictionary<,>)
                        .MakeGenericType(kvTypes[0], kvTypes[1])
                        .GetConstructors()
                        .Where(ci => ci.GetParameters().Length == 1)
                        .Single()
                    ;
        object? convert(IDictionary d) => (ctor!.Invoke([d]) as IDictionary)
                                                    ?? throw new InternalTransformErrorException($"Could not create object of type 'ReadOnlyDictionary<{kvTypes[0].Name},{kvTypes[1].Name}>'.");

        return new(dict, kvTypes, convert);
    }

    static DictBuildData PrepForSortedDictionary(Type[] kvTypes)
    {
        var dict = Activator.CreateInstance(typeof(SortedDictionary<,>).MakeGenericType(kvTypes[0], kvTypes[1])) as IDictionary
                                    ?? throw new InternalTransformErrorException($"Could not create object of type 'SortedDictionary<{kvTypes[0].Name},{kvTypes[1].Name}>'.");

        return new(dict, kvTypes, d => d);
    }

    static DictBuildData PrepForImmutableDictionary(Type[] kvTypes)
    {
        var dict = Activator.CreateInstance(typeof(Dictionary<,>)
                            .MakeGenericType(kvTypes[0], kvTypes[1])) as IDictionary
                                    ?? throw new InternalTransformErrorException($"Could not create object of type 'Dictionary<{kvTypes[0].Name},{kvTypes[1].Name}>'.");
        return new(dict, kvTypes, d => _toImmutableDictionary.MakeGenericMethod(kvTypes).Invoke(null, [d]));
    }

    static DictBuildData PrepForImmutableSortedDictionary(Type[] kvTypes)
    {
        var dict = Activator.CreateInstance(typeof(SortedDictionary<,>)
                            .MakeGenericType(kvTypes[0], kvTypes[1])) as IDictionary
                                    ?? throw new InternalTransformErrorException($"Could not create object of type 'SortedDictionary<{kvTypes[0].Name},{kvTypes[1].Name}>'.");
        return new(dict, kvTypes, d => _toImmutableSortedDictionary.MakeGenericMethod(kvTypes).Invoke(null, [d]));
    }

    static DictBuildData PrepForFrozenDictionary(Type[] kvTypes)
    {
        var dict = Activator.CreateInstance(typeof(Dictionary<,>)
                            .MakeGenericType(kvTypes[0], kvTypes[1])) as IDictionary
                                    ?? throw new InternalTransformErrorException($"Could not create object of type 'Dictionary<{kvTypes[0].Name},{kvTypes[1].Name}>'.");
        return new(dict, kvTypes, d => _toFrozenDictionary.MakeGenericMethod(kvTypes).Invoke(null, [d, null]));
    }

    static DictBuildData PrepForConcurrentDictionary(Type[] kvTypes)
    {
        var ctor = typeof(ConcurrentDictionary<,>)
                        .MakeGenericType(kvTypes[0], kvTypes[1])
                        .GetConstructors()
                        .Where(ci => ci.GetParameters().Length == 0)
                        .Single()
                    ;
        var dict = ctor!.Invoke([]) as IDictionary
                                    ?? throw new InternalTransformErrorException($"Could not create object of type 'ConcurrentDictionary<{kvTypes[0].Name},{kvTypes[1].Name}>'.");
        return new(dict, kvTypes, d => d);
    }

    static IEnumerable<KeyValuePair<Type, PrepForDict>> TypeToPrep()
    {
        yield return new(typeof(Hashtable), kvTypes => new(new Hashtable(), kvTypes, d => d));
        yield return new(typeof(Dictionary<,>), PrepForDictionary);
        yield return new(typeof(ReadOnlyDictionary<,>), PrepForReadOnlyDictionary);
        yield return new(typeof(SortedDictionary<,>), PrepForSortedDictionary);
        yield return new(typeof(ImmutableDictionary<,>), PrepForImmutableDictionary);
        yield return new(typeof(ImmutableSortedDictionary<,>), PrepForImmutableSortedDictionary);
        yield return new(typeof(FrozenDictionary<,>), PrepForFrozenDictionary);
        yield return new(typeof(ConcurrentDictionary<,>), PrepForConcurrentDictionary);
    }

    static FrozenDictionary<Type, PrepForDict> _typeToPrep = TypeToPrep().ToFrozenDictionary();

    static DictBuildData PrepForDictionary(Type dictType)
    {
        if (!dictType.IsAssignableTo(typeof(IDictionary)))
            throw new InternalTransformErrorException($"The type of the element is not 'IDictionary'.");

        Type[] kvTypes;
        Type genericType = typeof(Hashtable);

        if (dictType.IsGenericType)
        {
            var dictInterface = dictType.GetInterfaces()
                                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>));

            if (dictInterface is not null)
            {
                kvTypes = dictInterface.GetGenericArguments();
            }
            else
            {
                kvTypes = dictType.GetGenericArguments();
            }

            genericType = dictType.GetGenericTypeDefinition()
                            ?? throw new InternalTransformErrorException($"Could not get the generic type definition of a generic type '{dictType.FullName}'.");
        }
        else
        if (dictType == typeof(Hashtable))
            kvTypes = [typeof(object), typeof(object)];
        else
            throw new InternalTransformErrorException($"Don't know how to deserialize '{dictType}'.");

        if (kvTypes.Length is not 2)
            throw new InternalTransformErrorException("The elements of 'Dictionary' do not have key-type and element-type.");

        if (_typeToPrep.TryGetValue(genericType, out var prep))
            return prep(kvTypes);
        else
        // TODO: this is pretty wonky but I don't know how to detect the internal "SmallValueTypeComparableFrozenDictionary`2" or "SmallFrozenDictionary`2"
        if (dictType.IsAssignableTo(typeof(IDictionary)) && dictType.FullName?.Contains("FrozenDictionary") == true)
            return PrepForFrozenDictionary(kvTypes);

        throw new InternalTransformErrorException($"Don't know how to deserialize '{dictType}'.");
    }

    static object? TransformDictionary(
        JElement element,
        ref Type type)
    {
        if (element.IsNil())
            return null;

        var kvpArray = element.GetArray()
                            ?? throw new SerializationException($"Could not get the dictionary at '{element.GetPath()}'.");

        var (dict, kvTypes, convertToFinal) = PrepForDictionary(type);
        foreach (var kve in kvpArray)
        {
            if (kve is not JsonObject kvElement)
                throw new SerializationException($"Expected arrayFlags of key-value objects at '{element.GetPath()}'.");

            var (key, kt) = ValueTransform(
                                new JElement(
                                        kvElement
                                            .GetObject(Vocabulary.Key)
                                            .GetOneOf(ConstantTypes)));

            if (key is null)
                throw new SerializationException($"Could not transform a value of a key at '{kvElement.GetPath()}'.");

            if (!kt.IsAssignableTo(kvTypes[0]))
                throw new SerializationException($"Invalid type of a key at '{kvElement.GetPath()}'.");

            var (value, vt) = ValueTransform(
                                new JElement(
                                        kvElement.GetObject(Vocabulary.Value).GetOneOf(ConstantTypes)));

            if (!vt.IsAssignableTo(kvTypes[1]))
                throw new SerializationException($"Invalid type of a value at '{kvElement.GetPath()}'.");

            dict[key] = value;
        }

        return convertToFinal(dict);
    }
}
