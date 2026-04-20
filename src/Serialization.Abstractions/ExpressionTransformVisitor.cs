namespace vm2.Linq.Expressions.Serialization;

/// <summary>
/// Base class for expression visitors that transform expression trees into document elements using a stack-based
/// reverse-Polish notation approach.
/// </summary>
/// <typeparam name="TElement">The type of the document nodes (e.g. <c>XElement</c> or <c>JsonObject</c>).</typeparam>
public abstract class ExpressionTransformVisitor<TElement> : ExpressionVisitor
{
    /// <summary>
    /// Stack of intermediate results. Elements are pushed as they are created and popped to be placed as operands
    /// into parent elements — as in reverse Polish notation.
    /// After a successful visit the stack should contain exactly one element: the root of the document.
    /// </summary>
    protected readonly Stack<TElement> _elements = new();

    /// <summary>
    /// Gets and removes the single top-level result element. Resets the visitor afterward.
    /// </summary>
    /// <exception cref="InternalTransformErrorException">More than one element remains on the stack.</exception>
    /// <exception cref="NoAvailableResultException">The stack is empty.</exception>
    public virtual TElement Result
    {
        get
        {
            if (_elements.Count > 1)
                throw new InternalTransformErrorException($"There must be exactly one element on the stack but there are {_elements.Count}.");
            if (_elements.Count < 1)
                throw new NoAvailableResultException();

            var element = _elements.Pop();
            Reset();

            return element;
        }
    }

    /// <summary>
    /// Pops one element from the stack.
    /// </summary>
    protected TElement Pop() => _elements.Pop();

    /// <summary>
    /// Pops a number of elements in the order they entered the stack (FIFO, not LIFO).
    /// </summary>
    /// <param name="numberOfExpressions">The number of elements to pop.</param>
    /// <returns>The elements in their original push order.</returns>
    protected IEnumerable<TElement> Pop(int numberOfExpressions)
    {
        Stack<TElement> tempElements = new(numberOfExpressions);

        for (var i = 0; i < numberOfExpressions; i++)
            tempElements.Push(_elements.Pop());

        return tempElements;
    }

#if DEBUG
    /// <inheritdoc/>
    public override Expression? Visit(Expression? node)
    {
        if (node is null)
            return null;

        using var _ = OutputDebugScope(node.NodeType.ToString());

        return base.Visit(node);
    }
#endif

    /// <summary>
    /// Invokes the base visitor, creates an empty document node, invokes the transform delegate, then pushes the result.
    /// </summary>
    /// <typeparam name="TExpression">The concrete expression type.</typeparam>
    /// <param name="node">The expression node to transform.</param>
    /// <param name="baseVisit">The base class visit method.</param>
    /// <param name="thisVisit">Delegate that populates the document node.</param>
    /// <returns>The (possibly reduced) expression.</returns>
    protected virtual Expression GenericVisit<TExpression>(
        TExpression node,
        Func<TExpression, Expression> baseVisit,
        Action<TExpression, TElement> thisVisit)
        where TExpression : Expression
    {
        var resNode = baseVisit(node)
                        ?? throw new InternalTransformErrorException($"The base visit of a {node.NodeType} node returned different node or null.");

        if (resNode is not TExpression n)
            return resNode;

        var x = GetEmptyNode(n);

        thisVisit(n, x);
        _elements.Push(x);

        return n;
    }

    /// <summary>
    /// Gets a properly named empty document node corresponding to the given expression node.
    /// </summary>
    /// <param name="node">The expression node.</param>
    /// <returns>An empty document element.</returns>
    protected abstract TElement GetEmptyNode(Expression node);

    /// <summary>
    /// Resets the visitor by clearing the element stack.
    /// </summary>
    protected virtual void Reset() => _elements.Clear();

    #region Not implemented:
    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    protected override Expression VisitDebugInfo(DebugInfoExpression node)
        => GenericVisit(
            node,
            base.VisitDebugInfo,
            (n, x) => throw new NotImplementedExpressionException(param: n.GetType().Name));

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    protected override Expression VisitDynamic(DynamicExpression node)
        => GenericVisit(
            node,
            base.VisitDynamic,
            (n, x) => throw new NotImplementedExpressionException(param: n.GetType().Name));

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        => GenericVisit(
            node,
            base.VisitRuntimeVariables,
            (n, x) => throw new NotImplementedExpressionException(param: n.GetType().Name));

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    protected override Expression VisitExtension(Expression node)
        => GenericVisit(
            node,
            base.VisitExtension,
            (n, x) => throw new NotImplementedExpressionException($"{nameof(ExpressionVisitor)}.{nameof(VisitExtension)}(Expression n)"));
    #endregion
}
