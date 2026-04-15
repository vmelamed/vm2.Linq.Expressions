namespace vm2.Linq.Expressions.DeepEquals.Tests;

#pragma warning disable IL2026 // RequiresUnreferencedCode — test code only
public class ExpressionExtensionsDeepEqualsTests
{
    [Fact]
    public void DeepEquals_ReferenceEqualsSameInstance_ReturnsTrueAndEmptyDifference()
    {
        var expr = Expression.Constant(42);

        var result = expr.DeepEquals(expr, out var difference);

        result.Should().BeTrue();
        difference.Should().BeEmpty();
    }

    [Fact]
    public void DeepEquals_DifferentTypes_ReturnsFalseAndDescribesTypes()
    {
        Expression left = Expression.Constant(1);
        Expression right = Expression.Constant("1");

        var result = left.DeepEquals(right, out var difference);

        result.Should().BeFalse();
        difference.Should().Contain("different types");
        difference.Should().Contain(typeof(int).FullName);
        difference.Should().Contain(typeof(string).FullName);
    }

    [Fact]
    public void DeepEquals_IdenticalStructureDifferentInstances_ReturnsTrue()
    {
        Expression<Func<int, int>> e1 = x => x + 1;
        Expression<Func<int, int>> e2 = x => x + 1;

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_StructuralDifference_ReportsNonEmptyDifference()
    {
        Expression<Func<int, int>> left = x => x + 1;
        Expression<Func<int, int>> right = x => x + 2;

        var result = left.DeepEquals(right, out var difference);

        result.Should().BeFalse();
        difference.Should().NotBeNullOrWhiteSpace();
        difference.Should().Contain("Difference");
    }

    [Fact]
    public void DeepEquals_ParameterNameMismatch_ReturnsFalseAndReportsNames()
    {
        // two lambdas with same shape but different parameter names
        Expression<Func<int, int>> e1 = a => a + 1;
        Expression<Func<int, int>> e2 = b => b + 1;

        var eq = e1.DeepEquals(e2, out var difference);

        eq.Should().BeFalse();
        difference.Should().NotBeNullOrWhiteSpace();
        // expect parameter names to appear in the diagnostic
        difference.Should().Contain("a");
        difference.Should().Contain("b");
    }

    [Fact]
    public void DeepEquals_LabelTargetNameMismatch_ReturnsFalse()
    {
        var ret1 = Expression.Label("ret1");
        var block1 = Expression.Block(
            Expression.Goto(ret1),
            Expression.Label(ret1)
        );

        var ret2 = Expression.Label("ret2");
        var block2 = Expression.Block(
            Expression.Goto(ret2),
            Expression.Label(ret2)
        );

        var eq = block1.DeepEquals(block2, out var difference);

        eq.Should().BeFalse();
        difference.Should().NotBeNullOrWhiteSpace();
        difference.Should().Contain("ret1");
        difference.Should().Contain("ret2");
    }

    [Fact]
    public void DeepEquals_Constants_ObjectDifferentInstances_TreatedAsEqual()
    {
        // constants typed as System.Object - implementation treats System.Object constants specially
        var c1 = Expression.Constant(new object(), typeof(object));
        var c2 = Expression.Constant(new object(), typeof(object));

        c1.DeepEquals(c2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_ConstantArrays_DifferentElements_ReturnsFalse()
    {
        var a1 = Expression.Constant(new int[] { 1, 2, 3 }, typeof(int[]));
        var a2 = Expression.Constant(new int[] { 1, 2, 4 }, typeof(int[]));

        var eq = a1.DeepEquals(a2, out var difference);

        eq.Should().BeFalse();
        difference.Should().NotBeNullOrWhiteSpace();
    }

    // ── Dispatch arm coverage ───────────────────────────────────

    [Fact]
    public void DeepEquals_DefaultExpression_SameType_Equal()
    {
        var e1 = Expression.Default(typeof(int));
        var e2 = Expression.Default(typeof(int));
        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_IndexExpression_SameStructure_Equal()
    {
        var indexer = typeof(TestMembersInitialized1).GetProperty("Item")!;

        var p1 = Expression.Parameter(typeof(TestMembersInitialized1), "m");
        var l1 = Expression.Lambda(Expression.MakeIndex(p1, indexer, [Expression.Constant(0)]), p1);

        var p2 = Expression.Parameter(typeof(TestMembersInitialized1), "m");
        var l2 = Expression.Lambda(Expression.MakeIndex(p2, indexer, [Expression.Constant(0)]), p2);

        l1.DeepEquals(l2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_IndexExpression_DifferentArgument_NotEqual()
    {
        var indexer = typeof(TestMembersInitialized1).GetProperty("Item")!;
        var p1 = Expression.Parameter(typeof(TestMembersInitialized1), "m");
        var p2 = Expression.Parameter(typeof(TestMembersInitialized1), "m");

        var e1 = Expression.Lambda(Expression.MakeIndex(p1, indexer, [Expression.Constant(0)]), p1);
        var e2 = Expression.Lambda(Expression.MakeIndex(p2, indexer, [Expression.Constant(1)]), p2);

        e1.DeepEquals(e2).Should().BeFalse();
    }

    [Fact]
    public void DeepEquals_LabelExpression_SameStructure_Equal()
    {
        var lbl1 = Expression.Label(typeof(int), "ret");
        var e1 = Expression.Label(lbl1, Expression.Constant(42));

        var lbl2 = Expression.Label(typeof(int), "ret");
        var e2 = Expression.Label(lbl2, Expression.Constant(42));

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_TypeBinaryExpression_SameStructure_Equal()
    {
        var p1 = Expression.Parameter(typeof(object), "o");
        var e1 = Expression.Lambda(Expression.TypeIs(p1, typeof(string)), p1);

        var p2 = Expression.Parameter(typeof(object), "o");
        var e2 = Expression.Lambda(Expression.TypeIs(p2, typeof(string)), p2);

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_TypeBinaryExpression_DifferentTypeOperand_NotEqual()
    {
        var p1 = Expression.Parameter(typeof(object), "o");
        var e1 = Expression.Lambda(Expression.TypeIs(p1, typeof(string)), p1);

        var p2 = Expression.Parameter(typeof(object), "o");
        var e2 = Expression.Lambda(Expression.TypeIs(p2, typeof(int)), p2);

        e1.DeepEquals(e2).Should().BeFalse();
    }

    // ── CompareConstant reflection paths ────────────────────────

    [Fact]
    public void DeepEquals_MemoryConstant_SameElements_Equal()
    {
        var e1 = Expression.Constant(new Memory<int>([1, 2, 3]), typeof(Memory<int>));
        var e2 = Expression.Constant(new Memory<int>([1, 2, 3]), typeof(Memory<int>));
        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_MemoryConstant_DifferentElements_NotEqual()
    {
        var e1 = Expression.Constant(new Memory<int>([1, 2, 3]), typeof(Memory<int>));
        var e2 = Expression.Constant(new Memory<int>([1, 2, 4]), typeof(Memory<int>));
        e1.DeepEquals(e2).Should().BeFalse();
    }

    [Fact]
    public void DeepEquals_ReadOnlyMemoryConstant_SameElements_Equal()
    {
        var e1 = Expression.Constant(new ReadOnlyMemory<int>([1, 2, 3]), typeof(ReadOnlyMemory<int>));
        var e2 = Expression.Constant(new ReadOnlyMemory<int>([1, 2, 3]), typeof(ReadOnlyMemory<int>));
        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_ArraySegmentDefault_BothNullArray_Equal()
    {
        var e1 = Expression.Constant(default(ArraySegment<int>), typeof(ArraySegment<int>));
        var e2 = Expression.Constant(default(ArraySegment<int>), typeof(ArraySegment<int>));
        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_ArraySegmentDefault_NullVsNonNullArray_NotEqual()
    {
        var e1 = Expression.Constant(default(ArraySegment<int>), typeof(ArraySegment<int>));
        var e2 = Expression.Constant(new ArraySegment<int>([1, 2]), typeof(ArraySegment<int>));
        e1.DeepEquals(e2).Should().BeFalse();
    }

    [Fact]
    public void DeepEquals_ArrayConstant_DifferentLengths_NotEqual()
    {
        var e1 = Expression.Constant(new int[] { 1, 2, 3 });
        var e2 = Expression.Constant(new int[] { 1, 2, 3, 4 });
        e1.DeepEquals(e2).Should().BeFalse();
    }

    // ── Try / Catch / Finally / Fault ───────────────────────────

    [Fact]
    public void DeepEquals_TryCatch_SameStructure_Equal()
    {
        var e1 = Expression.TryCatch(
            Expression.Constant(1),
            Expression.Catch(typeof(Exception), Expression.Constant(0)));
        var e2 = Expression.TryCatch(
            Expression.Constant(1),
            Expression.Catch(typeof(Exception), Expression.Constant(0)));

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_TryCatch_WithVariableAndFilter_Equal()
    {
        var ex1 = Expression.Parameter(typeof(ArgumentException), "ex");
        var e1 = Expression.TryCatch(
            Expression.Constant(1),
            Expression.MakeCatchBlock(
                typeof(ArgumentException),
                ex1,
                Expression.Constant(0),
                Expression.Equal(
                    Expression.MakeMemberAccess(ex1, typeof(ArgumentException).GetProperty("ParamName")!),
                    Expression.Constant("x"))));

        var ex2 = Expression.Parameter(typeof(ArgumentException), "ex");
        var e2 = Expression.TryCatch(
            Expression.Constant(1),
            Expression.MakeCatchBlock(
                typeof(ArgumentException),
                ex2,
                Expression.Constant(0),
                Expression.Equal(
                    Expression.MakeMemberAccess(ex2, typeof(ArgumentException).GetProperty("ParamName")!),
                    Expression.Constant("x"))));

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_TryCatch_DifferentCatchType_NotEqual()
    {
        var e1 = Expression.TryCatch(
            Expression.Constant(1),
            Expression.Catch(typeof(ArgumentException), Expression.Constant(0)));
        var e2 = Expression.TryCatch(
            Expression.Constant(1),
            Expression.Catch(typeof(InvalidOperationException), Expression.Constant(0)));

        e1.DeepEquals(e2).Should().BeFalse();
    }

    [Fact]
    public void DeepEquals_TryCatchFinally_SameStructure_Equal()
    {
        var e1 = Expression.TryCatchFinally(
            Expression.Constant(1),
            Expression.Empty(),
            Expression.Catch(typeof(Exception), Expression.Constant(0)));
        var e2 = Expression.TryCatchFinally(
            Expression.Constant(1),
            Expression.Empty(),
            Expression.Catch(typeof(Exception), Expression.Constant(0)));

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_TryFault_SameStructure_Equal()
    {
        var e1 = Expression.TryFault(Expression.Constant(1), Expression.Empty());
        var e2 = Expression.TryFault(Expression.Constant(1), Expression.Empty());

        e1.DeepEquals(e2).Should().BeTrue();
    }

    // ── Switch / SwitchCase ─────────────────────────────────────

    [Fact]
    public void DeepEquals_Switch_SameStructure_Equal()
    {
        var p1 = Expression.Parameter(typeof(int), "x");
        var e1 = Expression.Lambda(Expression.Switch(p1, Expression.Constant(-1),
            Expression.SwitchCase(Expression.Constant(10), Expression.Constant(1)),
            Expression.SwitchCase(Expression.Constant(20), Expression.Constant(2))), p1);

        var p2 = Expression.Parameter(typeof(int), "x");
        var e2 = Expression.Lambda(Expression.Switch(p2, Expression.Constant(-1),
            Expression.SwitchCase(Expression.Constant(10), Expression.Constant(1)),
            Expression.SwitchCase(Expression.Constant(20), Expression.Constant(2))), p2);

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_SwitchCase_DifferentTestValueCount_NotEqual()
    {
        var p1 = Expression.Parameter(typeof(int), "x");
        var e1 = Expression.Lambda(Expression.Switch(p1, Expression.Constant(-1),
            Expression.SwitchCase(Expression.Constant(10), Expression.Constant(1))), p1);

        var p2 = Expression.Parameter(typeof(int), "x");
        var e2 = Expression.Lambda(Expression.Switch(p2, Expression.Constant(-1),
            Expression.SwitchCase(Expression.Constant(10), Expression.Constant(1), Expression.Constant(2))), p2);

        e1.DeepEquals(e2).Should().BeFalse();
    }

    // ── Goto / Label ────────────────────────────────────────────

    [Fact]
    public void DeepEquals_GotoBlock_SameStructure_Equal()
    {
        var lbl1 = Expression.Label("target");
        var e1 = Expression.Block(
            Expression.Constant(1, typeof(object)),
            Expression.Goto(lbl1),
            Expression.Constant(2, typeof(object)),
            Expression.Label(lbl1));

        var lbl2 = Expression.Label("target");
        var e2 = Expression.Block(
            Expression.Constant(1, typeof(object)),
            Expression.Goto(lbl2),
            Expression.Constant(2, typeof(object)),
            Expression.Label(lbl2));

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_Goto_DifferentKind_NotEqual()
    {
        var lbl1 = Expression.Label("target");
        var e1 = Expression.Block(Expression.Goto(lbl1), Expression.Label(lbl1));

        var lbl2 = Expression.Label("target");
        var e2 = Expression.Block(Expression.Return(lbl2), Expression.Label(lbl2));

        e1.DeepEquals(e2).Should().BeFalse();
    }

    // ── Block sequence mismatch ─────────────────────────────────

    [Fact]
    public void DeepEquals_Block_SecondExpressionDiffers_NotEqual()
    {
        var e1 = Expression.Block(Expression.Constant(1), Expression.Constant(2));
        var e2 = Expression.Block(Expression.Constant(1), Expression.Constant(3));

        e1.DeepEquals(e2).Should().BeFalse();
    }

    // ── MemberBinding subtypes ──────────────────────────────────

    [Fact]
    public void DeepEquals_MemberListBinding_SameStructure_Equal()
    {
        var listProp = typeof(TestMembersInitialized1).GetProperty("ListProperty")!;
        var addMethod = typeof(List<Inner>).GetMethod("Add")!;
        var innerCtor = typeof(Inner).GetConstructor(Type.EmptyTypes)!;
        var intProp = typeof(Inner).GetProperty("IntProperty")!;
        var outerCtor = typeof(TestMembersInitialized1).GetConstructor(Type.EmptyTypes)!;

        var e1 = Expression.MemberInit(
            Expression.New(outerCtor),
            Expression.ListBind(listProp,
                Expression.ElementInit(addMethod, Expression.MemberInit(
                    Expression.New(innerCtor),
                    Expression.Bind(intProp, Expression.Constant(1))))));

        var e2 = Expression.MemberInit(
            Expression.New(outerCtor),
            Expression.ListBind(listProp,
                Expression.ElementInit(addMethod, Expression.MemberInit(
                    Expression.New(innerCtor),
                    Expression.Bind(intProp, Expression.Constant(1))))));

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_MemberMemberBinding_SameStructure_Equal()
    {
        var outerCtor = typeof(TestMembersInitialized).GetConstructor(Type.EmptyTypes)!;
        var innerProp = typeof(TestMembersInitialized).GetProperty("InnerProperty")!;
        var intProp = typeof(Inner).GetProperty("IntProperty")!;
        var strProp = typeof(Inner).GetProperty("StringProperty")!;

        var e1 = Expression.MemberInit(
            Expression.New(outerCtor),
            Expression.MemberBind(innerProp,
                Expression.Bind(intProp, Expression.Constant(23)),
                Expression.Bind(strProp, Expression.Constant("hello"))));

        var e2 = Expression.MemberInit(
            Expression.New(outerCtor),
            Expression.MemberBind(innerProp,
                Expression.Bind(intProp, Expression.Constant(23)),
                Expression.Bind(strProp, Expression.Constant("hello"))));

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_MemberBinding_DifferentMember_NotEqual()
    {
        var ctor = typeof(Inner).GetConstructor(Type.EmptyTypes)!;
        var intProp = typeof(Inner).GetProperty("IntProperty")!;
        var strProp = typeof(Inner).GetProperty("StringProperty")!;

        var e1 = Expression.MemberInit(Expression.New(ctor),
            Expression.Bind(intProp, Expression.Constant(1)));
        var e2 = Expression.MemberInit(Expression.New(ctor),
            Expression.Bind(strProp, Expression.Constant("a")));

        e1.DeepEquals(e2).Should().BeFalse();
    }

    [Fact]
    public void DeepEquals_MemberInit_SecondBindingDiffers_NotEqual()
    {
        var ctor = typeof(Inner).GetConstructor(Type.EmptyTypes)!;
        var intProp = typeof(Inner).GetProperty("IntProperty")!;
        var strProp = typeof(Inner).GetProperty("StringProperty")!;

        var e1 = Expression.MemberInit(Expression.New(ctor),
            Expression.Bind(intProp, Expression.Constant(1)),
            Expression.Bind(strProp, Expression.Constant("a")));
        var e2 = Expression.MemberInit(Expression.New(ctor),
            Expression.Bind(intProp, Expression.Constant(1)),
            Expression.Bind(strProp, Expression.Constant("b")));

        e1.DeepEquals(e2).Should().BeFalse();
    }

    // ── ListInit element mismatch ───────────────────────────────

    [Fact]
    public void DeepEquals_ListInit_SecondElementDiffers_NotEqual()
    {
        var addMethod = typeof(List<int>).GetMethod("Add")!;

        var e1 = Expression.ListInit(Expression.New(typeof(List<int>)),
            Expression.ElementInit(addMethod, Expression.Constant(1)),
            Expression.ElementInit(addMethod, Expression.Constant(2)));
        var e2 = Expression.ListInit(Expression.New(typeof(List<int>)),
            Expression.ElementInit(addMethod, Expression.Constant(1)),
            Expression.ElementInit(addMethod, Expression.Constant(3)));

        e1.DeepEquals(e2).Should().BeFalse();
    }

    // ── NewExpression with Members ──────────────────────────────

    [Fact]
    public void DeepEquals_NewExpression_WithMembers_SameStructure_Equal()
    {
        var ctor = typeof(ClassDataContract1).GetConstructor([typeof(int), typeof(string)])!;
        var intProp = typeof(ClassDataContract1).GetProperty("IntProperty")!;
        var strProp = typeof(ClassDataContract1).GetProperty("StringProperty")!;

        var e1 = Expression.New(ctor,
            [Expression.Constant(1), Expression.Constant("a")],
            [intProp, strProp]);
        var e2 = Expression.New(ctor,
            [Expression.Constant(1), Expression.Constant("a")],
            [intProp, strProp]);

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_NewExpression_MembersVsNoMembers_NotEqual()
    {
        var ctor = typeof(ClassDataContract1).GetConstructor([typeof(int), typeof(string)])!;
        var intProp = typeof(ClassDataContract1).GetProperty("IntProperty")!;
        var strProp = typeof(ClassDataContract1).GetProperty("StringProperty")!;

        var e1 = Expression.New(ctor,
            [Expression.Constant(1), Expression.Constant("a")],
            [intProp, strProp]);
        var e2 = Expression.New(ctor,
            Expression.Constant(1), Expression.Constant("a"));

        e1.DeepEquals(e2).Should().BeFalse();
    }

    // ── Lambda TailCall mismatch ────────────────────────────────

    [Fact]
    public void DeepEquals_Lambda_DifferentTailCall_NotEqual()
    {
        var p1 = Expression.Parameter(typeof(int), "x");
        var p2 = Expression.Parameter(typeof(int), "x");

        var e1 = Expression.Lambda(typeof(Func<int, int>), Expression.Constant(1), tailCall: false, [p1]);
        var e2 = Expression.Lambda(typeof(Func<int, int>), Expression.Constant(1), tailCall: true, [p2]);

        e1.DeepEquals(e2).Should().BeFalse();
    }

    // ── Invocation ──────────────────────────────────────────────

    [Fact]
    public void DeepEquals_Invocation_SameStructure_Equal()
    {
        var f1 = Expression.Parameter(typeof(Func<int, int>), "f");
        var x1 = Expression.Parameter(typeof(int), "x");
        var e1 = Expression.Lambda(Expression.Invoke(f1, x1), f1, x1);

        var f2 = Expression.Parameter(typeof(Func<int, int>), "f");
        var x2 = Expression.Parameter(typeof(int), "x");
        var e2 = Expression.Lambda(Expression.Invoke(f2, x2), f2, x2);

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_Invocation_DifferentArgCount_NotEqual()
    {
        var f1 = Expression.Parameter(typeof(Func<int, int>), "f");
        var f2 = Expression.Parameter(typeof(Func<int, int, int>), "f");
        var x = Expression.Parameter(typeof(int), "x");
        var y = Expression.Parameter(typeof(int), "y");

        var e1 = Expression.Invoke(f1, x);
        var e2 = Expression.Invoke(f2, x, y);

        e1.DeepEquals(e2).Should().BeFalse();
    }

    // ── MemberAccess ────────────────────────────────────────────

    [Fact]
    public void DeepEquals_MemberAccess_SameStructure_Equal()
    {
        var p1 = Expression.Parameter(typeof(string), "s");
        var e1 = Expression.Lambda(Expression.Property(p1, "Length"), p1);

        var p2 = Expression.Parameter(typeof(string), "s");
        var e2 = Expression.Lambda(Expression.Property(p2, "Length"), p2);

        e1.DeepEquals(e2).Should().BeTrue();
    }

    [Fact]
    public void DeepEquals_MemberAccess_DifferentMember_NotEqual()
    {
        var p1 = Expression.Parameter(typeof(TestMethods), "t");
        var e1 = Expression.Lambda(Expression.Field(p1, "_a"), p1);

        var p2 = Expression.Parameter(typeof(TestMethods), "t");
        var e2 = Expression.Lambda(Expression.Field(p2, "_b"), p2);

        e1.DeepEquals(e2).Should().BeFalse();
    }

    // ── Loop ────────────────────────────────────────────────────

    [Fact]
    public void DeepEquals_Loop_SameStructure_Equal()
    {
        var brk1 = Expression.Label("break");
        var cont1 = Expression.Label("continue");
        var e1 = Expression.Loop(Expression.Break(brk1), brk1, cont1);

        var brk2 = Expression.Label("break");
        var cont2 = Expression.Label("continue");
        var e2 = Expression.Loop(Expression.Break(brk2), brk2, cont2);

        e1.DeepEquals(e2).Should().BeTrue();
    }
}
