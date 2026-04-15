namespace vm2.Linq.Expressions.Serialization.Xml;

partial class ToXmlDataTransform
{
    static T Is<T>(object? v) where T : struct
        => v is T tv ? tv : throw new InternalTransformErrorException($"Expected {typeof(T).Name} v but got {(v is null ? "null" : v.GetType().Name)}");

    static T? Is<T>(object? v, bool nullable = true) where T : class
        => v is T || (nullable && v is null) ? (T?)v : throw new InternalTransformErrorException($"Expected {typeof(T).Name} v but got {(v is null ? "null" : v.GetType().Name)}");

    static IEnumerable<KeyValuePair<Type, TransformConstant>> ConstantTransformsDict()
    {
        yield return new(typeof(bool), (v, t) => new XElement(ElementNames.Boolean, XmlConvert.ToString(Is<bool>(v))));
        yield return new(typeof(byte), (v, t) => new XElement(ElementNames.Byte, XmlConvert.ToString(Is<byte>(v))));
        yield return new(typeof(char), (v, t) => new XElement(ElementNames.Char, XmlConvert.ToString(Is<char>(v))));
        yield return new(typeof(double), (v, t) => new XElement(ElementNames.Double, XmlConvert.ToString(Is<double>(v))));
        yield return new(typeof(float), (v, t) => new XElement(ElementNames.Float, XmlConvert.ToString(Is<float>(v))));
        yield return new(typeof(int), (v, t) => new XElement(ElementNames.Int, XmlConvert.ToString(Is<int>(v))));
        yield return new(typeof(IntPtr), (v, t) => new XElement(ElementNames.IntPtr, PtrToXmlString(Is<IntPtr>(v))));
        yield return new(typeof(long), (v, t) => new XElement(ElementNames.Long, XmlConvert.ToString(Is<long>(v))));
        yield return new(typeof(sbyte), (v, t) => new XElement(ElementNames.SignedByte, XmlConvert.ToString(Is<sbyte>(v))));
        yield return new(typeof(short), (v, t) => new XElement(ElementNames.Short, XmlConvert.ToString(Is<short>(v))));
        yield return new(typeof(uint), (v, t) => new XElement(ElementNames.UnsignedInt, XmlConvert.ToString(Is<uint>(v))));
        yield return new(typeof(UIntPtr), (v, t) => new XElement(ElementNames.UnsignedIntPtr, PtrToXmlString(Is<UIntPtr>(v))));
        yield return new(typeof(ulong), (v, t) => new XElement(ElementNames.UnsignedLong, XmlConvert.ToString(Is<ulong>(v))));
        yield return new(typeof(ushort), (v, t) => new XElement(ElementNames.UnsignedShort, XmlConvert.ToString(Is<ushort>(v))));

        yield return new(typeof(DateTime), (v, t) => new XElement(ElementNames.DateTime, XmlConvert.ToString(Is<DateTime>(v), XmlDateTimeSerializationMode.RoundtripKind)));
        yield return new(typeof(DateTimeOffset), (v, t) => new XElement(ElementNames.DateTimeOffset, XmlConvert.ToString(Is<DateTimeOffset>(v), "O")));
        yield return new(typeof(TimeSpan), (v, t) => new XElement(ElementNames.Duration, XmlConvert.ToString(Is<TimeSpan>(v))));
        yield return new(typeof(DBNull), (v, t) => new XElement(ElementNames.DBNull));
        yield return new(typeof(decimal), (v, t) => new XElement(ElementNames.Decimal, XmlConvert.ToString(Is<decimal>(v))));
        yield return new(typeof(Guid), (v, t) => new XElement(ElementNames.Guid, XmlConvert.ToString(Is<Guid>(v))));
        yield return new(typeof(Half), (v, t) => new XElement(ElementNames.Half, XmlConvert.ToString((double)Is<Half>(v))));
        yield return new(typeof(string), (v, t) => new XElement(ElementNames.String, (object?)Is<string>(v) ?? new XAttribute(AttributeNames.Nil, true)));
        yield return new(typeof(Uri), (v, t) => new XElement(ElementNames.Uri, (object?)Is<Uri>(v)?.ToString() ?? new XAttribute(AttributeNames.Nil, true)));
    }

    static FrozenDictionary<Type, TransformConstant> _constantTransforms = ConstantTransformsDict().ToFrozenDictionary();

#pragma warning disable IDE0049 // Simplify Names
    static string PtrToXmlString(IntPtr v)
        => Environment.Is64BitProcess
                ? XmlConvert.ToString(checked((Int64)v))
                : XmlConvert.ToString(checked((Int32)v));

    static string PtrToXmlString(UIntPtr v)
        => Environment.Is64BitProcess
                ? XmlConvert.ToString(checked((UInt64)v))
                : XmlConvert.ToString(checked((UInt32)v));
#pragma warning restore IDE0049 // Simplify Names
}
