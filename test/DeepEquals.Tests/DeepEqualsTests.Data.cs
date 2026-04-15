namespace vm2.Linq.Expressions.DeepEquals.Tests;

#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0056 // Use index operator

public partial class DeepEqualsTests
{
    static readonly ParameterExpression _paramA = Expression.Parameter(typeof(int), "a");
    static readonly ParameterExpression _paramB = Expression.Parameter(typeof(int), "b");
    static readonly ConstantExpression _const1 = Expression.Constant(1, typeof(int));
    static readonly ConstantExpression _const2 = Expression.Constant(2, typeof(int));
    static readonly ConstantExpression _nil =  Expression.Constant(null, typeof(object));
    static readonly ConstantExpression _obj =  Expression.Constant(new object(), typeof(object));
    static readonly ConstantExpression _obj1 = Expression.Constant(new object(), typeof(object));

    static readonly ParameterExpression _paramC = Expression.Parameter(typeof(int), "c");
    static readonly ParameterExpression _paramD = Expression.Parameter(typeof(int), "d");

    static readonly Expression _block =
        Expression.Lambda(
            Expression.Block(
                [_paramD, ],
                Expression.Assign(_paramD, Expression.Constant(42)),
                Expression.AddAssign(_paramA, _paramD),
                Expression.SubtractAssign(_paramA, _paramB),
                _paramC,
                Expression.Assign(_paramC, Expression.Constant(2)),
                Expression.LeftShiftAssign(_paramA, _paramC)),
            _paramA, _paramB);

    static readonly ParameterExpression _value  = Expression.Parameter(typeof(int), "_value");
    static readonly ParameterExpression _result = Expression.Parameter(typeof(int), "_result");
    static readonly LabelTarget _labelContinue  = Expression.Label("continue");
    static readonly LabelTarget _labelBreak = Expression.Label("break");

    static readonly Expression _lambdaWithLoopContinueBreak =
        Expression.Block(
            new[] { _result },
            Expression.Assign(_value, Expression.Constant(5)),
            Expression.Assign(_result, Expression.Constant(1)),
            Expression.Loop(
                Expression.Block(
                    [],
                    Expression.IfThenElse(
                        Expression.GreaterThan(_value, Expression.Constant(1)),
                        Expression.Block(
                            [],
                            Expression.MultiplyAssign(_result, Expression.PostDecrementAssign(_value)),
                            Expression.Continue(_labelContinue)),
                        Expression.Break(_labelBreak))),
                _labelBreak,
                _labelContinue))
    ;

    static readonly MethodInfo _miWriteLine = typeof(Console).GetMethod("WriteLine", [ typeof(string) ])!;
    static Expression WriteLine1Expression(string s) => Expression.Call(null, _miWriteLine, Expression.Constant(s));
    static Expression ExceptionDefaultCtor() => Expression.New(typeof(Exception).GetConstructor(Type.EmptyTypes)!);
    static Expression ThrowException() => Expression.Throw(ExceptionDefaultCtor());

    static readonly Expression _switch =
        Expression.Switch(
            _paramA,
            WriteLine1Expression("Default"),
            Expression.SwitchCase(
                WriteLine1Expression("FirstChild"),
                Expression.Constant(1)),
            Expression.SwitchCase(
                WriteLine1Expression("Second"),
                Expression.Constant(2),
                Expression.Constant(3),
                Expression.Constant(4))
            );

    static readonly Expression _throw =
        Expression.Block(
            WriteLine1Expression("Before throwing"),
            Expression.Throw(ExceptionDefaultCtor())
        );

    static readonly Expression _try1 =
        Expression.TryFault(
            Expression.Block(
                new Expression[]
                {
                    WriteLine1Expression("TryBody"),
                    ThrowException(),
                }),
            WriteLine1Expression("caught {}"));

    static readonly Expression _try2 =
        Expression.TryCatch(
            Expression.Block(
                WriteLine1Expression("TryBody"),
                ThrowException()
            ),
            [
                Expression.MakeCatchBlock(
                    typeof(ArgumentException),
                    null,
                    WriteLine1Expression("caught (ArgumentException) {}"),
                    null),
            ]);

    static readonly Expression _try3 =
        Expression.TryCatchFinally(
            Expression.Block(
                WriteLine1Expression("TryBody"),
                ThrowException()
            ),
            WriteLine1Expression("finally {}"),
            [
                Expression.MakeCatchBlock(
                    typeof(ArgumentException),
                    null,
                    WriteLine1Expression("caught (ArgumentException) {}"),
                    null),
            ]);

    static readonly Expression _try4 =
        Expression.TryFinally(
            Expression.Block(
                WriteLine1Expression("TryBody"),
                ThrowException()
            ),
            WriteLine1Expression("finally {}"));

    static readonly ParameterExpression _exception = Expression.Parameter(typeof(ArgumentException), "x");
    static readonly Expression _try5 =
        Expression.TryCatch(
            Expression.Block(
                WriteLine1Expression("TryBody"),
                ThrowException()
            ),
            [
                Expression.MakeCatchBlock(
                    typeof(ArgumentException),
                    _exception,
                    Expression.Call(
                                null,
                                _miWriteLine,
                                Expression.MakeMemberAccess(_exception, typeof(ArgumentException).GetProperty("Message")!)),
                    null),
            ]);

    static readonly Expression _try6 =
        Expression.TryCatch(
            Expression.Block(
                WriteLine1Expression("TryBody"),
                ThrowException()),
            [
                Expression.MakeCatchBlock(
                    typeof(ArgumentException),
                    _exception,
                    Expression.Call(
                                null,
                                _miWriteLine,
                                Expression.MakeMemberAccess(_exception, typeof(ArgumentException).GetProperty("ParamName")!)),
                    Expression.Equal(
                        Expression.MakeMemberAccess(_exception, typeof(ArgumentException).GetProperty("ParamName")!),
                        Expression.Constant("x"))),
            ]);

    static readonly LabelTarget _returnTarget = Expression.Label();
    static readonly ParameterExpression _a = Expression.Parameter(typeof(int), "a");

    static readonly Expression _goto1 =
        Expression.Block(
            WriteLine1Expression("GoTo"),
            Expression.Goto(_returnTarget),
            WriteLine1Expression("Unreachable"),
            Expression.Label(_returnTarget)
        );
    static readonly Expression _goto2 =
        Expression.Block(
            WriteLine1Expression("GoTo"),
            //Expression.Goto(_returnTarget),
            WriteLine1Expression("Reachable"),
            Expression.Label(_returnTarget)
        );

    static readonly Expression _goto3 =
        Expression.Block(
            [ _a ],
            Expression.Assign(_a, Expression.Constant(0)),
            Expression.Increment(_a),
            Expression.Goto(_returnTarget),
            Expression.Increment(_a),
            Expression.Label(_returnTarget)
        );

    static readonly Expression _goto4 =
        Expression.Block(
            [ _a ],
            Expression.Assign(_a, Expression.Constant(0)),
            Expression.Increment(_a),
            Expression.Goto(_returnTarget, type: typeof(void)),
            Expression.Increment(_a),
            Expression.Label(_returnTarget)
        );

    static readonly Expression _return1 =
        Expression.Block(
            [ _a ],
            Expression.Assign(_a, Expression.Constant(0)),
            Expression.Increment(_a),
            Expression.Return(_returnTarget),
            Expression.Increment(_a),
            Expression.Label(_returnTarget)
        );

    static readonly Expression _return2 =
        Expression.Block(
            [ _a ],
            Expression.Assign(_a, Expression.Constant(0)),
            Expression.Increment(_a),
            Expression.Return(_returnTarget, _a),
            Expression.Increment(_a),
            Expression.Label(_returnTarget)
        );

    static readonly ParameterExpression _m = Expression.Parameter(typeof(TestMembersInitialized), "m");
    static readonly LabelTarget _return3 = Expression.Label(typeof(int));
    static readonly Expression _accessMemberMember1 =
        Expression.Block(
            [_m],
            Expression.Assign(
                _m,
                Expression.New(typeof(TestMembersInitialized).GetConstructor(Type.EmptyTypes)!)),
            Expression.Return(
                _return3,
                Expression.Add(
                    Expression.MakeMemberAccess(
                        _m,
                        typeof(TestMembersInitialized).GetProperty(nameof(TestMembersInitialized.TheOuterIntProperty))!
                    ),
                    Expression.MakeMemberAccess(
                        Expression.MakeMemberAccess(_m, typeof(TestMembersInitialized).GetProperty(nameof(TestMembersInitialized.InnerProperty))!),
                        typeof(Inner).GetProperty(nameof(Inner.IntProperty))!)
                ),
                typeof(int)
            ),
            Expression.Label(_return3, Expression.Constant(0))
        );

    static readonly ParameterExpression _arrayExpr = Expression.Parameter(typeof(int[]), "Array");
    static readonly ParameterExpression _indexExpr = Expression.Parameter(typeof(int), "Index");
    static readonly ParameterExpression _valueExpr = Expression.Parameter(typeof(int), "Value");
    static readonly Expression _arrayAccessExpr = Expression.ArrayAccess(_arrayExpr, _indexExpr);

    // Array[Index] = (Array[Index] + Value)
    static readonly Expression _lambdaExpr =
            Expression.Assign(
                _arrayAccessExpr,
                Expression.Add(
                    _arrayAccessExpr,
                    _valueExpr));

    // ── Variants for inequality testing ────────────────────────

    // Block: different variable count
    static readonly ParameterExpression _paramE = Expression.Parameter(typeof(int), "e");
    static readonly Expression _blockDiffVars =
        Expression.Lambda(
            Expression.Block(
                [_paramD, _paramE],
                Expression.Assign(_paramD, Expression.Constant(42)),
                Expression.AddAssign(_paramA, _paramD),
                Expression.SubtractAssign(_paramA, _paramB),
                _paramC,
                Expression.Assign(_paramC, Expression.Constant(2)),
                Expression.LeftShiftAssign(_paramA, _paramC)),
            _paramA, _paramB);

    // Block: different expression count
    static readonly Expression _blockDiffExprs =
        Expression.Lambda(
            Expression.Block(
                [_paramD, ],
                Expression.Assign(_paramD, Expression.Constant(42)),
                Expression.AddAssign(_paramA, _paramD),
                _paramC,
                Expression.Assign(_paramC, Expression.Constant(2)),
                Expression.LeftShiftAssign(_paramA, _paramC)),
            _paramA, _paramB);

    // Switch: different comparison
    static readonly Expression _switchDiffDefault =
        Expression.Switch(
            _paramA,
            WriteLine1Expression("OtherDefault"),
            Expression.SwitchCase(
                WriteLine1Expression("FirstChild"),
                Expression.Constant(1)),
            Expression.SwitchCase(
                WriteLine1Expression("Second"),
                Expression.Constant(2),
                Expression.Constant(3),
                Expression.Constant(4))
            );

    // Switch: different number of cases
    static readonly Expression _switchDiffCases =
        Expression.Switch(
            _paramA,
            WriteLine1Expression("Default"),
            Expression.SwitchCase(
                WriteLine1Expression("FirstChild"),
                Expression.Constant(1))
            );

    // Try: different handler count
    static readonly Expression _try2DiffHandlers =
        Expression.TryCatch(
            Expression.Block(
                WriteLine1Expression("TryBody"),
                ThrowException()
            ),
            [
                Expression.MakeCatchBlock(
                    typeof(ArgumentException),
                    null,
                    WriteLine1Expression("caught (ArgumentException) {}"),
                    null),
                Expression.MakeCatchBlock(
                    typeof(InvalidOperationException),
                    null,
                    WriteLine1Expression("caught (InvalidOperationException) {}"),
                    null),
            ]);

    // Try: different catch type
    static readonly Expression _try2DiffCatchType =
        Expression.TryCatch(
            Expression.Block(
                WriteLine1Expression("TryBody"),
                ThrowException()
            ),
            [
                Expression.MakeCatchBlock(
                    typeof(InvalidOperationException),
                    null,
                    WriteLine1Expression("caught (InvalidOperationException) {}"),
                    null),
            ]);

    // Loop with different break label name
    static readonly LabelTarget _labelBreak2 = Expression.Label("break2");
    static readonly Expression _lambdaWithLoopDiffLabel =
        Expression.Block(
            new[] { _result },
            Expression.Assign(_value, Expression.Constant(5)),
            Expression.Assign(_result, Expression.Constant(1)),
            Expression.Loop(
                Expression.Block(
                    [],
                    Expression.IfThenElse(
                        Expression.GreaterThan(_value, Expression.Constant(1)),
                        Expression.Block(
                            [],
                            Expression.MultiplyAssign(_result, Expression.PostDecrementAssign(_value)),
                            Expression.Continue(_labelContinue)),
                        Expression.Break(_labelBreak2))),
                _labelBreak2,
                _labelContinue))
    ;

    // Goto with different kind (Return vs Goto)
    static readonly Expression _goto1Return =
        Expression.Block(
            WriteLine1Expression("GoTo"),
            Expression.Return(_returnTarget),
            WriteLine1Expression("Unreachable"),
            Expression.Label(_returnTarget)
        );

    // Index: different indexer
    static readonly ParameterExpression _m1Param = Expression.Parameter(typeof(TestMembersInitialized1), "m1");
    static readonly Expression _indexObject1DiffValue =
        Expression.Lambda(
            Expression.MakeIndex(
                _m1Param,
                typeof(TestMembersInitialized1).GetProperty("Item"),
                [Expression.Constant(2)]),
            _m1Param);

    // Invocation: different argument count
    static readonly ParameterExpression _func2 = Expression.Parameter(typeof(Func<int, int, int>), "f2");
    static readonly Expression _invoke2 = Expression.Lambda(
        Expression.Invoke(_func2, _paramA, _paramB),
        _func2, _paramA, _paramB);
    static readonly ParameterExpression _func1 = Expression.Parameter(typeof(Func<int, int>), "f1");
    static readonly Expression _invoke1 = Expression.Lambda(
        Expression.Invoke(_func1, _paramA),
        _func1, _paramA);

    // NewArray: different element count
    static readonly Expression _newArrayItems4 = Expression.Lambda(Expression.NewArrayInit(typeof(string),
        Expression.Constant("aaa"), Expression.Constant("bbb"), Expression.Constant("ccc"), Expression.Constant("ddd")));
    static readonly Expression _newArrayItems3 = Expression.Lambda(Expression.NewArrayInit(typeof(string),
        Expression.Constant("aaa"), Expression.Constant("bbb"), Expression.Constant("ccc")));

    // ListInit: different initializer count
    static readonly Expression _newListInit2 = Expression.Lambda(Expression.ListInit(
        Expression.New(typeof(List<string>)),
        Expression.ElementInit(typeof(List<string>).GetMethod("Add")!, Expression.Constant("aaa")),
        Expression.ElementInit(typeof(List<string>).GetMethod("Add")!, Expression.Constant("bbb"))));
    static readonly Expression _newListInit3 = Expression.Lambda(Expression.ListInit(
        Expression.New(typeof(List<string>)),
        Expression.ElementInit(typeof(List<string>).GetMethod("Add")!, Expression.Constant("aaa")),
        Expression.ElementInit(typeof(List<string>).GetMethod("Add")!, Expression.Constant("bbb")),
        Expression.ElementInit(typeof(List<string>).GetMethod("Add")!, Expression.Constant("ccc"))));

    // MemberInit: different binding count
    static readonly ConstructorInfo _innerCtor = typeof(Inner).GetConstructor(Type.EmptyTypes)!;
    static readonly Expression _memberInit1Binding = Expression.Lambda(Expression.MemberInit(
        Expression.New(_innerCtor),
        Expression.Bind(typeof(Inner).GetProperty("IntProperty")!, Expression.Constant(1))));
    static readonly Expression _memberInit2Bindings = Expression.Lambda(Expression.MemberInit(
        Expression.New(_innerCtor),
        Expression.Bind(typeof(Inner).GetProperty("IntProperty")!, Expression.Constant(1)),
        Expression.Bind(typeof(Inner).GetProperty("StringProperty")!, Expression.Constant("hello"))));

    // MethodCall: different method
    static readonly MethodInfo _miMethod1 = typeof(TestMethods).GetMethod("Method1")!;
    static readonly MethodInfo _miMethod2 = typeof(TestMethods).GetMethod("Method2")!;
    static readonly Expression _callMethod1 = Expression.Lambda(Expression.Call(null, _miMethod1));
    static readonly Expression _callMethod2 = Expression.Lambda(Expression.Call(null, _miMethod2, Expression.Constant(1), Expression.Constant("s")));

    // New: different constructor args
    static readonly Expression _newStruct1 = Expression.Lambda(Expression.New(
        typeof(ClassDataContract1).GetConstructor([typeof(int), typeof(string)])!,
        Expression.Constant(1), Expression.Constant("a")));
    static readonly Expression _newStruct2 = Expression.Lambda(Expression.New(
        typeof(ClassDataContract1).GetConstructor([typeof(int), typeof(string)])!,
        Expression.Constant(2), Expression.Constant("b")));

    // Conditional: different IfFalse
    static readonly Expression _condTrue3 = Expression.Lambda<Func<bool, int>>(
        Expression.Condition(Expression.Parameter(typeof(bool), "b"), Expression.Constant(1), Expression.Constant(3)),
        Expression.Parameter(typeof(bool), "b"));
    static readonly Expression _condTrue5 = Expression.Lambda<Func<bool, int>>(
        Expression.Condition(Expression.Parameter(typeof(bool), "b"), Expression.Constant(1), Expression.Constant(5)),
        Expression.Parameter(typeof(bool), "b"));

    static readonly Func<Expression> _newMembersInitialized = () => () => new TestMembersInitialized
    {
        TheOuterIntProperty = 42,
        Time = new DateTime(1776, 7, 4),
        InnerProperty = new Inner
            {
            IntProperty = 23,
            StringProperty = "inner string"
        },
        EnumerableProperty = new List<string>
            {
                "aaa",
                "bbb",
                "ccc",
            },
    };

    static readonly Func<Expression> _newMembersInitialized1 = () => () => new TestMembersInitialized1()
    {
        TheOuterIntProperty = 42,
        Time = new DateTime(1776, 7, 4),
        InnerProperty = new Inner
        {
            IntProperty = 23,
            StringProperty = "inner string"
        },
        ArrayProperty = new[] { 4, 5, 6 },
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
                    StringProperty = "next inner string"
                }
            },
    };

    static readonly Func<Expression> _newMembersInitialized2 = () => () => new TestMembersInitialized1()
    {
        TheOuterIntProperty = 42,
        Time = new DateTime(1776, 7, 4),
        InnerProperty =
            {
            IntProperty = 23,
                StringProperty = "inner string"
            },
        ArrayProperty = new int[] { 4, 5, 6 },
        ListProperty =
            {
            new Inner() {
                IntProperty = 23,
                StringProperty = "inner string"
            },
                new Inner() {
                    IntProperty = 42,
                    StringProperty = "next inner string"
                }
            },
    };

    static readonly Dictionary<object, Func<Expression>> _substituteExpressions = new()
    {
        ["null"]                                                                            = () => _nil,
        ["object"]                                                                          = () => _obj,
        ["object1"]                                                                         = () => _obj1,
        ["true"]                                                                            = () => Expression.Constant(true),
        ["false"]                                                                           = () => Expression.Constant(false),
        ["(bool?)true"]                                                                     = () => Expression.Constant(true, typeof(bool?)),
        ["(bool?)false"]                                                                    = () => Expression.Constant(false, typeof(bool?)),
        ["(bool?)null"]                                                                     = () => Expression.Constant(null, typeof(bool?)),
        ["'V'"]                                                                             = () => Expression.Constant('V'),
        ["'M'"]                                                                             = () => Expression.Constant('M'),
        ["(char?)null"]                                                                     = () => Expression.Constant(null, typeof(char?)),
        ["(char?)'V'"]                                                                      = () => Expression.Constant('V', typeof(char?)),
        ["(char?)'M'"]                                                                      = () => Expression.Constant('M', typeof(char?)),
        ["(byte)5"]                                                                         = () => Expression.Constant((byte)5),
        ["(byte)6"]                                                                         = () => Expression.Constant((byte)6),
        ["(byte?)5"]                                                                        = () => Expression.Constant((byte?)5, typeof(byte?)),
        ["(byte?)6"]                                                                        = () => Expression.Constant((byte?)6, typeof(byte?)),
        ["(byte?)null"]                                                                     = () => Expression.Constant(null, typeof(byte?)),
        ["(sbyte)5"]                                                                        = () => Expression.Constant((sbyte)5),
        ["(short)5"]                                                                        = () => Expression.Constant((short)5),
        ["(ushort)5"]                                                                       = () => Expression.Constant((ushort)5),
        ["1"]                                                                               = () => Expression.Constant(1),
        ["2"]                                                                               = () => Expression.Constant(2),
        ["1.0"]                                                                             = () => Expression.Constant(1.0),
        ["5"]                                                                               = () => Expression.Constant(5),
        ["5.0"]                                                                             = () => Expression.Constant(5.0),
        ["(int?)null"]                                                                      = () => Expression.Constant(null, typeof(int?)),
        ["(int?)1"]                                                                         = () => Expression.Constant((int?)1),
        ["(int?)2"]                                                                         = () => Expression.Constant((int?)2),
        ["(int?)5"]                                                                         = () => Expression.Constant((int?)5),
        ["(int?)5.0"]                                                                       = () => Expression.Constant((int?)5.0),
        ["(uint)5"]                                                                         = () => Expression.Constant((uint)5),
        ["5L"]                                                                              = () => Expression.Constant(5L),
        ["(ulong)5"]                                                                        = () => Expression.Constant((ulong)5),
        ["5.5123453E-34F"]                                                                  = () => Expression.Constant(5.5123453E-34F),
        ["5.1234567891234567E-123"]                                                         = () => Expression.Constant(5.1234567891234567E-123),
        ["5.5M"]                                                                            = () => Expression.Constant(5.5M),
        ["Enum.One"]                                                                        = () => Expression.Constant(EnumTest.One),
        ["Enum.Two"]                                                                        = () => Expression.Constant(EnumTest.Two),
        ["Enum.Three"]                                                                      = () => Expression.Constant(EnumTest.Three),
        ["EnumFlagsTest.One | EnumFlagsTest.Three"]                                         = () => Expression.Constant(EnumFlagsTest.One | EnumFlagsTest.Three),
        ["Enum?.null"]                                                                      = () => Expression.Constant(null, typeof(EnumTest?)),
        ["Enum?.One"]                                                                       = () => Expression.Constant(EnumTest.One, typeof(EnumTest?)),
        ["Enum?.Two"]                                                                       = () => Expression.Constant(EnumTest.Two, typeof(EnumTest?)),
        ["Enum?.Three"]                                                                     = () => Expression.Constant(EnumTest.Three, typeof(EnumTest?)),
        ["one"]                                                                             = () => Expression.Constant("one"),
        ["two"]                                                                             = () => Expression.Constant("two"),
        ["(string)null"]                                                                    = () => Expression.Constant(null, typeof(string)),
        ["new DateTime(2024, 4, 13, 23, 18, 26, 234, DateTimeKind.Local)"]                  = () => Expression.Constant(new DateTime(2024, 4, 13, 23, 18, 26, 234, DateTimeKind.Local)),
        ["new DateTime(2024, 4, 13, 23, 18, 26, 345, DateTimeKind.Local)"]                  = () => Expression.Constant(new DateTime(2024, 4, 13, 23, 18, 26, 345, DateTimeKind.Local)),
        ["new TimeSpan(3, 4, 15, 32, 123)"]                                                 = () => Expression.Constant(new TimeSpan(3, 4, 15, 32, 123)),
        ["new TimeSpan(3, 4, 15, 32, 234)"]                                                 = () => Expression.Constant(new TimeSpan(3, 4, 15, 32, 234)),
        ["new DateTimeOffset(2024, 4, 13, 23, 18, 26, 234, new TimeSpan(0, -300, 0))"]      = () => Expression.Constant(new DateTimeOffset(2024, 4, 13, 23, 18, 26, 234, new TimeSpan(0, -300, 0))),
        ["new DateTimeOffset(2024, 4, 13, 23, 18, 26, 345, new TimeSpan(0, -300, 0))"]      = () => Expression.Constant(new DateTimeOffset(2024, 4, 13, 23, 18, 26, 345, new TimeSpan(0, -300, 0))),
        ["new int[]{ 1, 2, 3, 4 }"]                                                         = () => Expression.Constant(new int[]{ 1, 2, 3, 4 }),
        ["new int[]{ 1, 2, 3, 5 }"]                                                         = () => Expression.Constant(new int[]{ 1, 2, 3, 5 }),
        ["new int?[]{ 1, 2, null, null }"]                                                  = () => Expression.Constant(new int?[]{ 1, 2, null, null }),
        ["new byte[]{ 1, 2, 3, 1, 2, 3, 1, 2, 3, 10 }"]                                     = () => Expression.Constant(new byte[]{ 1, 2, 3, 1, 2, 3, 1, 2, 3, 10 }),
        ["ArraySegment<byte>1"]                                                             = () => Expression.Constant(new ArraySegment<byte>([1, 2, 3, 1, 2, 3, 1, 2, 3, 10], 1, 8)),
        ["ArraySegment<byte>2"]                                                             = () => Expression.Constant(new ArraySegment<byte>([1, 2, 3, 4, 2, 3, 1, 2, 3, 10], 1, 8)),
        ["(Half)3.14"]                                                                      = () => Expression.Constant((Half)3.14),
        ["(IntPtr)5"]                                                                       = () => Expression.Constant((IntPtr)5),
        ["(IntPtr)6"]                                                                       = () => Expression.Constant((IntPtr)6),
        ["(UIntPtr)5"]                                                                      = () => Expression.Constant((UIntPtr)5),
        ["new Guid(\"00112233-4455-6677-8899-AABBCCDDEEFF\")"]                              = () => Expression.Constant(new Guid("00112233-4455-6677-8899-AABBCCDDEEFF")),
        ["new Uri(\"http://www.some.com\")"]                                                = () => Expression.Constant(new Uri("http://www.acme.com")),
        ["DBNull.Value"]                                                                    = () => Expression.Constant(DBNull.Value, typeof(DBNull)),

        ["new ArraySegment<int>([ 1, 2, 3, 4 ], 1, 2)"]                                     = () => Expression.Constant(new ArraySegment<int>([ 1, 2, 3, 4 ], 1, 2)),
        ["new decimal[]{ 1, 2, 3, 4 }.ToFrozenSet()"]                                       = () => Expression.Constant(new decimal[]{ 1, 2, 3, 4 }.ToFrozenSet()),
        ["new decimal[]{ 1, 2, 3, 5 }.ToFrozenSet()"]                                       = () => Expression.Constant(new decimal[]{ 1, 2, 3, 5 }.ToFrozenSet()),
        ["new Queue<int>([ 1, 2, 3, 4 ])"]                                                  = () => Expression.Constant(new Queue<int>([ 1, 2, 3, 4 ])),
        ["new Queue<int>([ 1, 2, 3, 5 ])"]                                                  = () => Expression.Constant(new Queue<int>([ 1, 2, 3, 5 ])),
        ["new Stack<int>([ 1, 2, 3, 4 ])"]                                                  = () => Expression.Constant(new Stack<int>([ 1, 2, 3, 4 ])),
        ["ImmutableArray.Create(1, 2, 3, 4 )"]                                              = () => Expression.Constant(ImmutableArray.Create(1, 2, 3, 4 )),
        ["ImmutableArray.Create(1, 2, 3, 5 )"]                                              = () => Expression.Constant(ImmutableArray.Create(1, 2, 3, 5 )),
        ["ImmutableHashSet.Create(1, 2, 3, 4 )"]                                            = () => Expression.Constant(ImmutableHashSet.Create(1, 2, 3, 4 )),
        ["ImmutableList.Create(1, 2, 3, 4 )"]                                               = () => Expression.Constant(ImmutableList.Create(1, 2, 3, 4 )),
        ["ImmutableQueue.Create(1, 2, 3, 4 )"]                                              = () => Expression.Constant(ImmutableQueue.Create(1, 2, 3, 4 )),
        ["ImmutableSortedSet.Create(1, 2, 3, 4 )"]                                          = () => Expression.Constant(ImmutableSortedSet.Create(1, 2, 3, 4 )),
        ["ImmutableStack.Create(1, 2, 3, 4 )"]                                              = () => Expression.Constant(ImmutableStack.Create(1, 2, 3, 4 )),
        ["new ConcurrentBag<int>([1, 2, 3, 4])"]                                            = () => Expression.Constant(new ConcurrentBag<int>([1, 2, 3, 4])),
        ["new ConcurrentQueue<int>([1, 2, 3, 4])"]                                          = () => Expression.Constant(new ConcurrentQueue<int>([1, 2, 3, 4])),
        ["new ConcurrentStack<int>([1, 2, 3, 4])"]                                          = () => Expression.Constant(new ConcurrentStack<int>([1, 2, 3, 4])),
        ["new ConcurrentStack<int>([1, 2, 3, 5])"]                                          = () => Expression.Constant(new ConcurrentStack<int>([1, 2, 3, 5])),
        ["new Collection<int>([1, 2, 3, 4])"]                                               = () => Expression.Constant(new Collection<int>([1, 2, 3, 4])),
        ["new ReadOnlyCollection<int>([1, 2, 3, 4])"]                                       = () => Expression.Constant(new ReadOnlyCollection<int>([1, 2, 3, 4])),
        ["new HashSet<int>([1, 2, 3, 4])"]                                                  = () => Expression.Constant(new HashSet<int>([1, 2, 3, 4])),
        ["new LinkedList<int>([1, 2, 3, 4])"]                                               = () => Expression.Constant(new LinkedList<int>([1, 2, 3, 4])),
        ["new List<int>([1, 2, 3, 4])"]                                                     = () => Expression.Constant(new List<int>([1, 2, 3, 4])),
        ["new List<int?>{ 1, 2, null, null }"]                                              = () => Expression.Constant(new List<int?>{ 1, 2, null, null }),
        ["new Queue<int>([1, 2, 3, 4])"]                                                    = () => Expression.Constant(new Queue<int>([1, 2, 3, 4])),
        ["new SortedSet<int>([1, 2, 3, 4])"]                                                = () => Expression.Constant(new SortedSet<int>([1, 2, 3, 4])),
        ["new Stack<int>([1, 2, 3, 4])"]                                                    = () => Expression.Constant(new Stack<int>([1, 2, 3, 4])),
        ["new Memory<int>([ 1, 2, 3, 4 ])"]                                                 = () => Expression.Constant(new Memory<int>([ 1, 2, 3, 4 ])),
        ["new ReadOnlyMemory<int>([ 1, 2, 3, 4 ])"]                                         = () => Expression.Constant(new ReadOnlyMemory<int>([ 1, 2, 3, 4 ])),

        ["ArraySegment<byte>1"]                                                             = () => Expression.Constant(new ArraySegment<byte>([1, 2, 3, 1, 2, 3, 1, 2, 3, 10], 1, 8)),
        ["ArraySegment<byte>2"]                                                             = () => Expression.Constant(new ArraySegment<byte>([1, 2, 3, 1, 2, 3, 1, 2, 3, 10], 2, 8)),
        ["new Dictionary<int, string>{ [1] =\"one\", [2]=\"two\" }.ToFrozenDictionary()"]   = () => Expression.Constant(new Dictionary<int, string>{ [1] ="one", [2]="two" }.ToFrozenDictionary()),
        ["new Dictionary<int, string>{ [1] =\"one\", [3]=\"three\" }.ToFrozenDictionary()"] = () => Expression.Constant(new Dictionary<int, string>{ [1] ="one", [3]="three" }.ToFrozenDictionary()),
        ["new Hashtable{ [1] =\"one\", [2]=\"two\" }"]                                      = () => Expression.Constant(new Hashtable{ [1] ="one", [2]="two" }),
        ["new Hashtable{ [1] =\"one\", [3]=\"three\" }"]                                    = () => Expression.Constant(new Hashtable{ [1] ="one", [3]="three" }),
        ["new ClassDataContract1()"]                                                        = () => Expression.Constant(new ClassDataContract1()),
        ["new ClassDataContract2()"]                                                        = () => Expression.Constant(new ClassDataContract1() { IntProperty = 8 }),
        ["new ClassDataContract1[]"]                                                        = () => Expression.Constant(new ClassDataContract1[] { new(0, "vm"), new(1, "vm2 vm"), }),
        ["new ClassDataContract2[]"]                                                        = () => Expression.Constant(new ClassDataContract1[] { new(2, "vm3"), new(3, "vm3 vm"), }),

        ["(a, b) => checked(a - b)"]                                                        = () => () => (int a, int b) => checked(a - b),
        ["(a, b) => a - b"]                                                                 = () => () => (int a, int b) => a - b,
        ["(a, b) => b - a"]                                                                 = () => () => (int a, int b) => b - a,
        ["(a, b) => a >> b"]                                                                = () => () => (int a, int b) => a >> b,
        ["(a, b) => a ^ b"]                                                                 = () => () => (int a, int b) => a ^ b,
        ["(a, b) => a || b"]                                                                = () => () => (bool a, bool b) => a || b,
        ["(a, b) => a | b"]                                                                 = () => () => (int a, int b) => a | b,
        ["(a, b) => a != b"]                                                                = () => () => (int a, int b) => a != b,
        ["(a, b) => checked(a * b)"]                                                        = () => () => (int a, int b) => checked(a * b),
        ["(a, b) => checked(b * a)"]                                                        = () => () => (int a, int b) => checked(b * a),
        ["(b, a) => checked(a * b)"]                                                        = () => () => (int b, int a) => checked(a * b),
        ["(a, b) => a * b"]                                                                 = () => () => (int a, int b) => a * b,
        ["(a, b) => a % b"]                                                                 = () => () => (int a, int b) => a % b,
        ["(a, b) => a <= b"]                                                                = () => () => (int a, int b) => a <= b,
        ["(a, b) => a < b"]                                                                 = () => () => (int a, int b) => a < b,
        ["(a, b) => a << b"]                                                                = () => () => (int a, int b) => a << b,
        ["(a, b) => a >= b"]                                                                = () => () => (int a, int b) => a >= b,
        ["(a, b) => a > b"]                                                                 = () => () => (int a, int b) => a > b,
        ["(a, b) => a == b"]                                                                = () => () => (int a, int b) => a == b,
        ["(a, b) => a / b"]                                                                 = () => () => (int a, int b) => a / b,
        ["(a, b) => a ?? b"]                                                                = () => () => (int? a, int b) => a ?? b,
        ["(a, i) => a[i]"]                                                                  = () => () => (int[] a, int i) => a[i],
        ["(a, b) => a && b"]                                                                = () => () => (bool a, bool b) => a && b,
        ["(a, b) => a & b"]                                                                 = () => () => (int a, int b) => a & b,
        ["(a, b) => (a + b) * 42"]                                                          = () => () => (int a, int b) => (a + b) * 42,
        ["(a, b) => a + b * 42"]                                                            = () => () => (int a, int b) => a + b * 42,
        ["(a, b) => checked(a + b)"]                                                        = () => () => (int a, int b) => checked(a + b),
        ["(a, b) => a + (b + c)"]                                                           = () => () => (int a, int b, int c) => a + (b + c),
        ["(a, b) => a + b + c"]                                                             = () => () => (int a, int b, int c) => a + b + c,
        ["(a, b) => a + b"]                                                                 = () => () => (int a, int b) => a + b,
        ["a => a as b"]                                                                     = () => () => (ClassDataContract2 a) => a as ClassDataContract1,
        ["a => a is b"]                                                                     = () => () => (object a) => a is ClassDataContract1,
        ["a => a equals int"]                                                               = () => () => Expression.Lambda(Expression.TypeEqual(_paramA, typeof(int)), _paramA),
        ["(a, b) => a ** b"]                                                                = () => () => Expression.Lambda(Expression.Power(Expression.Constant(2.0), Expression.Constant(3.0))),

        ["a = 1"]                                                                           = () => Expression.Assign(_paramA, _const1),
        ["a = 2"]                                                                           = () => Expression.Assign(_paramA, _const2),
        ["a = b"]                                                                           = () => Expression.Assign(_paramA, _paramB),
        ["a += b"]                                                                          = () => Expression.AddAssign(_paramA, _paramB),
        ["a += 1"]                                                                          = () => Expression.AddAssign(_paramA, _const1),
        ["a -= b"]                                                                          = () => Expression.SubtractAssign(_paramA, _paramB),
        ["a *= b"]                                                                          = () => Expression.MultiplyAssign(_paramA, _paramB),
        ["checked(a *= b)"]                                                                 = () => Expression.MultiplyAssignChecked(_paramA, _paramB),
        ["checked(a *= 2)"]                                                                 = () => Expression.MultiplyAssignChecked(_paramA, _const2),

        ["a => increment(a)"]                                                               = () => Expression.Lambda(Expression.Increment(_paramA), _paramA),
        ["a => decrement(a)"]                                                               = () => Expression.Lambda(Expression.Decrement(_paramA), _paramA),
        ["a => ++a"]                                                                        = () => Expression.Lambda(Expression.PreIncrementAssign(_paramA), _paramA),
        ["a => a++"]                                                                        = () => Expression.Lambda(Expression.PostIncrementAssign(_paramA), _paramA),
        ["a => --a"]                                                                        = () => Expression.Lambda(Expression.PreDecrementAssign(_paramA), _paramA),
        ["a => a - 1"]                                                                      = () => Expression.Lambda(Expression.Subtract(_paramA, _const1)),
        ["a => a--"]                                                                        = () => Expression.Lambda(Expression.PostDecrementAssign(_paramA), _paramA),
        ["b => increment(b)"]                                                               = () => Expression.Lambda(Expression.Increment(_paramB), _paramB),
        ["b => decrement(b)"]                                                               = () => Expression.Lambda(Expression.Decrement(_paramB), _paramB),
        ["b => ++b"]                                                                        = () => Expression.Lambda(Expression.PreIncrementAssign(_paramB), _paramB),
        ["b => b++"]                                                                        = () => Expression.Lambda(Expression.PostIncrementAssign(_paramB), _paramB),
        ["b => --b"]                                                                        = () => Expression.Lambda(Expression.PreDecrementAssign(_paramB), _paramB),
        ["b => b--"]                                                                        = () => Expression.Lambda(Expression.PostDecrementAssign(_paramB), _paramB),

        ["(s,d) => true"]                                                                   = () => () => (string s, DateTime d) => true,
        ["(s`,d`) => true"]                                                                 = () => () => (DateTime s, string d) => true,
        ["(s,d) => false"]                                                                  = () => () => (string s, DateTime d) => false,
        ["(d,s) => true"]                                                                   = () => () => (string d, DateTime s) => true,
        ["i => true"]                                                                       = () => () => (int i) => true,
        ["i => false"]                                                                      = () => () => (int i) => false,
        ["a => a._a"]                                                                       = () => () => (TestMethods a) => a._a,
        ["a => a._b"]                                                                       = () => () => (TestMethods a) => a._b,
        ["a => a.A"]                                                                        = () => () => (TestMethods a) => a.A,
        ["a => a.B"]                                                                        = () => () => (TestMethods a) => a.B,
        ["a => a.Method1()"]                                                                = () => () => () => TestMethods.Method1(),
        ["a => a.Method3(1,1)"]                                                             = () => () => (TestMethods a) => a.Method3(1, 1.1),
        ["a => a.Method3(1,2)"]                                                             = () => () => (TestMethods a) => a.Method3(1, 2.1),
        ["a => a.Method4(42,3.14)"]                                                         = () => () => (TestMethods a) => a.Method4(42, 3.14),
        ["a => a.Method4(23,2.71)"]                                                         = () => () => (TestMethods a) => a.Method4(23, 2.71),

        ["() => new StructDataContract1"]                                                   = () => () => new StructDataContract1(42, "don't panic"),
        ["(a,b) => { ... }"]                                                                = () => _block,
        ["(f,a) => f(a)"]                                                                   = () => (Func<int, int> f, int a) => f(a),
        ["accessMemberMember"]                                                              = () => (TestMembersInitialized m) => m.InnerProperty.IntProperty,
        ["accessMemberMember1"]                                                             = () => _accessMemberMember1,
        ["arrayAccessExpr"]                                                                 = () => _arrayAccessExpr,
        ["b => b ? 1 : 3"]                                                                  = () => (bool b) => b ? 1 : 3,
        ["Console.WriteLine"]                                                               = () => WriteLine1Expression("Default"),
        ["goto1"]                                                                           = () => _goto1,
        ["goto2"]                                                                           = () => _goto2,
        ["goto3"]                                                                           = () => _goto3,
        ["goto4"]                                                                           = () => _goto4,
        ["indexMember"]                                                                     = () => (TestMembersInitialized m) => m.ArrayProperty.Length > 0 ? m.ArrayProperty[m.ArrayProperty.Length - 1] : -1,
        ["array[index]"]                                                                    = () => _lambdaExpr,
        ["indexObject1"]                                                                    = () => (TestMembersInitialized1 m) => m[1],
        ["loop"]                                                                            = () => _lambdaWithLoopContinueBreak,
        ["newArrayBounds"]                                                                  = () => () => new string[2, 3, 4],
        ["newArrayItems"]                                                                   = () => () => new string[] { "aaa", "bbb", "ccc" },
        ["newDictionaryInit"]                                                               = () => () => new Dictionary<int, string> { { 1, "one" }, { 2, "two" }, { 3, "three" }, },
        ["newListInit"]                                                                     = () => () => new List<string> { "aaa", "bbb", "ccc", },
        ["return1"]                                                                         = () => _return1,
        ["return2"]                                                                         = () => _return2,
        ["switch(a){ ... }"]                                                                = () => _switch,
        ["throw"]                                                                           = () => _throw,
        ["try1"]                                                                            = () => _try1,
        ["try2"]                                                                            = () => _try2,
        ["try3"]                                                                            = () => _try3,
        ["try4"]                                                                            = () => _try4,
        ["try5"]                                                                            = () => _try5,
        ["try6"]                                                                            = () => _try6,
        ["newMembersInit"]                                                                  = _newMembersInitialized,
        ["newMembersInit1"]                                                                 = _newMembersInitialized1,
        ["newMembersInit2"]                                                                 = _newMembersInitialized2,

        // Inequality variants
        ["(a,b) => { diffVars }"]                                                           = () => _blockDiffVars,
        ["(a,b) => { diffExprs }"]                                                          = () => _blockDiffExprs,
        ["switch(a){ diffDefault }"]                                                        = () => _switchDiffDefault,
        ["switch(a){ diffCases }"]                                                          = () => _switchDiffCases,
        ["try2DiffHandlers"]                                                                = () => _try2DiffHandlers,
        ["try2DiffCatchType"]                                                               = () => _try2DiffCatchType,
        ["loopDiffLabel"]                                                                   = () => _lambdaWithLoopDiffLabel,
        ["goto1Return"]                                                                     = () => _goto1Return,
        ["indexObject1DiffValue"]                                                           = () => _indexObject1DiffValue,
        ["invoke1"]                                                                         = () => _invoke1,
        ["invoke2"]                                                                         = () => _invoke2,
        ["newArrayItems3"]                                                                  = () => _newArrayItems3,
        ["newArrayItems4"]                                                                  = () => _newArrayItems4,
        ["newListInit2"]                                                                    = () => _newListInit2,
        ["newListInit3"]                                                                    = () => _newListInit3,
        ["memberInit1Binding"]                                                              = () => _memberInit1Binding,
        ["memberInit2Bindings"]                                                             = () => _memberInit2Bindings,
        ["callMethod1"]                                                                     = () => _callMethod1,
        ["callMethod2"]                                                                     = () => _callMethod2,
        ["newCDC1_1a"]                                                                      = () => _newStruct1,
        ["newCDC1_2b"]                                                                      = () => _newStruct2,
        ["condTrue3"]                                                                       = () => _condTrue3,
        ["condTrue5"]                                                                       = () => _condTrue5,
    };
}
