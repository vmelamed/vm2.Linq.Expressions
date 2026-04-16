namespace vm2.Linq.Expressions.Serialization.TestData;

#pragma warning disable IDE0300 // Simplify collection initialization
#pragma warning disable IDE0056 // Use index operator

public static class StatementTestData
{
    /// <summary>
    /// Gets the expression mapped to the specified identifier.
    /// </summary>
    /// <param name="id">The identifier.</param>
    /// <returns>Expression.</returns>
    public static Expression? GetExpression(string id) => _substitutes.GetValueOrDefault(id);

    public static readonly TheoryData<string, string, string> Data = new ()
    {
        { TestLine(), "() => new",                      "New" },
        { TestLine(), "(a,b) => { ... }",               "Block" },
        { TestLine(), "(f,a) => f(a)",                  "Invocation" },
        { TestLine(), "accessMemberMember",             "AccessMemberMember" },
        { TestLine(), "accessMemberMember1",            "AccessMemberMember1" },
        { TestLine(), "arrayAccessExpr",                "ArrayAccessExpr" },
        { TestLine(), "array[index]",                   "ArrayIndex" },
        { TestLine(), "b => b ? 1 : 3",                 "Conditional" },
        { TestLine(), "Console.WriteLine",              "MethodCall" },
        { TestLine(), "goto1",                          "Goto1" },
        { TestLine(), "goto2",                          "Goto2" },
        { TestLine(), "goto3",                          "Goto3" },
        { TestLine(), "goto4",                          "Goto4" },
        { TestLine(), "newHashtableInit",               "NewHashtable" },
        { TestLine(), "indexMember",                    "IndexMember" },
        { TestLine(), "indexObject1",                   "IndexObject1" },
        { TestLine(), "genericArrayCall",               "GenericArrayCall" },
        { TestLine(), "genericByRefCall",               "GenericByRefCall" },
        { TestLine(), "loop",                           "Loop" },
        { TestLine(), "linqCall",                       "LinqCall" },
        { TestLine(), "newArrayBounds",                 "NewArrayBounds" },
        { TestLine(), "newArrayItems",                  "NewArrayInit" },
        { TestLine(), "newDictionaryInit",              "NewDictionaryInit" },
        { TestLine(), "newHashtableInit",               "NewHashtableInit" },
        { TestLine(), "newListInit",                    "NewListInit" },
        { TestLine(), "newMembersInit",                 "NewMembersInit" },
        { TestLine(), "return1",                        "Return1" },
        { TestLine(), "return2",                        "Return2" },
        { TestLine(), "switch(a){ ... }",               "Switch" },
        { TestLine(), "throw",                          "Throw" },
        { TestLine(), "try1",                           "TryCatch1" },
        { TestLine(), "try2",                           "TryCatch2" },
        { TestLine(), "try3",                           "TryCatch3" },
        { TestLine(), "try4",                           "TryCatch4" },
        { TestLine(), "try5",                           "TryCatch5" },
        { TestLine(), "try6",                           "TryCatch6" },
    };

    static readonly ParameterExpression _paramA    = Expression.Parameter(typeof(int), "a");
    static readonly ParameterExpression _paramB    = Expression.Parameter(typeof(int), "b");
    static readonly ParameterExpression _paramC    = Expression.Parameter(typeof(int), "c");
    static readonly ParameterExpression _paramD    = Expression.Parameter(typeof(int), "d");
    static readonly ParameterExpression _value     = Expression.Parameter(typeof(int), "_value");
    static readonly ParameterExpression _result    = Expression.Parameter(typeof(int), "_result");
    static readonly ParameterExpression _var       = Expression.Parameter(typeof(int), "a");
    static readonly ParameterExpression _array     = Expression.Parameter(typeof(int[]), "Array");
    static readonly ParameterExpression _index     = Expression.Parameter(typeof(int), "Index");

    static readonly LabelTarget _labelContinue  = Expression.Label("continue");
    static readonly LabelTarget _labelBreak = Expression.Label("break");
    static readonly LabelTarget _returnTarget = Expression.Label();

    static Expression WriteLine1Expression(string s) => Expression.Call(null, _miWriteLine, Expression.Constant(s));
    static Expression ExceptionDefaultCtor() => Expression.New(typeof(Exception).GetConstructor(Type.EmptyTypes)!);
    static Expression ThrowException() => Expression.Throw(ExceptionDefaultCtor());

    static readonly MethodInfo _miWriteLine = typeof(Console).GetMethod("WriteLine", [ typeof(string) ])!;
    static readonly MethodInfo _miEchoByRefInt = typeof(GenericMethodFixtures).GetMethod(nameof(GenericMethodFixtures.EchoByRef))!.MakeGenericMethod(typeof(int));
    static readonly ParameterExpression _byRefVar = Expression.Variable(typeof(int), "byRefVar");

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
            [ _var ],
            Expression.Assign(_var, Expression.Constant(0)),
            Expression.Increment(_var),
            Expression.Goto(_returnTarget),
            Expression.Increment(_var),
            Expression.Label(_returnTarget)
        );

    static readonly Expression _goto4 =
        Expression.Block(
            [ _var ],
            Expression.Assign(_var, Expression.Constant(0)),
            Expression.Increment(_var),
            Expression.Goto(_returnTarget, type: typeof(void)),
            Expression.Increment(_var),
            Expression.Label(_returnTarget)
        );

    static readonly Expression _return1 =
        Expression.Block(
            [ _var ],
            Expression.Assign(_var, Expression.Constant(0)),
            Expression.Increment(_var),
            Expression.Return(_returnTarget),
            Expression.Increment(_var),
            Expression.Label(_returnTarget)
        );

    static readonly Expression _return2 =
        Expression.Block(
            [ _var ],
            Expression.Assign(_var, Expression.Constant(0)),
            Expression.Increment(_var),
            Expression.Return(_returnTarget, _var),
            Expression.Increment(_var),
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

    static readonly Expression _arrayAccessExpr = Expression.ArrayAccess(_array, _index);

    // Array[Index] = (Array[Index] + Value)
    static readonly Expression _lambdaExpr = Expression.Assign(
                                                _arrayAccessExpr,
                                                Expression.Add(
                                                    _arrayAccessExpr,
                                                    _value));

    static readonly Expression _genericByRefCall = Expression.Lambda<Func<int>>(
        Expression.Block(
            [_byRefVar],
            Expression.Assign(_byRefVar, Expression.Constant(7)),
            Expression.Call(_miEchoByRefInt, _byRefVar)));

    static readonly Dictionary<string, Expression> _substitutes = new()
    {
        ["() => new"]               = () => () => new StructDataContract1(42, "don't panic"),
        ["(a,b) => { ... }"]        = _block,
        ["(f,a) => f(a)"]           = (Func<int, int> f, int a) => f(a),
        ["accessMemberMember"]      = (TestMembersInitialized m) => m.InnerProperty.IntProperty,
        ["accessMemberMember1"]     = _accessMemberMember1,
        ["arrayAccessExpr"]         = _arrayAccessExpr,
        ["b => b ? 1 : 3"]          = (bool b) => b ? 1 : 3,
        ["Console.WriteLine"]       = WriteLine1Expression("Default"),
        ["goto1"]                   = _goto1,
        ["goto2"]                   = _goto2,
        ["goto3"]                   = _goto3,
        ["goto4"]                   = _goto4,
        ["indexMember"]             = (TestMembersInitialized m) => m.ArrayProperty.Length > 0 ? m.ArrayProperty[m.ArrayProperty.Length - 1] : -1,
        ["array[index]"]            = _lambdaExpr,
        ["indexObject1"]            = (TestMembersInitialized1 m) => m[1],
        ["genericArrayCall"]        = (Expression<Func<int[], int>>)(arr => GenericMethodFixtures.CountArray(arr)),
        ["genericByRefCall"]        = _genericByRefCall,
        ["loop"]                    = _lambdaWithLoopContinueBreak,
        ["linqCall"]                = (Expression<Func<int[], int>>)(arr => arr.Where(n => n > 0).Sum()),
        ["newArrayBounds"]          = () => new string[2, 3, 4],
        ["newArrayItems"]           = () => new string[] { "aaa", "bbb", "ccc" },
        ["newDictionaryInit"]       = () => new Dictionary<int, string> { { 1, "one" }, { 2, "two" }, { 3, "three" }, },
        ["newHashtableInit"]        = () => new Hashtable { { 1, "one" }, { 2, "two" }, { 3, "three" }, },
        ["newListInit"]             = () => new List<string> { "aaa", "bbb", "ccc", },
        ["newMembersInit"]          = () => new TestMembersInitialized
        {
            TheOuterIntProperty = 42,
            Time = new DateTime(1776, 7, 4),
            InnerProperty = new Inner
            {
                IntProperty = 23,
                StringProperty = "inner string"
            },
            EnumerableProperty = new List<string> { "aaa", "bbb", "ccc", },
        },
        ["return1"]                 = _return1,
        ["return2"]                 = _return2,
        ["switch(a){ ... }"]        = _switch,
        ["throw"]                   = _throw,
        ["try1"]                    = _try1,
        ["try2"]                    = _try2,
        ["try3"]                    = _try3,
        ["try4"]                    = _try4,
        ["try5"]                    = _try5,
        ["try6"]                    = _try6,
    };
}
