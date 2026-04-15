namespace vm2.Linq.Expressions.DeepEquals;

/// <summary>
/// Compares two expression trees simultaneously by recursively walking both in lockstep.
/// A single-pass design that replaces the previous two-class enqueue/dequeue approach,
/// eliminating the risk of traversal de-synchronization.
/// </summary>
#pragma warning disable IL2070, IL2075, IL2060 // Reflection on well-known BCL types (Nullable<>, Memory<>, ArraySegment<>)
class DeepEqualsComparer
{
    // Reflection caches — one entry per closed generic type, shared across all comparer instances.
    static readonly ConcurrentDictionary<Type, (PropertyInfo HasValue, PropertyInfo Value)> s_nullableCache = new();
    static readonly ConcurrentDictionary<Type, MethodInfo> s_toArrayCache = new();
    static readonly ConcurrentDictionary<Type, PropertyInfo> s_arrayPropertyCache = new();

    public bool Equal { get; private set; } = true;

    public string Difference { get; private set; } = "";

    bool Fail(object? left, object? right)
    {
        if (Equal)
        {
            Equal = false;
            Difference = $"Difference at sub-nodes: `{left}` != `{right}`";
        }
        return false;
    }

    // ── Main dispatch ───────────────────────────────────────────

    public bool Compare(Expression? left, Expression? right)
    {
        if (!Equal) return false;
        if (ReferenceEquals(left, right)) return true;
        if (left is null) return right is null || Fail(left, right);
        if (right is null) return Fail(left, right);
        if (left.Type != right.Type)
        {
            Equal = false;
            Difference = $"Left and right are of different types: `{left.Type.FullName}` != `{right.Type.FullName}` (`{left}` != `{right}`)";
            return false;
        }
        if (left.NodeType != right.NodeType)
            return Fail(left, right);

        return (left, right) switch
        {
            (BinaryExpression l, BinaryExpression r)         => CompareBinary(l, r),
            (BlockExpression l, BlockExpression r)           => CompareBlock(l, r),
            (ConditionalExpression l, ConditionalExpression r) => CompareConditional(l, r),
            (ConstantExpression l, ConstantExpression r)     => CompareConstant(l, r),
            (DefaultExpression, DefaultExpression)           => true,
            (GotoExpression l, GotoExpression r)             => CompareGoto(l, r),
            (IndexExpression l, IndexExpression r)           => CompareIndex(l, r),
            (InvocationExpression l, InvocationExpression r) => CompareInvocation(l, r),
            (LabelExpression l, LabelExpression r)           => CompareLabel(l, r),
            (LambdaExpression l, LambdaExpression r)         => CompareLambda(l, r),
            (ListInitExpression l, ListInitExpression r)     => CompareListInit(l, r),
            (LoopExpression l, LoopExpression r)             => CompareLoop(l, r),
            (MemberExpression l, MemberExpression r)         => CompareMember(l, r),
            (MemberInitExpression l, MemberInitExpression r) => CompareMemberInit(l, r),
            (MethodCallExpression l, MethodCallExpression r) => CompareMethodCall(l, r),
            (NewExpression l, NewExpression r)               => CompareNew(l, r),
            (NewArrayExpression l, NewArrayExpression r)     => CompareNewArray(l, r),
            (ParameterExpression l, ParameterExpression r)   => CompareParameter(l, r),
            (SwitchExpression l, SwitchExpression r)         => CompareSwitch(l, r),
            (TryExpression l, TryExpression r)               => CompareTry(l, r),
            (TypeBinaryExpression l, TypeBinaryExpression r) => CompareTypeBinary(l, r),
            (UnaryExpression l, UnaryExpression r)           => CompareUnary(l, r),
            // DebugInfo, Dynamic, RuntimeVariables, Extension — not compared
            _                                                => true,
        };
    }

    // ── Sequence helpers ────────────────────────────────────────

    // Callers must pre-check that counts match before calling these.
    // If an element differs, Compare/CompareElementInit/CompareMemberBinding calls Fail internally.

    bool CompareExpressions(IReadOnlyList<Expression> left, IReadOnlyList<Expression> right)
    {
        for (var i = 0; i < left.Count; i++)
            if (!Compare(left[i], right[i]))
                return false;
        return true;
    }

    bool CompareElementInits(IReadOnlyList<ElementInit> left, IReadOnlyList<ElementInit> right)
    {
        for (var i = 0; i < left.Count; i++)
            if (!CompareElementInit(left[i], right[i]))
                return false;
        return true;
    }

    bool CompareMemberBindings(IReadOnlyList<MemberBinding> left, IReadOnlyList<MemberBinding> right)
    {
        for (var i = 0; i < left.Count; i++)
            if (!CompareMemberBinding(left[i], right[i]))
                return false;
        return true;
    }

    // ── Expression node comparers ───────────────────────────────

    bool CompareBinary(BinaryExpression left, BinaryExpression right)
    {
        if (left.IsLifted != right.IsLifted
            || left.IsLiftedToNull != right.IsLiftedToNull
            || left.Method != right.Method)
            return Fail(left, right);

        return Compare(left.Left, right.Left)
            && Compare(left.Right, right.Right)
            && Compare(left.Conversion, right.Conversion);
    }

    bool CompareBlock(BlockExpression left, BlockExpression right)
    {
        if (left.Variables.Count != right.Variables.Count
            || left.Expressions.Count != right.Expressions.Count)
            return Fail(left, right);

        return CompareExpressions(left.Variables, right.Variables)
            && CompareExpressions(left.Expressions, right.Expressions);
    }

    bool CompareConditional(ConditionalExpression left, ConditionalExpression right)
        => Compare(left.Test, right.Test)
        && Compare(left.IfTrue, right.IfTrue)
        && Compare(left.IfFalse, right.IfFalse);

    bool CompareConstant(ConstantExpression left, ConstantExpression right)
    {
        if (left.Value is null != right.Value is null)
            return Fail(left, right);

        // Both null, or both System.Object — treat as equal
        if (left.Value is null || left.Type == typeof(object))
            return true;

        var lType = left.Type;
        var lValue = left.Value;
        var rValue = right.Value;

        Debug.Assert(lValue is not null);
        Debug.Assert(rValue is not null);

        IEnumerable? enumL = null;
        IEnumerable? enumR = null;

        // Unwrap Nullable<T>
        if (lType.IsGenericType && lType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            var (piHasValue, piValue) = s_nullableCache.GetOrAdd(lType, static t => (
                t.GetProperty("HasValue") ?? throw new InvalidOperationException("Could not get the property Nullable<>.HasValue."),
                t.GetProperty("Value")    ?? throw new InvalidOperationException("Could not get the property Nullable<>.Value.")
            ));

            var hasValueL = (bool)piHasValue.GetValue(lValue, null)!;
            var hasValueR = (bool)(piHasValue.GetValue(rValue, null) ?? throw new InvalidOperationException("Could not get the value of the property Nullable<>.HasValue."));

            if (hasValueL != hasValueR)
                return Fail(left, right);
            if (!hasValueL)
                return true;

            lType = lValue.GetType();
            lValue = piValue.GetValue(lValue) ?? throw new InvalidOperationException("Could not get the value of the property Nullable<>.Value.");
            rValue = piValue.GetValue(rValue) ?? throw new InvalidOperationException("Could not get the value of the property Nullable<>.Value.");
        }

        // Memory<T> / ReadOnlyMemory<T> — convert to arrays for element comparison
        if (lValue is not IEnumerable)
        {
            var genType = lType.IsGenericType ? lType.GetGenericTypeDefinition() : null;

            if (genType is not null)
            {
                var elemType = lType.GetGenericArguments()[0];

                if (genType == typeof(Memory<>) || genType == typeof(ReadOnlyMemory<>))
                {
                    var mi = s_toArrayCache.GetOrAdd(lType, static t =>
                    {
                        var m = t.GetMethod("ToArray") ?? throw new InvalidOperationException("Could not get the method Memory<T>.ToArray.");
                        return m.IsGenericMethod ? m.MakeGenericMethod(t.GetGenericArguments()[0]) : m;
                    });

                    enumL = mi.Invoke(lValue, []) as IEnumerable;
                    enumR = mi.Invoke(rValue, []) as IEnumerable;
                }
            }
        }
        else if (lType.IsArray || lType.Namespace?.StartsWith("System") is true)
        {
            // ArraySegment<T> — check backing array null-ness
            if (lType.IsGenericType && lType.GetGenericTypeDefinition() == typeof(ArraySegment<>))
            {
                var piArray = s_arrayPropertyCache.GetOrAdd(lType, static t =>
                    t.GetProperty("Array") ?? throw new InvalidOperationException("Could not get the property ArraySegment<T>.Array."));
                var lArray = piArray.GetValue(lValue);
                var rArray = piArray.GetValue(rValue);

                if (lArray is null != rArray is null)
                    return Fail(left, right);
                if (lArray is null)
                    return true;
            }

            enumL = lValue as IEnumerable;
            enumR = rValue as IEnumerable;
        }

        // Element-by-element comparison in a single pass
        if (enumL is not null && enumR is not null)
        {
            var itrL = enumL.GetEnumerator();
            var itrR = enumR.GetEnumerator();

            while (true)
            {
                var hasL = itrL.MoveNext();
                var hasR = itrR.MoveNext();

                if (hasL != hasR)       // different lengths
                    return Fail(left, right);
                if (!hasL)              // both exhausted
                    break;
                if (!Equals(itrL.Current, itrR.Current))
                    return Fail(left, right);
            }
        }
        else if (!Equals(lValue, rValue))
        {
            return Fail(left, right);
        }

        return true;
    }

    bool CompareGoto(GotoExpression left, GotoExpression right)
    {
        if (left.Kind != right.Kind)
            return Fail(left, right);

        return CompareLabelTarget(left.Target, right.Target)
            && Compare(left.Value, right.Value);
    }

    bool CompareIndex(IndexExpression left, IndexExpression right)
    {
        if (left.Indexer != right.Indexer
            || left.Arguments.Count != right.Arguments.Count)
            return Fail(left, right);

        return Compare(left.Object, right.Object)
            && CompareExpressions(left.Arguments, right.Arguments);
    }

    bool CompareInvocation(InvocationExpression left, InvocationExpression right)
    {
        if (left.Arguments.Count != right.Arguments.Count)
            return Fail(left, right);

        return Compare(left.Expression, right.Expression)
            && CompareExpressions(left.Arguments, right.Arguments);
    }

    bool CompareLabel(LabelExpression left, LabelExpression right)
        => CompareLabelTarget(left.Target, right.Target)
        && Compare(left.DefaultValue, right.DefaultValue);

    bool CompareLambda(LambdaExpression left, LambdaExpression right)
    {
        if (left.ReturnType != right.ReturnType
            || left.Parameters.Count != right.Parameters.Count
            || left.TailCall != right.TailCall)
            return Fail(left, right);

        return CompareExpressions(left.Parameters, right.Parameters)
            && Compare(left.Body, right.Body);
    }

    bool CompareListInit(ListInitExpression left, ListInitExpression right)
    {
        if (left.Initializers.Count != right.Initializers.Count)
            return Fail(left, right);

        return Compare(left.NewExpression, right.NewExpression)
            && CompareElementInits(left.Initializers, right.Initializers);
    }

    bool CompareLoop(LoopExpression left, LoopExpression right)
        => CompareLabelTarget(left.BreakLabel, right.BreakLabel)
        && CompareLabelTarget(left.ContinueLabel, right.ContinueLabel)
        && Compare(left.Body, right.Body);

    bool CompareMember(MemberExpression left, MemberExpression right)
    {
        if (left.Member != right.Member)
            return Fail(left, right);

        return Compare(left.Expression, right.Expression);
    }

    bool CompareMemberInit(MemberInitExpression left, MemberInitExpression right)
    {
        if (left.Bindings.Count != right.Bindings.Count)
            return Fail(left, right);

        return Compare(left.NewExpression, right.NewExpression)
            && CompareMemberBindings(left.Bindings, right.Bindings);
    }

    bool CompareMethodCall(MethodCallExpression left, MethodCallExpression right)
    {
        if (left.Method != right.Method
            || left.Arguments.Count != right.Arguments.Count)
            return Fail(left, right);

        return Compare(left.Object, right.Object)
            && CompareExpressions(left.Arguments, right.Arguments);
    }

    bool CompareNew(NewExpression left, NewExpression right)
    {
        if (left.Constructor != right.Constructor
            || left.Arguments.Count != right.Arguments.Count
            || left.Members is null != right.Members is null)
            return Fail(left, right);

        if (left.Members is not null && right.Members is not null
            && !left.Members.SequenceEqual(right.Members))
            return Fail(left, right);

        return CompareExpressions(left.Arguments, right.Arguments);
    }

    bool CompareNewArray(NewArrayExpression left, NewArrayExpression right)
    {
        if (left.Expressions.Count != right.Expressions.Count)
            return Fail(left, right);

        return CompareExpressions(left.Expressions, right.Expressions);
    }

    bool CompareParameter(ParameterExpression left, ParameterExpression right)
    {
        if (left.IsByRef != right.IsByRef || left.Name != right.Name)
            return Fail(left, right);

        return true;
    }

    bool CompareSwitch(SwitchExpression left, SwitchExpression right)
    {
        if (left.Comparison != right.Comparison
            || left.Cases.Count != right.Cases.Count)
            return Fail(left, right);

        if (!Compare(left.SwitchValue, right.SwitchValue))
            return false;

        for (var i = 0; i < left.Cases.Count; i++)
            if (!CompareSwitchCase(left.Cases[i], right.Cases[i]))
                return false;

        return Compare(left.DefaultBody, right.DefaultBody);
    }

    bool CompareTry(TryExpression left, TryExpression right)
    {
        if (left.Handlers.Count != right.Handlers.Count)
            return Fail(left, right);

        if (!Compare(left.Body, right.Body))
            return false;

        for (var i = 0; i < left.Handlers.Count; i++)
            if (!CompareCatchBlock(left.Handlers[i], right.Handlers[i]))
                return false;

        return Compare(left.Finally, right.Finally)
            && Compare(left.Fault, right.Fault);
    }

    bool CompareTypeBinary(TypeBinaryExpression left, TypeBinaryExpression right)
    {
        if (left.TypeOperand != right.TypeOperand)
            return Fail(left, right);

        return Compare(left.Expression, right.Expression);
    }

    bool CompareUnary(UnaryExpression left, UnaryExpression right)
    {
        if (left.IsLifted != right.IsLifted
            || left.IsLiftedToNull != right.IsLiftedToNull
            || left.Method != right.Method)
            return Fail(left, right);

        return Compare(left.Operand, right.Operand);
    }

    // ── Non-Expression node comparers ───────────────────────────

    bool CompareLabelTarget(LabelTarget? left, LabelTarget? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left is null) return right is null || Fail(left, right);
        if (right is null) return Fail(left, right);

        if (left.Name != right.Name || left.Type != right.Type)
            return Fail(left, right);

        return true;
    }

    bool CompareCatchBlock(CatchBlock left, CatchBlock right)
    {
        if (left.Test != right.Test)
            return Fail(left, right);

        return Compare(left.Variable, right.Variable)
            && Compare(left.Filter, right.Filter)
            && Compare(left.Body, right.Body);
    }

    bool CompareSwitchCase(SwitchCase left, SwitchCase right)
    {
        if (left.TestValues.Count != right.TestValues.Count)
            return Fail(left, right);

        return CompareExpressions(left.TestValues, right.TestValues)
            && Compare(left.Body, right.Body);
    }

    bool CompareElementInit(ElementInit left, ElementInit right)
    {
        if (left.AddMethod != right.AddMethod
            || left.Arguments.Count != right.Arguments.Count)
            return Fail(left, right);

        return CompareExpressions(left.Arguments, right.Arguments);
    }

    bool CompareMemberBinding(MemberBinding left, MemberBinding right)
    {
        if (left.BindingType != right.BindingType || left.Member != right.Member)
            return Fail(left, right);

        return (left, right) switch
        {
            (MemberAssignment l, MemberAssignment r)       => Compare(l.Expression, r.Expression),
            (MemberListBinding l, MemberListBinding r)     => CompareListBinding(l, r),
            (MemberMemberBinding l, MemberMemberBinding r) => CompareMemberMemberBinding(l, r),
            _                                              => Fail(left, right),
        };
    }

    bool CompareListBinding(MemberListBinding left, MemberListBinding right)
    {
        if (left.Initializers.Count != right.Initializers.Count)
            return Fail(left, right);

        return CompareElementInits(left.Initializers, right.Initializers);
    }

    bool CompareMemberMemberBinding(MemberMemberBinding left, MemberMemberBinding right)
    {
        if (left.Bindings.Count != right.Bindings.Count)
            return Fail(left, right);

        return CompareMemberBindings(left.Bindings, right.Bindings);
    }
}
#pragma warning restore IL2070, IL2075, IL2060
