namespace vm2.Linq.Expressions.Serialization.Xml;

/// <summary>
/// Class that visits the nodes of an XML element to produce a LINQ expression tree.
/// </summary>
public partial class FromXmlTransformVisitor
{
    #region Dispatch map for the concrete XML element Expression transforming visitors
    /// <summary>
    /// Holds dictionary of expression element name - delegate to the respective transform.
    /// </summary>
    static IEnumerable<KeyValuePair<string, Func<FromXmlTransformVisitor, XElement, Expression>>> Transforms()
    {
        yield return new(Vocabulary.Expression, (v, e) => v.VisitChild(e, 0));
        yield return new(Vocabulary.Constant, (v, e) => v.VisitConstant(e));
        yield return new(Vocabulary.ParameterSpec, (v, e) => v.VisitParameter(e));
        yield return new(Vocabulary.Parameter, (v, e) => v.VisitParameter(e));
        yield return new(Vocabulary.Lambda, (v, e) => v.VisitLambda(e));
        // unary
        yield return new(Vocabulary.ArrayLength, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.Convert, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.ConvertChecked, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.Negate, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.NegateChecked, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.Not, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.OnesComplement, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.Quote, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.TypeAs, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.UnaryPlus, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.IsFalse, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.IsTrue, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.Unbox, (v, e) => v.VisitUnary(e));
        // change by one
        yield return new(Vocabulary.Decrement, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.Increment, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.PostDecrementAssign, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.PostIncrementAssign, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.PreDecrementAssign, (v, e) => v.VisitUnary(e));
        yield return new(Vocabulary.PreIncrementAssign, (v, e) => v.VisitUnary(e));
        // binary
        yield return new(Vocabulary.Add, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.AddChecked, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.And, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.AndAlso, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.Coalesce, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.Divide, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.Equal, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.ExclusiveOr, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.GreaterThan, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.GreaterThanOrEqual, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.LeftShift, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.LessThan, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.LessThanOrEqual, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.Modulo, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.Multiply, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.MultiplyChecked, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.NotEqual, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.Or, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.OrElse, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.Power, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.RightShift, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.Subtract, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.SubtractChecked, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.ArrayIndex, (v, e) => v.VisitBinary(e));
        // assignments
        yield return new(Vocabulary.AddAssign, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.AddAssignChecked, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.AndAssign, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.Assign, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.DivideAssign, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.LeftShiftAssign, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.ModuloAssign, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.MultiplyAssign, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.MultiplyAssignChecked, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.OrAssign, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.PowerAssign, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.RightShiftAssign, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.SubtractAssign, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.SubtractAssignChecked, (v, e) => v.VisitBinary(e));
        yield return new(Vocabulary.ExclusiveOrAssign, (v, e) => v.VisitBinary(e));

        yield return new(Vocabulary.TypeEqual, (v, e) => v.VisitTypeBinary(e));
        yield return new(Vocabulary.TypeIs, (v, e) => v.VisitTypeBinary(e));
        yield return new(Vocabulary.Block, (v, e) => v.VisitBlock(e));
        yield return new(Vocabulary.Conditional, (v, e) => v.VisitConditional(e));
        yield return new(Vocabulary.Index, (v, e) => v.VisitIndex(e));
        yield return new(Vocabulary.New, (v, e) => v.VisitNew(e));
        yield return new(Vocabulary.Throw, (v, e) => v.VisitThrow(e));
        yield return new(Vocabulary.Default, (v, e) => v.VisitDefault(e));
        yield return new(Vocabulary.MemberAccess, (v, e) => v.VisitMember(e));
        yield return new(Vocabulary.Call, (v, e) => v.VisitMethodCall(e));
        yield return new(Vocabulary.Invoke, (v, e) => v.VisitInvocation(e));
        yield return new(Vocabulary.Exception, (v, e) => v.VisitParameter(e));
        yield return new(Vocabulary.Label, (v, e) => v.VisitLabel(e));
        yield return new(Vocabulary.Goto, (v, e) => v.VisitGoto(e));
        yield return new(Vocabulary.Loop, (v, e) => v.VisitLoop(e));
        yield return new(Vocabulary.Switch, (v, e) => v.VisitSwitch(e));
        yield return new(Vocabulary.Try, (v, e) => v.VisitTry(e));
        yield return new(Vocabulary.MemberInit, (v, e) => v.VisitMemberInit(e));
        yield return new(Vocabulary.ListInit, (v, e) => v.VisitListInit(e));
        yield return new(Vocabulary.NewArrayInit, (v, e) => v.VisitNewArrayInit(e));
        yield return new(Vocabulary.NewArrayBounds, (v, e) => v.VisitNewArrayBounds(e));
    }

    static readonly FrozenDictionary<string, Func<FromXmlTransformVisitor, XElement, Expression>> _transforms = Transforms().ToFrozenDictionary();
    #endregion
}
