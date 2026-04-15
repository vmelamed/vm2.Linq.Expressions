namespace vm2.Linq.Expressions.Serialization;

/// <summary>
/// Specifies when to validate expression documents against a schema.
/// </summary>
public enum ValidateExpressionDocuments
{
    /// <summary>
    /// Never validate the expression documents.
    /// </summary>
    Never,

    /// <summary>
    /// Always validate the expression documents.
    /// </summary>
    Always,

    /// <summary>
    /// Validate the expression documents only if a schema is present.
    /// </summary>
    IfSchemaPresent,
}
