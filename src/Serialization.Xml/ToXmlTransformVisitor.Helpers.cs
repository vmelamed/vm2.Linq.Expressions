namespace vm2.Linq.Expressions.Serialization.Xml;

public partial class ToXmlTransformVisitor
{
    int _lastParamIdNumber;
    int _lastLabelIdNumber;
    // labels, parameters, and variables are created in one value and references to them are used in another.
    // These dictionaries keep their id-s so we can create `XAttribute` id-s and idRef-s to them.
    readonly Dictionary<ParameterExpression, XElement> _parameters = [];
    readonly Dictionary<LabelTarget, XElement> _labelTargets = [];

    /// <inheritdoc/>
    protected override void Reset()
    {
        base.Reset();

        _parameters.Clear();
        _labelTargets.Clear();
        _lastParamIdNumber = 0;
        _lastLabelIdNumber = 0;
    }

    string NewParameterId => $"P{++_lastParamIdNumber}";

    string NewLabelId => $"L{++_lastLabelIdNumber}";

    bool IsDefined(ParameterExpression parameterExpression)
        => _parameters.ContainsKey(parameterExpression);

    bool IsDefined(LabelTarget labelTarget)
        => _labelTargets.ContainsKey(labelTarget);

    XElement GetParameter(ParameterExpression parameterExpression)
        => _parameters.TryGetValue(parameterExpression, out var parameterElement)
                ? parameterElement
                : _parameters[parameterExpression] =
                        new XElement(
                                ElementNames.Parameter,
                                    AttributeType(parameterExpression),
                                    new XAttribute(AttributeNames.Id, NewParameterId),
                                    !string.IsNullOrWhiteSpace(parameterExpression.Name) ? new XAttribute(AttributeNames.Name, parameterExpression.Name) : null,
                                    parameterExpression.IsByRef ? new XAttribute(AttributeNames.IsByRef, parameterExpression.IsByRef) : null);

    XElement GetLabelTarget(LabelTarget labelTarget)
        => _labelTargets.TryGetValue(labelTarget, out var targetElement)
                ? targetElement
                : _labelTargets[labelTarget] =
                        new XElement(
                                ElementNames.LabelTarget,
                                    new XAttribute(AttributeNames.Id, NewLabelId),
                                    !string.IsNullOrWhiteSpace(labelTarget.Name) ? new XAttribute(AttributeNames.Name, labelTarget.Name) : null,
                                    labelTarget.Type != typeof(void) ? AttributeType(labelTarget.Type) : null);

    XAttribute? AttributeName(string? identifier)
        => !string.IsNullOrWhiteSpace(identifier)
                ? new XAttribute(AttributeNames.Name, Transform.Identifier(identifier, options.Identifiers))
                : null;

    static XAttribute? AttributeType(Expression? node)
        => node is not null
                ? AttributeType(node.Type)
                : null;

    static XAttribute? AttributeType(Type? type, bool force = false)
        => type is not null && (type != typeof(void) || force)
                ? new(AttributeNames.Type, Transform.TypeName(type))
                : null;

    XAttribute? AttributeDelegateType(Type? type)
        => options.AddLambdaTypes && type is not null
                ? new(AttributeNames.DelegateType, Transform.TypeName(type))
                : null;

    // Reflection stuff

    static XElement? VisitMethodInfo(BinaryExpression node)
        => node.Method is MemberInfo mi
                ? VisitMemberInfo(mi)
                : null;

    static XElement? VisitMethodInfo(UnaryExpression node)
        => node.Method is MemberInfo mi
                ? VisitMemberInfo(mi)
                : null;

    static XElement? VisitMemberInfo(MemberInfo? member)
    {
        if (member is null)
            return null;

        XAttribute? declaringType = member.DeclaringType is Type dt ? new XAttribute(AttributeNames.DeclaringType, Transform.TypeName(dt)) : null;
        XAttribute? nameAttribute = member is not ConstructorInfo && member.Name is not null ? new XAttribute(AttributeNames.Name, member.Name) : null;
        XAttribute? visibility = member switch
            {
                ConstructorInfo ci => ci.IsPublic
                                        ? null
                                        : (ci.Attributes & MethodAttributes.MemberAccessMask) switch
                                            {
                                                MethodAttributes.Private     => new XAttribute(AttributeNames.Visibility, AttributeNames.Private),
                                                MethodAttributes.Assembly    => new XAttribute(AttributeNames.Visibility, AttributeNames.Assembly),
                                                MethodAttributes.Family      => new XAttribute(AttributeNames.Visibility, AttributeNames.Family),
                                                MethodAttributes.FamANDAssem => new XAttribute(AttributeNames.Visibility, AttributeNames.FamilyAndAssembly),
                                                MethodAttributes.FamORAssem  => new XAttribute(AttributeNames.Visibility, AttributeNames.FamilyOrAssembly),
                                                _                            => null
                                            },
                MethodInfo mi => mi.IsPublic
                                        ? null
                                        : (mi.Attributes & MethodAttributes.MemberAccessMask) switch
                                            {
                                                MethodAttributes.Private     => new XAttribute(AttributeNames.Visibility, AttributeNames.Private),
                                                MethodAttributes.Assembly    => new XAttribute(AttributeNames.Visibility, AttributeNames.Assembly),
                                                MethodAttributes.Family      => new XAttribute(AttributeNames.Visibility, AttributeNames.Family),
                                                MethodAttributes.FamANDAssem => new XAttribute(AttributeNames.Visibility, AttributeNames.FamilyAndAssembly),
                                                MethodAttributes.FamORAssem  => new XAttribute(AttributeNames.Visibility, AttributeNames.FamilyOrAssembly),
                                                _                            => null
                                            },
                FieldInfo fi =>  fi.IsPublic
                                        ? null
                                        : (fi.Attributes & FieldAttributes.FieldAccessMask) switch
                                            {
                                                FieldAttributes.Private     => new XAttribute(AttributeNames.Visibility, AttributeNames.Private),
                                                FieldAttributes.Assembly    => new XAttribute(AttributeNames.Visibility, AttributeNames.Assembly),
                                                FieldAttributes.Family      => new XAttribute(AttributeNames.Visibility, AttributeNames.Family),
                                                FieldAttributes.FamANDAssem => new XAttribute(AttributeNames.Visibility, AttributeNames.FamilyAndAssembly),
                                                FieldAttributes.FamORAssem  => new XAttribute(AttributeNames.Visibility, AttributeNames.FamilyOrAssembly),
                                                _                            => null
                                            },
                _ => null
            };

        return member switch {
            ConstructorInfo ci => new XElement(
                                    ElementNames.Constructor,
                                        declaringType,
                                        visibility,
                                        !ci.IsStatic ? null : throw new InternalTransformErrorException($"Don't know how to use static constructors."),
                                        new XElement(
                                                ElementNames.ParameterSpecs,
                                                VisitParameters(ci.GetParameters()))),

            PropertyInfo pi => new XElement(
                                    ElementNames.Property,
                                        declaringType,
                                        visibility,
                                        AttributeType(pi.PropertyType ?? throw new InternalTransformErrorException("PropertyInfo.PropertyType is null."), true),
                                        nameAttribute,
                                        pi.GetIndexParameters().Length != 0
                                            ? new XElement(
                                                    ElementNames.ParameterSpecs,
                                                    VisitParameters(pi.GetIndexParameters()))
                                            : null),

            MethodInfo mi => new XElement(
                                    ElementNames.Method,
                                        declaringType,
                                        mi.IsStatic ? new XAttribute(AttributeNames.Static, true) : null,
                                        visibility,
                                        AttributeType(mi.ReturnType, true),
                                        nameAttribute,
                                        new XElement(ElementNames.ParameterSpecs, VisitParameters(mi.GetParameters()))),

            EventInfo ei => new XElement(
                                    ElementNames.Event,
                                        declaringType,
                                        AttributeType(ei.EventHandlerType ?? throw new InternalTransformErrorException("EventInfo's EventHandlerType is null."), true),
                                        nameAttribute),

            FieldInfo fi => new XElement(
                                    ElementNames.Field,
                                        declaringType,
                                        fi.IsStatic ? new XAttribute(AttributeNames.Static, true) : null,
                                        visibility,
                                        fi.IsInitOnly ? new XAttribute(AttributeNames.ReadOnly, true) : null,
                                        AttributeType(fi.FieldType ?? throw new InternalTransformErrorException("FieldInfo.FieldType is null."), true),
                                        nameAttribute),

            _ => throw new InternalTransformErrorException("Unknown MemberInfo.")
        };
    }

    /// <summary>
    /// Creates a sequence of XML elements for each of the <paramref name="parameters"/>.
    /// </summary>
    /// <param name="parameters">The parameters.</param>
    /// <returns>A sequence of elements.</returns>
    static IEnumerable<XElement> VisitParameters(IEnumerable<ParameterInfo> parameters)
        => parameters.Select(param => new XElement(
                                            ElementNames.ParameterSpec,
                                                AttributeType(param.ParameterType),
                                                param.Name is not null ? new XAttribute(AttributeNames.Name, param.Name) : null,
                                                param.ParameterType.IsByRef || param.IsOut ? new XAttribute(AttributeNames.IsByRef, true) : null));
}
