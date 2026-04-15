namespace vm2.Linq.Expressions.Serialization.TestData;

public static class ChangeByOneTestData
{
    /// <summary>
    /// Gets the expression mapped to the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Expression.</returns>
    public static Expression GetExpression(string id) => _substitutes[id];

    public static readonly TheoryData<string, string, string> Data = new ()
    {
        { TestLine(), "a => increment(a)", "Increment" },
        { TestLine(), "a => decrement(a)", "Decrement" },
        { TestLine(), "a => ++a",          "PreIncrementAssign" },
        { TestLine(), "a => a++",          "PostIncrementAssign" },
        { TestLine(), "a => --a",          "PreDecrementAssign" },
        { TestLine(), "a => a--",          "PostDecrementAssign" },
    };

    static ParameterExpression _pa = Expression.Parameter(typeof(int), "a");

    static Dictionary<string, Expression> _substitutes = new()
    {
        ["a => increment(a)"] = Expression.Lambda(Expression.Increment(_pa), _pa),
        ["a => decrement(a)"] = Expression.Lambda(Expression.Decrement(_pa), _pa),
        ["a => ++a"]          = Expression.Lambda(Expression.PreIncrementAssign(_pa), _pa),
        ["a => a++"]          = Expression.Lambda(Expression.PostIncrementAssign(_pa), _pa),
        ["a => --a"]          = Expression.Lambda(Expression.PreDecrementAssign(_pa), _pa),
        ["a => a--"]          = Expression.Lambda(Expression.PostDecrementAssign(_pa), _pa),
    };
}
