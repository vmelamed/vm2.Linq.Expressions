namespace vm2.Linq.Expressions.Serialization.TestData;

public static class ConstantTestDataNs
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
    public static Expression GetExpression(string id) => _substitutes[id];

    public static readonly TheoryData<string, string, string> Data = new ()
    {
        // double - see https://github.com/gregsdennis/json-everything/issues/747#issuecomment-2171905465
        { TestLine(), "Double.MinValue",                                                        "Double.MinValue" },
        { TestLine(), "Double.MaxValue",                                                        "Double.MaxValue" },
        { TestLine(), "Double.float.MinValue",                                                  "Double.Float.MinValue" },
        { TestLine(), "Double.float.MaxValue",                                                  "Double.Float.MaxValue" },
        { TestLine(), "Double.BigValue",                                                        "Double.BigValue" },
        { TestLine(), "Double.SmallValue",                                                      "Double.SmallValue" },
        { TestLine(), "Double.-3.4028234663852886E+38",                                         "Double.3.40..E38" },
        { TestLine(), "Double.3.4028234663852886E+38",                                          "Double.-3.40..E38" },
        // float
        { TestLine(), "Float.MinValue",                                                         "Float.MinValue" },
        { TestLine(), "Float.MaxValue",                                                         "Float.MaxValue" },
        // Half
        { TestLine(), "Half.NaN",                                                               "Half.NaN" },  // Jason.Schema doesn't like this either
    };

    static readonly Dictionary<string, ConstantExpression> _substitutes = new()
    {
        // double
        ["Double.MinValue"]                                                                     = Expression.Constant(double.MinValue),
        ["Double.MaxValue"]                                                                     = Expression.Constant(double.MaxValue),
        ["Double.float.MinValue"]                                                               = Expression.Constant((double)float.MinValue),
        ["Double.float.MaxValue"]                                                               = Expression.Constant((double)float.MaxValue),
        ["Double.BigValue"]                                                                     = Expression.Constant(1.7976931348623157e308),
        ["Double.SmallValue"]                                                                   = Expression.Constant(-1.7976931348623157e+308),
        ["Double.-3.4028234663852886E+38"]                                                      = Expression.Constant(-3.4028234663852886E+38),
        ["Double.3.4028234663852886E+38"]                                                       = Expression.Constant(3.4028234663852886E+38),
        // float
        ["Float.MinValue"]                                                                      = Expression.Constant(float.MinValue),
        ["Float.MaxValue"]                                                                      = Expression.Constant(float.MaxValue),
        // Half
        ["Half.NaN"]                                                                            = Expression.Constant(Half.NaN),
    };
}
