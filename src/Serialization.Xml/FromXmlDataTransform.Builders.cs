namespace vm2.Linq.Expressions.Serialization.Xml;

partial class FromXmlDataTransform
{
    static object BuildCollectionFromEnumerable(
        Type genericType,
        Type elementType,
        IEnumerable elements)
    {
        var ctor = genericType
                        .MakeGenericType(elementType)
                        .GetConstructors()
                        .Where(ci => ci.ConstructorHas1EnumerableParameter())
                        .Single()
                        ;
        var collection = CastSequence(elements, elementType);

        return ctor!.Invoke([collection]);
    }

    static object BuildCollectionFromArray(
        Type genericType,
        Type elementType,
        IEnumerable elements)
    {
        var ctor = genericType
                        .MakeGenericType(elementType)
                        .GetConstructors()
                        .Where(ci => ci.ConstructorHas1ArrayParameter())
                        .Single()
                        ;
        var collection = CastSequence(elements, elementType);

        return ctor!.Invoke([collection]);
    }

    static object BuildCollectionFromList(
        Type genericType,
        Type elementType,
        IEnumerable elements)
    {
        var ctor = genericType
                        .MakeGenericType(elementType)
                        .GetConstructors()
                        .Where(ci => ci.ConstructorHas1ListParameter())
                        .Single()
                        ;

        var collection = _toList.MakeGenericMethod(elementType).Invoke(null, [CastSequence(elements, elementType)]);

        return ctor!.Invoke([collection]);
    }

    static object BuildConcurrentBag(
        Type genericType,
        Type elementType,
        int _,
        IEnumerable elements)
    {
        var ctor = genericType
                        .MakeGenericType(elementType)
                        .GetConstructors()
                        .Where(ci => ci.ConstructorHas1EnumerableParameter())
                        .Single()
                        ;

        var collection = CastSequence(elements, elementType);

        collection = _reverse.MakeGenericMethod(elementType).Invoke(null, [collection]);

        return ctor!.Invoke([collection]);
    }

    static object BuildBlockingCollection(
        Type genericType,
        Type elementType,
        int length,
        IEnumerable elements)
    {
        var bcCtor = genericType
                        .MakeGenericType(elementType)
                        .GetConstructors()
                        .Where(ci => ci.GetParameters().Length == 0)
                        .Single()
                        ;
        var bc = bcCtor.Invoke([]);

        var addMi = genericType.MakeGenericType(elementType)
                        .GetMethods()
                        .Where(ci => ci.Name == "Add" && ci.GetParameters().Length == 1)
                        .Single()
                        ;
        var added = elements.Cast<object?>().Select(e => { addMi.Invoke(bc, [e]); return 1; }).Count();

        if (added != length)
            throw new InternalTransformErrorException("Could not add some or all members of the input sequence to BlockingCollection<T>.");

        return bc;
    }
}
