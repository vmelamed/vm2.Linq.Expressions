// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

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
    public static bool IsBasicType([NotNull] this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.IsPrimitive ||
               type.IsEnum ||
               Transform.NonPrimitiveBasicTypes.Contains(type);
    }

    const string anonymousTypePrefix = "<>f__AnonymousType";

    /// <summary>
    /// Determines whether the specified type is an anonymous type.
    /// </summary>
    public static bool IsAnonymous([NotNull] this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.IsGenericType && type.Name.StartsWith(anonymousTypePrefix, StringComparison.Ordinal);
    }

    /// <summary>
    /// Determines whether the specified type is a generic <see cref="Nullable{T}"/>.
    /// </summary>
    public static bool IsNullable([NotNull] this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// Determines whether the specified type is a generic tuple class.
    /// </summary>
    public static bool IsTupleClass(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)][NotNull] this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.IsClass && type.ImplementsInterface(typeof(ITuple));
    }

    /// <summary>
    /// Determines whether the specified type is a generic tuple struct.
    /// </summary>
    public static bool IsTupleValue([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)][NotNull] this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.IsValueType && type.ImplementsInterface(typeof(ITuple));
    }

    /// <summary>
    /// Determines whether the specified type is a tuple (class or struct).
    /// </summary>
    public static bool IsTuple([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)][NotNull] this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.ImplementsInterface(typeof(ITuple));
    }

    /// <summary>
    /// Determines whether the specified type is a <see cref="Memory{T}"/> or <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    public static bool IsMemory([NotNull] this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

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
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)][NotNull] this Type type,
        Type interfaceType)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(interfaceType);

        return type.GetInterface(interfaceType.Name) is not null;
    }

    /// <summary>
    /// Determines whether the type implements an interface with the specified name.
    /// </summary>
    public static bool ImplementsInterface(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)][NotNull] this Type type,
        string interfaceName)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(interfaceName);

        return type.GetInterface(interfaceName) is not null;
    }

    /// <summary>
    /// Determines whether the specified type is a byte sequence:
    /// <c>byte[]</c>, <c>Memory&lt;byte&gt;</c>, <c>ReadOnlyMemory&lt;byte&gt;</c>, <c>ArraySegment&lt;byte&gt;</c>.
    /// </summary>
    public static bool IsByteSequence([NotNull] this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.IsArray && type.GetElementType() == typeof(byte) ||
               Transform.ByteSequences.Contains(type);
    }

    /// <summary>
    /// Determines whether the specified type is a sequence of objects: array, list, set, etc.
    /// </summary>
    public static bool IsSequence([NotNull] this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

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
    public static bool IsDictionary([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)][NotNull] this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        return type.ImplementsInterface(typeof(IDictionary));
    }

    /// <summary>
    /// Determines whether the method has the specified name and a single parameter of type <c>IEnumerable&lt;&gt;</c>.
    /// </summary>
    public static bool MethodHas1EnumerableParameter(
        [NotNull] this MethodInfo mi,
        string name)
    {
        ArgumentNullException.ThrowIfNull(mi);

        if (mi.Name == name)
        {
            ParameterInfo[] pis = mi.GetParameters();
            return pis.Length == 1 && pis[0].ParameterType.Name == typeof(IEnumerable<>).Name;
        }
        else
            return false;
    }

    /// <summary>
    /// Determines whether the method has the specified name and a single parameter of type <c>IEnumerable&lt;&gt;</c>.
    /// </summary>
    public static bool MethodHas1EnumerableParameterAndComparer(
        [NotNull] this MethodInfo mi,
        string name)
    {
        ArgumentNullException.ThrowIfNull(mi);

        if (mi.Name == name)
        {
            ParameterInfo[] pis = mi.GetParameters();
            return pis.Length == 2 &&
                   pis[0].ParameterType.Name == typeof(IEnumerable<>).Name &&
                   pis[1].ParameterType.Name == typeof(IEqualityComparer<>).Name;
        }
        else
            return false;
    }

    /// <summary>
    /// Determines whether the constructor has a single array parameter.
    /// </summary>
    public static bool ConstructorHas1ArrayParameter(
        [NotNull] this ConstructorInfo ci)
    {
        ArgumentNullException.ThrowIfNull(ci);

        ParameterInfo[] pis = ci.GetParameters();
        return pis.Length == 1 &&
               pis[0].ParameterType.IsArray;
    }

    /// <summary>
    /// Determines whether the constructor has a single parameter of type <c>IEnumerable&lt;&gt;</c>.
    /// </summary>
    public static bool ConstructorHas1EnumerableParameter(
        [NotNull] this ConstructorInfo ci)
    {
        ArgumentNullException.ThrowIfNull(ci);

        ParameterInfo[] pis = ci.GetParameters();
        return pis.Length == 1 &&
               pis[0].ParameterType.Name == typeof(IEnumerable<>).Name;
    }

    /// <summary>
    /// Determines whether the constructor has a single parameter of type <c>IList&lt;&gt;</c>.
    /// </summary>
    public static bool ConstructorHas1ListParameter(
        [NotNull] this ConstructorInfo ci)
    {
        ArgumentNullException.ThrowIfNull(ci);

        ParameterInfo[] pis = ci.GetParameters();
        return pis.Length == 1 &&
               pis[0].ParameterType.Name == typeof(IList<>).Name;
    }
}
