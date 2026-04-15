namespace vm2.Linq.Expressions.DeepEquals.Tests;

public partial class DeepEqualsTests(
    DeepEqualsTestsFixture fixture,
    ITestOutputHelper output) : IClassFixture<DeepEqualsTestsFixture>
{
    public ITestOutputHelper Out { get; } = output;

    protected DeepEqualsTestsFixture _fixture = fixture;

    [Theory]
    [InlineData("null", "null", true)]
    [InlineData("object", "object", true)]
    [InlineData("object", "object1", true)]

    [InlineData("(byte)5", "(byte)5", true)]
    [InlineData("(byte)5", "(byte)6", false)]
    [InlineData("(byte)5", "5", false)]

    [InlineData("(byte?)5", "(byte?)5", true)]
    [InlineData("(byte?)5", "(byte?)6", false)]
    [InlineData("(byte?)5", "(byte?)null", false)]
    [InlineData("(byte?)null", "(byte?)null", true)]
    [InlineData("(byte?)5", "5", false)]
    [InlineData("(byte?)5", "(int?)5", false)]

    [InlineData("true", "true", true)]
    [InlineData("true", "false", false)]
    [InlineData("true", "1", false)]

    [InlineData("(bool?)true", "(bool?)true", true)]
    [InlineData("(bool?)true", "(bool?)false", false)]
    [InlineData("(bool?)true", "(bool?)null", false)]
    [InlineData("(bool?)null", "(bool?)null", true)]
    [InlineData("(bool?)true", "(int?)1", false)]

    [InlineData("'V'", "'V'", true)]
    [InlineData("'V'", "'M'", false)]
    [InlineData("'V'", "1", false)]

    [InlineData("(char?)'V'", "(char?)'V'", true)]
    [InlineData("(char?)'V'", "(char?)'M'", false)]
    [InlineData("(char?)'V'", "(char?)null", false)]
    [InlineData("(char?)null", "(char?)null", true)]
    [InlineData("(char?)'V'", "(int?)1", false)]

    [InlineData("1", "1", true)]
    [InlineData("1", "5", false)]
    [InlineData("1", "5.0", false)]

    [InlineData("(int?)1", "(int?)1", true)]
    [InlineData("(int?)1", "(int?)2", false)]
    [InlineData("(int?)1", "(int?)null", false)]
    [InlineData("(int?)null", "(int?)null", true)]
    [InlineData("(int?)1", "(int?)5.0", false)]

    [InlineData("Enum.One", "Enum.One", true)]
    [InlineData("Enum.One", "Enum.Two", false)]
    [InlineData("Enum.One", "1.0", false)]

    [InlineData("Enum?.One", "Enum?.One", true)]
    [InlineData("Enum?.One", "Enum?.Two", false)]
    [InlineData("Enum?.One", "Enum?.null", false)]
    [InlineData("Enum?.null", "Enum?.null", true)]
    [InlineData("Enum?.One", "1.0", false)]

    [InlineData("one", "one", true)]
    [InlineData("one", "two", false)]
    [InlineData("one", "(string)null", false)]
    [InlineData("(string)null", "(string)null", true)]

    [InlineData("new DateTime(2024, 4, 13, 23, 18, 26, 234, DateTimeKind.Local)", "new DateTime(2024, 4, 13, 23, 18, 26, 234, DateTimeKind.Local)", true)]
    [InlineData("new DateTime(2024, 4, 13, 23, 18, 26, 234, DateTimeKind.Local)", "new DateTime(2024, 4, 13, 23, 18, 26, 345, DateTimeKind.Local)", false)]

    [InlineData("new TimeSpan(3, 4, 15, 32, 123)", "new TimeSpan(3, 4, 15, 32, 123)", true)]
    [InlineData("new TimeSpan(3, 4, 15, 32, 123)", "new TimeSpan(3, 4, 15, 32, 234)", false)]

    [InlineData("new DateTimeOffset(2024, 4, 13, 23, 18, 26, 234, new TimeSpan(0, -300, 0))", "new DateTimeOffset(2024, 4, 13, 23, 18, 26, 234, new TimeSpan(0, -300, 0))", true)]
    [InlineData("new DateTimeOffset(2024, 4, 13, 23, 18, 26, 234, new TimeSpan(0, -300, 0))", "new DateTimeOffset(2024, 4, 13, 23, 18, 26, 345, new TimeSpan(0, -300, 0))", false)]

    [InlineData("ArraySegment<byte>1", "ArraySegment<byte>1", true)]
    [InlineData("ArraySegment<byte>1", "ArraySegment<byte>2", false)]

    [InlineData("(IntPtr)5", "(IntPtr)5", true)]
    [InlineData("(IntPtr)5", "(IntPtr)6", false)]

    [InlineData("new decimal[]{ 1, 2, 3, 4 }.ToFrozenSet()", "new decimal[]{ 1, 2, 3, 4 }.ToFrozenSet()", true)]
    [InlineData("new decimal[]{ 1, 2, 3, 4 }.ToFrozenSet()", "new decimal[]{ 1, 2, 3, 5 }.ToFrozenSet()", false)]

    [InlineData("new Queue<int>([ 1, 2, 3, 4 ])", "new Queue<int>([ 1, 2, 3, 4 ])", true)]
    [InlineData("new Queue<int>([ 1, 2, 3, 4 ])", "new Queue<int>([ 1, 2, 3, 5 ])", false)]

    [InlineData("ImmutableArray.Create(1, 2, 3, 5 )", "ImmutableArray.Create(1, 2, 3, 5 )", true)]
    [InlineData("ImmutableArray.Create(1, 2, 3, 5 )", "ImmutableArray.Create(1, 2, 3, 4 )", false)]

    [InlineData("new ConcurrentStack<int>([1, 2, 3, 4])", "new ConcurrentStack<int>([1, 2, 3, 4])", true)]
    [InlineData("new ConcurrentStack<int>([1, 2, 3, 4])", "new ConcurrentStack<int>([1, 2, 3, 5])", false)]

    [InlineData("new Dictionary<int, string>{ [1] =\"one\", [2]=\"two\" }.ToFrozenDictionary()", "new Dictionary<int, string>{ [1] =\"one\", [2]=\"two\" }.ToFrozenDictionary()", true)]
    [InlineData("new Dictionary<int, string>{ [1] =\"one\", [2]=\"two\" }.ToFrozenDictionary()", "new Dictionary<int, string>{ [1] =\"one\", [3]=\"three\" }.ToFrozenDictionary()", false)]

    [InlineData("new Hashtable{ [1] =\"one\", [2]=\"two\" }", "new Hashtable{ [1] =\"one\", [2]=\"two\" }", true)]
    [InlineData("new Hashtable{ [1] =\"one\", [2]=\"two\" }", "new Hashtable{ [1] =\"one\", [3]=\"three\" }", false)]

    [InlineData("new ClassDataContract1()", "new ClassDataContract1()", true)]
    [InlineData("new ClassDataContract1()", "new ClassDataContract2()", false)]

    [InlineData("new ClassDataContract1[]", "new ClassDataContract1[]", true)]
    [InlineData("new ClassDataContract1[]", "new ClassDataContract2[]", false)]

    // assign:
    [InlineData("a = 1", "a = 1", true)]
    [InlineData("a = 1", "a = 2", false)]
    [InlineData("a = b", "a = b", true)]
    [InlineData("a = b", "a = 2", false)]
    [InlineData("a = b", "a += 1", false)]

    [InlineData("a += b", "a += b", true)]
    [InlineData("a += b", "a += 1", false)]
    [InlineData("a += b", "a -= b", false)]
    [InlineData("checked(a *= b)", "checked(a *= b)", true)]
    [InlineData("checked(a *= b)", "a *= b", false)]
    [InlineData("checked(a *= b)", "checked(a *= 2)", false)]

    // unary:
    [InlineData("a => increment(a)", "a => increment(a)", true)]
    [InlineData("a => increment(a)", "b => increment(b)", false)]
    [InlineData("a => increment(a)", "a => decrement(a)", false)]
    [InlineData("a => decrement(a)", "a => decrement(a)", true)]
    [InlineData("a => decrement(a)", "b => decrement(b)", false)]
    [InlineData("a => decrement(a)", "a => increment(a)", false)]
    [InlineData("a => ++a", "a => ++a", true)]
    [InlineData("a => ++a", "a => a++", false)]
    [InlineData("a => a++", "a => a++", true)]
    [InlineData("a => a++", "a => ++a", false)]
    [InlineData("a => --a", "a => --a", true)]
    [InlineData("a => a - 1", "a => --a", false)]
    [InlineData("a => a--", "a => a--", true)]

    // binary:
    [InlineData("(a, b) => a - b", "(a, b) => a - b", true)]
    [InlineData("(a, b) => a - b", "(a, b) => b - a", false)]
    [InlineData("(a, b) => a - b", "(a, b) => a + b", false)]
    [InlineData("(a, b) => checked(a * b)", "(a, b) => checked(a * b)", true)]
    [InlineData("(a, b) => checked(a * b)", "(a, b) => checked(b * a)", false)]
    [InlineData("(a, b) => checked(a * b)", "(b, a) => checked(a * b)", false)]

    // lambda:
    [InlineData("(s,d) => true", "(s,d) => true", true)]
    [InlineData("(s,d) => true", "(s`,d`) => true", false)]
    [InlineData("(s,d) => true", "(d,s) => true", false)]
    [InlineData("(s,d) => true", "(s,d) => false", false)]
    [InlineData("i => true", "i => true", true)]
    [InlineData("i => true", "i => false", false)]
    [InlineData("a => a._a", "a => a._a", true)]
    [InlineData("a => a._a", "a => a._b", false)]
    [InlineData("a => a.A", "a => a.A", true)]
    [InlineData("a => a.A", "a => a.B", false)]
    [InlineData("a => a.Method1()", "a => a.Method1()", true)]
    [InlineData("a => a.Method3(1,1)", "a => a.Method3(1,1)", true)]
    [InlineData("a => a.Method3(1,2)", "a => a.Method3(1,1)", false)]
    [InlineData("a => a.Method4(42,3.14)", "a => a.Method4(42,3.14)", true)]
    [InlineData("a => a.Method4(42,3.14)", "a => a.Method4(23,2.71)", false)]

    // statements:
    [InlineData("() => new StructDataContract1", "() => new StructDataContract1", true)]
    [InlineData("(a,b) => { ... }", "(a,b) => { ... }", true)]
    [InlineData("(f,a) => f(a)", "(f,a) => f(a)", true)]
    [InlineData("accessMemberMember", "accessMemberMember", true)]
    [InlineData("accessMemberMember1", "accessMemberMember1", true)]
    [InlineData("arrayAccessExpr", "arrayAccessExpr", true)]
    [InlineData("b => b ? 1 : 3", "b => b ? 1 : 3", true)]
    [InlineData("Console.WriteLine", "Console.WriteLine", true)]
    [InlineData("goto1", "goto1", true)]
    [InlineData("goto2", "goto2", true)]
    [InlineData("goto3", "goto3", true)]
    [InlineData("goto4", "goto4", true)]
    [InlineData("indexMember", "indexMember", true)]
    [InlineData("array[index]", "array[index]", true)]
    [InlineData("indexObject1", "indexObject1", true)]
    [InlineData("loop", "loop", true)]
    [InlineData("newArrayBounds", "newArrayBounds", true)]
    [InlineData("newArrayItems", "newArrayItems", true)]
    [InlineData("newDictionaryInit", "newDictionaryInit", true)]
    [InlineData("newListInit", "newListInit", true)]
    [InlineData("newMembersInit", "newMembersInit", true)]
    [InlineData("newMembersInit1", "newMembersInit1", true)]
    [InlineData("newMembersInit2", "newMembersInit2", true)]
    [InlineData("return1", "return1", true)]
    [InlineData("return2", "return2", true)]
    [InlineData("switch(a){ ... }", "switch(a){ ... }", true)]
    [InlineData("throw", "throw", true)]
    [InlineData("try1", "try1", true)]
    [InlineData("try2", "try2", true)]
    [InlineData("try3", "try3", true)]
    [InlineData("try4", "try4", true)]
    [InlineData("try5", "try5", true)]
    [InlineData("try6", "try6", true)]

    // inequality: block
    [InlineData("(a,b) => { ... }", "(a,b) => { diffVars }", false)]
    [InlineData("(a,b) => { ... }", "(a,b) => { diffExprs }", false)]

    // inequality: conditional
    [InlineData("condTrue3", "condTrue3", true)]
    [InlineData("condTrue3", "condTrue5", false)]

    // inequality: switch
    [InlineData("switch(a){ ... }", "switch(a){ diffDefault }", false)]
    [InlineData("switch(a){ ... }", "switch(a){ diffCases }", false)]

    // inequality: try/catch
    [InlineData("try2", "try2DiffHandlers", false)]
    [InlineData("try2", "try2DiffCatchType", false)]

    // inequality: loop labels
    [InlineData("loop", "loopDiffLabel", false)]

    // inequality: goto kind
    [InlineData("goto1", "goto1Return", false)]

    // inequality: index
    [InlineData("indexObject1", "indexObject1DiffValue", false)]

    // inequality: invocation argument count
    [InlineData("invoke1", "invoke1", true)]
    [InlineData("invoke1", "invoke2", false)]

    // inequality: newarray element count
    [InlineData("newArrayItems3", "newArrayItems3", true)]
    [InlineData("newArrayItems3", "newArrayItems4", false)]

    // inequality: list init
    [InlineData("newListInit2", "newListInit2", true)]
    [InlineData("newListInit2", "newListInit3", false)]

    // inequality: member init binding count
    [InlineData("memberInit1Binding", "memberInit1Binding", true)]
    [InlineData("memberInit1Binding", "memberInit2Bindings", false)]

    // inequality: method call different method
    [InlineData("callMethod1", "callMethod1", true)]
    [InlineData("callMethod1", "callMethod2", false)]

    // inequality: new with different args
    [InlineData("newCDC1_1a", "newCDC1_1a", true)]
    [InlineData("newCDC1_1a", "newCDC1_2b", false)]

    //[InlineData("", "", true)]
    public void ExpressionTest(string expr1, string expr2, bool areEqual)
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
