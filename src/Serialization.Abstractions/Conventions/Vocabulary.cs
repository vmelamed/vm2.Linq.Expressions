namespace vm2.Linq.Expressions.Serialization.Conventions;

#pragma warning disable IDE0079
#pragma warning disable CS1591

/// <summary>
/// Contains all document token names used in LINQ expression serialization (XML local names, JSON property names, etc.).
/// </summary>
public static class Vocabulary
{
    // Basic type names common to all text document formats.
    public const string Void                    = "void";

    public const string Boolean                 = "boolean";
    public const string Byte                    = "byte";
    public const string Char                    = "char";
    public const string Double                  = "double";
    public const string Float                   = "float";
    public const string Int                     = "int";
    public const string IntPtr                  = "intPtr";
    public const string Long                    = "long";
    public const string SignedByte              = "signedByte";
    public const string Short                   = "short";
    public const string UnsignedInt             = "unsignedInt";
    public const string UnsignedIntPtr          = "unsignedIntPtr";
    public const string UnsignedLong            = "unsignedLong";
    public const string UnsignedShort           = "unsignedShort";

    public const string DateTime                = "dateTime";
    public const string DateTimeOffset          = "dateTimeOffset";
    public const string Duration                = "duration";
    public const string DBNull                  = "dbNull";
    public const string Decimal                 = "decimal";
    public const string Guid                    = "guid";
    public const string Half                    = "half";
    public const string String                  = "string";
    public const string Uri                     = "uri";

    // Other types and type categories
    public const string Object                  = "object";
    public const string Nullable                = "nullable";
    public const string Enum                    = "enum";
    public const string Anonymous               = "anonymous";
    public const string ByteSequence            = "byteSequence";
    public const string Sequence                = "sequence";
    public const string Dictionary              = "dictionary";
    public const string KeyValuePair            = "key-value";
    public const string Tuple                   = "tuple";
    public const string TupleItem               = "item";

    // Names for other elements, attributes, and properties
    public const string Null                    = "nil";
    public const string NaN                     = "NaN";
    public const string PosInfinity             = "INF";
    public const string NegInfinity             = "-INF";
    public const string Type                    = "type";
    public const string DeclaringType           = "declaringType";
    public const string ConcreteType            = "concreteType";
    public const string BaseType                = "baseType";
    public const string Name                    = "name";
    public const string Key                     = "key";
    public const string Value                   = "value";
    public const string Id                      = "id";
    public const string IdRef                   = "idref";
    public const string BaseValue               = "baseValue";
    public const string Length                   = "length";
    public const string Assembly                = "assembly";
    public const string DelegateType            = "delegateType";
    public const string Family                  = "family";
    public const string FamilyAndAssembly       = "familyAndAssembly";
    public const string FamilyOrAssembly        = "familyOrAssembly";
    public const string IsByRef                 = "isByRef";
    public const string IsLifted                = "isLifted";
    public const string IsLiftedToNull          = "isLiftedToNull";
    public const string Kind                    = "kind";
    public const string Private                 = "private";
    public const string Public                  = "public";
    public const string Static                  = "static";
    public const string TailCall                = "tailCall";
    public const string TypeOperand             = "typeOperand";
    public const string Visibility              = "visibility";
    public const string Indexer                 = "indexer";
    public const string ElementType             = "elementType";
    public const string ReadOnly                = "readOnly";

    public const string ParameterSpec           = "parameterSpec";
    public const string Parameter               = "parameter";
    public const string Parameters              = "parameters";
    public const string ParameterSpecs          = "parameterSpecs";
    public const string Variables               = "variables";
    public const string Expressions             = "expressions";

    public const string Comment                 = "$comment";
    public const string Schema                  = "$schema";

    public const string Expression              = "expression";
    public const string Operands                = "operands";
    public const string Operand                 = "operand";

    public const string Body                    = "body";
    public const string Indexes                 = "indexes";
    public const string Delegate                = "delegate";
    public const string Arguments               = "arguments";
    public const string Left                    = "left";
    public const string Method                  = "method";
    public const string Right                   = "right";
    public const string ArrayIndex              = "arrayIndex";
    public const string Add                     = "add";
    public const string AddAssign               = "addAssign";
    public const string AddAssignChecked        = "addAssignChecked";
    public const string AddChecked              = "addChecked";
    public const string And                     = "and";
    public const string AndAlso                 = "andAlso";
    public const string AndAssign               = "andAssign";
    public const string ArrayLength             = "arrayLength";
    public const string Assign                  = "assign";
    public const string Bindings                = "bindings";
    public const string Block                   = "block";
    public const string Bounds                  = "bounds";
    public const string BreakLabel              = "breakLabel";
    public const string Case                    = "case";
    public const string Cases                   = "cases";
    public const string Catch                   = "catch";
    public const string Catches                 = "catches";
    public const string Call                    = "call";
    public const string CaseValues              = "caseValues";
    public const string Coalesce                = "coalesce";
    public const string Comparison              = "comparison";
    public const string Conditional             = "conditional";
    public const string Constant                = "constant";
    public const string Constructor             = "constructor";
    public const string ContinueLabel           = "continueLabel";
    public const string Convert                 = "convert";
    public const string ConvertChecked          = "convertChecked";
    public const string ConvertLambda           = "convertLambda";
    public const string Decrement               = "decrement";
    public const string Default                 = "default";
    public const string DefaultCase             = "defaultCase";
    public const string Divide                  = "divide";
    public const string DivideAssign            = "divideAssign";
    public const string ElementInit             = "elementInit";
    public const string ArrayElements           = "elements";
    public const string Equal                   = "equal";
    public const string Event                   = "event";
    public const string Exception               = "exception";
    public const string ExclusiveOr             = "exclusiveOr";
    public const string ExclusiveOrAssign       = "exclusiveOrAssign";
    public const string Fault                   = "fault";
    public const string Field                   = "field";
    public const string Filter                  = "filter";
    public const string Finally                 = "finally";
    public const string Goto                    = "goto";
    public const string GreaterThan             = "greaterThan";
    public const string GreaterThanOrEqual      = "greaterThanOrEqual";
    public const string Increment               = "increment";
    public const string Index                   = "index";
    public const string Invoke                  = "invoke";
    public const string IsFalse                 = "isFalse";
    public const string IsTrue                  = "isTrue";
    public const string Label                   = "label";
    public const string Lambda                  = "lambda";
    public const string LeftShift               = "leftShift";
    public const string LeftShiftAssign         = "leftShiftAssign";
    public const string LessThan                = "lessThan";
    public const string LessThanOrEqual         = "lessThanOrEqual";
    public const string ListInit                = "listInit";
    public const string Initializers            = "initializers";
    public const string Loop                    = "loop";
    public const string Member                  = "member";
    public const string MemberAccess            = "memberAccess";
    public const string MemberInit              = "memberInit";
    public const string Members                 = "members";
    public const string Modulo                  = "modulo";
    public const string ModuloAssign            = "moduloAssign";
    public const string Multiply                = "multiply";
    public const string MultiplyAssign          = "multiplyAssign";
    public const string MultiplyAssignChecked   = "multiplyAssignChecked";
    public const string MultiplyChecked         = "multiplyChecked";
    public const string Negate                  = "negate";
    public const string NegateChecked           = "negateChecked";
    public const string New                     = "new";
    public const string NewArrayBounds          = "newArrayBounds";
    public const string NewArrayInit            = "newArrayInit";
    public const string Not                     = "not";
    public const string NotEqual                = "notEqual";
    public const string OnesComplement          = "onesComplement";
    public const string Or                      = "or";
    public const string OrAssign                = "orAssign";
    public const string OrElse                  = "orElse";
    public const string PostDecrementAssign     = "postDecrementAssign";
    public const string PostIncrementAssign     = "postIncrementAssign";
    public const string Power                   = "power";
    public const string PowerAssign             = "powerAssign";
    public const string PreDecrementAssign      = "preDecrementAssign";
    public const string PreIncrementAssign      = "preIncrementAssign";
    public const string Property                = "property";
    public const string Quote                   = "quote";
    public const string RightShift              = "rightShift";
    public const string RightShiftAssign        = "rightShiftAssign";
    public const string Subtract                = "subtract";
    public const string SubtractAssign          = "subtractAssign";
    public const string SubtractAssignChecked   = "subtractAssignChecked";
    public const string SubtractChecked         = "subtractChecked";
    public const string Switch                  = "switch";
    public const string If                      = "if";
    public const string Then                    = "then";
    public const string Else                    = "else";
    public const string LabelTarget             = "target";
    public const string Throw                   = "throw";
    public const string Try                     = "try";
    public const string TypeAs                  = "typeAs";
    public const string TypeEqual               = "typeEqual";
    public const string TypeIs                  = "typeIs";
    public const string UnaryPlus               = "unaryPlus";
    public const string Unbox                   = "unbox";
    public const string AssignmentBinding       = "assignmentBinding";
    public const string MemberMemberBinding     = "memberMemberBinding";
    public const string MemberListBinding       = "memberListBinding";

    static IEnumerable<KeyValuePair<Type, string>> EnumTypesToNames()
    {
        yield return new(typeof(void), Void);
        yield return new(typeof(bool), Boolean);
        yield return new(typeof(byte), Byte);
        yield return new(typeof(char), Char);
        yield return new(typeof(DateTime), DateTime);
        yield return new(typeof(DateTimeOffset), DateTimeOffset);
        yield return new(typeof(DBNull), DBNull);
        yield return new(typeof(decimal), Decimal);
        yield return new(typeof(double), Double);
        yield return new(typeof(float), Float);
        yield return new(typeof(Guid), Guid);
        yield return new(typeof(Half), Half);
        yield return new(typeof(int), Int);
        yield return new(typeof(IntPtr), IntPtr);
        yield return new(typeof(long), Long);
        yield return new(typeof(sbyte), SignedByte);
        yield return new(typeof(short), Short);
        yield return new(typeof(string), String);
        yield return new(typeof(TimeSpan), Duration);
        yield return new(typeof(uint), UnsignedInt);
        yield return new(typeof(UIntPtr), UnsignedIntPtr);
        yield return new(typeof(ulong), UnsignedLong);
        yield return new(typeof(Uri), Uri);
        yield return new(typeof(ushort), UnsignedShort);
        yield return new(typeof(Enum), Enum);
        yield return new(typeof(Nullable<>), Nullable);
        yield return new(typeof(object), Object);
    }

    /// <summary>
    /// Maps basic types to names used in text documents.
    /// </summary>
    public static readonly FrozenDictionary<Type, string> TypesToNames = EnumTypesToNames().ToFrozenDictionary();

    static IEnumerable<KeyValuePair<string, Type>> EnumNamesToTypes()
    {
        yield return new(Void, typeof(void));
        yield return new(Boolean, typeof(bool));
        yield return new(Byte, typeof(byte));
        yield return new(Char, typeof(char));
        yield return new(DateTime, typeof(DateTime));
        yield return new(DateTimeOffset, typeof(DateTimeOffset));
        yield return new(DBNull, typeof(DBNull));
        yield return new(Decimal, typeof(decimal));
        yield return new(Double, typeof(double));
        yield return new(Float, typeof(float));
        yield return new(Guid, typeof(Guid));
        yield return new(Half, typeof(Half));
        yield return new(Int, typeof(int));
        yield return new(IntPtr, typeof(IntPtr));
        yield return new(Long, typeof(long));
        yield return new(SignedByte, typeof(sbyte));
        yield return new(Short, typeof(short));
        yield return new(String, typeof(string));
        yield return new(Duration, typeof(TimeSpan));
        yield return new(UnsignedInt, typeof(uint));
        yield return new(UnsignedIntPtr, typeof(UIntPtr));
        yield return new(UnsignedLong, typeof(ulong));
        yield return new(Uri, typeof(Uri));
        yield return new(UnsignedShort, typeof(ushort));
        yield return new(Enum, typeof(Enum));
        yield return new(Nullable, typeof(Nullable<>));
        yield return new(Object, typeof(object));
    }

    /// <summary>
    /// Maps names used in text documents to basic types.
    /// </summary>
    public static readonly FrozenDictionary<string, Type> NamesToTypes = EnumNamesToTypes().ToFrozenDictionary();

    /// <summary>
    /// The set of all basic type names.
    /// </summary>
    public static readonly FrozenSet<string> BasicTypeNames = NamesToTypes.Keys.ToFrozenSet();
}
