namespace vm2.Linq.Expressions.Serialization.Xml;

[ExcludeFromCodeCoverage]
static class AttributeNames
{
    public static XName Nil => Namespaces.Xsi + "nil";
    public static XName Assembly => Vocabulary.Assembly;
    public static XName DelegateType => Vocabulary.DelegateType;
    public static XName Family => Vocabulary.Family;
    public static XName FamilyAndAssembly => Vocabulary.FamilyAndAssembly;
    public static XName FamilyOrAssembly => Vocabulary.FamilyOrAssembly;
    public static XName IsByRef => Vocabulary.IsByRef;
    public static XName IsLifted => Vocabulary.IsLifted;
    public static XName IsLiftedToNull => Vocabulary.IsLiftedToNull;
    public static XName Kind => Vocabulary.Kind;
    public static XName Name => Vocabulary.Name;
    public static XName Private => Vocabulary.Private;
    public static XName Public => Vocabulary.Public;
    public static XName Property => Vocabulary.Property;
    public static XName Static => Vocabulary.Static;
    public static XName TailCall => Vocabulary.TailCall;
    public static XName Type => Vocabulary.Type;
    public static XName DeclaringType => Vocabulary.DeclaringType;
    public static XName TypeOperand => Vocabulary.TypeOperand;
    public static XName ConcreteType => Vocabulary.ConcreteType;   // e.g. derived from ConstantExpression.GetType
    public static XName BaseType => Vocabulary.BaseType;
    public static XName Id => Vocabulary.Id;
    public static XName Value => Vocabulary.Value;
    public static XName BaseValue => Vocabulary.BaseValue;
    public static XName Visibility => Vocabulary.Visibility;
    public static XName Length => Vocabulary.Length;
    public static XName ElementType => Vocabulary.ElementType;
    public static XName ReadOnly => Vocabulary.ReadOnly;
};
