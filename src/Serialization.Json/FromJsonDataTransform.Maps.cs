namespace vm2.Linq.Expressions.Serialization.Json;

#pragma warning disable IDE0049

static partial class FromJsonDataTransform
{
    static IEnumerable<KeyValuePair<string, Transformation>> ConstantTransformations()
    {
        yield return new(Vocabulary.Boolean, (x, ref t) => x.GetValue<bool>());
        yield return new(Vocabulary.Byte, (x, ref t) => x.GetValue<byte>());
        yield return new(Vocabulary.Char, (x, ref t) => JsonToChar(x));
        yield return new(Vocabulary.Double, (x, ref t) => JsonToDouble(x));
        yield return new(Vocabulary.Float, (x, ref t) => JsonToFloat(x));
        yield return new(Vocabulary.Int, (x, ref t) => x.GetValue<int>());
        yield return new(Vocabulary.IntPtr, (x, ref t) => JsonToIntPtr(x));
        yield return new(Vocabulary.Long, (x, ref t) => JsonToLong(x));
        yield return new(Vocabulary.SignedByte, (x, ref t) => x.GetValue<sbyte>());
        yield return new(Vocabulary.Short, (x, ref t) => x.GetValue<short>());
        yield return new(Vocabulary.UnsignedInt, (x, ref t) => x.GetValue<uint>());
        yield return new(Vocabulary.UnsignedIntPtr, (x, ref t) => JsonToUIntPtr(x));
        yield return new(Vocabulary.UnsignedLong, (x, ref t) => JsonToULong(x));
        yield return new(Vocabulary.UnsignedShort, (x, ref t) => x.GetValue<ushort>());

        yield return new(Vocabulary.DateTime, (x, ref t) => JsonToDateTime(x));
        yield return new(Vocabulary.DateTimeOffset, (x, ref t) => JsonToDateTimeOffset(x));
        yield return new(Vocabulary.Duration, (x, ref t) => JsonToTimeSpan(x));
        yield return new(Vocabulary.DBNull, (x, ref t) => DBNull.Value);
        yield return new(Vocabulary.Decimal, (x, ref t) => JsonToDecimal(x));
        yield return new(Vocabulary.Guid, (x, ref t) => JsonToGuid(x));
        yield return new(Vocabulary.Half, (x, ref t) => JsonToHalf(x));
        yield return new(Vocabulary.String, (x, ref t) => x.GetValue<string>());
        yield return new(Vocabulary.Uri, (x, ref t) => JsonToUri(x));

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
    internal static readonly FrozenSet<string> ConstantTypes = _constantTransformations.Keys.ToFrozenSet();

    static char JsonToChar(JElement x)
    {
        var s = x.GetValue<string>();

        if (string.IsNullOrEmpty(s))
            throw new SerializationException($"Could not convert the valueElement of property `{x.Name}` to `char` - the string is `null` or empty.");

        return s[0];
    }

    static double JsonToDouble(JElement x)
        => x.TryGetValue<string>(out var fpStr)
                ? fpStr switch {
                    Vocabulary.NaN => double.NaN,
                    Vocabulary.NegInfinity => double.NegativeInfinity,
                    Vocabulary.PosInfinity => double.PositiveInfinity,
                    _ => double.Parse(fpStr!)
                }
                : x.GetValue<double>();

    static float JsonToFloat(JElement x)
        => x.TryGetValue<string>(out var fpStr)
            ? fpStr switch {
                Vocabulary.NaN => float.NaN,
                Vocabulary.NegInfinity => float.NegativeInfinity,
                Vocabulary.PosInfinity => float.PositiveInfinity,
                _ => float.Parse(fpStr!)
            }
            : x.GetValue<float>();

    static Half JsonToHalf(JElement x)
        => x.TryGetValue<string>(out var fpStr)
            ? fpStr switch {
                Vocabulary.NaN => Half.NaN,
                Vocabulary.NegInfinity => Half.NegativeInfinity,
                Vocabulary.PosInfinity => Half.PositiveInfinity,
                _ => Half.Parse(fpStr!)
            }
            : (Half)x.GetValue<float>();

    static long JsonToLong(JElement x)
        => x.Node?.GetValueKind() switch {
            JsonValueKind.Number => x.GetValue<long>(),
            JsonValueKind.String => long.Parse(x.GetValue<string>()!),
            _ => throw new SerializationException($"Could not convert the valueElement of property `{x.Name}` to `long` - unexpected JSON type {x.Node?.GetValueKind()}."),
        };

    static ulong JsonToULong(JElement x)
        => x.Node?.GetValueKind() switch {
            JsonValueKind.Number => x.GetValue<ulong>(),
            JsonValueKind.String => ulong.Parse(x.GetValue<string>()!),
            _ => throw new SerializationException($"Could not convert the valueElement of property `{x.Name}` to `unsigned long` - unexpected JSON type {x.Node?.GetValueKind()}."),
        };

    static IntPtr JsonToIntPtr(JElement x)
    {
        if (x.GetValueKind() is JsonValueKind.String)
        {
            var ptrStr = x.GetValue<string>();

            if (string.IsNullOrWhiteSpace(ptrStr))
                throw new SerializationException($"Could not convert the valueElement of property `{x.Name}` to `IntPtr` - the string is `null`, or empty, or consists of whitespaces only.");

            return checked(
                Environment.Is64BitProcess
                    ? (IntPtr)Int64.Parse(ptrStr)
                    : Int32.Parse(ptrStr));
        }

        return checked(
            Environment.Is64BitProcess
                ? (IntPtr)x.GetValue<Int64>()
                : x.GetValue<Int32>());
    }

    static UIntPtr JsonToUIntPtr(JElement x)
    {
        if (x.GetValueKind() is JsonValueKind.String)
        {
            var ptrStr = x.GetValue<string>();

            if (string.IsNullOrWhiteSpace(ptrStr))
                throw new SerializationException($"Could not convert the valueElement of property `{x.Name}` to `IntPtr` - the string is `null`, or empty, or consists of whitespaces only.");

            return checked(
                Environment.Is64BitProcess
                    ? (UIntPtr)UInt64.Parse(ptrStr)
                    : UInt32.Parse(ptrStr));
        }

        return checked(
            Environment.Is64BitProcess
                ? (UIntPtr)x.GetValue<UInt64>()
                : x.GetValue<UInt32>());
    }

    static string GetJsonStringToParse(JElement x, string typeName)
    {
        if (!x.TryGetValue<string>(out var str))
            throw new SerializationException($"Could not convert the valueElement of property `{x.Name}` to `{typeName}` - the JSON valueElement is not string.");

        if (string.IsNullOrWhiteSpace(str))
            throw new SerializationException($"Could not convert the valueElement of property `{x.Name}` to `{typeName}` - the JSON string is a null, empty, or consists of whitespace characters only.");

        return str;
    }

    static DateTime JsonToDateTime(JElement x)
        => DateTime.Parse(GetJsonStringToParse(x, nameof(DateTime)), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

    static DateTimeOffset JsonToDateTimeOffset(JElement x)
        => DateTimeOffset.Parse(GetJsonStringToParse(x, nameof(DateTimeOffset)), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

    const string durationRegex = @"^(?<neg>-)?P(((?<days3>[0-9]+)D|(?<months2>[0-9]+)M((?<days2>[0-9]+)D)?|(?<years1>[0-9]+)Y((?<months1>[0-9]+)M((?<days1>[0-9]+)D)?)?)"+
                                 @"(T(((?<hours2>[0-9]+)H((?<minutes4>[0-9]+)M((?<seconds6>[0-9]+)S)?)?|(?<minutes3>[0-9]+)M((?<seconds5>[0-9]+)S)?|(?<seconds4>[0-9]+)S)))?|"+
                                 @"T(((?<hours1>[0-9]+)H((?<minutes2>[0-9]+)M((?<seconds3>[0-9]+)S)?)?|(?<minutes1>[0-9]+)M((?<seconds2>[0-9]+)S)?|(?<seconds1>[0-9]+)S))|(?<weeks>[0-9]+)W)$";

    [GeneratedRegex(durationRegex, RegexOptions.Singleline |
                                   RegexOptions.IgnoreCase |
                                   RegexOptions.IgnorePatternWhitespace |
                                   RegexOptions.CultureInvariant |
                                   RegexOptions.Compiled)]
    private static partial Regex DurationRegex();

    static TimeSpan JsonToTimeSpan(JElement x)
    {
        var str = GetJsonStringToParse(x, nameof(TimeSpan));
        var match = DurationRegex().Match(str);

        if (!match.Success)
            throw new SerializationException($"Could not convert the valueElement of property `{x.Name}` to `{nameof(TimeSpan)}` - the JSON string does not represent a valid ISO8601 duration of time.");

        var negative =  match.Groups["neg"].Success && match.Groups["neg"].Value == "-";

        var seconds =   match.Groups["seconds1"].Success ? match.Groups["seconds1"].Value :
                        match.Groups["seconds2"].Success ? match.Groups["seconds2"].Value :
                        match.Groups["seconds3"].Success ? match.Groups["seconds3"].Value :
                        match.Groups["seconds4"].Success ? match.Groups["seconds4"].Value :
                        match.Groups["seconds5"].Success ? match.Groups["seconds5"].Value :
                        match.Groups["seconds6"].Success ? match.Groups["seconds6"].Value : "0";

        var minutes =   match.Groups["minutes1"].Success ? match.Groups["minutes1"].Value :
                        match.Groups["minutes2"].Success ? match.Groups["minutes2"].Value :
                        match.Groups["minutes3"].Success ? match.Groups["minutes3"].Value :
                        match.Groups["minutes4"].Success ? match.Groups["minutes4"].Value : "0";

        var hours =     match.Groups["hours1"].Success ? match.Groups["hours1"].Value :
                        match.Groups["hours2"].Success ? match.Groups["hours2"].Value : "0";

        var days =      match.Groups["days1"].Success ? match.Groups["days1"].Value :
                        match.Groups["days2"].Success ? match.Groups["days2"].Value :
                        match.Groups["days3"].Success ? match.Groups["days3"].Value : "0";

        var months =    match.Groups["days1"].Success ? match.Groups["days1"].Value :
                        match.Groups["days2"].Success ? match.Groups["days2"].Value : "0";

        var years =     match.Groups["years1"].Success ? match.Groups["years1"].Value : "0";

        var weeks =     match.Groups["weeks"].Success ? match.Groups["weeks"].Value : "0";

        // having years and months in the duration is clearly gray area for ISO8601. Some suggest to use the banking loan depreciation type of cycle of 360/30.
        // Our serialization counterpart never puts years and months - just days, etc.
        var ts = new TimeSpan(int.Parse(years) * 360 + int.Parse(months) * 30 + int.Parse(weeks) + int.Parse(days), int.Parse(hours), int.Parse(minutes), int.Parse(seconds));

        return negative ? ts.Negate() : ts;
    }

    static decimal JsonToDecimal(JElement x)
        => decimal.Parse(GetJsonStringToParse(x, nameof(Decimal)), CultureInfo.InvariantCulture);

    static Guid JsonToGuid(JElement x)
        => Guid.Parse(GetJsonStringToParse(x, nameof(Guid)));

    static Uri JsonToUri(JElement x)
        => new UriBuilder(GetJsonStringToParse(x, nameof(Uri))).Uri;

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
        yield return new(typeof(ArraySegment<>), (gt, et, len, seq) => BuildCollectionFromArray(gt, et, TransformArray(et, len, seq)));
        yield return new(typeof(Memory<>), (gt, et, len, seq) => BuildCollectionFromArray(gt, et, TransformArray(et, len, seq)));
        yield return new(typeof(ReadOnlyMemory<>), (gt, et, len, seq) => BuildCollectionFromArray(gt, et, TransformArray(et, len, seq)));
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
