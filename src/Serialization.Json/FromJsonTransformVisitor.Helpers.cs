namespace vm2.Linq.Expressions.Serialization.Json;

public partial class FromJsonTransformVisitor
{
    readonly Dictionary<string, ParameterExpression> _parameters = [];
    readonly Dictionary<string, LabelTarget> _labelTargets = [];

    internal void ResetVisitState()
    {
        _parameters.Clear();
        _labelTargets.Clear();
    }

    ParameterExpression GetParameter(JElement e)
    {
        var id = e.GetId();

        if (_parameters.TryGetValue(id, out var expression))
            return expression;

        var type = e.GetTypeFromProperty();

        if (e.TryGetPropertyValue<bool>(out var isByRef, Vocabulary.IsByRef) && isByRef is true)
            type = type.MakeByRefType();

        return _parameters[id] = Expression.Parameter(type, e.TryGetName(out var name) ? name : null);
    }

    LabelTarget GetTarget(JElement e)
    {
        var id = e.GetId();

        if (_labelTargets.TryGetValue(id, out var target))
            return target;

        e.TryGetName(out var name);
        e.TryGetTypeFromProperty(out var type);

        return _labelTargets[id] = type is not null ? Expression.Label(type, name) : Expression.Label(name);
    }

    static MemberInfo? TryGetMemberInfo(JElement e, string memberInfoName)
        => e.TryGetElement(out var member, memberInfoName) && member is not null
                ? TryVisitMemberInfo(member.Value)
                : null;

#pragma warning disable IDE0079, IDE0051 // Remove unnecessary suppression. Remove unused private members
    [ExcludeFromCodeCoverage]
    static MemberInfo GetMemberInfo(JElement e, string memberInfoName)
        => VisitMemberInfo(e.GetElement(memberInfoName));
#pragma warning restore IDE0051, IDE0079

    /// <summary>
    /// Gets the member information that may be attached to the expression.
    /// </summary>
    /// <param name="e">The element representing the member info.</param>
    /// <returns>System.Reflection.MemberInfo.</returns>
    internal static MemberInfo? TryVisitMemberInfo(JElement e)
    {
        // get the declaring type - where to get the member info from
        var declTypeName = e.GetPropertyValue<string>(Vocabulary.DeclaringType);

        if (!Vocabulary.NamesToTypes.TryGetValue(declTypeName, out var declType))
            declType = Type.GetType(declTypeName)
                                ?? e.ThrowSerializationException<Type?>($"Could not get the required declaring type of the member for a member info");

        Debug.Assert(declType is not null);

        // get the name of the member
        e.TryGetName(out var name);
        if (name is null && e.Name != Vocabulary.Constructor)
            e.ThrowSerializationException<MemberInfo?>($"Could not get the name of the member for a member info");

        // get the visibility flags into BindingFlags
        var isStatic = e.TryGetPropertyValue<bool>(out var stat, Vocabulary.Static) && stat;
        var visibility = e.TryGetPropertyValue<string>(out var vis, Vocabulary.Visibility) ? vis : "";
        var bindingFlags = (isStatic ? BindingFlags.Static : BindingFlags.Instance) |
                           (visibility is Vocabulary.Public or "" ? BindingFlags.Public : BindingFlags.NonPublic);
        var (paramTypes, modifiers) = GetParameterSpecs(e);
        var returnType = e.TryGetTypeFromProperty(out var type) ? type : null;

        return e.Name switch {
            Vocabulary.Constructor => declType.GetConstructor(bindingFlags, null, paramTypes, [modifiers]),
            Vocabulary.Property => declType.GetProperty(name!, bindingFlags, null, e.GetTypeFromProperty(), paramTypes, [modifiers]),
            Vocabulary.Method => ResolveMethodInfo(declType, name!, bindingFlags, paramTypes, modifiers, returnType),
            Vocabulary.Field => declType.GetField(name!, bindingFlags),
            Vocabulary.Event => declType.GetEvent(name!, bindingFlags),
            _ => null,
        };
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

        foreach (var iface in actualType.GetInterfaces())
            if (iface.IsGenericType && iface.GetGenericTypeDefinition() == genericTypeDefinition)
                return iface;

        for (var current = actualType.BaseType; current is not null; current = current.BaseType)
            if (current.IsGenericType && current.GetGenericTypeDefinition() == genericTypeDefinition)
                return current;

        return null;
    }

    internal static MemberInfo VisitMemberInfo(JElement e)
        => TryVisitMemberInfo(e)
                ?? e.ThrowSerializationException<MemberInfo>($"Could not create the member info");

    internal static (Type[], ParameterModifier) GetParameterSpecs(JElement e)
    {
        int? paramCount = !e.TryGetArray(out var parameters, Vocabulary.ParameterSpecs) || parameters is null
                            ? null
                            : parameters.Count;

        if (paramCount is null || paramCount.Value == 0)
            return ([], new ParameterModifier());

        Debug.Assert(parameters is not null);

        var types = new Type[paramCount.Value];
        var mods = new ParameterModifier(paramCount.Value);

        parameters?
            .Select(
                (n, i) =>
                {
                    var jsObj = n?.GetValueKind() is JsonValueKind.Object ? n.AsObject() : parameters.ThrowSerializationException<JsonObject>($"Invalid parameter info");

                    types[i] = jsObj.TryGetType(out var type) && type is not null ? type : parameters.ThrowSerializationException<Type>($"Could not get the type for a parameter info");
                    mods[i] = jsObj.TryGetPropertyValue<bool>(Vocabulary.IsByRef, out var isByRef) && isByRef;
                    return 1;
                })
            .Count();
        return (types, mods);
    }
}
