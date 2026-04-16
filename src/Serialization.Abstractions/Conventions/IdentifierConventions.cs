namespace vm2.Linq.Expressions.Serialization;

/// <summary>
/// Specifies how to transform C# identifiers to document names.
/// </summary>
public enum IdentifierConventions
{
    /// <summary>
    /// Preserve identifiers as-is: ThisIsName → ThisIsName
    /// </summary>
    Preserve,

    /// <summary>
    /// Transform identifiers to camel-case: ThisIsName → thisIsName
    /// </summary>
    Camel,

    /// <summary>
    /// Transform identifiers to Pascal-case: thisIsName → ThisIsName
    /// </summary>
    Pascal,

    /// <summary>
    /// Transform identifiers to snake_lower_case: ThisIsName → this_is_name
    /// </summary>
    SnakeLower,

    /// <summary>
    /// Transform identifiers to SNAKE_UPPER_CASE: ThisIsName → THIS_IS_NAME
    /// </summary>
    SnakeUpper,

    /// <summary>
    /// Transform identifiers to kebab-lower-case: ThisIsName → this-is-name
    /// </summary>
    KebabLower,

    /// <summary>
    /// Transform identifiers to KEBAB-UPPER-CASE: ThisIsName → THIS-IS-NAME
    /// </summary>
    KebabUpper,
}
