using System.Diagnostics.CodeAnalysis;

namespace vm2.Linq.Expressions.Serialization.Extensions;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
[ExcludeFromCodeCoverage]
public static partial class TypeExtensions
{
    /// <summary>
    /// Determines whether the specified type is a basic type: primitive, enum, decimal, string, Guid, Uri, DateTime,
    /// TimeSpan, DateTimeOffset, IntPtr, UIntPtr.
    /// </summary>
    public static bool IsBasicType(this Type type)
        => type.IsPrimitive ||
           type.IsEnum ||
           Transform.NonPrimitiveBasicTypes.Contains(type);

    const string anonymousTypePrefix = "<>f__AnonymousType";

    /// <summary>
    /// Determines whether the specified type is an anonymous type.
    /// </summary>
    public static bool IsAnonymous(this Type type)
        => type.IsGenericType && type.Name.StartsWith(anonymousTypePrefix, StringComparison.Ordinal);

    /// <summary>
    /// Determines whether the specified type is a generic <see cref="Nullable{T}"/>.
    /// </summary>
    public static bool IsNullable(this Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    /// <summary>
    /// Determines whether the specified type is a generic tuple class.
    /// </summary>
    public static bool IsTupleClass([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type type)
        => type.IsClass && type.ImplementsInterface(typeof(ITuple));

    /// <summary>
    /// Determines whether the specified type is a generic tuple struct.
    /// </summary>
    public static bool IsTupleValue([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type type)
        => type.IsValueType && type.ImplementsInterface(typeof(ITuple));

    /// <summary>
    /// Determines whether the specified type is a tuple (class or struct).
    /// </summary>
    public static bool IsTuple([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type type)
        => type.ImplementsInterface(typeof(ITuple));

    /// <summary>
    /// Determines whether the specified type is a <see cref="Memory{T}"/> or <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    public static bool IsMemory(this Type type)
    {
        if (!type.IsGenericType)
            return false;

        var genType = type.GetGenericTypeDefinition();

        return genType == typeof(Memory<>) ||
               genType == typeof(ReadOnlyMemory<>);
    }

    /// <summary>
    /// Determines whether the type implements the specified interface type.
    /// </summary>
    public static bool ImplementsInterface(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type type,
        Type interfaceType)
        => type.GetInterface(interfaceType.Name) is not null;

    /// <summary>
    /// Determines whether the type implements an interface with the specified name.
    /// </summary>
    public static bool ImplementsInterface(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type type,
        string interfaceName)
        => type.GetInterface(interfaceName) is not null;

    /// <summary>
    /// Determines whether the specified type is a byte sequence:
    /// <c>byte[]</c>, <c>Memory&lt;byte&gt;</c>, <c>ReadOnlyMemory&lt;byte&gt;</c>, <c>ArraySegment&lt;byte&gt;</c>.
    /// </summary>
    public static bool IsByteSequence(this Type type)
        => type.IsArray && type.GetElementType() == typeof(byte) ||
           Transform.ByteSequences.Contains(type);

    /// <summary>
    /// Determines whether the specified type is a sequence of objects: array, list, set, etc.
    /// </summary>
    public static bool IsSequence(this Type type)
    {
        if (type.IsArray)
            return true;

        var isGeneric = type.IsGenericType;

        if (isGeneric)
        {
            var genType = type.GetGenericTypeDefinition();

            if (Transform.SequenceTypes.Contains(genType) ||
                genType.Name.EndsWith("FrozenSet`1"))
                return true;
        }

        return type == typeof(Queue) || type == typeof(Stack) || type == typeof(Hashtable);
    }

    /// <summary>
    /// Determines whether the specified type implements <see cref="IDictionary"/>.
    /// </summary>
    public static bool IsDictionary([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type type)
        => type.ImplementsInterface(typeof(IDictionary));

    /// <summary>
    /// Determines whether the method has the specified name and a single parameter of type <c>IEnumerable&lt;&gt;</c>.
    /// </summary>
    public static bool MethodHas1EnumerableParameter(this MethodInfo mi, string name)
    {
        if (mi.Name == name)
        {
            ParameterInfo[] pis = mi.GetParameters();
            return pis.Length == 1 && pis[0].ParameterType.Name == typeof(IEnumerable<>).Name;
        }
        else
            return false;
    }

    /// <summary>
    /// Determines whether the constructor has a single array parameter.
    /// </summary>
    public static bool ConstructorHas1ArrayParameter(this ConstructorInfo ci)
    {
        ParameterInfo[] pis = ci.GetParameters();
        return pis.Length == 1 &&
               pis[0].ParameterType.IsArray;
    }

    /// <summary>
    /// Determines whether the constructor has a single parameter of type <c>IEnumerable&lt;&gt;</c>.
    /// </summary>
    public static bool ConstructorHas1EnumerableParameter(this ConstructorInfo ci)
    {
        ParameterInfo[] pis = ci.GetParameters();
        return pis.Length == 1 &&
               pis[0].ParameterType.Name == typeof(IEnumerable<>).Name;
    }

    /// <summary>
    /// Determines whether the constructor has a single parameter of type <c>IList&lt;&gt;</c>.
    /// </summary>
    public static bool ConstructorHas1ListParameter(this ConstructorInfo ci)
    {
        ParameterInfo[] pis = ci.GetParameters();
        return pis.Length == 1 &&
               pis[0].ParameterType.Name == typeof(IList<>).Name;
    }
}
