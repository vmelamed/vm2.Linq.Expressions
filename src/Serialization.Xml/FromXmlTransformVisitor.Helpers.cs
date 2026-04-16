namespace vm2.Linq.Expressions.Serialization.Xml;

public partial class FromXmlTransformVisitor
{
    readonly Dictionary<string, ParameterExpression> _parameters = [];
    readonly Dictionary<string, LabelTarget> _labelTargets = [];

    [ExcludeFromCodeCoverage]
    internal void ResetVisitState()
    {
        _parameters.Clear();
        _labelTargets.Clear();
    }

    ParameterExpression GetParameter(XElement e)
    {
        var id = e.Attribute(AttributeNames.Id)?.Value
                        ?? throw new SerializationException($"Could not get the Id or the IdRef of a parameter or variable in {e.Name}");

        if (_parameters.TryGetValue(id, out var expression))
            return expression;

        var type = e.GetElementType();

        if (XmlConvert.ToBoolean(e.Attribute(AttributeNames.IsByRef)?.Value ?? "false"))
            type = type.MakeByRefType();

        return _parameters[id] = Expression.Parameter(type, e.TryGetName(out var name) ? name : null);
    }

    LabelTarget GetTarget(XElement e)
    {
        var id = e.Attribute(AttributeNames.Id)?.Value
                        ?? throw new SerializationException($"Could not get the Id or the IdRef of a label target in `{e.Name}`");

        if (_labelTargets.TryGetValue(id, out var target))
            return target;

        e.TryGetName(out var name);
        e.TryGetElementType(out var type);

        return _labelTargets[id] = type is not null ? Expression.Label(type, name) : Expression.Label(name);
    }

    /// <summary>
    /// Gets the member information that may be attached to the expression.
    /// </summary>
    /// <param name="e">The element representing the member info.</param>
    /// <returns>System.Reflection.MemberInfo.</returns>
    protected virtual MemberInfo? VisitMemberInfo(XElement? e)
    {
        if (e is null)
            return null;

        // get the declaring type - where to get the member info from
        var declTypeName = e.Attribute(AttributeNames.DeclaringType)?.Value
                ?? throw new SerializationException($"Could not get the declaring type of the member info of the element '{e.Name}'.");

        if (!Vocabulary.NamesToTypes.TryGetValue(declTypeName, out var declType))
            declType = Type.GetType(declTypeName);

        if (declType is null)
            throw new SerializationException($"Could not get the required declaring type of the member info of the element '{e.Name}'.");

        // get the name of the member
        e.TryGetName(out var name);
        if (name is null && e.Name.LocalName != Vocabulary.Constructor)
            throw new SerializationException($"Could not get the name in the member info of the element '{e.Name}'");

        // get the visibility flags into BindingFlags
        var isStatic = XmlConvert.ToBoolean(e.Attribute(AttributeNames.Static)?.Value ?? "false");
        var visibilityName = e.Attribute(AttributeNames.Visibility)?.Value ?? "";
        var bindingFlags = (isStatic ? BindingFlags.Static : BindingFlags.Instance) |
                           visibilityName switch
                           {
                               Vocabulary.Public => BindingFlags.Public,
                               "" => BindingFlags.Public,
                               _ => BindingFlags.NonPublic,
                           };
        var (paramTypes, modifiers) = GetParameterSpecs(e);
        var returnType = e.TryGetElementType(out var type) ? type : null;

        return e.Name.LocalName switch {
            Vocabulary.Constructor => declType.GetConstructor(bindingFlags, null, paramTypes, [modifiers]) as MemberInfo,
            Vocabulary.Property => declType.GetProperty(name!, bindingFlags, null, e.GetElementType(), paramTypes, [modifiers]),
            Vocabulary.Method => ResolveMethodInfo(declType, name!, bindingFlags, paramTypes, modifiers, returnType),
            Vocabulary.Field => declType.GetField(name!, bindingFlags),
            Vocabulary.Event => declType.GetEvent(name!, bindingFlags),
            _ => throw new SerializationException($"Could not get the member info type represented by the element '{e.Name}'"),
        }
        ?? throw new SerializationException($"Could not get the member info type represented by the element '{e.Name}'");
    }

    static MethodInfo? ResolveMethodInfo(
        Type declaringType,
        string name,
        BindingFlags bindingFlags,
        Type[] parameterTypes,
        ParameterModifier modifiers,
        Type? returnType)
    {
        // Fast path for non-generic and straightforward overloads.
        var direct = declaringType.GetMethod(name, bindingFlags, null, parameterTypes, [modifiers]);
        if (direct is not null && (returnType is null || direct.ReturnType == returnType))
            return direct;

        foreach (var method in declaringType.GetMethods(bindingFlags).Where(m => m.Name == name && m.GetParameters().Length == parameterTypes.Length))
        {
            if (!TryCloseGenericMethod(method, parameterTypes, out var candidate))
                continue;

            if (returnType is not null && candidate.ReturnType != returnType)
                continue;

            return candidate;
        }

        return null;
    }

    static bool TryCloseGenericMethod(MethodInfo method, Type[] parameterTypes, out MethodInfo candidate)
    {
        candidate = method;

        if (!method.IsGenericMethodDefinition)
            return ParametersMatch(method.GetParameters(), parameterTypes);

        var inferred = new Dictionary<Type, Type>();
        var formalParams = method.GetParameters();

        for (var i = 0; i < formalParams.Length; i++)
            if (!TryInferGenericArguments(formalParams[i].ParameterType, parameterTypes[i], inferred))
                return false;

        var genericArgs = method.GetGenericArguments();
        var closedArgs = new Type[genericArgs.Length];

        for (var i = 0; i < genericArgs.Length; i++)
            if (!inferred.TryGetValue(genericArgs[i], out closedArgs[i]!))
                return false;

        try
        {
            candidate = method.MakeGenericMethod(closedArgs);
        }
        catch (ArgumentException)
        {
            return false;
        }

        return ParametersMatch(candidate.GetParameters(), parameterTypes);
    }

    static bool ParametersMatch(ParameterInfo[] formalParams, Type[] actualParamTypes)
    {
        if (formalParams.Length != actualParamTypes.Length)
            return false;

        for (var i = 0; i < formalParams.Length; i++)
            if (formalParams[i].ParameterType != actualParamTypes[i])
                return false;

        return true;
    }

    static bool TryInferGenericArguments(Type patternType, Type actualType, Dictionary<Type, Type> inferred)
    {
        if (patternType.IsByRef)
        {
            if (!actualType.IsByRef)
                return false;

            return TryInferGenericArguments(patternType.GetElementType()!, actualType.GetElementType()!, inferred);
        }

        if (patternType.IsGenericParameter)
        {
            if (inferred.TryGetValue(patternType, out var existing))
                return existing == actualType;

            inferred[patternType] = actualType;
            return true;
        }

        if (patternType.IsArray)
        {
            if (!actualType.IsArray || patternType.GetArrayRank() != actualType.GetArrayRank())
                return false;

            return TryInferGenericArguments(patternType.GetElementType()!, actualType.GetElementType()!, inferred);
        }

        if (patternType.IsGenericType)
        {
            var patternDef = patternType.GetGenericTypeDefinition();
            var actualMatch = GetMatchingGenericType(actualType, patternDef);

            if (actualMatch is null)
                return false;

            var patternArgs = patternType.GetGenericArguments();
            var actualArgs = actualMatch.GetGenericArguments();

            for (var i = 0; i < patternArgs.Length; i++)
                if (!TryInferGenericArguments(patternArgs[i], actualArgs[i], inferred))
                    return false;

            return true;
        }

        return patternType == actualType;
    }

    static Type? GetMatchingGenericType(Type actualType, Type genericTypeDefinition)
    {
        if (actualType.IsGenericType && actualType.GetGenericTypeDefinition() == genericTypeDefinition)
            return actualType;

        foreach (var i in actualType.GetInterfaces())
            if (i.IsGenericType && i.GetGenericTypeDefinition() == genericTypeDefinition)
                return i;

        for (var current = actualType.BaseType; current is not null; current = current.BaseType)
            if (current.IsGenericType && current.GetGenericTypeDefinition() == genericTypeDefinition)
                return current;

        return null;
    }

    static (Type[], ParameterModifier) GetParameterSpecs(XElement element)
    {
        var paramCount = element.Element(ElementNames.ParameterSpecs)?.Elements(ElementNames.ParameterSpec)?.Count();

        if (paramCount is null or 0)
            return ([], new ParameterModifier());

        var types = new Type[paramCount.Value];
        var mods = new ParameterModifier(paramCount.Value);
        var i = 0;

        element
            .Element(ElementNames.ParameterSpecs)?
            .Elements(ElementNames.ParameterSpec)
            .Select(
                p =>
                {
                    types[i] = p.GetElementType();
                    mods[i] = XmlConvert.ToBoolean(p.Attribute(AttributeNames.IsByRef)?.Value ?? "false");
                    i++;
                    return 1;
                })
            .Count();
        return (types, mods);
    }
}
