namespace vm2.Linq.Expressions.Serialization.TestData;

public static class BinaryTestData
{
    /// <summary>
    /// Gets the expression mapped to the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Expression.</returns>
    public static Expression GetExpression(string id) => _substitutes[id];

    public static readonly TheoryData<string, string, string> Data = new ()
    {
        { TestLine(), "(a, b) => checked(a - b)",     "SubtractChecked" },
        { TestLine(), "(a, b) => a - b",              "Subtract" },
        { TestLine(), "(a, b) => a >> b",             "RightShift" },
        { TestLine(), "(a, b) => a ^ b",              "Xor" },
        { TestLine(), "(a, b) => a || b",             "OrElse" },
        { TestLine(), "(a, b) => a | b",              "Or" },
        { TestLine(), "(a, b) => a != b",             "NotEqual" },
        { TestLine(), "(a, b) => checked(a * b)",     "MultiplyChecked" },
        { TestLine(), "(a, b) => a * b",              "Multiply" },
        { TestLine(), "(a, b) => a % b",              "Modulo" },
        { TestLine(), "(a, b) => a <= b",             "LessThanOrEqual" },
        { TestLine(), "(a, b) => a < b",              "LessThan" },
        { TestLine(), "(a, b) => a << b",             "LeftShift" },
        { TestLine(), "(a, b) => a >= b",             "GreaterThanOrEqual" },
        { TestLine(), "(a, b) => a > b",              "GreaterThan" },
        { TestLine(), "(a, b) => a ^ b",              "ExclusiveOr" },
        { TestLine(), "(a, b) => a == b",             "Equal" },
        { TestLine(), "(a, b) => a / b",              "Divide" },
        { TestLine(), "(a, b) => a ?? b",             "Coalesce" },
        { TestLine(), "(a, i) => a[i]",               "ArrayIndex" },
        { TestLine(), "(a, b) => a && b",             "AndAlso" },
        { TestLine(), "(a, b) => a & b",              "And" },
        { TestLine(), "(a, b) => (a + b) * 42",       "MultiplyAdd" },
        { TestLine(), "(a, b) => a + b * 42",         "AddMultiply" },
        { TestLine(), "(a, b) => checked(a + b)",     "AddChecked" },
        { TestLine(), "(a, b) => a + (b + c)",        "AddParenAdd" },
        { TestLine(), "(a, b) => a + b + c",          "AddAdd" },
        { TestLine(), "(a, b) => a + b",              "Add" },
        { TestLine(), "a => a as b",                  "ExprAsType" },
        { TestLine(), "a => a is b",                  "ExprIsType" },
        { TestLine(), "a => a equals int",            "ExprTypeEqual" },
        { TestLine(), "(a, b) => a ** b",             "Power" },
    };

    static ParameterExpression _paramA = Expression.Parameter(typeof(int), "a");

    static Dictionary<string, Expression> _substitutes = new()
    {
        ["(a, b) => checked(a - b)"]            = (int a, int b) => checked(a - b),
        ["(a, b) => a - b"]                     = (int a, int b) => a - b,
        ["(a, b) => a >> b"]                    = (int a, int b) => a >> b,
        ["(a, b) => a ^ b"]                     = (int a, int b) => a ^ b,
        ["(a, b) => a || b"]                    = (bool a, bool b) => a || b,
        ["(a, b) => a | b"]                     = (int a, int b) => a | b,
        ["(a, b) => a != b"]                    = (int a, int b) => a != b,
        ["(a, b) => checked(a * b)"]            = (int a, int b) => checked(a * b),
        ["(a, b) => a * b"]                     = (int a, int b) => a * b,
        ["(a, b) => a % b"]                     = (int a, int b) => a % b,
        ["(a, b) => a <= b"]                    = (int a, int b) => a <= b,
        ["(a, b) => a < b"]                     = (int a, int b) => a < b,
        ["(a, b) => a << b"]                    = (int a, int b) => a << b,
        ["(a, b) => a >= b"]                    = (int a, int b) => a >= b,
        ["(a, b) => a > b"]                     = (int a, int b) => a > b,
        ["(a, b) => a == b"]                    = (int a, int b) => a == b,
        ["(a, b) => a / b"]                     = (int a, int b) => a / b,
        ["(a, b) => a ?? b"]                    = (int? a, int b) => a ?? b,
        ["(a, i) => a[i]"]                      = (int[] a, int i) => a[i],
        ["(a, b) => a && b"]                    = (bool a, bool b) => a && b,
        ["(a, b) => a & b"]                     = (int a, int b) => a & b,
        ["(a, b) => (a + b) * 42"]              = (int a, int b) => (a + b) * 42,
        ["(a, b) => a + b * 42"]                = (int a, int b) => a + b * 42,
        ["(a, b) => checked(a + b)"]            = (int a, int b) => checked(a + b),
        ["(a, b) => a + (b + c)"]               = (int a, int b, int c) => a + (b + c),
        ["(a, b) => a + b + c"]                 = (int a, int b, int c) => a + b + c,
        ["(a, b) => a + b"]                     = (int a, int b) => a + b,
        ["a => a as b"]                         = (ClassDataContract2 a) => a as ClassDataContract1,
        ["a => a is b"]                         = (object a) => a is ClassDataContract1,
        ["a => a equals int"]                   = Expression.Lambda(Expression.TypeEqual(_paramA, typeof(int)), _paramA),
        ["(a, b) => a ** b"]                    = Expression.Lambda(Expression.Power(Expression.Constant(2.0), Expression.Constant(3.0))),
    };
}
