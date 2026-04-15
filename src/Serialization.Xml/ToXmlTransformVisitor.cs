namespace vm2.Linq.Expressions.Serialization.Xml;

/// <summary>
/// Class ToXmlTransformVisitor.
/// Implements the <see cref="ExpressionTransformVisitor{XElement}" />
/// </summary>
/// <seealso cref="ExpressionTransformVisitor{XElement}" />
public partial class ToXmlTransformVisitor(XmlOptions options) : ExpressionTransformVisitor<XElement>
{
    ToXmlDataTransform _dataTransform = new(options);

    /// <summary>
    /// Gets a properly named n corresponding to the current value n.
    /// </summary>
    /// <param name="node">The currently visited value n from the value tree.</param>
    /// <returns>TDocument.</returns>
    protected override XElement GetEmptyNode(Expression node)
        => new(
            Namespaces.Exs + Transform.Identifier(node.NodeType.ToString(), IdentifierConventions.Camel),
            node switch {
                // omit the type for expressions where their element says it all
                ConstantExpression => null,
                ListInitExpression => null,
                NewExpression => null,
                NewArrayExpression => null,
                LabelExpression => null,
                // do not omit the void return type for these nodes:
                LambdaExpression n => new(AttributeNames.Type, Transform.TypeName(n.ReturnType)),
                MethodCallExpression n => new(AttributeNames.Type, Transform.TypeName(n.Method.ReturnType)),
                InvocationExpression n => new(AttributeNames.Type, Transform.TypeName(n.Expression.Type)),
                // for the rest: add attribute type if it is not void:
                _ => AttributeType(node),
            });

    /// <inheritdoc/>
    protected override Expression VisitConstant(ConstantExpression node)
    {
        try
        {
            return GenericVisit(
                     node,
                     base.VisitConstant,
                     (n, x) =>
                     {
                         // options.AddComment(x, n);
                         x.Add(
                            options.TypeComment(n.Type),
                            _dataTransform.TransformNode(n));
                     });
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
        var n = (ParameterExpression)base.VisitParameter(node);

        using var _ = OutputDebugScope(node.NodeType.ToString());

        _elements.Push(GetParameter(node));
        return node;
    }

    #region Lambda
    IEnumerable<XElement> VisitParameterDefinitionList(ReadOnlyCollection<ParameterExpression> parameterList)
=> parameterList.Select(p => !IsDefined(p)
                                ? GetParameter(p)
                                : throw new InternalTransformErrorException($"Parameter with a name `{p.Name}` is already defined."));

    /// <inheritdoc/>
    protected override Expression VisitLambda<T>(Expression<T> node)
        => GenericVisit(
            node,
            base.VisitLambda,
            (n, x) => x.Add(
                        AttributeDelegateType(n.Type),
                        new XElement(ElementNames.Parameters, Pop(n.Parameters.Count)),
                        new XElement(ElementNames.Body, Pop()),
                        AttributeName(node.Name),
                        node.TailCall ? new XAttribute(AttributeNames.TailCall, node.TailCall) : null));
    #endregion

    /// <inheritdoc/>
    protected override Expression VisitUnary(UnaryExpression node)
        => GenericVisit(
            node,
            base.VisitUnary,
            (n, x) => x.Add(
                        Pop(),    // pop the operand
                        n.IsLifted ? new XAttribute(AttributeNames.IsLifted, true) : null,
                        n.IsLiftedToNull ? new XAttribute(AttributeNames.IsLiftedToNull, true) : null,
                        VisitMethodInfo(n)));

    /// <inheritdoc/>
    protected override Expression VisitBinary(BinaryExpression node)
        => GenericVisit(
            node,
            base.VisitBinary,
            (n, x) =>
            {
                var right   = Pop();
                var convert = n.Conversion is not null ? Pop() : null;
                var left    = Pop();

                convert?.Name = ElementNames.Convert;

                x.Add(
                        left,
                        right,
                        convert,
                        n.IsLifted ? new XAttribute(AttributeNames.IsLifted, true) : null,
                        n.IsLiftedToNull ? new XAttribute(AttributeNames.IsLiftedToNull, true) : null,
                        VisitMethodInfo(n));
            });

    /// <inheritdoc/>
    protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        => GenericVisit(
            node,
            base.VisitTypeBinary,
            (n, x) => x.Add(new XAttribute(AttributeNames.TypeOperand, Transform.TypeName(n.TypeOperand)), Pop()));  // pop the value operand

    /// <inheritdoc/>
    protected override Expression VisitIndex(IndexExpression node)
        => GenericVisit(
            node,
            base.VisitIndex,
            (n, x) =>
            {
                var indexes = new XElement(ElementNames.Indexes, Pop(n.Arguments.Count));

                x.Add(
                    Pop(),    // pop the indexes
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
                        ? new XElement(
                                ElementNames.Object,
                                Pop())  // pop the expression/value that will give the object whose requested member is being accessed
                        : null,         // or null if the property/field is static
                    VisitMemberInfo(n.Member)));

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression node)
        => GenericVisit(
            node,
            base.VisitMethodCall,
            (n, x) =>
            {
                var arguments = new XElement(ElementNames.Arguments, Pop(n.Arguments.Count));   // pop the argument expressions
                var instance = n.Object!=null ? Pop() : null; // pop the object
                var method = VisitMemberInfo(n.Method);

                x.Add(
                    instance is not null
                        ? new XElement(ElementNames.Object, instance)
                        : null,
                    method,
                    arguments);
            });

    /// <inheritdoc/>
    protected override Expression VisitInvocation(InvocationExpression node)
        => GenericVisit(
            node,
            base.VisitInvocation,
            (n, x) =>
            {
                var arguments = new XElement(ElementNames.Arguments, Pop(n.Arguments.Count));   // pop the argument expressions

                x.Add(
                    Pop(),    // pop the delegate or lambda
                    arguments);
            });

    /// <inheritdoc/>
    protected override Expression VisitBlock(BlockExpression node)
        => GenericVisit(
            node,
            base.VisitBlock,
            (n, x) =>
            {
                var variables = node.Variables.Count is > 0
                                        ? new XElement(ElementNames.Variables, Pop(node.Variables.Count))
                                        : null;
                var body = Pop(node.Expressions.Count);

                x.Add(variables, body);
            });

    /// <inheritdoc/>
    protected override Expression VisitConditional(ConditionalExpression node)
        => GenericVisit(
            node,
            base.VisitConditional,
            (n, x) =>
            {
                var op3 = n.IfFalse is not null ? Pop() : null;
                var op2 = n.IfTrue  is not null ? Pop() : null;
                var op1 = Pop();

                Debug.Assert(n.Type != null, "The value n's type is null - remove the default type value of typeof(void) below.");
                x.Add(op1, op2, op3);
            });

    /// <inheritdoc/>
    protected override Expression VisitNew(NewExpression node)
        => GenericVisit(
            node,
            base.VisitNew,
            (n, x) => x.Add(
                        VisitMemberInfo(n.Constructor!),
                        new XElement(ElementNames.Arguments, Pop(n.Arguments.Count)),   // pop the c-tor arguments
                        n.Members is not null
                            ? new XElement(ElementNames.Members, n.Members.Select(m => VisitMemberInfo(m)))
                            : null));

    /// <inheritdoc/>
    protected override Expression VisitNewArray(NewArrayExpression node)
        => GenericVisit(
            node,
            base.VisitNewArray,
            (n, x) =>
            {
                var elemType = n.Type.GetElementType() ?? throw new InternalTransformErrorException($"Could not get the type of the element.");

                x.Add();

                var expressions = Pop(n.Expressions.Count);

                x.Add(
                    new XAttribute(AttributeNames.Type, Transform.TypeName(elemType)),
                    new XElement(
                            n.NodeType == ExpressionType.NewArrayInit
                                ? ElementNames.ArrayElements
                                : ElementNames.Bounds,
                                    expressions));
            });

    #region Member initialization
    /// <inheritdoc/>
    protected override Expression VisitMemberInit(MemberInitExpression node)
        => GenericVisit(
            node,
            base.VisitMemberInit,
            (n, x) =>
            {
                var bindings = Pop(n.Bindings.Count).ToList();     // pop the expressions to assign to members
                x.Add(
                    Pop(),        // the new value
                    new XElement(ElementNames.Bindings, bindings));
            });

    ///// <inheritdoc/>
    //protected override MemberBinding VisitMemberBinding(MemberBinding node) => base.VisitMemberBinding(node);
    // the base is doing exactly what we need - dispatch the call to the concrete binding below:

    /// <inheritdoc/>
    protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
    {
        using var _ = OutputDebugScope(nameof(MemberAssignment));
        var binding = base.VisitMemberAssignment(node);

        _elements.Push(
            new XElement(
                    ElementNames.AssignmentBinding,
                        VisitMemberInfo(node.Member),
                        Pop()));  // pop the value to assign

        return binding;
    }

    /// <inheritdoc/>
    protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
    {
        using var _ = OutputDebugScope(nameof(MemberMemberBinding));
        var binding = base.VisitMemberMemberBinding(node);

        _elements.Push(
            new XElement(
                    ElementNames.MemberMemberBinding,
                        VisitMemberInfo(node.Member),
                        Pop(node.Bindings.Count)));

        return binding;
    }
    #endregion

    #region new List with initializers
    /// <inheritdoc/>
    protected override Expression VisitListInit(ListInitExpression node)
        => GenericVisit(
            node,
            base.VisitListInit,
            (n, x) =>
            {
                var initializers = Pop(n.Initializers.Count);

                x.Add(
                    Pop(),            // the new n
                    new XElement(ElementNames.Initializers, initializers));                // the elementsInit n
            });

    /// <inheritdoc/>
    protected override ElementInit VisitElementInit(ElementInit node)
    {
        using var _ = OutputDebugScope(nameof(ElementInit));
        var elementInit = base.VisitElementInit(node);

        _elements.Push(
            new XElement(
                    ElementNames.ElementInit,
                        VisitMemberInfo(node.AddMethod),
                        new XElement(ElementNames.Arguments, Pop(node.Arguments.Count))));  // pop the elements init expressions

        return elementInit;
    }

    /// <inheritdoc/>
    protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
    {
        using var _ = OutputDebugScope(nameof(MemberListBinding));
        var binding = base.VisitMemberListBinding(node);

        _elements.Push(
            new XElement(
                    ElementNames.MemberListBinding,
                        VisitMemberInfo(node.Member),
                        Pop(node.Initializers.Count)));

        return binding;
    }
    #endregion

    #region Label, Target and Goto
    /// <inheritdoc/>
    protected override LabelTarget? VisitLabelTarget(LabelTarget? node)
    {
        using var _ = OutputDebugScope(nameof(LabelTarget));
        var n = base.VisitLabelTarget(node);

        if (n is not null)
            _elements.Push(new XElement(GetLabelTarget(n)));

        return n;
    }

    /// <inheritdoc/>
    protected override Expression VisitLabel(LabelExpression node)
        => GenericVisit(
            node,
            base.VisitLabel,
            (n, x) =>
            {
                var value = n.DefaultValue is not null
                                ? Pop()   // pop the default result value if present
                                : null;

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
                var value = n.Value is not null ? Pop() : null;

                // VisitLabelTarget adds an attribute with name id - fixup: remove it and put attribute with name idRef instead
                XElement targetElement = new(Pop());

                x.Add(
                    targetElement,
                    value,
                    new XAttribute(AttributeNames.Kind, Transform.Identifier(node.Kind.ToString(), IdentifierConventions.Camel)));
            });

    #endregion

    /// <inheritdoc/>
    protected override Expression VisitLoop(LoopExpression node)
        => GenericVisit(
            node,
            base.VisitLoop,
            (n, x) => x.Add(
                        Pop(),
                        n.ContinueLabel is not null
                            ? new XElement(ElementNames.ContinueLabel, Pop())
                            : null,
                        n.BreakLabel is not null
                            ? new XElement(ElementNames.BreakLabel, Pop())
                            : null));

    #region Switch statement
    /// <inheritdoc/>
    protected override Expression VisitSwitch(SwitchExpression node)
        => GenericVisit(
            node,
            base.VisitSwitch,
            (n, x) =>
            {
                var comparison = n.Comparison != null                // get the non-default comparison method
                                        ? VisitMemberInfo(n.Comparison)
                                        : null;
                var @default = n.DefaultBody != null                 // the body of the default case
                                        ? new XElement(ElementNames.DefaultCase, Pop())
                                        : null;
                var cases = Pop(n.Cases.Count);                     // the cases
                var value = Pop();                                  // the value to switch on

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
        var switchCase = base.VisitSwitchCase(node);
        var caseExpression = Pop();
        var tempElements = Pop(node.TestValues.Count);

        _elements.Push(new XElement(
                                ElementNames.Case,
                                    new XElement(ElementNames.CaseValues, tempElements),
                                    new XElement(ElementNames.Body, caseExpression)));

        return switchCase;
    }
    #endregion

    #region Try-catch
    /// <inheritdoc/>
    protected override Expression VisitTry(TryExpression node)
        => GenericVisit(
            node,
            base.VisitTry,
            (n, x) =>
            {
                var @finally = n.Finally!=null
                                ? new XElement(ElementNames.Finally, Pop())
                                : null;
                var @catch = n.Fault!=null
                                ? new XElement(ElementNames.Fault, Pop())
                                : null;
                var catches = Pop(n.Handlers?.Count ?? 0);

                var @try = Pop();

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
        // here we do not want the base.Visit to drive the immediate subexpressions - it visits them in an inconvenient order.
        // var catchBlock = base.VisitCatchBlock(node);

        XElement? exception = null;

        if (node.Variable is not null)
        {
            base.Visit(node.Variable);
            exception = new XElement(
                                ElementNames.Exception,
                                Pop().Attributes());
        }

        XElement? filter = null;

        if (node.Filter is not null)
        {
            base.Visit(node.Filter);
            filter = new XElement(
                                ElementNames.Filter,
                                Pop());
        }

        base.Visit(node.Body);

        var body = new XElement(
                            ElementNames.Body,
                            Pop());

        _elements.Push(
            new XElement(
                    ElementNames.Catch,
                    AttributeType(node.Test),
                    exception,
                    filter,
                    body));

        return node;
    }
    #endregion
}
