namespace vm2.Linq.Expressions.Serialization.Json;

partial class ToJsonDataTransform
{
    /// <summary>
    /// The maximum long number that can be expressed as &quot;JSON integer&quot; without loosing fidelity.
    /// </summary>
    /// <remarks>
    /// In JavaScript, the maximum safe integer is 2^53 - 1, which is 9007199254740991. This is because JavaScript uses
    /// double-precision floating-point format numbers as specified in IEEE 754, and can only safely represent integers
    /// between [-(2^53-1), 2^53 - 1].
    /// Therefore we serialize numbers outside of that range as strings, e.g. <c>&quot;9007199254740992&quot;</c>.
    /// </remarks>
    public const long MaxJsonInteger = 9_007_199_254_740_991L;

    /// <summary>
    /// The minimum long number that can be expressed as &quot;JSON integer&quot; without loosing fidelity.
    /// </summary>
    /// <remarks>
    /// In JavaScript, the maximum safe integer is 2^53 - 1, which is 9007199254740991. This is because JavaScript uses
    /// double-precision floating-point format numbers as specified in IEEE 754, and can only safely represent integers
    /// from the range [-(2^53-1), 2^53-1].
    /// Therefore we serialize numbers outside of that range as strings, e.g. <c>&quot;-9007199254740992&quot;</c>.
    /// </remarks>
    public const long MinJsonInteger = -MaxJsonInteger;

    static T Is<T>(object? v) where T : struct
        => v is T tv ? tv : throw new InternalTransformErrorException($"Expected {typeof(T).Name} v but got {(v is null ? "null" : v.GetType().Name)}");

    static T? Is<T>(object? v, bool nullable = true) where T : class
        => v is T || (nullable && v is null) ? (T?)v : throw new InternalTransformErrorException($"Expected {typeof(T).Name} v but got {(v is null ? "null" : v.GetType().Name)}");

    #region constant ToJson transforms
    static IEnumerable<KeyValuePair<Type, TransformConstant>> ConstantTransformsDict()
    {
        yield return new(typeof(bool), (v, _) => new JElement(Vocabulary.Boolean, JsonValue.Create(Is<bool>(v))));
        yield return new(typeof(byte), (v, _) => new JElement(Vocabulary.Byte, JsonValue.Create(Is<byte>(v))));
        yield return new(typeof(char), (v, _) => new JElement(Vocabulary.Char, JsonValue.Create(Is<char>(v).ToString())));
        yield return new(typeof(double), (v, _) => new JElement(Vocabulary.Double, DoubleToJson(Is<double>(v))));
        yield return new(typeof(float), (v, _) => new JElement(Vocabulary.Float, FloatToJson(Is<float>(v))));
        yield return new(typeof(int), (v, _) => new JElement(Vocabulary.Int, JsonValue.Create(Is<int>(v))));
        yield return new(typeof(IntPtr), (v, _) => new JElement(Vocabulary.IntPtr, PtrToJson(Is<IntPtr>(v))));
        yield return new(typeof(long), (v, _) => new JElement(Vocabulary.Long, Is<long>(v) is <= MaxJsonInteger and >= MinJsonInteger ? JsonValue.Create((long)v!) : JsonValue.Create(v!.ToString())));
        yield return new(typeof(sbyte), (v, _) => new JElement(Vocabulary.SignedByte, JsonValue.Create(Is<sbyte>(v))));
        yield return new(typeof(short), (v, _) => new JElement(Vocabulary.Short, JsonValue.Create(Is<short>(v))));
        yield return new(typeof(uint), (v, _) => new JElement(Vocabulary.UnsignedInt, JsonValue.Create(Is<uint>(v))));
        yield return new(typeof(UIntPtr), (v, _) => new JElement(Vocabulary.UnsignedIntPtr, PtrToJson(Is<UIntPtr>(v))));
        yield return new(typeof(ulong), (v, _) => new JElement(Vocabulary.UnsignedLong, Is<ulong>(v) is <= MaxJsonInteger ? JsonValue.Create((long)(ulong)v!) : JsonValue.Create(v!.ToString())));
        yield return new(typeof(ushort), (v, _) => new JElement(Vocabulary.UnsignedShort, JsonValue.Create(Is<ushort>(v))));

        yield return new(typeof(DateTime), (v, _) => new JElement(Vocabulary.DateTime, JsonValue.Create(Is<DateTime>(v).ToString("o"))));
        yield return new(typeof(DateTimeOffset), (v, _) => new JElement(Vocabulary.DateTimeOffset, JsonValue.Create(Is<DateTimeOffset>(v).ToString("o"))));
        yield return new(typeof(TimeSpan), (v, _) => new JElement(Vocabulary.Duration, JsonValue.Create(Duration(Is<TimeSpan>(v)))));
        yield return new(typeof(DBNull), (v, _) => new JElement(Vocabulary.DBNull));
        yield return new(typeof(decimal), (v, _) => new JElement(Vocabulary.Decimal, JsonValue.Create(Is<decimal>(v).ToString("G", CultureInfo.InvariantCulture))));
        yield return new(typeof(Guid), (v, _) => new JElement(Vocabulary.Guid, JsonValue.Create(Is<Guid>(v).ToString())));
        yield return new(typeof(Half), (v, _) => new JElement(Vocabulary.Half, HalfToJson(Is<Half>(v))));
        yield return new(typeof(string), (v, _) => new JElement(Vocabulary.String, JsonValue.Create(Is<string>(v))));
        yield return new(typeof(Uri), (v, _) => new JElement(Vocabulary.Uri, JsonValue.Create(Is<Uri>(v)?.ToString())));
    }

    static FrozenDictionary<Type, TransformConstant> _constantTransforms = ConstantTransformsDict().ToFrozenDictionary();

    static JsonValue DoubleToJson(double d)
        => d switch {
            double.NaN => JsonValue.Create(Vocabulary.NaN),
            double.NegativeInfinity => JsonValue.Create(Vocabulary.NegInfinity),
            double.PositiveInfinity => JsonValue.Create(Vocabulary.PosInfinity),
            _ => JsonValue.Create(d),
        };

    static JsonValue FloatToJson(float f)
        => f switch {
            float.NaN => JsonValue.Create(Vocabulary.NaN),
            float.NegativeInfinity => JsonValue.Create(Vocabulary.NegInfinity),
            float.PositiveInfinity => JsonValue.Create(Vocabulary.PosInfinity),
            _ => JsonValue.Create(f),
        };

    static JsonValue HalfToJson(Half h)
    {
        if (Half.IsNaN(h))
            return JsonValue.Create(Vocabulary.NaN);
        if (Half.IsPositiveInfinity(h))
            return JsonValue.Create(Vocabulary.PosInfinity);
        if (Half.IsNegativeInfinity(h))
            return JsonValue.Create(Vocabulary.NegInfinity);
        return JsonValue.Create((float)h);
    }

#pragma warning disable IDE0049 // Simplify Names
    static JsonValue PtrToJson(IntPtr v)
        => checked(
                Environment.Is64BitProcess
                    ? ((Int64)v is <= MaxJsonInteger and >= MinJsonInteger ? JsonValue.Create((Int64)v) : JsonValue.Create(((Int64)v).ToString()))
                    : JsonValue.Create((Int32)v)
            );

    static JsonValue PtrToJson(UIntPtr v)
        => checked(
                Environment.Is64BitProcess
                    ? JsonValue.Create(((UInt64)v).ToString()) // Should be `((UInt64)v is <= MaxJsonInteger ? JsonValue.Create((UInt64)v) : JsonValue.Create(((UInt64)v).ToString()))` but the Json.Schema doesn't like it
                    : JsonValue.Create((UInt32)v)
        );
#pragma warning restore IDE0049 // Simplify Names

    static string Duration(TimeSpan ts)
        => ts.ToString($@"{(ts < TimeSpan.Zero ? @"\-" : "")}\P{(ts.Days != 0 ? @"d\D" : "")}\Th\Hm\Ms\S");
    #endregion
}
