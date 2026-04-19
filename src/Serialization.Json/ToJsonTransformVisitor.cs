namespace vm2.Linq.Expressions.Serialization.Json;

/// <summary>
/// Class ToJsonTransformVisitor.
/// Implements the <see cref="ExpressionTransformVisitor{XNode}" />
/// </summary>
/// <seealso cref="ExpressionTransformVisitor{XNode}" />
public partial class ToJsonTransformVisitor(JsonOptions options) : ExpressionTransformVisitor<JElement>
{
    /// <summary>
    /// Gets a properly named node corresponding to the current expression node.
    /// </summary>
    /// <param name="node">The expression node for which to create an empty document node.</param>
    /// <returns>TDocument.</returns>
    protected override JElement GetEmptyNode(Expression node)
        => new(
            Transform.Identifier(node.NodeType.ToString(), IdentifierConventions.Camel),
                node switch {
                    // omit the type for expressions where their element says it all
                    ConstantExpression => null,
                    ListInitExpression => null,
                    NewExpression => null,
                    NewArrayExpression => null,
                    LabelExpression => null,
                    // do not omit the void return type for these nodes:
                    LambdaExpression n => new(Vocabulary.Type, Transform.TypeName(n.ReturnType)),
                    MethodCallExpression n => new(Vocabulary.Type, Transform.TypeName(n.Method.ReturnType)),
                    InvocationExpression n => new(Vocabulary.Type, Transform.TypeName(n.Expression.Type)),
                    // for the rest: add attribute type if it is not void:
                    _ => PropertyType(node),
                });

    /// <inheritdoc/>
    protected override Expression VisitConstant(ConstantExpression node)
    {
        try
        {
            return GenericVisit(
                     node,
                     base.VisitConstant,
                     (n, x) => x.Add(
                                    options.TypeComment(n.Type),
                                    _dataTransform.TransformNode(n)));
        }
        catch (Exception x)
        {
            throw new SerializationException($"Don't know how to serialize {node.Type}", x);
        }
    }

    /// <inheritdoc/>
    protected override Expression VisitDefault(DefaultExpression node)
        => GenericVisit(
            node,
            base.VisitDefault,
            (n, x) => x.Add(options.TypeComment(n.Type)));

    /// <inheritdoc/>
    protected override Expression VisitParameter(ParameterExpression node)
    {
        using var _ = OutputDebugScope(node.NodeType.ToString());
        var n = (ParameterExpression)base.VisitParameter(node);

        _elements.Push(GetParameter(n));
        return node;
    }

    #region Lambda
    [ExcludeFromCodeCoverage]
    IEnumerable<JsonNode?> VisitParameterDefinitionList(ReadOnlyCollection<ParameterExpression> parameterList)
        => parameterList.Select(p => !IsDefined(p)
                                    ? GetParameter(p).Node
                                    : throw new InternalTransformErrorException($"Parameter with a name `{p.Name}` is already defined."));

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    protected override Expression VisitLambda<T>(Expression<T> node)
        => GenericVisit(
            node,
            base.VisitLambda,
            (n, x) => x.Add(
                        PropertyDelegateType(node.Type),
                        new JElement(Vocabulary.Parameters, PopElementsValues(n.Parameters.Count)),
                        new JElement(Vocabulary.Body, Pop()),
                        PropertyName(node.Name),
                        node.TailCall ? new JElement(Vocabulary.TailCall, node.TailCall) : null));
    #endregion

    /// <inheritdoc/>
    protected override Expression VisitUnary(UnaryExpression node)
        => GenericVisit(
            node,
            base.VisitUnary,
            (n, x) => x.Add(
                        new JElement(Vocabulary.Operand, PopWrappedElement()),    // pop the operand
                        VisitMethodInfo(n),
                        n.IsLifted ? new JElement(Vocabulary.IsLifted, true) : null,
                        n.IsLiftedToNull ? new JElement(Vocabulary.IsLiftedToNull, true) : null));

    /// <inheritdoc/>
    protected override Expression VisitBinary(BinaryExpression node)
        => GenericVisit(
            node,
            base.VisitBinary,
            (n, x) =>
            {
                var right = PopWrappedElement();
                var convert = n.Conversion is not null ? PopElementValue() : null;
                var left = PopWrappedElement();

                x.Add(
                        new JElement(
                            Vocabulary.Operands, left, right),
                        VisitMethodInfo(n),
                        n.IsLifted ? new JElement(Vocabulary.IsLifted, true) : null,
                        n.IsLiftedToNull ? new JElement(Vocabulary.IsLiftedToNull, true) : null,
                        convert is not null ? new JElement(Vocabulary.ConvertLambda, convert) : null);
            });

    /// <inheritdoc/>
    protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        => GenericVisit(
            node,
            base.VisitTypeBinary,
            (n, x) => x.Add(
                        new JElement(Vocabulary.TypeOperand, Transform.TypeName(n.TypeOperand)),
                        new JElement(Vocabulary.Operands, new JsonArray(PopWrappedElement()))));

    /// <inheritdoc/>
    protected override Expression VisitIndex(IndexExpression node)
        => GenericVisit(
            node,
            base.VisitIndex,
            (n, x) =>
            {
                var indexes = new JElement(Vocabulary.Indexes, PopWrappedElements(n.Arguments.Count));   // pop the index expressions

                x.Add(
                    new JElement(Vocabulary.Object, Pop()),    // pop the object being indexed
                    indexes,
                    VisitMemberInfo(n.Indexer));
            });

    /// <inheritdoc/>
    protected override Expression VisitMember(MemberExpression node)
        => GenericVisit(
            node,
            base.VisitMember,
            (n, x) =>
                x.Add(
                    node.Expression is not null
                        ? new JElement(Vocabulary.Object, PopWrappedElement())  // pop the expression/value that will give the object whose requested member is being accessed
                        : null,                                                 // or null if the property/field is static
                    new JElement(Vocabulary.Member, VisitMemberInfo(n.Member))));

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression node)
        => GenericVisit(
            node,
            base.VisitMethodCall,
            (n, x) =>
            {
                var arguments = new JElement(Vocabulary.Arguments, PopWrappedElements(n.Arguments.Count));        // pop the argument expressions

                x.Add(
                    n.Object != null
                        ? new JElement(Vocabulary.Object, Pop())
                        : null,
                    VisitMemberInfo(n.Method),
                    arguments);
            });

    /// <inheritdoc/>
    protected override Expression VisitInvocation(InvocationExpression node)
        => GenericVisit(
            node,
            base.VisitInvocation,
            (n, x) =>
            {
                var arguments = new JElement(Vocabulary.Arguments, PopWrappedElements(n.Arguments.Count));   // pop the argument expressions

                x.Add(
                    new JElement(Vocabulary.Delegate, Pop()),        // pop the delegate or lambda
                    arguments);
            });

    /// <inheritdoc/>
    protected override Expression VisitBlock(BlockExpression node)
        => GenericVisit(
            node,
            base.VisitBlock,
            (n, x) => x.Add(
                        node.Variables.Count is > 0 ? new JElement(Vocabulary.Variables, PopElementsValues(node.Variables.Count)) : null,
                        new JElement(Vocabulary.Expressions, PopWrappedElements(node.Expressions.Count))));

    /// <inheritdoc/>
    protected override Expression VisitConditional(ConditionalExpression node)
        => GenericVisit(
            node,
            base.VisitConditional,
            (n, x) =>
            {
                JElement? @else = n.IfFalse is not null ? new JElement(Vocabulary.Else, Pop()) : null;
                JElement? then = n.IfTrue   is not null ? new JElement(Vocabulary.Then, Pop()) : null;
                JElement @if = new JElement(Vocabulary.If, Pop());
                x.Add(@if, then, @else);
            });

    /// <inheritdoc/>
    protected override Expression VisitNew(NewExpression node)
        => GenericVisit(
            node,
            base.VisitNew,
            (n, x) => x.Add(
                        VisitMemberInfo(n.Constructor),
                        new JElement(Vocabulary.Arguments, PopWrappedElements(n.Arguments.Count)),   // pop the c-tor arguments
                        n.Members is not null && n.Members.Count > 0
                            ? new JElement(Vocabulary.Members, new JsonObject(n.Members.Select(m => (KeyValuePair<string, JsonNode?>)VisitMemberInfo(m)!)))
                            : null));

    /// <inheritdoc/>
    protected override Expression VisitNewArray(NewArrayExpression node)
        => GenericVisit(
            node,
            base.VisitNewArray,
            (n, x) => x.Add(
                        PropertyType(n.Type.GetElementType()),
                        new JElement(
                                n.NodeType is ExpressionType.NewArrayInit ? Vocabulary.ArrayElements : Vocabulary.Bounds,
                                PopWrappedElements(n.Expressions.Count))));

    #region new list with items initializers
    /// <inheritdoc/>
    protected override Expression VisitListInit(ListInitExpression node)
        => GenericVisit(
            node,
            base.VisitListInit,
            (n, x) =>
            {
                var initializers = new JElement(Vocabulary.Initializers, PopElementsValues(n.Initializers.Count));

                x.Add(
                    Pop(),              // the new list()
                    initializers);      // the elementsInit
            });

    /// <inheritdoc/>
    protected override ElementInit VisitElementInit(ElementInit node)
    {
        using var _ = OutputDebugScope(nameof(ElementInit));
        var elementInit = base.VisitElementInit(node);

        _elements.Push(
            new JElement(
                    Vocabulary.ElementInit,
                        VisitMemberInfo(node.AddMethod),
                        new JElement(Vocabulary.Arguments, PopWrappedElements(node.Arguments.Count))));  // pop the elements init expressions

        return elementInit;
    }
    #endregion

    #region new object with member initialization
    /// <inheritdoc/>
    protected override Expression VisitMemberInit(MemberInitExpression node)
        => GenericVisit(
            node,
            base.VisitMemberInit,
            (n, x) =>
            {
                var bindings = PopWrappedElements(n.Bindings.Count).ToList();   // pop the expressions to assign to members
                x.Add(
                    new JElement(Vocabulary.New, PopElementValue()),            // the new value to be initialized
                    new JElement(Vocabulary.Bindings, bindings));               // bindings for the properties
            });

    ////protected override MemberBinding VisitMemberBinding(MemberBinding node) => base.VisitMemberBinding(node);
    //                  the base is doing exactly what we need - dispatch the call to the concrete binding below:

    /// <inheritdoc/>
    protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
    {
        using var _ = OutputDebugScope(nameof(MemberAssignment));
        var binding = base.VisitMemberAssignment(node);

        _elements.Push(
            new JElement(
                    Vocabulary.AssignmentBinding,
                        new JElement(Vocabulary.Member, VisitMemberInfo(binding.Member)),
                        new JElement(Vocabulary.Value, PopWrappedElement())));  // pop the value (expression) to assign to the member

        return binding;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
    {
        using var _ = OutputDebugScope(nameof(MemberMemberBinding));
        var binding = base.VisitMemberMemberBinding(node);

        _elements.Push(
            new JElement(
                    Vocabulary.MemberMemberBinding,
                        new JElement(Vocabulary.Member, VisitMemberInfo(binding.Member)),
                        new JElement(Vocabulary.Bindings, PopElementsValues(binding.Bindings.Count))));    // pop the bindings for the members of the member

        return binding;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
    {
        using var _ = OutputDebugScope(nameof(MemberListBinding));
        var binding = base.VisitMemberListBinding(node);

        _elements.Push(
            new JElement(
                    Vocabulary.MemberListBinding,
                        new JElement(Vocabulary.Member, VisitMemberInfo(binding.Member)),
                        new JElement(Vocabulary.Initializers, PopElementsValues(binding.Initializers.Count))));    // pop the initializers for the list in the member

        return binding;
    }
    #endregion

    #region Label, target, goto
    /// <inheritdoc/>
    protected override LabelTarget? VisitLabelTarget(LabelTarget? node)
    {
        using var _ = OutputDebugScope(nameof(LabelTarget));
        var n = base.VisitLabelTarget(node);

        if (n is not null)
            _elements.Push(GetLabelTarget(n));

        return n;
    }

    /// <inheritdoc/>
    protected override Expression VisitLabel(LabelExpression node)
        => GenericVisit(
            node,
            base.VisitLabel,
            (n, x) =>
            {
                var value = n.DefaultValue is not null ? new JElement(Vocabulary.Default, PopWrappedElement()) : (JElement?)null;   // pop the default result value if present

                x.Add(
                    Pop(),   // add the target
                    value);
            });

    /// <inheritdoc/>
    protected override Expression VisitGoto(GotoExpression node)
        => GenericVisit(
            node,
            base.VisitGoto,
            (n, x) =>
            {
                JElement? value = n.Value is not null ? Pop() : null;

                x.Add(
                    Pop(),
                    value is not null ? new JElement(Vocabulary.Value, value) : null,
                    new JElement(Vocabulary.Kind, Transform.Identifier(node.Kind.ToString(), IdentifierConventions.Camel)));
            });
    #endregion

    /// <inheritdoc/>
    protected override Expression VisitLoop(LoopExpression node)
        => GenericVisit(
            node,
            base.VisitLoop,
            (n, x) => x.Add(
                        new JElement(Vocabulary.Body, Pop()),
                        n.ContinueLabel is not null
                            ? new JElement(Vocabulary.ContinueLabel, Pop())
                            : null,
                        n.BreakLabel is not null
                            ? new JElement(Vocabulary.BreakLabel, Pop())
                            : null));

    #region Switch
    /// <inheritdoc/>
    protected override Expression VisitSwitch(SwitchExpression node)
        => GenericVisit(
            node,
            base.VisitSwitch,
            (n, x) =>
            {
                var @default = n.DefaultBody is not null ? new JElement(Vocabulary.DefaultCase, PopWrappedElement()) : (JElement?)null;    // the body of the default case
                var cases = new JElement(Vocabulary.Cases, PopElementsValues(n.Cases.Count));                       // the cases
                var value = new JElement(Vocabulary.Value, PopWrappedElement());  // the value to switch on
                var comparison = n.Comparison is not null ? new JElement(Vocabulary.Comparison, VisitMemberInfo(n.Comparison)) : (JElement?)null; // get the non-default comparison method

                x.Add(
                    value,
                    comparison,
                    cases,
                    @default);
            });

    /// <inheritdoc/>
    protected override SwitchCase VisitSwitchCase(SwitchCase node)
    {
        using var _ = OutputDebugScope(nameof(SwitchCase));
        var n = base.VisitSwitchCase(node);

        var caseExpression = new JElement(Vocabulary.Body, PopWrappedElement());
        var caseValues = new JElement(Vocabulary.CaseValues, PopWrappedElements(n.TestValues.Count));

        _elements.Push(
            new JElement(
                    Vocabulary.Case,
                        caseValues,
                        caseExpression));

        return n;
    }
    #endregion

    #region try-catch
    /// <inheritdoc/>
    protected override Expression VisitTry(TryExpression node)
        => GenericVisit(
            node,
            base.VisitTry,
            (n, x) =>
            {
                var @finally = n.Finally is not null ? new JElement(Vocabulary.Finally, PopWrappedElement()) : (JElement?)null;
                var @catch = n.Fault is not null ? new JElement(Vocabulary.Fault, PopWrappedElement()) : (JElement?)null;
                var catches = new JElement(Vocabulary.Catches, PopElementsValues(n.Handlers?.Count ?? 0));
                var @try = new JElement(Vocabulary.Body, PopWrappedElement());

                x.Add(
                    @try,
                    catches,
                    @catch,
                    @finally);
            });

    /// <inheritdoc/>
    protected override CatchBlock VisitCatchBlock(CatchBlock node)
    {
        using var _ = OutputDebugScope(nameof(CatchBlock));
        var catchBlock = base.VisitCatchBlock(node);

        var body = new JElement(Vocabulary.Body, PopWrappedElement());
        var filter = catchBlock.Filter is not null ? new JElement(Vocabulary.Filter, PopWrappedElement()) : (JElement?)null;
        var exception = catchBlock.Variable is not null ? new JElement(Vocabulary.Exception, PopElementValue()) : (JElement?)null;

        _elements.Push(
            new JElement(
                    Vocabulary.Catch,
                        PropertyType(catchBlock.Test),
                        exception,
                        filter,
                        body));

        return catchBlock;
    }
    #endregion
}
