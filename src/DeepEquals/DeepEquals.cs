namespace vm2.Linq.Expressions.DeepEquals;

/// <summary>
/// Class ExpressionExtensions.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// Visits the left expression nodes and compares them with the nodes of the right expression.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <returns><c>true</c> if the expressions are equal, <c>false</c> otherwise.</returns>
    public static bool DeepEquals(this Expression left, Expression right)
    {
        var comparer = new DeepEqualsComparer();
        return comparer.Compare(left, right);
    }

    /// <summary>
    /// Visits the left expression nodes and compares them with the nodes of the right expression.
    /// </summary>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <param name="difference">Explains the difference.</param>
    /// <returns><c>true</c> if the expressions are equal, <c>false</c> otherwise.</returns>
    public static bool DeepEquals(this Expression left, Expression right, out string difference)
    {
        var comparer = new DeepEqualsComparer();
        var result = comparer.Compare(left, right);
        difference = comparer.Difference;
        return result;
    }

    /// <summary>
    /// Computes a hash code for the specified <see cref="Expression"/> by traversing its structure.
    /// </summary>
    /// <remarks>
    /// This method generates a hash code based on the structure of the expression tree, making it suitable for scenarios where
    /// structural equality of expressions is important. The hash code is not guaranteed to be stable across different versions
    /// of the framework or runtime environments.
    /// </remarks>
    /// <param name="expr">
    /// The <see cref="Expression"/> for which to compute the hash code. Cannot be <see langword="null"/>.
    /// </param>
    /// <returns>
    /// An integer representing the hash code of the expression's structure.
    /// </returns>
    public static int GetDeepHashCode(this Expression expr)
    {
        var visitor = new HashCodeVisitor();

        visitor.Visit(expr);

        return visitor.ToHashCode();
    }
}
