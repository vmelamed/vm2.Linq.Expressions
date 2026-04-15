namespace vm2.Linq.Expressions.Serialization.TestData;

public static class AssignmentTestData
{
    /// <summary>
    /// Gets the expression mapped to the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Expression.</returns>
    public static Expression GetExpression(string id) => _substitutes[id];

    public static readonly TheoryData<string, string, string> Data = new ()
    {
        { TestLine(), "a = 1",              "AssignConstant" },
        { TestLine(), "a = b",              "AssignVariable" },
        { TestLine(), "a += b",             "AddAssign" },
        { TestLine(), "checked(a += b)",    "AddAssignChecked" },
        { TestLine(), "a -= b",             "SubtractAssign" },
        { TestLine(), "checked(a -= b)",    "SubtractAssignChecked" },
        { TestLine(), "a *= b",             "MultiplyAssign" },
        { TestLine(), "checked(a *= b)",    "MultiplyAssignChecked" },
        { TestLine(), "a /= b",             "DivideAssign" },
        { TestLine(), "a %= b",             "ModuloAssign" },
        { TestLine(), "a &= b",             "AndAssign" },
        { TestLine(), "a |= b",             "OrAssign" },
        { TestLine(), "a ^= b",             "XorAssign" },
        { TestLine(), "x **= z",            "PowerAssign" },
        { TestLine(), "a <<= b",            "LShiftAssign" },
        { TestLine(), "a >>= b",            "RShiftAssign" },
    };

    static readonly ParameterExpression _paramA = Expression.Parameter(typeof(int), "a");
    static readonly ParameterExpression _paramB = Expression.Parameter(typeof(int), "b");
    static readonly ParameterExpression _paramX = Expression.Parameter(typeof(double), "x");
    static readonly ParameterExpression _paramZ = Expression.Parameter(typeof(double), "z");

    static readonly Dictionary<string, Expression> _substitutes = new()
    {
        ["a = 1"]           = Expression.Assign(_paramA, Expression.Constant(1)),
        ["a = b"]           = Expression.Assign(_paramA, _paramB),
        ["a += b"]          = Expression.AddAssign(_paramA, _paramB),
        ["checked(a += b)"] = Expression.AddAssignChecked(_paramA, _paramB),
        ["a -= b"]          = Expression.SubtractAssign(_paramA, _paramB),
        ["checked(a -= b)"] = Expression.SubtractAssignChecked(_paramA, _paramB),
        ["a *= b"]          = Expression.MultiplyAssign(_paramA, _paramB),
        ["checked(a *= b)"] = Expression.MultiplyAssignChecked(_paramA, _paramB),
        ["a /= b"]          = Expression.DivideAssign(_paramA, _paramB),
        ["a %= b"]          = Expression.ModuloAssign(_paramA, _paramB),
        ["a &= b"]          = Expression.AndAssign(_paramA, _paramB),
        ["a |= b"]          = Expression.OrAssign(_paramA, _paramB),
        ["a ^= b"]          = Expression.ExclusiveOrAssign(_paramA, _paramB),
        ["a <<= b"]         = Expression.LeftShiftAssign(_paramA, _paramB),
        ["a >>= b"]         = Expression.RightShiftAssign(_paramA, _paramB),
        ["x **= z"]         = Expression.PowerAssign(_paramX, _paramZ)
    };
}
