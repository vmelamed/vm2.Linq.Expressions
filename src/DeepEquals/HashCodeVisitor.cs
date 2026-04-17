namespace vm2.Linq.Expressions.DeepEquals;

/// <summary>
/// A visitor that computes a hash code for an expression tree.
/// Only overrides for node types with scalar (non-child) properties are needed —
/// the base <see cref="ExpressionVisitor"/> handles all child traversal.
/// </summary>
public sealed class HashCodeVisitor : ExpressionVisitor
{
    HashCode _hc = new();

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(node))]
    public override Expression? Visit(Expression? node)
    {
        if (node is null)
            return null;

        _hc.Add(node.NodeType);
        _hc.Add(node.Type.GetHashCode());

        return base.Visit(node);
    }

    /// <summary>
    /// Computes the hash code for the current expression.
    /// </summary>
    /// <returns>An integer representing the hash code of the current expression.</returns>
    public int ToHashCode() => _hc.ToHashCode();

    /// <inheritdoc/>
    protected override Expression VisitConstant(ConstantExpression node)
    {
        _hc.Add(node.Value is null);
        if (node.Value is not null)
            _hc.Add(node.Value);
        return base.VisitConstant(node);
    }

    /// <inheritdoc/>
    protected override Expression VisitUnary(UnaryExpression node)
    {
        _hc.Add(node.IsLifted);
        _hc.Add(node.IsLiftedToNull);
        VisitMemberInfo(node.Method);
        return base.VisitUnary(node);
    }

    /// <inheritdoc/>
    protected override Expression VisitTypeBinary(TypeBinaryExpression node)
    {
        _hc.Add(node.TypeOperand.GetHashCode());
        return base.VisitTypeBinary(node);
    }

    /// <inheritdoc/>
    protected override Expression VisitBinary(BinaryExpression node)
    {
        _hc.Add(node.IsLifted);
        _hc.Add(node.IsLiftedToNull);
        VisitMemberInfo(node.Method);
        return base.VisitBinary(node);
    }

    /// <inheritdoc/>
    protected override CatchBlock VisitCatchBlock(CatchBlock node)
    {
        _hc.Add(node.Test.GetHashCode());
        return base.VisitCatchBlock(node);
    }

    /// <inheritdoc/>
    protected override ElementInit VisitElementInit(ElementInit node)
    {
        VisitMemberInfo(node.AddMethod);
        return base.VisitElementInit(node);
    }

    /// <inheritdoc/>
    protected override Expression VisitIndex(IndexExpression node)
    {
        VisitMemberInfo(node.Indexer);
        return base.VisitIndex(node);
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(node))]
    protected override LabelTarget? VisitLabelTarget(LabelTarget? node)
    {
        if (node is not null)
        {
            _hc.Add(node.Name ?? string.Empty);
            _hc.Add(node.Type?.GetHashCode() ?? 0);
        }
        return base.VisitLabelTarget(node);
    }

    /// <inheritdoc/>
    protected override Expression VisitLambda<T>(Expression<T> node)
    {
        _hc.Add(node.Name ?? string.Empty);
        return base.VisitLambda(node);
    }

    /// <inheritdoc/>
    protected override Expression VisitMember(MemberExpression node)
    {
        VisitMemberInfo(node.Member);
        return base.VisitMember(node);
    }

    /// <inheritdoc/>
    protected override MemberBinding VisitMemberBinding(MemberBinding node)
    {
        _hc.Add(node.BindingType.GetHashCode());
        VisitMemberInfo(node.Member);
        return base.VisitMemberBinding(node);
    }

    /// <inheritdoc/>
    protected override Expression VisitMethodCall(MethodCallExpression node)
    {
        VisitMemberInfo(node.Method);
        return base.VisitMethodCall(node);
    }

    /// <inheritdoc/>
    protected override Expression VisitNew(NewExpression node)
    {
        VisitMemberInfo(node.Constructor);
        return base.VisitNew(node);
    }

    /// <inheritdoc/>
    protected override Expression VisitParameter(ParameterExpression node)
    {
        _hc.Add(node.Name ?? string.Empty);
        _hc.Add(node.IsByRef.GetHashCode());
        return base.VisitParameter(node);
    }

    /// <inheritdoc/>
    protected override Expression VisitSwitch(SwitchExpression node)
    {
        VisitMemberInfo(node.Comparison);
        return base.VisitSwitch(node);
    }

    void VisitMemberInfo(MemberInfo? member)
    {
        if (member is null)
            return;
        _hc.Add(member.GetHashCode());
    }
}
