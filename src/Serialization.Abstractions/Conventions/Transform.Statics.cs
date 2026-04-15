namespace vm2.Linq.Expressions.Serialization.Conventions;

public static partial class Transform
{
    static IEnumerable<Type> EnumNonPrimitiveBasicTypes()
    {
        yield return typeof(DateTime);
        yield return typeof(DateTimeOffset);
        yield return typeof(DBNull);
        yield return typeof(decimal);
        yield return typeof(TimeSpan);
        yield return typeof(Guid);
        yield return typeof(Half);
        yield return typeof(string);
        yield return typeof(Uri);
    }

    /// <summary>
    /// Non-primitive basic types: types that are considered "basic" but for which <c>Type.IsPrimitive</c> is <see langword="false"/>.
    /// </summary>
    public static readonly FrozenSet<Type> NonPrimitiveBasicTypes = EnumNonPrimitiveBasicTypes().ToFrozenSet();

    static IEnumerable<Type> EnumSequenceTypes()
    {
        yield return typeof(ArraySegment<>);
        yield return typeof(BlockingCollection<>);
        yield return typeof(Collection<>);
        yield return typeof(ConcurrentBag<>);
        yield return typeof(ConcurrentQueue<>);
        yield return typeof(ConcurrentStack<>);
        yield return typeof(FrozenSet<>);
        yield return typeof(HashSet<>);
        yield return typeof(ImmutableArray<>);
        yield return typeof(ImmutableHashSet<>);
        yield return typeof(ImmutableList<>);
        yield return typeof(ImmutableQueue<>);
        yield return typeof(ImmutableSortedSet<>);
        yield return typeof(ImmutableStack<>);
        yield return typeof(LinkedList<>);
        yield return typeof(List<>);
        yield return typeof(Memory<>);
        yield return typeof(Queue<>);
        yield return typeof(ReadOnlyCollection<>);
        yield return typeof(ReadOnlyMemory<>);
        yield return typeof(SortedSet<>);
        yield return typeof(Stack<>);
    }

    /// <summary>
    /// Supported generic types that represent sequences of elements — mostly <see cref="IEnumerable{T}"/> implementations.
    /// </summary>
    public static readonly FrozenSet<Type> SequenceTypes = EnumSequenceTypes().ToFrozenSet();

    static IEnumerable<Type> EnumByteSequences()
    {
        yield return typeof(ArraySegment<byte>);
        yield return typeof(Memory<byte>);
        yield return typeof(ReadOnlyMemory<byte>);
    }

    /// <summary>
    /// Supported types that represent contiguous sequences of bytes with similar behavior as <c>byte[]</c>.
    /// </summary>
    public static readonly FrozenSet<Type> ByteSequences = EnumByteSequences().ToFrozenSet();
}
