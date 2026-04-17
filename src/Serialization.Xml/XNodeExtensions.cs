namespace vm2.Linq.Expressions.Serialization.Xml;

/// <summary>
/// Class XNodeExtensions defines extension methods to XElement-s.
/// </summary>
public static class XNodeExtensions
{
    /// <summary>
    /// Determines whether the specified element is <c>null</c> from the attribute `i:nil="true"`.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> if the specified element is nil; otherwise, <c>false</c>.</returns>
    public static bool IsNil(this XElement element)
        => element.Attribute(AttributeNames.Nil)?.Value is string v && XmlConvert.ToBoolean(v);

    /// <summary>
    /// Translates an element's name to the enum ExpressionType.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The <see cref="ExpressionType"/> represented by the element.</returns>
    public static ExpressionType ExpressionType(this XElement element)
        => Enum.Parse<ExpressionType>(element.Name.LocalName, true);

    /// <summary>
    /// Tries to gets the child node with index <paramref name="childIndex" /> of the element <paramref name="element" />.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="childIndex">The index of the child.</param>
    /// <param name="child">The output child.</param>
    /// <returns><c>true</c> if the child was successfully obtained, <c>false</c> otherwise.</returns>
    public static bool TryGetChild(this XElement element, int childIndex, out XElement? child)
    {
        child = element.Elements().Skip(childIndex).FirstOrDefault();
        return child is not null;
    }

    /// <summary>
    /// Gets the child node with index <paramref name="childIndex"/> of the element <paramref name="element"/>.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="childIndex">The index of the child.</param>
    /// <returns>The <see cref="XElement" /> of the child element.</returns>
    /// <exception cref="SerializationException"/>
    public static XElement GetChild(this XElement element, int childIndex = 0)
        => element.TryGetChild(childIndex, out var child) && child is not null
                ? child
                : throw new SerializationException($"The parent element {element.Name} does not have a child with index {childIndex}.");

    /// <summary>
    /// Tries to get the child node with name <paramref name="childName"/> of the element <paramref name="element"/>.
    /// </summary>
    /// <param name="element">The parent element.</param>
    /// <param name="childName">The local name of the child.</param>
    /// <param name="child">The output child.</param>
    /// <returns><c>true</c> if the child was successfully obtained, <c>false</c> otherwise.</returns>
    public static bool TryGetChild(this XElement element, string childName, out XElement? child)
        => (child = element.Element(Namespaces.Exs + childName)) is not null;

    /// <summary>
    /// Gets the child node with name <paramref name="childName"/> of the element <paramref name="element"/>.
    /// </summary>
    /// <param name="element">The parent element.</param>
    /// <param name="childName">The local name of the child.</param>
    /// <returns>The <see cref="XElement" /> of the child element.</returns>
    /// <exception cref="SerializationException"/>
    public static XElement GetChild(this XElement element, string childName)
        => element.TryGetChild(childName, out var child) && child is not null
                ? child
                : throw new SerializationException($"The parent element {element.Name} does not have a child with name {childName}.");

    /// <summary>
    /// Tries to get the name of the element from attribute <see cref="AttributeNames.Name" />
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="name">The name.</param>
    /// <returns><c>true</c> if the specified element is nil; otherwise, <c>false</c>.</returns>
    public static bool TryGetName(this XElement element, out string? name)
    {
        name = null;
        if (element.Attribute(AttributeNames.Name)?.Value is not string v)
            return false;

        name = v;
        return true;
    }

    /// <summary>
    /// Gets the name of the element from attribute <see cref="AttributeNames.Name"/>
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> if the specified element is nil; otherwise, <c>false</c>.</returns>
    [ExcludeFromCodeCoverage]
    public static string GetName(this XElement element)
        => TryGetName(element, out var name)
                ? name ?? throw new InternalTransformErrorException($"Could not get the name attribute of the element `{element.Name}`.")
                : throw new SerializationException($"Could not get the name attribute of the element `{element.Name}`.");

    /// <summary>
    /// Tries to get the length of the sub-elements in the element from attribute <see cref="AttributeNames.Length" />
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="length">The length.</param>
    /// <returns><c>true</c> if the specified element is nil; otherwise, <c>false</c>.</returns>
    public static bool TryGetLength(this XElement element, out int length)
    {
        length = 0;

        if (element.Attribute(AttributeNames.Length)?.Value is not string v)
            return false;

        length = XmlConvert.ToInt32(v);
        return true;
    }

    /// <summary>
    /// Gets the length of the sub-elements in the element from attribute <see cref="AttributeNames.Length"/>
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns><c>true</c> if the specified element is nil; otherwise, <c>false</c>.</returns>
    /// <exception cref="SerializationException"/>
    [ExcludeFromCodeCoverage]
    public static int GetLength(this XElement element)
        => TryGetLength(element, out var length)
                ? length
                : throw new SerializationException($"Could not get the length attribute of the element `{element.Name}`.");

    /// <summary>
    /// Tries to get the .NET type of the element from its attribute (default "type").
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="type">The type.</param>
    /// <param name="attributeName">Name of the attribute (if null - defaults to <see cref="AttributeNames.Type"/>).</param>
    /// <returns><c>true</c> if getting the type was successful; otherwise, <c>false</c>.</returns>
    public static bool TryGetTypeFromAttribute(this XElement element, out Type? type, XName? attributeName = null)
    {
        type = null;

        var typeName = element.Attribute(attributeName ?? AttributeNames.Type)?.Value;

        if (typeName is null)
            return false;

        return Vocabulary.NamesToTypes.TryGetValue(typeName, out type) ||
               (type = Type.GetType(typeName)) is not null;
    }

    /// <summary>
    /// Gets the .NET type of the element only from its attribute (default "type"). If not found - throws Exception.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute (if null - defaults to <see cref="AttributeNames.Type"/>).</param>
    /// <returns>The <see cref="Type"/>  if getting the type was successful; otherwise, <c>false</c>.</returns>
    [ExcludeFromCodeCoverage]
    public static Type GetTypeFromAttribute(this XElement element, XName? attributeName = null)
        => element.TryGetElementType(out var type, attributeName ?? AttributeNames.Type)
                ? type!
                : throw new SerializationException($"Could not get the .NET type of element `{element.Name}`.");

    /// <summary>
    /// Tries to get the name of the type of an element
    /// <list type="bullet">
    /// <item>either from its attribute "type".</item>
    /// <item>or from the name of the element</item>
    /// </list>
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="name">The name.</param>
    /// <param name="attributeName">Name of the attribute (if null - defaults to <see cref="AttributeNames.Type"/>).</param>
    /// <returns><c>true</c> if getting the type was successful; otherwise, <c>false</c>.</returns>
    public static bool TryGetTypeName(this XElement element, out string name, XName? attributeName = null)
    {
        name = "";

        var nm = element.Attribute(attributeName ?? AttributeNames.Type)?.Value ??
                    (Vocabulary.NamesToTypes.ContainsKey(element.Name.LocalName)
                            ? element.Name.LocalName
                            : null);

        if (string.IsNullOrWhiteSpace(nm))
            return false;

        name = nm;
        return true;
    }

    /// <summary>
    /// Gets the name of the type of an element
    /// <list type="bullet">
    /// <item>either from its attribute "type".</item>
    /// <item>or from the name of the element</item>
    /// </list>
    /// If it fails, throws exception.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute (if null - defaults to <see cref="AttributeNames.Type"/>).</param>
    /// <returns><c>true</c> if getting the type was successful; otherwise, <c>false</c>.</returns>
    /// <exception cref="SerializationException"/>
    [ExcludeFromCodeCoverage]
    public static string GetTypeName(this XElement element, XName? attributeName = null)
        => element.TryGetTypeName(out var name, attributeName) && name is not null
                        ? name
                        : throw new SerializationException($"Could not get the type name of the element `{element.Name}`.");

    /// <summary>
    /// Tries to get the .NET type of the element
    /// <list type="bullet">
    /// <item>either from its attribute "type".</item>
    /// <item>or from the name of the element</item>
    /// </list>
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="type">The type.</param>
    /// <param name="attributeName">Name of the attribute (if null - defaults to <see cref="AttributeNames.Type"/>).</param>
    /// <returns><c>true</c> if getting the type was successful; otherwise, <c>false</c>.</returns>
    public static bool TryGetElementType(this XElement element, out Type? type, XName? attributeName = null)
    {
        type = null;

        return element.TryGetTypeFromAttribute(out type, attributeName) ||
               Vocabulary.NamesToTypes.TryGetValue(element.Name.LocalName, out type);
    }

    /// <summary>
    /// Tries to get the .NET type of the element
    /// <list type="bullet">
    /// <item>either from its attribute "type".</item>
    /// <item>or from the name of the element</item>
    /// </list>
    /// </summary>
    /// <param name="element">The element.</param>
    /// <param name="attributeName">Name of the attribute (if null - defaults to <see cref="AttributeNames.Type"/>).</param>
    /// <returns>The <see cref="Type"/>  if getting the type was successful; otherwise, <c>false</c>.</returns>
    public static Type GetElementType(this XElement element, XName? attributeName = null)
        => element.TryGetElementType(out var type, attributeName)
                ? type!
                : throw new SerializationException($"Could not get the .NET type of the element `{element.Name}`.");

}
