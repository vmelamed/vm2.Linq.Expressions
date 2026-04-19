namespace vm2.Linq.Expressions.DeepEquals.Tests;

public class HashCodeTests(ITestOutputHelper output) : TestBase(output)
{
    [Fact]
    public void GetDeepHashCode_Visit_Null_ReturnsNull()
    {
        var visitor = new HashCodeVisitor();
        var result = visitor.Visit((Expression?)null);
        result.Should().BeNull();
    }

    [Fact]
    public void GetDeepHashCode_IdenticalStructure_ProducesSameHash()
    {
        Expression<Func<int,int>> e1 = x => x + 1;
        Expression<Func<int,int>> e2 = x => x + 1;

        var h1 = e1.GetDeepHashCode();
        var h2 = e2.GetDeepHashCode();

        h1.Should().Be(h2);
    }

    [Fact]
    public void GetDeepHashCode_DifferentConstants_ProducesDifferentHash()
    {
        Expression<Func<int,int>> e1 = x => x + 1;
        Expression<Func<int,int>> e2 = x => x + 2;

        var h1 = e1.GetDeepHashCode();
        var h2 = e2.GetDeepHashCode();

        h1.Should().NotBe(h2);
    }

    // ── Node-type coverage ──────────────────────────────────────

    [Fact]
    public void GetDeepHashCode_Unary_SameStructure_SameHash()
    {
        Expression<Func<int, int>> e1 = x => -x;
        Expression<Func<int, int>> e2 = x => -x;

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_Binary_SameStructure_SameHash()
    {
        Expression<Func<int, int, int>> e1 = (a, b) => a * b;
        Expression<Func<int, int, int>> e2 = (a, b) => a * b;

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_TypeBinary_SameStructure_SameHash()
    {
        var p = Expression.Parameter(typeof(object), "o");
        var e1 = Expression.Lambda(Expression.TypeIs(p, typeof(string)), p);
        var e2 = Expression.Lambda(Expression.TypeIs(p, typeof(string)), p);

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_MethodCall_SameStructure_SameHash()
    {
        Expression<Func<string, string>> e1 = s => s.ToUpper();
        Expression<Func<string, string>> e2 = s => s.ToUpper();

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_MemberAccess_SameStructure_SameHash()
    {
        Expression<Func<string, int>> e1 = s => s.Length;
        Expression<Func<string, int>> e2 = s => s.Length;

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_New_SameStructure_SameHash()
    {
        Expression<Func<ClassDataContract1>> e1 = () => new ClassDataContract1(1, "a");
        Expression<Func<ClassDataContract1>> e2 = () => new ClassDataContract1(1, "a");

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_NewArray_SameStructure_SameHash()
    {
        Expression<Func<int[]>> e1 = () => new int[] { 1, 2, 3 };
        Expression<Func<int[]>> e2 = () => new int[] { 1, 2, 3 };

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_ListInit_SameStructure_SameHash()
    {
        Expression<Func<List<int>>> e1 = () => new List<int> { 1, 2, 3 };
        Expression<Func<List<int>>> e2 = () => new List<int> { 1, 2, 3 };

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_MemberInit_SameStructure_SameHash()
    {
        Expression<Func<Inner>> e1 = () => new Inner { IntProperty = 1, StringProperty = "a" };
        Expression<Func<Inner>> e2 = () => new Inner { IntProperty = 1, StringProperty = "a" };

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_Conditional_SameStructure_SameHash()
    {
        Expression<Func<bool, int>> e1 = b => b ? 1 : 0;
        Expression<Func<bool, int>> e2 = b => b ? 1 : 0;

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_Index_SameStructure_SameHash()
    {
        var p = Expression.Parameter(typeof(TestMembersInitialized1), "m");
        var e1 = Expression.Lambda(Expression.MakeIndex(p, typeof(TestMembersInitialized1).GetProperty("Item"), [Expression.Constant(0)]), p);
        var e2 = Expression.Lambda(Expression.MakeIndex(p, typeof(TestMembersInitialized1).GetProperty("Item"), [Expression.Constant(0)]), p);

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_Switch_SameStructure_SameHash()
    {
        var p = Expression.Parameter(typeof(int), "x");
        var e1 = Expression.Lambda(Expression.Switch(p, Expression.Constant(-1),
            Expression.SwitchCase(Expression.Constant(10), Expression.Constant(1)),
            Expression.SwitchCase(Expression.Constant(20), Expression.Constant(2))), p);
        var e2 = Expression.Lambda(Expression.Switch(p, Expression.Constant(-1),
            Expression.SwitchCase(Expression.Constant(10), Expression.Constant(1)),
            Expression.SwitchCase(Expression.Constant(20), Expression.Constant(2))), p);

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_Block_GotoLabel_SameStructure_SameHash()
    {
        var lbl = Expression.Label("end");
        var e1 = Expression.Block(Expression.Goto(lbl), Expression.Label(lbl));
        var lbl2 = Expression.Label("end");
        var e2 = Expression.Block(Expression.Goto(lbl2), Expression.Label(lbl2));

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_TryCatch_SameStructure_SameHash()
    {
        var e1 = Expression.TryCatch(
            Expression.Constant(1),
            Expression.Catch(typeof(Exception), Expression.Constant(0)));
        var e2 = Expression.TryCatch(
            Expression.Constant(1),
            Expression.Catch(typeof(Exception), Expression.Constant(0)));

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_Invocation_SameStructure_SameHash()
    {
        var f = Expression.Parameter(typeof(Func<int, int>), "f");
        var a = Expression.Parameter(typeof(int), "a");
        var e1 = Expression.Lambda(Expression.Invoke(f, a), f, a);
        var e2 = Expression.Lambda(Expression.Invoke(f, a), f, a);

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }

    [Fact]
    public void GetDeepHashCode_Loop_SameStructure_SameHash()
    {
        var brk = Expression.Label("break");
        var e1 = Expression.Loop(Expression.Break(brk), brk);
        var brk2 = Expression.Label("break");
        var e2 = Expression.Loop(Expression.Break(brk2), brk2);

        e1.GetDeepHashCode().Should().Be(e2.GetDeepHashCode());
    }
}
