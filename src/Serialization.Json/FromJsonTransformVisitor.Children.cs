namespace vm2.Linq.Expressions.Serialization.Json;

/// <summary>
/// Class that visits the nodes of a JSON node to produce a LINQ expression tree.
/// </summary>
public partial class FromJsonTransformVisitor
{
    #region Visiting children and grandchildren helpers
    /// <summary>
    /// Tries to visits the JsonObject property value with property name <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="e">The JSON element which property's JsonObject to visit.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>Expression?</returns>
    [ExcludeFromCodeCoverage]
    protected Expression? TryVisitChild(JElement e, string propertyName)
        => e.TryGetElement(out var child, propertyName)
            && child is not null
            && child.Value.Node is JsonObject
                ? Visit(child.Value)
                : null;

    /// <summary>
    /// Visits the JsonObject property value with property name <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="e">The JSON element which property's JsonObject to visit.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>Expression.</returns>
    /// <exception cref="SerializationException"/>
    [ExcludeFromCodeCoverage]
    protected Expression VisitChild(JElement e, string propertyName)
        => Visit(e.GetElement(propertyName));

    /// <summary>
    /// Visits the first JsonObject property value regardless of its name.
    /// </summary>
    /// <param name="e">The element whose JsonObject must be visited.</param>
    /// <returns>Expression.</returns>
    /// <exception cref="SerializationException"/>
    public Expression VisitFirstChild(JElement e)
        => e.TryGetFirstElement(out var child) && child is not null
                ? Visit(child.Value)
                : e.ThrowSerializationException<Expression>($"Could not find a child of 'JElement'");

    /// <summary>
    /// Visits the first child of the child with name <paramref name="propertyName"/>.
    /// </summary>
    /// <param name="e">The JSON element which grandchild to visit.</param>
    /// <param name="propertyName">Name of the parent property.</param>
    /// <returns>Expression?</returns>
    protected Expression? TryVisitFirstGrandChildOf(
        JElement e,
        string propertyName)
        => e.TryGetElement(out var child, propertyName)
            && child is not null
            && child.Value.Node is JsonObject
            && child.Value.TryGetFirstElement(out var grandchild)
            && grandchild is not null
                ? Visit(grandchild.Value)
                : null;

    /// <summary>
    /// Visits the element at e.child-name/[0]
    /// </summary>
    /// <param name="e">The JSON element which value's first JsonObject to visit.</param>
    /// <param name="childName">Name of the jsObj.</param>
    /// <returns>Expression.</returns>
    /// <exception cref="SerializationException"/>
    protected Expression VisitFirstGrandChildOf(
        JElement e,
        string childName)
        => VisitFirstChild(e.GetElement(childName));

    /// <summary>
    /// Visits the grandchild <paramref name="grandChildPropertyName" /> of the child <paramref name="childPropertyName" />.
    /// </summary>
    /// <param name="e">The JSON element which grandchild to visit.</param>
    /// <param name="childPropertyName">Name of the child property.</param>
    /// <param name="grandChildPropertyName">Name of the grand child property.</param>
    /// <returns>Expression?</returns>
    [ExcludeFromCodeCoverage]
    protected Expression? TryVisitGrandchild(
        JElement e,
        string childPropertyName,
        string grandChildPropertyName)
        => e.TryGetElement(out var child, childPropertyName)
            && child is not null
            && child.Value.Node is JsonObject
            && child.Value.TryGetElement(out var grandchild, grandChildPropertyName)
            && grandchild is not null
            && grandchild.Value.Node is JsonObject
                ? Visit(grandchild.Value)
                : null;

    /// <summary>
    /// Visits the grandchild <paramref name="grandChildPropertyName" /> of the child <paramref name="childPropertyName" />.
    /// </summary>
    /// <param name="e">The JSON element which grandchild to visit.</param>
    /// <param name="childPropertyName">Name of the child property.</param>
    /// <param name="grandChildPropertyName">Name of the grand child property.</param>
    /// <returns>Expression.</returns>
    /// <exception cref="SerializationException"/>
    [ExcludeFromCodeCoverage]
    protected Expression VisitGrandchild(
        JElement e,
        string childPropertyName,
        string grandChildPropertyName)
        => Visit(
                e.GetElement(childPropertyName)
                 .GetElement(grandChildPropertyName));

    /// <summary>
    /// Visits the items of an array from the element <paramref name="e"/> with name <paramref name="arrayName"/>
    /// producing a sequence of results of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the result from visiting an array item, e.g. <see cref="Expression"/>.</typeparam>
    /// <param name="e">The element that has the array in a property.</param>
    /// <param name="arrayName">Name of the array property.</param>
    /// <param name="visitor">The visitor function.</param>
    /// <returns>IEnumerable&lt;T&gt;.</returns>
    protected static IEnumerable<T>? TryVisitArray<T>(
        JElement e,
        string arrayName,
        Func<JElement, T> visitor)
        => e.TryGetArray(out var array, arrayName) && array is not null
                ? array.Select((e, i) => visitor(new($"item{i}", e)))
                : null;

    /// <summary>
    /// Visits the items of an array from the element <paramref name="e"/> with name <paramref name="arrayName"/>
    /// producing a sequence of results of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the result from visiting an array item, e.g. <see cref="Expression"/>.</typeparam>
    /// <param name="e">The element that has the array in a property.</param>
    /// <param name="arrayName">Name of the array property.</param>
    /// <param name="visitor">The visitor function.</param>
    /// <returns>IEnumerable&lt;T&gt;.</returns>
    protected static IEnumerable<T> VisitArray<T>(
        JElement e,
        string arrayName,
        Func<JElement, T> visitor)
        => e.GetArray(arrayName).Select((e, i) => visitor(new($"item{i}", e)));
    #endregion
}
