using System.Diagnostics.CodeAnalysis;

namespace vm2.Linq.Expressions.Serialization.Conventions;

public static partial class Transform
{
    /// <summary>
    /// Gets the type corresponding to a type name written in a document string.
    /// </summary>
    /// <param name="typeName">The name of the type.</param>
    /// <returns>The specified type, or <see langword="null"/> if <paramref name="typeName"/> is null or whitespace.</returns>
    [UnconditionalSuppressMessage("Trimming", "IL2057", Justification = "Type names come from serialized expression documents; callers accept the trim risk.")]
    public static Type? GetType(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            return null;

        if (Vocabulary.NamesToTypes.TryGetValue(typeName, out var type))
            return type;

        return Type.GetType(typeName, true, false);
    }

    /// <summary>
    /// Gets the document name for the given <paramref name="type"/>.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The type name string.</returns>
    public static string TypeName(Type type)
    {
        if (Vocabulary.TypesToNames.TryGetValue(type, out var typeName))
            return typeName;

        return type.AssemblyQualifiedName ?? type.FullName ?? type.Name;
    }

    /// <summary>
    /// Gets the document name for the given <paramref name="type"/> according to the specified <paramref name="convention"/>.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="convention">The naming convention.</param>
    /// <returns>The type name string.</returns>
    public static string TypeName(
        Type type,
        TypeNameConventions convention)
    {
        if (Vocabulary.TypesToNames.TryGetValue(type, out var typeName))
            return typeName;

        if (type.IsGenericType && !type.IsGenericTypeDefinition && convention != TypeNameConventions.AssemblyQualifiedName)
        {
            var genericName = TypeName(type.GetGenericTypeDefinition(), convention).Split('`')[0];
            var parameters  = string.Join(", ", type.GetGenericArguments().Select(t => TypeName(t, convention)));

            return $"{genericName}<{parameters}>";
        }

        return convention switch {
            TypeNameConventions.AssemblyQualifiedName => type.AssemblyQualifiedName ?? type.FullName ?? type.Name,
            TypeNameConventions.FullName => type.FullName ?? type.Name,
            TypeNameConventions.Name => type.Name,
            _ => throw new InternalTransformErrorException("Invalid TypeNameConventions value.")
        };
    }
}
