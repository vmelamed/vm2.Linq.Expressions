namespace vm2.Linq.Expressions.Serialization.Xml;

static partial class FromXmlDataTransform
{
    static IEnumerable<KeyValuePair<string, Transformation>> ConstantTransformations()
    {
        yield return new(Vocabulary.Boolean, (x, ref t) => XmlConvert.ToBoolean(x.Value));
        yield return new(Vocabulary.Byte, (x, ref t) => XmlConvert.ToByte(x.Value));
        yield return new(Vocabulary.Char, (x, ref t) => x.Value[0]);
        yield return new(Vocabulary.Double, (x, ref t) => XmlConvert.ToDouble(x.Value));
        yield return new(Vocabulary.Float, (x, ref t) => XmlConvert.ToSingle(x.Value));
        yield return new(Vocabulary.Int, (x, ref t) => XmlConvert.ToInt32(x.Value));
        yield return new(Vocabulary.IntPtr, (x, ref t) => XmlStringToPtr(x.Value));
        yield return new(Vocabulary.Long, (x, ref t) => XmlConvert.ToInt64(x.Value));
        yield return new(Vocabulary.SignedByte, (x, ref t) => XmlConvert.ToSByte(x.Value));
        yield return new(Vocabulary.Short, (x, ref t) => XmlConvert.ToInt16(x.Value));
        yield return new(Vocabulary.UnsignedInt, (x, ref t) => XmlConvert.ToUInt32(x.Value));
        yield return new(Vocabulary.UnsignedIntPtr, (x, ref t) => XmlStringToUPtr(x.Value));
        yield return new(Vocabulary.UnsignedLong, (x, ref t) => XmlConvert.ToUInt64(x.Value));
        yield return new(Vocabulary.UnsignedShort, (x, ref t) => XmlConvert.ToUInt16(x.Value));

        yield return new(Vocabulary.DateTime, (x, ref t) => XmlConvert.ToDateTime(x.Value, XmlDateTimeSerializationMode.RoundtripKind));
        yield return new(Vocabulary.DateTimeOffset, (x, ref t) => XmlConvert.ToDateTimeOffset(x.Value, "O"));
        yield return new(Vocabulary.Duration, (x, ref t) => XmlConvert.ToTimeSpan(x.Value));
        yield return new(Vocabulary.DBNull, (x, ref t) => DBNull.Value);
        yield return new(Vocabulary.Decimal, (x, ref t) => XmlConvert.ToDecimal(x.Value));
        yield return new(Vocabulary.Guid, (x, ref t) => XmlConvert.ToGuid(x.Value));
        yield return new(Vocabulary.Half, (x, ref t) => (Half)XmlConvert.ToDouble(x.Value));
        yield return new(Vocabulary.String, (x, ref t) => x.IsNil() ? null : x.Value);
        yield return new(Vocabulary.Uri, (x, ref t) => x.IsNil() ? null : new Uri(x.Value));

        yield return new(Vocabulary.Anonymous, TransformAnonymous);
        yield return new(Vocabulary.ByteSequence, TransformByteSequence);
        yield return new(Vocabulary.Sequence, TransformCollection);
        yield return new(Vocabulary.Dictionary, TransformDictionary);
        yield return new(Vocabulary.Enum, TransformEnum);
        yield return new(Vocabulary.Nullable, TransformNullable);
        yield return new(Vocabulary.Object, TransformObject);
        yield return new(Vocabulary.Tuple, TransformTuple);
        yield return new(Vocabulary.TupleItem, TransformTuple);
    }

    static readonly FrozenDictionary<string, Transformation> _constantTransformations = ConstantTransformations().ToFrozenDictionary();

    static IntPtr XmlStringToPtr(string v)
        => (Environment.Is64BitProcess
                ? checked((IntPtr)XmlConvert.ToInt64(v))
                : checked(XmlConvert.ToInt32(v)));

    static UIntPtr XmlStringToUPtr(string v)
        => (Environment.Is64BitProcess
                ? checked((UIntPtr)XmlConvert.ToUInt64(v))
                : checked(XmlConvert.ToUInt32(v)));

    #region cache some method info-s used in deserialization
    static MethodInfo _toFrozenSet                  = typeof(FrozenSet).GetMethod("ToFrozenSet") ?? throw new InternalTransformErrorException($"Could not get reflection of the method FrozenSet.ToFrozenSet");
    static MethodInfo _toImmutableArray             = typeof(ImmutableArray).GetMethods().Where(mi => mi.MethodHas1EnumerableParameter(nameof(ImmutableArray.ToImmutableArray))).Single();
    static MethodInfo _toImmutableHashSet           = typeof(ImmutableHashSet).GetMethods().Where(mi => mi.MethodHas1EnumerableParameter(nameof(ImmutableHashSet.ToImmutableHashSet))).Single();
    static MethodInfo _toImmutableSortedSet         = typeof(ImmutableSortedSet).GetMethods().Where(mi => mi.MethodHas1EnumerableParameter(nameof(ImmutableSortedSet.ToImmutableSortedSet))).Single();
    static MethodInfo _toImmutableList              = typeof(ImmutableList).GetMethods().Where(mi => mi.MethodHas1EnumerableParameter(nameof(ImmutableList.ToImmutableList))).Single();
    static MethodInfo _toImmutableQueue             = typeof(ImmutableQueue).GetMethods().Where(mi => mi.MethodHas1EnumerableParameter(nameof(ImmutableQueue.CreateRange))).Single();
    static MethodInfo _toImmutableStack             = typeof(ImmutableStack).GetMethods().Where(mi => mi.MethodHas1EnumerableParameter(nameof(ImmutableStack.CreateRange))).Single();
    static MethodInfo _toList                       = typeof(Enumerable).GetMethods().Where(mi => mi.MethodHas1EnumerableParameter(nameof(Enumerable.ToList))).Single();
    static MethodInfo _toHashSet                    = typeof(Enumerable).GetMethods().Where(mi => mi.MethodHas1EnumerableParameter(nameof(Enumerable.ToHashSet))).Single();
    static MethodInfo _cast                         = typeof(Enumerable).GetMethod("Cast")!;
    static MethodInfo _reverse                      = typeof(Enumerable).GetMethods().Where(mi => mi.MethodHas1EnumerableParameter(nameof(Enumerable.Reverse))).Single();
    static MethodInfo _toImmutableDictionary        = typeof(ImmutableDictionary).GetMethods().Where(mi => mi.MethodHas1EnumerableParameter(nameof(ImmutableDictionary.CreateRange))).Single();
    static MethodInfo _toImmutableSortedDictionary  = typeof(ImmutableSortedDictionary).GetMethods().Where(mi => mi.MethodHas1EnumerableParameter(nameof(ImmutableDictionary.CreateRange))).Single();
    static MethodInfo _toFrozenDictionary           = typeof(FrozenDictionary).GetMethods().Where(mi => mi.Name == nameof(FrozenDictionary.ToFrozenDictionary) && mi.GetParameters().Length == 2).Single();
    #endregion

    static object? CastSequence(IEnumerable sequence, Type elementType) => _cast.MakeGenericMethod(elementType).Invoke(null, [sequence]);

    static IEnumerable<KeyValuePair<Type, Func<Type, Type, int, IEnumerable, object?>>> SequenceBuilders()
    {
        yield return new(typeof(FrozenSet<>), (gt, et, len, seq) => _toFrozenSet.MakeGenericMethod(et).Invoke(null, [CastSequence(seq, et), null]));
        yield return new(typeof(ImmutableArray<>), (gt, et, len, seq) => _toImmutableArray.MakeGenericMethod(et).Invoke(null, [CastSequence(seq, et)]));
        yield return new(typeof(ImmutableHashSet<>), (gt, et, len, seq) => _toImmutableHashSet.MakeGenericMethod(et).Invoke(null, [CastSequence(seq, et)]));
        yield return new(typeof(ImmutableList<>), (gt, et, len, seq) => _toImmutableList.MakeGenericMethod(et).Invoke(null, [CastSequence(seq, et)]));
        yield return new(typeof(ImmutableSortedSet<>), (gt, et, len, seq) => _toImmutableSortedSet.MakeGenericMethod(et).Invoke(null, [CastSequence(seq, et)]));
        yield return new(typeof(ImmutableQueue<>), (gt, et, len, seq) => _toImmutableQueue.MakeGenericMethod(et).Invoke(null, [CastSequence(seq, et)]));
        yield return new(typeof(ImmutableStack<>), (gt, et, len, seq) => _toImmutableStack.MakeGenericMethod(et).Invoke(null, [CastSequence(seq.Cast<object?>().Reverse(), et)]));
        yield return new(typeof(List<>), (gt, et, len, seq) => _toList.MakeGenericMethod(et).Invoke(null, [CastSequence(seq, et)]));
        yield return new(typeof(HashSet<>), (gt, et, len, seq) => _toHashSet.MakeGenericMethod(et).Invoke(null, [CastSequence(seq, et)]));
        yield return new(typeof(ArraySegment<>), (gt, et, len, seq) => BuildCollectionFromArray(gt, et, TransformToArray(et, len, seq)));
        yield return new(typeof(Memory<>), (gt, et, len, seq) => BuildCollectionFromArray(gt, et, TransformToArray(et, len, seq)));
        yield return new(typeof(ReadOnlyMemory<>), (gt, et, len, seq) => BuildCollectionFromArray(gt, et, TransformToArray(et, len, seq)));
        yield return new(typeof(ConcurrentQueue<>), (gt, et, len, seq) => BuildCollectionFromEnumerable(gt, et, seq));
        yield return new(typeof(ConcurrentStack<>), (gt, et, len, seq) => BuildCollectionFromEnumerable(gt, et, seq.Cast<object?>().Reverse()));
        yield return new(typeof(Stack<>), (gt, et, len, seq) => BuildCollectionFromEnumerable(gt, et, seq.Cast<object?>().Reverse()));
        yield return new(typeof(Collection<>), (gt, et, len, seq) => BuildCollectionFromList(gt, et, seq.Cast<object?>().ToList()));
        yield return new(typeof(ReadOnlyCollection<>), (gt, et, len, seq) => BuildCollectionFromList(gt, et, seq.Cast<object?>().ToList()));
        yield return new(typeof(LinkedList<>), (gt, et, len, seq) => BuildCollectionFromEnumerable(gt, et, seq));
        yield return new(typeof(Queue<>), (gt, et, len, seq) => BuildCollectionFromEnumerable(gt, et, seq.Cast<object?>()));
        yield return new(typeof(SortedSet<>), (gt, et, len, seq) => BuildCollectionFromEnumerable(gt, et, seq));
        yield return new(typeof(BlockingCollection<>), BuildBlockingCollection);
        yield return new(typeof(ConcurrentBag<>), BuildConcurrentBag);
    }

    static readonly FrozenDictionary<Type, Func<Type, Type, int, IEnumerable, object?>> _sequenceBuilders = SequenceBuilders().ToFrozenDictionary();
}
