namespace vm2.Linq.Expressions.Serialization.TestData;

public static class ConstantTestData
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
    public static readonly long MaxJsonInteger = (long)Math.Pow(2, 53);

    /// <summary>
    /// The minimum long number that can be expressed as &quot;JSON integer&quot; without loosing fidelity.
    /// </summary>
    /// <remarks>
    /// In JavaScript, the maximum safe integer is 2^53 - 1, which is 9007199254740991. This is because JavaScript uses
    /// double-precision floating-point format numbers as specified in IEEE 754, and can only safely represent integers
    /// from the range [-(2^53-1), 2^53-1].
    /// Therefore we serialize numbers outside of that range as strings, e.g. <c>&quot;-9007199254740992&quot;</c>.
    /// </remarks>
    public static readonly long MinJsonInteger = -MaxJsonInteger;

    /// <summary>
    /// Gets the expression mapped to the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Expression.</returns>
    public static Expression? GetExpression(string id) => _substitutes.GetValueOrDefault(id);

    public static readonly TheoryData<string, string, string> Data = new ()
    {
        // bool
        { TestLine(), "Bool.false",                                                             "Bool.False" },
        { TestLine(), "Bool.true",                                                              "Bool.True" },
        // byte
        { TestLine(), "Byte.5",                                                                 "Byte" },
        // char
        { TestLine(), "Char.'V'",                                                               "Char" },
        { TestLine(), "Double.Nan",                                                             "Double.Nan" },
        { TestLine(), "Double.NegativeInfinity",                                                "Double.NegativeInfinity" },
        { TestLine(), "Double.PositiveInfinity",                                                "Double.PositiveInfinity" },
        { TestLine(), "Double.NegativeZero",                                                    "Double.NegativeZero" },
        { TestLine(), "Double.Zero",                                                            "Double.Zero" },
        { TestLine(), "Double.Epsilon",                                                         "Double.Epsilon" },
        { TestLine(), "Double.PI",                                                              "Double.Pi" },
        { TestLine(), "Double.E",                                                               "Double.E" },
        { TestLine(), "Double.-2.234567891233658E-123",                                         "Double.-2.23..E-123" },
        { TestLine(), "Double.5.1234567891234567E-123",                                         "Double.5.12..E-123" },
        { TestLine(), "Float.Nan",                                                              "Float.Nan" },
        { TestLine(), "Float.NegativeInfinity",                                                 "Float.NegativeInfinity" },
        { TestLine(), "Float.PositiveInfinity",                                                 "Float.PositiveInfinity" },
        { TestLine(), "Float.Epsilon",                                                          "Float.Epsilon" },
        { TestLine(), "Float.NegativeZero",                                                     "Float.NegativeZero" },
        { TestLine(), "Float.Zero",                                                             "Float.Zero" },
        { TestLine(), "Float.-2.234568E-23F",                                                   "Float.-2.23..E-23" },
        { TestLine(), "Float.5.5123453E-34F",                                                   "Float.5.51..E-34" },
        // Half
        { TestLine(), "Half.3.14",                                                              "Half" },
        { TestLine(), "Half.E",                                                                 "Half.E" },
        { TestLine(), "Half.MinValue",                                                          "Half.MinValue" },
        { TestLine(), "Half.MaxValue",                                                          "Half.MaxValue" },
        { TestLine(), "Half.Zero",                                                              "Half.Zero" },
        { TestLine(), "Half.One",                                                               "Half.One" },
        { TestLine(), "Half.NegativeInfinity",                                                  "Half.NegativeInfinity" },
        { TestLine(), "Half.PositiveInfinity",                                                  "Half.PositiveInfinity" },
        { TestLine(), "Half.Pi",                                                                "Half.Pi" },
        { TestLine(), "Half.Epsilon",                                                           "Half.Epsilon" },
        { TestLine(), "Half.NegativeOne",                                                       "Half.NegativeOne" },
        { TestLine(), "Half.NegativeZero",                                                      "Half.NegativeZero" },
        // int
        { TestLine(), "Int.5",                                                                  "Int.5" },
        { TestLine(), "Int.42",                                                                 "Int.42" },
        { TestLine(), "Int.Min",                                                                "Int.Min" },
        { TestLine(), "Int.Max",                                                                "Int.Max" },
        // IntPtr
        { TestLine(), "IntPtr.5",                                                               "IntPtr.5" },
        { TestLine(), "IntPtr.23",                                                              "IntPtr.23" },
        { TestLine(), "IntPtr.MinValue",                                                        "IntPtr.MinValue" },
        { TestLine(), "IntPtr.MaxValue",                                                        "IntPtr.MaxValue" },
        // long
        { TestLine(), "Long.0",                                                                 "Long.0" },
        { TestLine(), "Long.5L",                                                                "Long.5" },
        { TestLine(), "Long.Min",                                                               "Long.Min" },
        { TestLine(), "Long.Max",                                                               "Long.Max" },
        { TestLine(), "Long.IntMin",                                                            "Long.IntMin" },
        { TestLine(), "Long.IntMax",                                                            "Long.IntMax" },
        { TestLine(), "Long.IntMin-1",                                                          "Long.IntMin-1" },
        { TestLine(), "Long.IntMax+1",                                                          "Long.IntMax+1" },
        { TestLine(), "Long.IntMin+1",                                                          "Long.IntMin+1" },
        { TestLine(), "Long.IntMax-1",                                                          "Long.IntMax-1" },
        // sbyte
        { TestLine(), "Sbyte.5",                                                                "SignedByte.5"},
        { TestLine(), "Sbyte.-5",                                                               "SignedByte.-5"},
        { TestLine(), "Sbyte.MinValue",                                                         "SignedByte.MinValue"},
        { TestLine(), "Sbyte.MaxValue",                                                         "SignedByte.MaxValue"},
        // short
        { TestLine(), "Short.32000",                                                            "Short"},
        { TestLine(), "Short.MinValue",                                                         "Short.MinValue"},
        { TestLine(), "Short.MaxValue",                                                         "Short.MaxValue"},
        // uint
        { TestLine(), "Uint.0",                                                                 "UnsignedInt.0"},
        { TestLine(), "Uint.5",                                                                 "UnsignedInt.5"},
        { TestLine(), "Uint.42",                                                                "UnsignedInt.42"},
        { TestLine(), "Uint.Min",                                                               "UnsignedInt.Min"},
        { TestLine(), "Uint.Max",                                                               "UnsignedInt.Max"},
        // UIntPtr
        { TestLine(), "UnsignedIntPtr.5",                                                       "UnsignedIntPtr.5"},
        { TestLine(), "UnsignedIntPtr.42",                                                      "UnsignedIntPtr.42"},
        { TestLine(), "UnsignedIntPtr.MinValue",                                                "UnsignedIntPtr.MinValue"},
        { TestLine(), "UnsignedIntPtr.MaxValue",                                                "UnsignedIntPtr.MaxValue"},
        // ulong
        { TestLine(), "Ulong.0",                                                                "UnsignedLong.0"},
        { TestLine(), "Ulong.5",                                                                "UnsignedLong.5"},
        { TestLine(), "Ulong.Min",                                                              "UnsignedLong.Min"},
        { TestLine(), "Ulong.Max",                                                              "UnsignedLong.Max"},
        { TestLine(), "Ulong.IntMax-1",                                                         "UnsignedLong.IntMax-1"},
        { TestLine(), "Ulong.IntMax",                                                           "UnsignedLong.IntMax"},
        { TestLine(), "Ulong.IntMax+1",                                                         "UnsignedLong.IntMax+1"},
        // ushort
        { TestLine(), "Ushort.5",                                                               "UnsignedShort.5"},
        { TestLine(), "Ushort.443",                                                             "UnsignedShort.443"},
        { TestLine(), "Ushort.MinValue",                                                        "UnsignedShort.MinValue"},
        { TestLine(), "Ushort.MaxValue",                                                        "UnsignedShort.MaxValue"},
        // DateTime
        { TestLine(), "DateTime.MinValue",                                                      "DateTime.MinValue" },
        { TestLine(), "DateTime.MaxValue",                                                      "DateTime.MaxValue" },
        { TestLine(), "DateTime(2024, 4, 13, 23, 18, 26, 234, DateTimeKind.Local)",             "DateTime" },
        { TestLine(), "DateTime(2024, 4, 13, 23, 18, 26, 234)",                                 "DateTime.Local" },
        // DateTimeOffsEt
        { TestLine(), "DateTimeOffset.MinValue",                                                "DateTimeOffset.MinValue" },
        { TestLine(), "DateTimeOffset.MaxValue",                                                "DateTimeOffset.MaxValue" },
        { TestLine(), "DateTimeOffset(2024, 4, 13, 23, 18, 26, 234, new TimeSpan(0, -300, 0))", "DateTimeOffset" },
        // TimeSpan
        { TestLine(), "TimeSpan.MinValue",                                                      "TimeSpan.MinValue" },
        { TestLine(), "TimeSpan.MaxValue",                                                      "TimeSpan.MaxValue" },
        { TestLine(), "TimeSpan.Zero",                                                          "TimeSpan.Zero" },
        { TestLine(), "TimeSpan(3, 4, 15, 32, 123)",                                            "TimeSpan" },
        { TestLine(), "TimeSpan(-3, 4, 15, 32, 123)",                                           "TimeSpan-" },
        // DBNull
        { TestLine(), "DBNull.Value",                                                           "DBNull" },
        // decimal
        { TestLine(), "Decimal.Zero",                                                           "Decimal.Zero" },
        { TestLine(), "Decimal.MinusOne",                                                       "Decimal.MinusOne" },
        { TestLine(), "Decimal.One",                                                            "Decimal.One" },
        { TestLine(), "Decimal.MinValue",                                                       "Decimal.MinValue" },
        { TestLine(), "Decimal.MaxValue",                                                       "Decimal.MaxValue" },
        { TestLine(), "5.5M",                                                                   "Decimal" },
        // GUID
        { TestLine(), "Guid.Empty",                                                             "Guid.Empty" },
        { TestLine(), "Guid(\"00112233-4455-6677-8899-aabbccddeeff\")",                         "Guid" },
        // string
        { TestLine(), "String.Empty",                                                           "String.string.Empty" },
        { TestLine(), "String.null",                                                            "String.string.null" },
        { TestLine(), "String.abrah-cadabrah",                                                  "String.abrah-cadabrah" },
        { TestLine(), "String.ала-бала",                                                        "String.ала-бала" },
        // Uri
        { TestLine(), "Uri(\"http://www.acme.com\")",                                           "Uri" },
        // enum
        { TestLine(), "EnumFlagsTest.One | EnumFlagsTest.Three",                                "EnumFlags" },
        { TestLine(), "EnumTest.Three",                                                         "Enum" },
        // nullable primitive
        { TestLine(), "Nullable.int.5",                                                         "Nullable.Int.5" },
        { TestLine(), "Nullable.int.null",                                                      "Nullable.Int.Null" },
        { TestLine(), "Nullable.long.5L",                                                       "Nullable.Long.5" },
        { TestLine(), "Nullable.long.null",                                                     "Nullable.Long.Null" },
        { TestLine(), "Nullable.long.long.Min",                                                 "Nullable.Long.Min" },
        { TestLine(), "Nullable.long.long.Max",                                                 "Nullable.Long.Max" },
        { TestLine(), "Nullable.long.long.IntMin",                                              "Nullable.Long.IntMin" },
        { TestLine(), "Nullable.long.long.IntMax",                                              "Nullable.Long.IntMax" },
        { TestLine(), "Nullable.long.long.IntMin-1",                                            "Nullable.Long.IntMin-1" },
        { TestLine(), "Nullable.long.long.IntMax+1",                                            "Nullable.Long.IntMax+1" },
        { TestLine(), "Nullable.long.long.IntMin+1",                                            "Nullable.Long.IntMin+1" },
        { TestLine(), "Nullable.long.long.IntMax-1",                                            "Nullable.Long.IntMax-1" },
        // objects
        { TestLine(), "Object.null",                                                            "Object.Null" },
        { TestLine(), "Object()",                                                               "Object" },
        { TestLine(), "Object1()",                                                              "Object1" },
        { TestLine(), "Object1.null",                                                           "Object1.Null" },
        { TestLine(), "ClassDataContract1()",                                                   "Object.ClassDataContract1" },
        { TestLine(), "ClassDataContract2()",                                                   "Object.ClassDataContract2" },
        { TestLine(), "ClassSerializable1()",                                                   "Object.ClassSerializable1" },
        // structs
        { TestLine(), "StructDataContract1.null",                                               "Struct.NullableStructDataContract1.Null" },
        { TestLine(), "StructDataContract1() { IntProperty = 7, StringProperty = \"vm\" }",     "Struct.StructDataContract1-2" },
        { TestLine(), "StructDataContract1()",                                                  "Struct.StructDataContract1" },
        { TestLine(), "StructDataContract1?() { IntProperty = 7, StringProperty = \"vm\" }",    "Struct.NullableStructDataContract1" },
        { TestLine(), "(StructSerializable1?)null",                                             "Struct.NullableStructSerializable1.Null" },
        { TestLine(), "StructSerializable1() { IntProperty = 7, StringProperty = \"vm\" }",     "Struct.NullableStructSerializable1" },
        { TestLine(), "StructSerializable1()",                                                  "Struct.StructSerializable1" },
        // anonymous
        { TestLine(), "Anonymous",                                                              "Anonymous" },
        // byte sequences
        { TestLine(), "Bytes.(byte[])null",                                                     "Bytes.Array.Null" },
        { TestLine(), "Bytes.byte[]{}",                                                         "Bytes.Array.Empty" },
        { TestLine(), "Bytes.Memory<byte>()",                                                   "Bytes.Memory.Empty" },
        { TestLine(), "Bytes.ReadOnlyMemory<byte>()",                                           "Bytes.ReadOnlyMemory.Empty" },
        { TestLine(), "Bytes.ArraySegment<byte>([])",                                           "Bytes.ArraySegment.Empty" },
        { TestLine(), "Bytes.byte[]{ 1, 2, 3, 1, 2, 3, 1, 2, 3, 10 }",                          "Bytes.Array" },
        { TestLine(), "Bytes.Memory<byte>([1, 2, 3, 1, 2, 3, 1, 2, 3, 10 ])",                   "Bytes.Memory" },
        { TestLine(), "Bytes.ReadOnlyMemory<byte>([1, 2, 3, 1, 2, 3, 1, 2, 3, 10 ])",           "Bytes.ReadOnlyMemory" },
        { TestLine(), "Bytes.ArraySegment<byte>([1, 2, 3, 1, 2, 3, 1, 2, 3, 10], 1, 8)",        "Bytes.ArraySegment" },
        // sequences
        { TestLine(), "Ints.(int[])null",                                                       "Array.Int.Null" },
        { TestLine(), "Ints.int[]{}",                                                           "Array.Int.Empty" },
        { TestLine(), "Ints.int[]{ 1, 2, 3, 4 }",                                               "Array.Int" },
        { TestLine(), "Ints.(int?[])null",                                                      "Array.NullableInt.Null" },
        { TestLine(), "Ints.int?[]{}",                                                          "Array.NullableInt.Empty" },
        { TestLine(), "Ints.int?[]{ 1, 2, null, null }",                                        "Array.NullableInt" },
        { TestLine(), "Ints.Memory<int>(null)",                                                 "Memory.Int.Null" },
        { TestLine(), "Ints.Memory<int>()",                                                     "Memory.Int.Empty" },
        { TestLine(), "Ints.Memory<int>([ 1, 2, 3, 4 ])",                                       "Memory.Int" },
        { TestLine(), "Ints.(Memory<int>?)null",                                                "Memory.Int0.Null" },
        { TestLine(), "Ints.(Memory<int>?)()",                                                  "Memory.Int0.Empty" },
        { TestLine(), "Ints.(Memory<int>?)([ 1, 2, 3, 4 ])",                                    "Memory.Int0" },
        { TestLine(), "EnumTest?[]{ EnumTest.One, EnumTest.Two, null, null }",                  "Array.NullableEnums" },
        { TestLine(), "EnumTest?[].null",                                                       "Array.NullableEnums.Null" },
        { TestLine(), "ArraySegment<int>([ 1, 2, 3, 4 ], 1, 2)",                                "ArraySegment.Int" },
        { TestLine(), "List<int>([1, 2, 3, 4])",                                                "List.Int" },
        { TestLine(), "List<int?>{ 1, 2, null, null }",                                         "List.NullableInt" },
        { TestLine(), "List<int?>.null",                                                        "List.NullableInt.Null" },
        { TestLine(), "LinkedList<int>([1, 2, 3, 4])",                                          "LinkedList.Int" },
        { TestLine(), "Sequence<int>([1, 2, 3, 4])",                                            "Collection.Int" },
        { TestLine(), "ReadOnlyCollection<int>([1, 2, 3, 4])",                                  "ReadOnlyCollection.Int" },
        { TestLine(), "ReadOnlyMemory<int>([ 1, 2, 3, 4 ])",                                    "ReadOnlyMemory.Int" },
        { TestLine(), "HashSet<int>([1, 2, 3, 4])",                                             "HashSet.Int" },
        { TestLine(), "SortedSet<int>([1, 2, 3, 4])",                                           "SortedSet.Int" },
        { TestLine(), "Queue<int>([1, 2, 3, 4])",                                               "Queue.Int" },
        { TestLine(), "Stack<int>([1, 2, 3, 4])",                                               "Stack.Int" },
        { TestLine(), "BlockingCollection<double>()",                                           "BlockingCollection" },
        { TestLine(), "ConcurrentBag<int>([1, 2, 3, 4])",                                       "ConcurrentBag.Int" },
        { TestLine(), "ConcurrentQueue<int>([1, 2, 3, 4])",                                     "ConcurrentQueue.Int" },
        { TestLine(), "ConcurrentStack<int>([1, 2, 3, 4])",                                     "ConcurrentStack.Int" },
        { TestLine(), "ImmutableArray.Create()",                                                "ImmutableSet.Int.Empty" },
        { TestLine(), "ImmutableArray.Create(1, 2, 3, 4 )",                                     "ImmutableSet.Int" },
        { TestLine(), "ImmutableHashSet.Create(1, 2, 3, 4 )",                                   "ImmutableHashSet.Int" },
        { TestLine(), "ImmutableList.Create(1, 2, 3, 4 )",                                      "ImmutableList.Int" },
        { TestLine(), "ImmutableQueue.Create(1, 2, 3, 4 )",                                     "ImmutableQueue.Int" },
        { TestLine(), "ImmutableSortedSet.Create(1, 2, 3, 4 )",                                 "ImmutableSortedSet.Int" },
        { TestLine(), "ImmutableStack.Create(1, 2, 3, 4 )",                                     "ImmutableStack.Int" },
        { TestLine(), "ClassDataContract1[] { new ClassDataContract1()...",                     "Array.ClassDataContract1and2" },
        { TestLine(), "ClassDataContract1[] { new(0, \"vm\"), new(1, \"vm2 vm\"), }",           "Array.ClassDataContract1" },
        { TestLine(), "Frozen.byte[]{}",                                                        "Bytes.Empty.SetFrozen" },
        { TestLine(), "Frozen.byte[]{ 1, 2, 3, 1, 2, 3, 1, 2, 3, 10 }",                         "Bytes.SetFrozen" },
        { TestLine(), "Frozen.int[]{ 1, 2, 3, 4 }",                                             "FrozenSet.Int" },
        { TestLine(), "Frozen.int?[]{ 1, 2, null, null }",                                      "FrozenSet.NullableInt" },
        { TestLine(), "Frozen.decimal[]{ 1, 2, 3, 4 }",                                         "FrozenSet.Decimal" },
        { TestLine(), "Frozen.EnumTest?[]{ EnumTest.One, EnumTest.Two, null, null }",           "FrozenSet.NullableEnums" },
        { TestLine(), "Frozen.anonymous[]",                                                     "FrozenSet.Anonymous" },
        // tuples
        { TestLine(), "Tuple.(Tuple<int, string>)null",                                         "Tuple.Int.String.Null" },
        { TestLine(), "Tuple.Tuple<int, string>",                                               "Tuple.Int.String" },
        { TestLine(), "Tuple.ValueTuple<int, string>",                                          "TupleValue.Int.String" },
        { TestLine(), "Tuple.Tuple<int, String, StructDataContract1>",                          "Tuple.Int.String.Struct" },
        { TestLine(), "Tuple.ValueTuple<int, String, StructDataContract1>",                     "TupleValue.Int.String.Struct" },
        // dictionaries
        { TestLine(), "Dictionary<int, string?>{ [1] = \"one\", [2] = \"two\"...",              "Dictionary.Int.NullableString" },
        { TestLine(), "Dictionary<int, string>{ [1] = \"one\", [2] = \"two\" }",                "Dictionary.Int.String" },
        { TestLine(), "Frozen.Dictionary<int, string?>...",                                     "Frozen.Dictionary.Int.NullableString" },
        { TestLine(), "Frozen.Dictionary<int, string>...",                                      "Frozen.Dictionary.Int.String" },
        { TestLine(), "Hashtable(new Dictionary<int, string>{ [1] =\"one\", [2]=\"two\" })",    "Hashtable" },
        { TestLine(), "ImmutableDictionary.Create<int,string>().Add(...)",                      "Immutable.Dictionary.Int.String" },
        { TestLine(), "ImmutableSortedDictionary.Create<int,string>().Add(...)",                "Immutable.SortedDictionary.Int.String" },
        { TestLine(), "ReadOnlyDictionary<int, string>...",                                     "ReadOnly.Dictionary.Int.String" },
        { TestLine(), "SortedDictionary<int, string>{ [1] =\"one\", [2]=\"two\" }",             "SortedDictionary.Int.String" },
        { TestLine(), "ConcurrentDictionary<int, string>{ [1] = \"one\", [2]=\"two\" }",        "Concurrent.Dictionary.Int.String" },

        { TestLine(), "StructDataContract1[]",                                                  "Array.StructDataContract1" },
        { TestLine(), "StructDataContract1?[]",                                                 "Array.NullableStructDataContract1" },
        { TestLine(), "StructSerializable1[]",                                                  "Array.StructSerializable1" },
        { TestLine(), "StructSerializable1?[]",                                                 "Array.NullableStructSerializable1" },
    };

    static readonly Dictionary<string, ConstantExpression> _substitutes = new()
    {
        // bool
        ["Bool.false"]                                                                          = Expression.Constant(false),
        ["Bool.true"]                                                                           = Expression.Constant(true),
        // byte
        ["Byte.5"]                                                                              = Expression.Constant((byte)5),
        // char
        ["Char.'V'"]                                                                            = Expression.Constant('V'),
        // double
        ["Double.Nan"]                                                                          = Expression.Constant(double.NaN),
        ["Double.NegativeInfinity"]                                                             = Expression.Constant(double.NegativeInfinity),
        ["Double.PositiveInfinity"]                                                             = Expression.Constant(double.PositiveInfinity),
        ["Double.NegativeZero"]                                                                 = Expression.Constant(double.NegativeZero),
        ["Double.Zero"]                                                                         = Expression.Constant(0.0),
        ["Double.Epsilon"]                                                                      = Expression.Constant(double.Epsilon),
        ["Double.PI"]                                                                           = Expression.Constant(double.Pi),
        ["Double.E"]                                                                            = Expression.Constant(double.E),
        ["Double.-2.234567891233658E-123"]                                                      = Expression.Constant(-2.234567891233658E-123),
        ["Double.5.1234567891234567E-123"]                                                      = Expression.Constant(5.1234567891234567E-123),
        // float
        ["Float.Nan"]                                                                           = Expression.Constant(float.NaN),
        ["Float.NegativeInfinity"]                                                              = Expression.Constant(float.NegativeInfinity),
        ["Float.PositiveInfinity"]                                                              = Expression.Constant(float.PositiveInfinity),
        ["Float.Epsilon"]                                                                       = Expression.Constant(float.Epsilon),
        ["Float.NegativeZero"]                                                                  = Expression.Constant(float.NegativeZero),
        ["Float.Zero"]                                                                          = Expression.Constant(0.0F),
        ["Float.-2.234568E-23F"]                                                                = Expression.Constant(-2.234568E-23F),
        ["Float.5.5123453E-34F"]                                                                = Expression.Constant(5.5123453E-34F),
        // Half
        ["Half.3.14"]                                                                          = Expression.Constant((Half)3.14),
        ["Half.E"]                                                                              = Expression.Constant(Half.E),
        ["Half.MinValue"]                                                                       = Expression.Constant(Half.MinValue),
        ["Half.MaxValue"]                                                                       = Expression.Constant(Half.MaxValue),
        ["Half.Zero"]                                                                           = Expression.Constant(Half.Zero),
        ["Half.One"]                                                                            = Expression.Constant(Half.One),
        ["Half.NegativeInfinity"]                                                               = Expression.Constant(Half.NegativeInfinity),
        ["Half.PositiveInfinity"]                                                               = Expression.Constant(Half.PositiveInfinity),
        ["Half.Pi"]                                                                             = Expression.Constant(Half.Pi),
        ["Half.Epsilon"]                                                                        = Expression.Constant(Half.Epsilon),
        ["Half.NegativeOne"]                                                                    = Expression.Constant(Half.NegativeOne),
        ["Half.NegativeZero"]                                                                   = Expression.Constant(Half.NegativeZero),
        // int
        ["Int.5"]                                                                               = Expression.Constant(5),
        ["Int.42"]                                                                              = Expression.Constant(42),
        ["Int.Min"]                                                                             = Expression.Constant(int.MinValue),
        ["Int.Max"]                                                                             = Expression.Constant(int.MaxValue),
        // IntPtr
        ["IntPtr.5"]                                                                            = Expression.Constant((IntPtr)5),
        ["IntPtr.23"]                                                                           = Expression.Constant((IntPtr)23),
        ["IntPtr.MinValue"]                                                                     = Expression.Constant(IntPtr.MinValue),
        ["IntPtr.MaxValue"]                                                                     = Expression.Constant(IntPtr.MaxValue),
        // long
        ["Long.0"]                                                                              = Expression.Constant(0L),
        ["Long.5L"]                                                                             = Expression.Constant(5L),
        ["Long.Min"]                                                                            = Expression.Constant(long.MinValue),
        ["Long.Max"]                                                                            = Expression.Constant(long.MaxValue),
        ["Long.IntMin"]                                                                         = Expression.Constant(MinJsonInteger),
        ["Long.IntMax"]                                                                         = Expression.Constant(MaxJsonInteger),
        ["Long.IntMin-1"]                                                                       = Expression.Constant(MinJsonInteger-1),
        ["Long.IntMax+1"]                                                                       = Expression.Constant(MaxJsonInteger+1),
        ["Long.IntMin+1"]                                                                       = Expression.Constant(MinJsonInteger+1),
        ["Long.IntMax-1"]                                                                       = Expression.Constant(MaxJsonInteger-1),
        // sbyte
        ["Sbyte.5"]                                                                             = Expression.Constant((sbyte)5),
        ["Sbyte.-5"]                                                                            = Expression.Constant((sbyte)-5),
        ["Sbyte.MinValue"]                                                                      = Expression.Constant(sbyte.MinValue),
        ["Sbyte.MaxValue"]                                                                      = Expression.Constant(sbyte.MaxValue),
        // short
        ["Short.32000"]                                                                         = Expression.Constant((short)32000),
        ["Short.MinValue"]                                                                      = Expression.Constant(short.MinValue),
        ["Short.MaxValue"]                                                                      = Expression.Constant(short.MaxValue),
        // uint
        ["Uint.0"]                                                                              = Expression.Constant((uint)0),
        ["Uint.5"]                                                                              = Expression.Constant((uint)5),
        ["Uint.42"]                                                                             = Expression.Constant((uint)42),
        ["Uint.Min"]                                                                            = Expression.Constant(uint.MinValue),
        ["Uint.Max"]                                                                            = Expression.Constant(uint.MaxValue),
        // UIntPtr
        ["UnsignedIntPtr.5"]                                                                    = Expression.Constant((UIntPtr)5),
        ["UnsignedIntPtr.42"]                                                                   = Expression.Constant((UIntPtr)42),
        ["UnsignedIntPtr.MinValue"]                                                             = Expression.Constant(UIntPtr.MinValue),
        ["UnsignedIntPtr.MaxValue"]                                                             = Expression.Constant(UIntPtr.MaxValue),
        // ulong
        ["Ulong.0"]                                                                             = Expression.Constant(0UL),
        ["Ulong.5"]                                                                             = Expression.Constant((ulong)5),
        ["Ulong.Min"]                                                                           = Expression.Constant(ulong.MinValue),
        ["Ulong.Max"]                                                                           = Expression.Constant(ulong.MaxValue),
        ["Ulong.IntMax-1"]                                                                      = Expression.Constant((ulong)MaxJsonInteger-1),
        ["Ulong.IntMax"]                                                                        = Expression.Constant((ulong)MaxJsonInteger),
        ["Ulong.IntMax+1"]                                                                      = Expression.Constant((ulong)MaxJsonInteger+1),
        // ushort
        ["Ushort.5"]                                                                            = Expression.Constant((ushort)5),
        ["Ushort.443"]                                                                          = Expression.Constant((ushort)443),
        ["Ushort.MinValue"]                                                                     = Expression.Constant(ushort.MinValue),
        ["Ushort.MaxValue"]                                                                     = Expression.Constant(ushort.MaxValue),
        // DateTime
        ["DateTime.MinValue"]                                                                   = Expression.Constant(DateTime.MinValue),
        ["DateTime.MaxValue"]                                                                   = Expression.Constant(DateTime.MaxValue),
        ["DateTime(2024, 4, 13, 23, 18, 26, 234)"]                                              = Expression.Constant(new DateTime(2024, 4, 13, 23, 18, 26, 234)),
        ["DateTime(2024, 4, 13, 23, 18, 26, 234, DateTimeKind.Local)"]                          = Expression.Constant(new DateTime(2024, 4, 13, 23, 18, 26, 234, DateTimeKind.Local)),
        // DateTimeOffset
        ["DateTimeOffset.MinValue"]                                                             = Expression.Constant(DateTimeOffset.MinValue),
        ["DateTimeOffset.MaxValue"]                                                             = Expression.Constant(DateTimeOffset.MaxValue),
        ["DateTimeOffset(2024, 4, 13, 23, 18, 26, 234, new TimeSpan(0, -300, 0))"]              = Expression.Constant(new DateTimeOffset(2024, 4, 13, 23, 18, 26, 234, new TimeSpan(0, -300, 0))),
        // TimeSpan
        ["TimeSpan.MinValue"]                                                                   = Expression.Constant(new TimeSpan(TimeSpan.MinValue.Days, TimeSpan.MinValue.Hours, TimeSpan.MinValue.Minutes, TimeSpan.MinValue.Seconds)),
        ["TimeSpan.MaxValue"]                                                                   = Expression.Constant(new TimeSpan(TimeSpan.MaxValue.Days, TimeSpan.MaxValue.Hours, TimeSpan.MaxValue.Minutes, TimeSpan.MaxValue.Seconds)),
        ["TimeSpan.Zero"]                                                                       = Expression.Constant(TimeSpan.Zero),
        ["TimeSpan(3, 4, 15, 32, 123)"]                                                         = Expression.Constant(new TimeSpan(3, 4, 15, 32)),
        ["TimeSpan(-3, 4, 15, 32, 123)"]                                                        = Expression.Constant(new TimeSpan(3, 4, 15, 32).Negate()),
        // DBNull
        ["DBNull.Value"]                                                                        = Expression.Constant(DBNull.Value),
        // decimal
        ["Decimal.Zero"]                                                                        = Expression.Constant(decimal.Zero),
        ["Decimal.MinusOne"]                                                                    = Expression.Constant(decimal.MinusOne),
        ["Decimal.One"]                                                                         = Expression.Constant(decimal.One),
        ["Decimal.MinValue"]                                                                    = Expression.Constant(decimal.MinValue),
        ["Decimal.MaxValue"]                                                                    = Expression.Constant(decimal.MaxValue),
        ["5.5M"]                                                                                = Expression.Constant(5.5M),
        // GUID
        ["Guid.Empty"]                                                                          = Expression.Constant(Guid.Empty),
        ["Guid(\"00112233-4455-6677-8899-aabbccddeeff\")"]                                      = Expression.Constant(new Guid("00112233-4455-6677-8899-aabbccddeeff")),
        // string
        ["String.Empty"]                                                                        = Expression.Constant(string.Empty),
        ["String.null"]                                                                         = Expression.Constant(null, typeof(string)),
        ["String.abrah-cadabrah"]                                                               = Expression.Constant("abrah-cadabrah"),
        ["String.ала-бала"]                                                                     = Expression.Constant("ала-бала"),
        // Uri
        ["Uri(\"http://www.acme.com\")"]                                                        = Expression.Constant(new Uri("http://www.acme.com")),
        // enum
        ["EnumFlagsTest.One | EnumFlagsTest.Three"]                                             = Expression.Constant(EnumFlagsTest.One | EnumFlagsTest.Three),
        ["EnumTest.Three"]                                                                      = Expression.Constant(EnumTest.Three),
        // nullable primitive
        ["Nullable.int.5"]                                                                      = Expression.Constant(5, typeof(int?)),
        ["Nullable.int.null"]                                                                   = Expression.Constant(null, typeof(int?)),
        ["Nullable.long.5L"]                                                                    = Expression.Constant(5L, typeof(long?)),
        ["Nullable.long.null"]                                                                  = Expression.Constant(null, typeof(long?)),
        ["Nullable.long.long.Min"]                                                              = Expression.Constant(long.MinValue, typeof(long?)),
        ["Nullable.long.long.Max"]                                                              = Expression.Constant(long.MaxValue, typeof(long?)),
        ["Nullable.long.long.IntMin"]                                                           = Expression.Constant(MinJsonInteger, typeof(long?)),
        ["Nullable.long.long.IntMax"]                                                           = Expression.Constant(MaxJsonInteger, typeof(long?)),
        ["Nullable.long.long.IntMin-1"]                                                         = Expression.Constant(MinJsonInteger-1, typeof(long?)),
        ["Nullable.long.long.IntMax+1"]                                                         = Expression.Constant(MaxJsonInteger+1, typeof(long?)),
        ["Nullable.long.long.IntMin+1"]                                                         = Expression.Constant(MinJsonInteger+1, typeof(long?)),
        ["Nullable.long.long.IntMax-1"]                                                         = Expression.Constant(MaxJsonInteger-1, typeof(long?)),
        // objects
        ["Object.null"]                                                                         = Expression.Constant(null),
        ["Object()"]                                                                            = Expression.Constant(new object()),
        ["Object1()"]                                                                           = Expression.Constant(new Object1(), typeof(Object1)),
        ["Object1.null"]                                                                        = Expression.Constant(null, typeof(Object1)),
        ["ClassDataContract1()"]                                                                = Expression.Constant(new ClassDataContract1()),
        ["ClassDataContract2()"]                                                                = Expression.Constant(new ClassDataContract2(1, "two", 3M), typeof(ClassDataContract1)),
        ["ClassSerializable1()"]                                                                = Expression.Constant(new ClassSerializable1()),
        // structs
        ["StructDataContract1.null"]                                                            = Expression.Constant(null, typeof(StructDataContract1?)),
        ["StructDataContract1() { IntProperty = 7, StringProperty = \"vm\" }"]                  = Expression.Constant(new StructDataContract1() { IntProperty = 7, StringProperty = "vm" }, typeof(StructDataContract1)),
        ["StructDataContract1()"]                                                               = Expression.Constant(new StructDataContract1()),
        ["StructDataContract1?() { IntProperty = 7, StringProperty = \"vm\" }"]                 = Expression.Constant((StructDataContract1?)new StructDataContract1() { IntProperty = 7, StringProperty = "vm" }, typeof(StructDataContract1?)),
        ["(StructSerializable1?)null"]                                                          = Expression.Constant(null, typeof(StructSerializable1?)),
        ["StructSerializable1() { IntProperty = 7, StringProperty = \"vm\" }"]                  = Expression.Constant(new StructSerializable1() { IntProperty = 7, StringProperty = "vm" }, typeof(StructSerializable1)),
        ["StructSerializable1()"]                                                               = Expression.Constant(new StructSerializable1()),
        // anonymous
        ["Anonymous"]                                                                           = Expression.Constant(
                                                                                                                        new
                                                                                                                        {
                                                                                                                            ObjectProperty = (object?)null,
                                                                                                                            NullIntProperty = (int?)null,
                                                                                                                            NullLongProperty = (long?)1L,
                                                                                                                            BoolProperty = true,
                                                                                                                            CharProperty = 'A',
                                                                                                                            ByteProperty = (byte)1,
                                                                                                                            SByteProperty = (sbyte)1,
                                                                                                                            ShortProperty = (short)1,
                                                                                                                            IntProperty = 1,
                                                                                                                            LongProperty = (long)1,
                                                                                                                            UShortProperty = (ushort)1,
                                                                                                                            UIntProperty = (uint)1,
                                                                                                                            ULongProperty = (ulong)1,
                                                                                                                            DoubleProperty = 1.0,
                                                                                                                            FloatProperty = (float)1.0,
                                                                                                                            DecimalProperty = 1M,
                                                                                                                            GuidProperty = Guid.Empty,
                                                                                                                            UriProperty = new Uri("http://localhost"),
                                                                                                                            DateTimeProperty = new DateTime(2013, 1, 13),
                                                                                                                            DateTimeOffsetProperty = new DateTimeOffset(new DateTime(2013, 1, 13)),
                                                                                                                            TimeSpanProperty = new TimeSpan(1, 2, 3, 4),
                                                                                                                        }),
        // byte sequences
        ["Bytes.(byte[])null"]                                                                  = Expression.Constant(null, typeof(byte[])),
        ["Bytes.byte[]{}"]                                                                      = Expression.Constant(Array.Empty<byte>()),
        ["Bytes.byte[]{ 1, 2, 3, 1, 2, 3, 1, 2, 3, 10 }"]                                       = Expression.Constant(new byte[]{ 1, 2, 3, 1, 2, 3, 1, 2, 3, 10 }),
        ["Bytes.Memory<byte>()"]                                                                = Expression.Constant(new Memory<byte>()),
        ["Bytes.Memory<byte>([1, 2, 3, 1, 2, 3, 1, 2, 3, 10 ])"]                                = Expression.Constant(new Memory<byte>([1, 2, 3, 1, 2, 3, 1, 2, 3, 10 ])),
        ["Bytes.ReadOnlyMemory<byte>()"]                                                        = Expression.Constant(new ReadOnlyMemory<byte>()),
        ["Bytes.ReadOnlyMemory<byte>([1, 2, 3, 1, 2, 3, 1, 2, 3, 10 ])"]                        = Expression.Constant(new ReadOnlyMemory<byte>([1, 2, 3, 1, 2, 3, 1, 2, 3, 10 ])),
        ["Bytes.ArraySegment<byte>([])"]                                                        = Expression.Constant(new ArraySegment<byte>([])),
        ["Bytes.ArraySegment<byte>([1, 2, 3, 1, 2, 3, 1, 2, 3, 10], 1, 8)"]                     = Expression.Constant(new ArraySegment<byte>([1, 2, 3, 1, 2, 3, 1, 2, 3, 10], 1, 8)),
        // sequences
        ["Ints.(int[])null"]                                                                    = Expression.Constant(null, typeof(int[])),
        ["Ints.int[]{}"]                                                                        = Expression.Constant(Array.Empty<int>()),
        ["Ints.int[]{ 1, 2, 3, 4 }"]                                                            = Expression.Constant(new int[]{ 1, 2, 3, 4 }),
        ["Ints.(int?[])null"]                                                                   = Expression.Constant(null, typeof(int?[])),
        ["Ints.int?[]{}"]                                                                       = Expression.Constant(Array.Empty<int?>()),
        ["Ints.int?[]{ 1, 2, null, null }"]                                                     = Expression.Constant(new int?[]{ 1, 2, null, null }),
        ["Ints.Memory<int>(null)"]                                                              = Expression.Constant(new Memory<int>(null)),
        ["Ints.Memory<int>()"]                                                                  = Expression.Constant(new Memory<int>()),
        ["Ints.Memory<int>([ 1, 2, 3, 4 ])"]                                                    = Expression.Constant(new Memory<int>([ 1, 2, 3, 4 ])),
        ["Ints.(Memory<int>?)null"]                                                             = Expression.Constant(null, typeof(Memory<int>?)),
        ["Ints.(Memory<int>?)([ 1, 2, 3, 4 ])"]                                                 = Expression.Constant(new Memory<int>([ 1, 2, 3, 4 ]), typeof(Memory<int>?)),
        ["Ints.(Memory<int>?)()"]                                                               = Expression.Constant(new Memory<int>(), typeof(Memory<int>?)),
        ["EnumTest?[]{}"]                                                                       = Expression.Constant(Array.Empty<EnumTest?>()),
        ["EnumTest?[]{ EnumTest.One, EnumTest.Two, null, null }"]                               = Expression.Constant(new EnumTest?[]{ EnumTest.One, EnumTest.Two, null, null }),
        ["EnumTest?[].null"]                                                                    = Expression.Constant(null, typeof(EnumTest?[])),
        ["ArraySegment<int>([ 1, 2, 3, 4 ], 1, 2)"]                                             = Expression.Constant(new ArraySegment<int>([ 1, 2, 3, 4 ], 1, 2)),
        ["ArraySegment<int>.null"]                                                              = Expression.Constant(null, typeof(ArraySegment<int>?)),
        ["List<int?>{ 1, 2, null, null }"]                                                      = Expression.Constant(new List<int?>{ 1, 2, null, null }),
        ["List<int?>.null"]                                                                     = Expression.Constant(null, typeof(List<int?>)),
        ["List<int>([1, 2, 3, 4])"]                                                             = Expression.Constant(new List<int>([1, 2, 3, 4])),
        ["LinkedList<int>([1, 2, 3, 4])"]                                                       = Expression.Constant(new LinkedList<int>([1, 2, 3, 4])),
        ["Sequence<int>([1, 2, 3, 4])"]                                                         = Expression.Constant(new Collection<int>([1, 2, 3, 4])),
        ["ReadOnlyCollection<int>([1, 2, 3, 4])"]                                               = Expression.Constant(new ReadOnlyCollection<int>([1, 2, 3, 4])),
        ["ReadOnlyMemory<int>([ 1, 2, 3, 4 ])"]                                                 = Expression.Constant(new ReadOnlyMemory<int>([ 1, 2, 3, 4 ])),
        ["HashSet<int>([1, 2, 3, 4])"]                                                          = Expression.Constant(new HashSet<int>([1, 2, 3, 4])),
        ["SortedSet<int>([1, 2, 3, 4])"]                                                        = Expression.Constant(new SortedSet<int>([1, 2, 3, 4])),
        ["Queue<int>([1, 2, 3, 4])"]                                                            = Expression.Constant(new Queue<int>([1, 2, 3, 4])),
        ["Stack<int>([1, 2, 3, 4])"]                                                            = Expression.Constant(new Stack<int>([1, 2, 3, 4])),
        ["BlockingCollection<double>()"]                                                        = Expression.Constant(new BlockingCollection<double>() { Math.PI, Math.Tau, Math.E }),
        ["ConcurrentBag<int>([1, 2, 3, 4])"]                                                    = Expression.Constant(new ConcurrentBag<int>([1, 2, 3, 4])),
        ["ConcurrentQueue<int>([1, 2, 3, 4])"]                                                  = Expression.Constant(new ConcurrentQueue<int>([1, 2, 3, 4])),
        ["ConcurrentStack<int>([1, 2, 3, 4])"]                                                  = Expression.Constant(new ConcurrentStack<int>([1, 2, 3, 4])),
        ["ImmutableArray.Create()"]                                                             = Expression.Constant(ImmutableArray.Create<int>()),
        ["ImmutableArray.Create(1, 2, 3, 4 )"]                                                  = Expression.Constant(ImmutableArray.Create(1, 2, 3, 4 )),
        ["ImmutableHashSet.Create(1, 2, 3, 4 )"]                                                = Expression.Constant(ImmutableHashSet.Create(1, 2, 3, 4 )),
        ["ImmutableList.Create(1, 2, 3, 4 )"]                                                   = Expression.Constant(ImmutableList.Create(1, 2, 3, 4 )),
        ["ImmutableQueue.Create(1, 2, 3, 4 )"]                                                  = Expression.Constant(ImmutableQueue.Create(1, 2, 3, 4 )),
        ["ImmutableSortedSet.Create(1, 2, 3, 4 )"]                                              = Expression.Constant(ImmutableSortedSet.Create(1, 2, 3, 4 )),
        ["ImmutableStack.Create(1, 2, 3, 4 )"]                                                  = Expression.Constant(ImmutableStack.Create(1, 2, 3, 4 )),
        ["ClassDataContract1[] { new ClassDataContract1()..."]                                  = Expression.Constant(new ClassDataContract1?[] { new(), new ClassDataContract2(), null }),
        ["ClassDataContract1[] { new(0, \"vm\"), new(1, \"vm2 vm\"), }"]                        = Expression.Constant(new ClassDataContract1[] { new(0, "vm"), new(1, "vm2 vm"), }),
        ["Frozen.byte[]{}"]                                                                     = Expression.Constant(Array.Empty<int>().ToFrozenSet()),
        ["Frozen.byte[]{ 1, 2, 3, 1, 2, 3, 1, 2, 3, 10 }"]                                      = Expression.Constant(new byte[]{ 1, 2, 3, 1, 2, 3, 1, 2, 3, 10 }.ToFrozenSet()),
        ["Frozen.int[]{ 1, 2, 3, 4 }"]                                                          = Expression.Constant(new int[]{ 1, 2, 3, 4 }.ToFrozenSet()),
        ["Frozen.int?[]{ 1, 2, null, null }"]                                                   = Expression.Constant(new int?[]{ 1, 2, null, null }.ToFrozenSet()),
        ["Frozen.decimal[]{ 1, 2, 3, 4 }"]                                                      = Expression.Constant(new decimal[]{ 1, 2, 3, 4 }.ToFrozenSet()),
        ["Frozen.EnumTest?[]{ EnumTest.One, EnumTest.Two, null, null }"]                        = Expression.Constant(new EnumTest?[]{ EnumTest.One, EnumTest.Two, null, null }.ToFrozenSet()),
        ["Frozen.anonymous[]"]                                                                  = Expression.Constant(new object?[] {
                                                                                                                        new
                                                                                                                        {
                                                                                                                            ObjectProperty = (object?)null,
                                                                                                                            NullIntProperty = (int?)null,
                                                                                                                            NullLongProperty = (long?)1L,
                                                                                                                            BoolProperty = true,
                                                                                                                            CharProperty = 'A',
                                                                                                                            ByteProperty = (byte)1,
                                                                                                                            SByteProperty = (sbyte)1,
                                                                                                                            ShortProperty = (short)1,
                                                                                                                            IntProperty = 1,
                                                                                                                            LongProperty = (long)1,
                                                                                                                            UShortProperty = (ushort)1,
                                                                                                                            UIntProperty = (uint)1,
                                                                                                                            ULongProperty = (ulong)1,
                                                                                                                            DoubleProperty = 1.0,
                                                                                                                            FloatProperty = (float)1.0,
                                                                                                                            DecimalProperty = 1M,
                                                                                                                            GuidProperty = Guid.Empty,
                                                                                                                            UriProperty = new Uri("http://localhost"),
                                                                                                                            DateTimeProperty = new DateTime(2013, 1, 13),
                                                                                                                            TimeSpanProperty = new TimeSpan(1, 2, 3, 4),
                                                                                                                            DateTimeOffsetProperty = new DateTimeOffset(new DateTime(2013, 1, 13)),
                                                                                                                        },
                                                                                                                        new
                                                                                                                        {
                                                                                                                            ObjectProperty = (object?)null,
                                                                                                                            NullIntProperty = (int?)null,
                                                                                                                            NullLongProperty = (long?)2L,
                                                                                                                            BoolProperty = true,
                                                                                                                            CharProperty = 'A',
                                                                                                                            ByteProperty = (byte)2,
                                                                                                                            SByteProperty = (sbyte)2,
                                                                                                                            ShortProperty = (short)2,
                                                                                                                            IntProperty = 2,
                                                                                                                            LongProperty = (long)2,
                                                                                                                            UShortProperty = (ushort)2,
                                                                                                                            UIntProperty = (uint)2,
                                                                                                                            ULongProperty = (ulong)2,
                                                                                                                            DoubleProperty = 2.0,
                                                                                                                            FloatProperty = (float)2.0,
                                                                                                                            DecimalProperty = 2M,
                                                                                                                            GuidProperty = Guid.Empty,
                                                                                                                            UriProperty = new Uri("http://localhost"),
                                                                                                                            DateTimeProperty = new DateTime(2013, 2, 13),
                                                                                                                            TimeSpanProperty = new TimeSpan(1, 2, 3, 4),
                                                                                                                            DateTimeOffsetProperty = new DateTimeOffset(new DateTime(2013, 2, 13)),
                                                                                                                        },
                                                                                                                        null
                                                                                                                    }.ToFrozenSet()),
        // tuples
        ["Tuple.(Tuple<int, string>)null"]                                                      = Expression.Constant(null, typeof(Tuple<int, string>)),
        ["Tuple.Tuple<int, string>"]                                                            = Expression.Constant(new Tuple<int, string>(1, "one")),
        ["Tuple.ValueTuple<int, string>"]                                                       = Expression.Constant((1, "one")),
        ["Tuple.Tuple<int, String, StructDataContract1>"]                                       = Expression.Constant(new Tuple<int, string, StructDataContract1>(1, "one", new StructDataContract1(2, "two"))),
        ["Tuple.ValueTuple<int, String, StructDataContract1>"]                                  = Expression.Constant((1, "one", new StructDataContract1(2, "two"))),
        // dictionaries
        ["Dictionary<int, string?>{ [1] = \"one\", [2] = \"two\"..."]                           = Expression.Constant(new Dictionary<int, string?>{ [1] = "one", [2] = "two", [3] = null, [4] = null }),
        ["Dictionary<int, string>{ [1] = \"one\", [2] = \"two\" }"]                             = Expression.Constant(new Dictionary<int, string>{ [1] ="one", [2]="two" }),
        ["Frozen.Dictionary<int, string?>..."]                                                  = Expression.Constant(new Dictionary<int, string?>{ [1] = "one", [2] = "two", [3] = null, [4] = null }.ToFrozenDictionary()),
        ["Frozen.Dictionary<int, string>..."]                                                   = Expression.Constant(new Dictionary<int, string>{ [1] = "one", [2] = "two", [3] = "three", }.ToFrozenDictionary()),
        ["Hashtable(new Dictionary<int, string>{ [1] =\"one\", [2]=\"two\" })"]                 = Expression.Constant(new Hashtable(new Dictionary<int, string>{ [1] ="one", [2]="two" })),
        ["ImmutableDictionary.Create<int,string>().Add(...)"]                                   = Expression.Constant(ImmutableDictionary.Create<int,string>().Add(1, "one").Add(2, "two")),
        ["ImmutableSortedDictionary.Create<int,string>().Add(...)"]                             = Expression.Constant(ImmutableSortedDictionary.Create<int,string>().Add(1, "one").Add(2, "two")),
        ["ReadOnlyDictionary<int, string>..."]                                                  = Expression.Constant(new ReadOnlyDictionary<int, string>(new Dictionary<int, string>{ [1] ="one", [2]="two" })),
        ["SortedDictionary<int, string>{ [1] =\"one\", [2]=\"two\" }"]                          = Expression.Constant(new SortedDictionary<int, string>{ [1] ="one", [2]="two" }),
        ["ConcurrentDictionary<int, string>{ [1] = \"one\", [2]=\"two\" }"]                     = Expression.Constant(new ConcurrentDictionary<int, string>{ [1] ="one", [2]="two" }),

        ["StructDataContract1[]"]                                                               = Expression.Constant(new StructDataContract1[]
                                                                                                                        {
                                                                                                                            new() {
                                                                                                                                IntProperty = 0,
                                                                                                                                StringProperty = "vm",
                                                                                                                            },
                                                                                                                            new() {
                                                                                                                                IntProperty = 1,
                                                                                                                                StringProperty = "vm vm",
                                                                                                                            },
                                                                                                                        }),
        ["StructDataContract1?[]"]                                                              = Expression.Constant(new StructDataContract1?[]
                                                                                                                        {
                                                                                                                            new() {
                                                                                                                                IntProperty = 0,
                                                                                                                                StringProperty = "vm",
                                                                                                                            },
                                                                                                                            null,
                                                                                                                            new() {
                                                                                                                                IntProperty = 1,
                                                                                                                                StringProperty = "vm vm",
                                                                                                                            },
                                                                                                                            null
                                                                                                                        }),
        ["StructSerializable1[]"]                                                               = Expression.Constant(new StructSerializable1[]
                                                                                                                        {
                                                                                                                            new () {
                                                                                                                                IntProperty = 0,
                                                                                                                                StringProperty = "vm",
                                                                                                                            },
                                                                                                                            new() {
                                                                                                                                IntProperty = 1,
                                                                                                                                StringProperty = "vm vm",
                                                                                                                            },
                                                                                                                        }),
        ["StructSerializable1?[]"]                                                              = Expression.Constant(new StructSerializable1?[]
                                                                                                                        {
                                                                                                                            new() {
                                                                                                                                IntProperty = 0,
                                                                                                                                StringProperty = "vm",
                                                                                                                            },
                                                                                                                            null,
                                                                                                                            new() {
                                                                                                                                IntProperty = 1,
                                                                                                                                StringProperty = "vm vm",
                                                                                                                            },
                                                                                                                            null
                                                                                                                        }),
    };
}
