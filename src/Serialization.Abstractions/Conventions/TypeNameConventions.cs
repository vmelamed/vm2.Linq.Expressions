namespace vm2.Linq.Expressions.Serialization;

/// <summary>
/// Specifies how to transform type names.
/// </summary>
public enum TypeNameConventions
{
    /// <summary>
    /// The full name of the type, e.g. "System.Int32".
    /// </summary>
    FullName,

    /// <summary>
    /// The short name of the type, e.g. "Int32".
    /// </summary>
    Name,

    /// <summary>
    /// The assembly-qualified name of the type.
    /// </summary>
    AssemblyQualifiedName,
}
