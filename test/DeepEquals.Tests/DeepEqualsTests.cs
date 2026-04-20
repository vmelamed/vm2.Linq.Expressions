namespace vm2.Linq.Expressions.DeepEquals.Tests;

using vm2.TestUtilities;

public partial class DeepEqualsTests(
    ITestOutputHelper output) : TestBase(output)
{
    public static readonly TheoryData<string, string, string, bool> Data = new ()
    {
        { TestLine(), "null", "null", true },
        { TestLine(), "object", "object", true },
        { TestLine(), "object", "object1", true },

        { TestLine(), "(byte)5", "(byte)5", true },
        { TestLine(), "(byte)5", "(byte)6", false },
        { TestLine(), "(byte)5", "5", false },

        { TestLine(), "(byte?)5", "(byte?)5", true },
        { TestLine(), "(byte?)5", "(byte?)6", false },
        { TestLine(), "(byte?)5", "(byte?)null", false },
        { TestLine(), "(byte?)null", "(byte?)null", true },
        { TestLine(), "(byte?)5", "5", false },
        { TestLine(), "(byte?)5", "(int?)5", false },

        { TestLine(), "true", "true", true },
        { TestLine(), "true", "false", false },
        { TestLine(), "true", "1", false },

        { TestLine(), "(bool?)true", "(bool?)true", true },
        { TestLine(), "(bool?)true", "(bool?)false", false },
        { TestLine(), "(bool?)true", "(bool?)null", false },
        { TestLine(), "(bool?)null", "(bool?)null", true },
        { TestLine(), "(bool?)true", "(int?)1", false },

        { TestLine(), "'V'", "'V'", true },
        { TestLine(), "'V'", "'M'", false },
        { TestLine(), "'V'", "1", false },

        { TestLine(), "(char?)'V'", "(char?)'V'", true },
        { TestLine(), "(char?)'V'", "(char?)'M'", false },
        { TestLine(), "(char?)'V'", "(char?)null", false },
        { TestLine(), "(char?)null", "(char?)null", true },
        { TestLine(), "(char?)'V'", "(int?)1", false },

        { TestLine(), "1", "1", true },
        { TestLine(), "1", "5", false },
        { TestLine(), "1", "5.0", false },

        { TestLine(), "(int?)1", "(int?)1", true },
        { TestLine(), "(int?)1", "(int?)2", false },
        { TestLine(), "(int?)1", "(int?)null", false },
        { TestLine(), "(int?)null", "(int?)null", true },
        { TestLine(), "(int?)1", "(int?)5.0", false },

        { TestLine(), "Enum.One", "Enum.One", true },
        { TestLine(), "Enum.One", "Enum.Two", false },
        { TestLine(), "Enum.One", "1.0", false },

        { TestLine(), "Enum?.One", "Enum?.One", true },
        { TestLine(), "Enum?.One", "Enum?.Two", false },
        { TestLine(), "Enum?.One", "Enum?.null", false },
        { TestLine(), "Enum?.null", "Enum?.null", true },
        { TestLine(), "Enum?.One", "1.0", false },

        { TestLine(), "one", "one", true },
        { TestLine(), "one", "two", false },
        { TestLine(), "one", "(string)null", false },
        { TestLine(), "(string)null", "(string)null", true },

        { TestLine(), "new DateTime(2024, 4, 13, 23, 18, 26, 234, DateTimeKind.Local)", "new DateTime(2024, 4, 13, 23, 18, 26, 234, DateTimeKind.Local)", true },
        { TestLine(), "new DateTime(2024, 4, 13, 23, 18, 26, 234, DateTimeKind.Local)", "new DateTime(2024, 4, 13, 23, 18, 26, 345, DateTimeKind.Local)", false },

        { TestLine(), "new TimeSpan(3, 4, 15, 32, 123)", "new TimeSpan(3, 4, 15, 32, 123)", true },
        { TestLine(), "new TimeSpan(3, 4, 15, 32, 123)", "new TimeSpan(3, 4, 15, 32, 234)", false },

        { TestLine(), "new DateTimeOffset(2024, 4, 13, 23, 18, 26, 234, new TimeSpan(0, -300, 0))", "new DateTimeOffset(2024, 4, 13, 23, 18, 26, 234, new TimeSpan(0, -300, 0))", true },
        { TestLine(), "new DateTimeOffset(2024, 4, 13, 23, 18, 26, 234, new TimeSpan(0, -300, 0))", "new DateTimeOffset(2024, 4, 13, 23, 18, 26, 345, new TimeSpan(0, -300, 0))", false },

        { TestLine(), "ArraySegment<byte>1", "ArraySegment<byte>1", true },
        { TestLine(), "ArraySegment<byte>1", "ArraySegment<byte>2", false },

        { TestLine(), "(IntPtr)5", "(IntPtr)5", true },
        { TestLine(), "(IntPtr)5", "(IntPtr)6", false },

        { TestLine(), "new decimal[]{ 1, 2, 3, 4 }.ToFrozenSet()", "new decimal[]{ 1, 2, 3, 4 }.ToFrozenSet()", true },
        { TestLine(), "new decimal[]{ 1, 2, 3, 4 }.ToFrozenSet()", "new decimal[]{ 1, 2, 3, 5 }.ToFrozenSet()", false },

        { TestLine(), "new Queue<int>([ 1, 2, 3, 4 ])", "new Queue<int>([ 1, 2, 3, 4 ])", true },
        { TestLine(), "new Queue<int>([ 1, 2, 3, 4 ])", "new Queue<int>([ 1, 2, 3, 5 ])", false },

        { TestLine(), "ImmutableArray.Create(1, 2, 3, 5 )", "ImmutableArray.Create(1, 2, 3, 5 )", true },
        { TestLine(), "ImmutableArray.Create(1, 2, 3, 5 )", "ImmutableArray.Create(1, 2, 3, 4 )", false },

        { TestLine(), "new ConcurrentStack<int>([1, 2, 3, 4])", "new ConcurrentStack<int>([1, 2, 3, 4])", true },
        { TestLine(), "new ConcurrentStack<int>([1, 2, 3, 4])", "new ConcurrentStack<int>([1, 2, 3, 5])", false },

        { TestLine(), "new Dictionary<int, string>{ [1] =\"one\", [2]=\"two\" }.ToFrozenDictionary()", "new Dictionary<int, string>{ [1] =\"one\", [2]=\"two\" }.ToFrozenDictionary()", true },
        { TestLine(), "new Dictionary<int, string>{ [1] =\"one\", [2]=\"two\" }.ToFrozenDictionary()", "new Dictionary<int, string>{ [1] =\"one\", [3]=\"three\" }.ToFrozenDictionary()", false },

        { TestLine(), "new Hashtable{ [1] =\"one\", [2]=\"two\" }", "new Hashtable{ [1] =\"one\", [2]=\"two\" }", true },
        { TestLine(), "new Hashtable{ [1] =\"one\", [2]=\"two\" }", "new Hashtable{ [1] =\"one\", [3]=\"three\" }", false },

        { TestLine(), "new ClassDataContract1()", "new ClassDataContract1()", true },
        { TestLine(), "new ClassDataContract1()", "new ClassDataContract2()", false },

        { TestLine(), "new ClassDataContract1[]", "new ClassDataContract1[]", true },
        { TestLine(), "new ClassDataContract1[]", "new ClassDataContract2[]", false },

        // assign:
        { TestLine(), "a = 1", "a = 1", true },
        { TestLine(), "a = 1", "a = 2", false },
        { TestLine(), "a = b", "a = b", true },
        { TestLine(), "a = b", "a = 2", false },
        { TestLine(), "a = b", "a += 1", false },

        { TestLine(), "a += b", "a += b", true },
        { TestLine(), "a += b", "a += 1", false },
        { TestLine(), "a += b", "a -= b", false },
        { TestLine(), "checked(a *= b)", "checked(a *= b)", true },
        { TestLine(), "checked(a *= b)", "a *= b", false },
        { TestLine(), "checked(a *= b)", "checked(a *= 2)", false },

        // unary:
        { TestLine(), "a => increment(a)", "a => increment(a)", true },
        { TestLine(), "a => increment(a)", "b => increment(b)", false },
        { TestLine(), "a => increment(a)", "a => decrement(a)", false },
        { TestLine(), "a => decrement(a)", "a => decrement(a)", true },
        { TestLine(), "a => decrement(a)", "b => decrement(b)", false },
        { TestLine(), "a => decrement(a)", "a => increment(a)", false },
        { TestLine(), "a => ++a", "a => ++a", true },
        { TestLine(), "a => ++a", "a => a++", false },
        { TestLine(), "a => a++", "a => a++", true },
        { TestLine(), "a => a++", "a => ++a", false },
        { TestLine(), "a => --a", "a => --a", true },
        { TestLine(), "a => a - 1", "a => --a", false },
        { TestLine(), "a => a--", "a => a--", true },

        // binary:
        { TestLine(), "(a, b) => a - b", "(a, b) => a - b", true },
        { TestLine(), "(a, b) => a - b", "(a, b) => b - a", false },
        { TestLine(), "(a, b) => a - b", "(a, b) => a + b", false },
        { TestLine(), "(a, b) => checked(a * b)", "(a, b) => checked(a * b)", true },
        { TestLine(), "(a, b) => checked(a * b)", "(a, b) => checked(b * a)", false },
        { TestLine(), "(a, b) => checked(a * b)", "(b, a) => checked(a * b)", false },

        // lambda:
        { TestLine(), "(s,d) => true", "(s,d) => true", true },
        { TestLine(), "(s,d) => true", "(s`,d`) => true", false },
        { TestLine(), "(s,d) => true", "(d,s) => true", false },
        { TestLine(), "(s,d) => true", "(s,d) => false", false },
        { TestLine(), "i => true", "i => true", true },
        { TestLine(), "i => true", "i => false", false },
        { TestLine(), "a => a._a", "a => a._a", true },
        { TestLine(), "a => a._a", "a => a._b", false },
        { TestLine(), "a => a.A", "a => a.A", true },
        { TestLine(), "a => a.A", "a => a.B", false },
        { TestLine(), "a => a.Method1()", "a => a.Method1()", true },
        { TestLine(), "a => a.Method3(1,1)", "a => a.Method3(1,1)", true },
        { TestLine(), "a => a.Method3(1,2)", "a => a.Method3(1,1)", false },
        { TestLine(), "a => a.Method4(42,3.14)", "a => a.Method4(42,3.14)", true },
        { TestLine(), "a => a.Method4(42,3.14)", "a => a.Method4(23,2.71)", false },

        // statements:
        { TestLine(), "() => new StructDataContract1", "() => new StructDataContract1", true },
        { TestLine(), "(a,b) => { ... }", "(a,b) => { ... }", true },
        { TestLine(), "(f,a) => f(a)", "(f,a) => f(a)", true },
        { TestLine(), "accessMemberMember", "accessMemberMember", true },
        { TestLine(), "accessMemberMember1", "accessMemberMember1", true },
        { TestLine(), "arrayAccessExpr", "arrayAccessExpr", true },
        { TestLine(), "b => b ? 1 : 3", "b => b ? 1 : 3", true },
        { TestLine(), "Console.WriteLine", "Console.WriteLine", true },
        { TestLine(), "goto1", "goto1", true },
        { TestLine(), "goto2", "goto2", true },
        { TestLine(), "goto3", "goto3", true },
        { TestLine(), "goto4", "goto4", true },
        { TestLine(), "indexMember", "indexMember", true },
        { TestLine(), "array[index]", "array[index]", true },
        { TestLine(), "indexObject1", "indexObject1", true },
        { TestLine(), "loop", "loop", true },
        { TestLine(), "newArrayBounds", "newArrayBounds", true },
        { TestLine(), "newArrayItems", "newArrayItems", true },
        { TestLine(), "newDictionaryInit", "newDictionaryInit", true },
        { TestLine(), "newListInit", "newListInit", true },
        { TestLine(), "newMembersInit", "newMembersInit", true },
        { TestLine(), "newMembersInit1", "newMembersInit1", true },
        { TestLine(), "newMembersInit2", "newMembersInit2", true },
        { TestLine(), "return1", "return1", true },
        { TestLine(), "return2", "return2", true },
        { TestLine(), "switch(a){ ... }", "switch(a){ ... }", true },
        { TestLine(), "throw", "throw", true },
        { TestLine(), "try1", "try1", true },
        { TestLine(), "try2", "try2", true },
        { TestLine(), "try3", "try3", true },
        { TestLine(), "try4", "try4", true },
        { TestLine(), "try5", "try5", true },
        { TestLine(), "try6", "try6", true },

        // inequality: block
        { TestLine(), "(a,b) => { ... }", "(a,b) => { diffVars }", false },
        { TestLine(), "(a,b) => { ... }", "(a,b) => { diffExprs }", false },

        // inequality: conditional
        { TestLine(), "condTrue3", "condTrue3", true },
        { TestLine(), "condTrue3", "condTrue5", false },

        // inequality: switch
        { TestLine(), "switch(a){ ... }", "switch(a){ diffDefault }", false },
        { TestLine(), "switch(a){ ... }", "switch(a){ diffCases }", false },

        // inequality: try/catch
        { TestLine(), "try2", "try2DiffHandlers", false },
        { TestLine(), "try2", "try2DiffCatchType", false },

        // inequality: loop labels
        { TestLine(), "loop", "loopDiffLabel", false },

        // inequality: goto kind
        { TestLine(), "goto1", "goto1Return", false },

        // inequality: index
        { TestLine(), "indexObject1", "indexObject1DiffValue", false },

        // inequality: invocation argument count
        { TestLine(), "invoke1", "invoke1", true },
        { TestLine(), "invoke1", "invoke2", false },

        // inequality: new array element count
        { TestLine(), "newArrayItems3", "newArrayItems3", true },
        { TestLine(), "newArrayItems3", "newArrayItems4", false },

        // inequality: list init
        { TestLine(), "newListInit2", "newListInit2", true },
        { TestLine(), "newListInit2", "newListInit3", false },

        // inequality: member init binding count
        { TestLine(), "memberInit1Binding", "memberInit1Binding", true },
        { TestLine(), "memberInit1Binding", "memberInit2Bindings", false },

        // inequality: method call different method
        { TestLine(), "callMethod1", "callMethod1", true },
        { TestLine(), "callMethod1", "callMethod2", false },

        // inequality: new with different args
        { TestLine(), "newCDC1_1a", "newCDC1_1a", true },
        { TestLine(), "newCDC1_1a", "newCDC1_2b", false },
    };

    [Theory]
    [MemberData(nameof(Data))]
    public void ExpressionTest(string _, string expr1, string expr2, bool areEqual)
    {
        Expression x1 = _substituteExpressions[expr1]();
        Expression x2 = _substituteExpressions[expr2]();
        string difference;

        if (areEqual)
            x1.DeepEquals(x2, out difference).Should().BeTrue(difference);
        else
            x1.DeepEquals(x2, out difference).Should().BeFalse(difference);

        Out.WriteLine(difference);
    }
}
