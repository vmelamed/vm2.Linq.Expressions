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

        return e.Name switch {
            Vocabulary.Constructor => declType.GetConstructor(bindingFlags, null, paramTypes, [modifiers]),
            Vocabulary.Property => declType.GetProperty(name!, bindingFlags, null, e.GetTypeFromProperty(), paramTypes, [modifiers]),
            Vocabulary.Method => declType.GetMethod(name!, bindingFlags, null, paramTypes, [modifiers]),
            Vocabulary.Field => declType.GetField(name!, bindingFlags),
            Vocabulary.Event => declType.GetEvent(name!, bindingFlags),
            _ => null,
        };
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
