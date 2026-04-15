namespace vm2.Linq.Expressions.Serialization.TestData;

public static class UnaryTestData
{
    /// <summary>
    /// Gets the expression mapped to the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Expression.</returns>
    public static Expression GetExpression(string id) => _substitutes[id];

    public static readonly TheoryData<string, string, string> Data = new ()
    {
        { TestLine(), "default(bool)",                  "Default.bool" },
        { TestLine(), "default(char)",                  "Default.char" },
        { TestLine(), "default(double)",                "Default.double" },
        { TestLine(), "default(half)",                  "Default.half" },
        { TestLine(), "default(int)",                   "Default.int" },
        { TestLine(), "default(long)",                  "Default.long" },
        { TestLine(), "default(DateTime)",              "Default.DateTime" },
        { TestLine(), "default(DateTimeOffset)",        "Default.DateTimeOffset" },
        { TestLine(), "default(TimeSpan)",              "Default.TimeSpan" },
        { TestLine(), "default(decimal)",               "Default.decimal" },
        { TestLine(), "default(Guid)",                  "Default.Guid" },
        { TestLine(), "default(string)",                "Default.string" },
        { TestLine(), "default(object)",                "Default.object" },
        { TestLine(), "default(ClassDataContract1)",    "Default.ClassDataContract1" },
        { TestLine(), "default(StructDataContract1)",   "Default.StructDataContract1" },
        { TestLine(), "default(int?)",                  "Default.int0" },
        { TestLine(), "default(StructDataContract1?)",  "Default.StructDataContract10" },

        { TestLine(), "(C c) => c as A",                "AsType" },
        { TestLine(), "(object c) => c as int?",        "ObjectAsNullable" },
        { TestLine(), "(int a) => () => a",             "Quote" },
        { TestLine(), "(double a) => checked((int)a)",  "ConvertChecked" },
        { TestLine(), "(double a) => (int)a",           "Convert" },
        { TestLine(), "(int[] a) => a.GetLength",       "ArrayLength" },
        { TestLine(), "(bool a) => !a",                 "Not" },
        { TestLine(), "(int a) => checked(-a)",         "NegateChecked" },
        { TestLine(), "(int a) => -a",                  "Negate" },
        { TestLine(), "(int a) => ~a",                  "BitwiseNot" },

        { TestLine(), "(A a) => +a",                    "UnaryPlusMethod" },
        { TestLine(), "(A a) => -a",                    "UnaryMinusMethod" },
        { TestLine(), "(B b) => !b",                    "UnaryNotMethod" },

        { TestLine(), "IsTrue(D)",                       "IsTrue" },
        { TestLine(), "IsFalse(D)",                      "IsFalse" },
        { TestLine(), "Unbox(object->int)",              "Unbox" },
    };

    static ParameterExpression _pa = Expression.Parameter(typeof(int), "a");

    static Dictionary<string, Expression> _substitutes = new()
    {
        ["default(bool)"]                   = Expression.Default(typeof(bool)),
        ["default(char)"]                   = Expression.Default(typeof(char)),
        ["default(double)"]                 = Expression.Default(typeof(double)),
        ["default(half)"]                   = Expression.Default(typeof(Half)),
        ["default(int)"]                    = Expression.Default(typeof(int)),
        ["default(long)"]                   = Expression.Default(typeof(long)),
        ["default(DateTime)"]               = Expression.Default(typeof(DateTime)),
        ["default(DateTimeOffset)"]         = Expression.Default(typeof(DateTimeOffset)),
        ["default(TimeSpan)"]               = Expression.Default(typeof(TimeSpan)),
        ["default(decimal)"]                = Expression.Default(typeof(decimal)),
        ["default(Guid)"]                   = Expression.Default(typeof(Guid)),
        ["default(string)"]                 = Expression.Default(typeof(string)),
        ["default(object)"]                 = Expression.Default(typeof(object)),
        ["default(ClassDataContract1)"]     = Expression.Default(typeof(ClassDataContract1)),
        ["default(StructDataContract1)"]    = Expression.Default(typeof(StructDataContract1)),
        ["default(int?)"]                   = Expression.Default(typeof(int?)),
        ["default(StructDataContract1?)"]   = Expression.Default(typeof(StructDataContract1?)),

        ["(C c) => c as A"]                 = (C c) => c as A,
        ["(object c) => c as int?"]         = (object c) => c as int?,
        ["(int a) => () => a"]              = Expression.Quote(Expression.Lambda(_pa)),
        ["(double a) => checked((int)a)"]   = (double a) => checked((int)a),
        ["(double a) => (int)a"]            = (double a) => (int)a,
        ["(int[] a) => a.GetLength"]        = (int[] a) => a.Length,
        ["(bool a) => !a"]                  = (bool a) => !a,
        ["(int a) => checked(-a)"]          = (int a) => checked(-a),
        ["(int a) => -a"]                   = (int a) => -a,
        ["(int a) => ~a"]                   = (int a) => ~a,

        ["(A a) => +a"]                     = (A a) => +a,
        ["(A a) => -a"]                     = (A a) => -a,
        ["(B b) => !b"]                     = (B b) => !b,

        ["IsTrue(D)"]                       = Expression.IsTrue(Expression.Parameter(typeof(D), "d")),
        ["IsFalse(D)"]                      = Expression.IsFalse(Expression.Parameter(typeof(D), "d")),
        ["Unbox(object->int)"]              = Expression.Unbox(Expression.Parameter(typeof(object), "o"), typeof(int)),
    };
}
