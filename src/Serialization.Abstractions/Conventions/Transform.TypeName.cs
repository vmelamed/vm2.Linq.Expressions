namespace vm2.Linq.Expressions.Serialization;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public static partial class Transform
{
    /// <summary>
    /// Gets the type corresponding to a type name written in a document string.
    /// </summary>
    /// <param name="typeName">The name of the type.</param>
    /// <returns>The specified type, or <see langword="null"/> if <paramref name="typeName"/> is null or whitespace.</returns>
    public static Type? GetType(string typeName)
        => string.IsNullOrWhiteSpace(typeName)
                ? null
                : Vocabulary.NamesToTypes.TryGetValue(typeName, out var type1)
                    ? type1
                    : Type.GetType(typeName, false, false) is Type type2
                        ? type2
                        : GetTypeFromFullName(typeName) is Type type3
                            ? type3
                            : null;

    // Resolves a type from a FullName-convention CLR bracket-notation string such as
    // "System.Collections.Generic.Dictionary`2[[System.Int32],[System.String]]"
    // by finding the generic type definition in loaded assemblies and then resolving each argument.
    static Type? GetTypeFromFullName(string typeName)
    {
        // Does it look like a constructed generic?  "SomeNs.Foo`N[...]"
        var bracketIdx = typeName.IndexOf('[');

        if (bracketIdx < 0 || !typeName.EndsWith(']'))
            return FindInLoadedAssemblies(typeName);

        var defName = typeName[..bracketIdx];                // e.g. "System.Collections.Generic.Dictionary`2"
        var argsStr = typeName[(bracketIdx + 1)..^1];        // e.g. "[System.Int32],[System.String]"
        var defType = FindInLoadedAssemblies(defName);

        if (defType is null)
            return null;

        if (!defType.IsGenericTypeDefinition)
            // Not a generic type definition; the brackets are array syntax — fall back to assembly search with the original name.
            return FindInLoadedAssemblies(typeName);

        var argNames = SplitArguments(argsStr);
        var argTypes = new Type[argNames.Length];

        for (var i = 0; i < argNames.Length; i++)
        {
            // Each argument is wrapped in [...]; strip brackets and whitespace without extra string allocations.
            var span = argNames[i].AsSpan().Trim();

            if (span.Length >= 2 && span[0] == '[' && span[^1] == ']')
                span = span[1..^1].Trim();

            var argType = GetType(new string(span));         // recursive, handles nested generics

            if (argType is null)
                return null;
            argTypes[i] = argType;
        }

        return defType.MakeGenericType(argTypes);
    }

    static Type? FindInLoadedAssemblies(string typeName)
    {
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            if (asm.GetType(typeName, false, false) is Type t)
                return t;

        return null;
    }

    // Splits top-level comma-separated type arguments in a string like "[A],[B`1[[C]]]" respecting bracket nesting.
    static string[] SplitArguments(string argsStr)
    {
        // Count top-level commas first so we can allocate the result array at the right size.
        var count = 1;
        var depth = 0;

        // Count the number of top-level commas to determine how many arguments there are.  We can't just Split(',') because of nested generic arguments.
        for (var i = 0; i < argsStr.Length; i++)
            if (argsStr[i] == '[')
                depth++;
            else
                if (argsStr[i] == ']')
                    depth--;
                else
                    if (argsStr[i] == ',' && depth == 0)
                        count++;

        // Now split the arguments at level 0 into the pre-allocated array args.
        var args  = new string[count];
        var idx   = 0;
        var start = 0;

        depth = 0;
        for (var i = 0; i < argsStr.Length; i++)
            if (argsStr[i] == '[')
                depth++;
            else
                if (argsStr[i] == ']')
                    depth--;
                else
                    if (argsStr[i] == ',' && depth == 0)
                    {
                        args[idx++] = argsStr[start..i];
                        start = i + 1;
                    }

        args[idx] = argsStr[start..];
        return args;
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
            if (convention == TypeNameConventions.FullName)
                // Build CLR bracket-notation so the result is parseable by Type.GetType() without version-sensitive assembly tokens.
                // Type arguments use CLR full names (e.g. System.Int32), NOT vocabulary aliases (e.g. "int"), because
                // the CLR generic type parser does not know about vocabulary aliases.
                // E.g.: Dictionary<int, int> is serialized as System.Collections.Generic.Dictionary`2[[System.Int32],[System.String]]
                return ClrFullName(type);

            // Name convention: use angle-bracket display form (not intended for round-tripping).
            // Avoid Split('`') — just scan for the backtick and take the prefix span.
            var defName  = TypeName(type.GetGenericTypeDefinition(), convention);
            var tickIdx  = defName.IndexOf('`');
            var baseName = tickIdx >= 0 ? defName.AsSpan(0, tickIdx) : defName.AsSpan();

            var args = type.GetGenericArguments();
            var sb   = new StringBuilder(baseName.Length + args.Length * 8);
            sb.Append(baseName);
            sb.Append('<');
            for (var i = 0; i < args.Length; i++)
            {
                if (i > 0)
                    sb.Append(", ");
                sb.Append(TypeName(args[i], convention));
            }
            sb.Append('>');
            return sb.ToString();
        }

        return convention switch {
            TypeNameConventions.AssemblyQualifiedName => type.AssemblyQualifiedName ?? type.FullName ?? type.Name,
            TypeNameConventions.FullName => type.FullName ?? type.Name,
            TypeNameConventions.Name => type.Name,
            _ => throw new InternalTransformErrorException("Invalid TypeNameConventions value.")
        };
    }

    // Produces CLR-style full names for generic types that are parseable by Type.GetType().
    // Does NOT use vocabulary aliases; always uses CLR type full names for all type arguments.
    // E.g.: System.Collections.Generic.Dictionary`2[[System.Int32],[System.String]]
    // Note: assembly tokens are omitted for types in mscorlib-equivalent assemblies since Type.GetType() can resolve them.
    static string ClrFullName(Type type)
    {
        if (!type.IsGenericType || type.IsGenericTypeDefinition)
            return type.FullName ?? type.Name;

        // Use a single StringBuilder for the whole recursive tree — no intermediate strings.
        var sb = new StringBuilder(64);
        AppendClrFullName(type, sb);
        return sb.ToString();
    }

    // Appends the CLR full name of <paramref name="type"/> into <paramref name="sb"/>.
    // Recurses for generic type arguments without allocating intermediate strings.
    static void AppendClrFullName(Type type, StringBuilder sb)
    {
        if (!type.IsGenericType || type.IsGenericTypeDefinition)
        {
            sb.Append(type.FullName ?? type.Name);
            return;
        }

        var def  = type.GetGenericTypeDefinition();
        var args = type.GetGenericArguments();

        sb.Append(def.FullName ?? def.Name);
        sb.Append('[');
        for (var i = 0; i < args.Length; i++)
        {
            if (i > 0)
                sb.Append(',');
            sb.Append('[');
            AppendClrFullName(args[i], sb);
            sb.Append(']');
        }
        sb.Append(']');
    }
}
