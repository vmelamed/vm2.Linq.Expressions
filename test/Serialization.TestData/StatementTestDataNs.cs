namespace vm2.Linq.Expressions.Serialization.TestData;

#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0056 // Use index operator

public static class StatementTestDataNs
{
    /// <summary>
    /// Gets the expression mapped to the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Expression.</returns>
    public static Expression GetExpression(string id) => _substitutes[id];

    public static readonly TheoryData<string, string, string> Data = new ()
    {
        { TestLine(), "newMembersInit1",                "NewMembersInit1" },
        { TestLine(), "newMembersInit2",                "NewMembersInit2" },
    };

    static readonly Dictionary<string, Expression> _substitutes = new()
    {
        ["newMembersInit1"]         = () => () => new TestMembersInitialized1()
        {
            TheOuterIntProperty = 42,
            Time = new DateTime(1776, 7, 4),
            InnerProperty = new Inner
            {
                IntProperty = 23,
                StringProperty = "inner string"
            },
            ArrayProperty = new[] { 4, 5, 6 },
            ListProperty = {
                new Inner()
                {
                    IntProperty = 23,
                    StringProperty = "inner string"
                },
                new Inner ()
                {
                    IntProperty = 42,
                    StringProperty = "next inner string"
                }
            },
        },
        ["newMembersInit2"]         = () => () => new TestMembersInitialized1()
        {
            TheOuterIntProperty = 42,
            Time = new DateTime(1776, 7, 4),
            InnerProperty = { IntProperty = 23, StringProperty = "inner string" },
            ArrayProperty = new int[] { 4, 5, 6 },
            ListProperty =
            {
                new Inner()
                {
                    IntProperty = 23,
                    StringProperty = "inner string"
                },
                new Inner ()
                {
                    IntProperty = 42,
                    StringProperty = "next inner string" }
            },
        },
    };
}
