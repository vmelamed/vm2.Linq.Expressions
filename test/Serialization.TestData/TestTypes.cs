namespace vm2.Linq.Expressions.Serialization.TestData;

public enum EnumTest
{
    One,
    Two,
    Three,
};

[Flags]
public enum EnumFlagsTest
{
    One = 1,
    Two = 2,
    Three = 4,
}

[DataContract(Namespace = "urn:vm.Test.Diagnostics", IsReference = true)]
public class Object1 : IEquatable<Object1>
{
    [DataMember]
    public object? ObjectProperty { get; set; }

    [DataMember]
    public int? NullIntProperty { get; set; } = null;

    [DataMember]
    public long? NullLongProperty { get; set; } = 1L;

    [DataMember]
    public bool BoolProperty { get; set; } = true;

    [DataMember]
    public char CharProperty { get; set; } = 'A';

    [DataMember]
    public byte ByteProperty { get; set; } = 1;

    [DataMember]
    public sbyte SByteProperty { get; set; } = 1;

    [DataMember]
    public short ShortProperty { get; set; } = 1;

    [DataMember]
    public int IntProperty { get; set; } = 1;

    [DataMember]
    public long LongProperty { get; set; } = 1L;

    [DataMember]
    public ushort UShortProperty { get; set; } = 1;

    [DataMember]
    public uint UIntProperty { get; set; } = 1;

    [DataMember]
    public ulong ULongProperty { get; set; } = 1;

    [DataMember]
    public double DoubleProperty { get; set; } = 1.0;

    [DataMember]
    public float FloatProperty { get; set; } = 1F;

    [DataMember]
    public Half HalfProperty { get; set; } = (Half)1.1;

    [DataMember]
    public decimal DecimalProperty { get; set; } = 1M;

    [DataMember]
    public Guid GuidProperty { get; set; } = Guid.Empty;

    [DataMember]
    public Uri UriProperty { get; set; } = new Uri("http://localhost");

    [DataMember]
    public DateTime DateTimeProperty { get; set; } = new DateTime(2024, 4, 14, 22, 48, 34, DateTimeKind.Local);

    [DataMember]
    public TimeSpan TimeSpanProperty { get; set; } = new TimeSpan(123L);

    [DataMember]
    public DateTimeOffset DateTimeOffsetProperty { get; set; } = new DateTimeOffset(2024, 4, 14, 22, 48, 34, new TimeSpan(0, 5, 0));

    public string StringField = "Hi there!";

    #region Identity rules implementation.
    #region IEquatable<Object1> Members
    public virtual bool Equals(Object1? other)
        => other is not null &&
           (ReferenceEquals(this, other) ||
            GetType() == other.GetType()
            && ObjectProperty is null == other.ObjectProperty is null
            && NullIntProperty == other.NullIntProperty
            && NullLongProperty == other.NullLongProperty
            && BoolProperty == other.BoolProperty
            && CharProperty == other.CharProperty
            && ByteProperty == other.ByteProperty
            && SByteProperty == other.SByteProperty
            && ShortProperty == other.ShortProperty
            && IntProperty == other.IntProperty
            && LongProperty == other.LongProperty
            && UShortProperty == other.UShortProperty
            && UIntProperty == other.UIntProperty
            && ULongProperty == other.ULongProperty
            && DoubleProperty == other.DoubleProperty
            && FloatProperty == other.FloatProperty
            // && HalfProperty == other.HalfProperty    // the DataContractSerializer cannot de/serialize Half values
            && DecimalProperty == other.DecimalProperty
            && GuidProperty == other.GuidProperty
            && UriProperty == other.UriProperty
            && DateTimeProperty == other.DateTimeProperty
            && TimeSpanProperty == other.TimeSpanProperty
            && DateTimeOffsetProperty == other.DateTimeOffsetProperty
           );
    #endregion

    public override bool Equals(object? obj) => Equals(obj as Object1);

    /// <summary>
    /// Serves as a hash function for the objects of <see cref="Object1"/> and its derived types.
    /// </summary>
    /// <returns>A hash code for the current <see cref="Object1"/> instance.</returns>
    public override int GetHashCode()
    {
        var hc = new HashCode();
        hc.Add(GetType());
        hc.Add(NullIntProperty);
        hc.Add(NullLongProperty);
        hc.Add(BoolProperty);
        hc.Add(CharProperty);
        hc.Add(ByteProperty);
        hc.Add(SByteProperty);
        hc.Add(ShortProperty);
        hc.Add(IntProperty);
        hc.Add(LongProperty);
        hc.Add(UShortProperty);
        hc.Add(UIntProperty);
        hc.Add(ULongProperty);
        hc.Add(DoubleProperty);
        hc.Add(FloatProperty);
        hc.Add(HalfProperty);
        hc.Add(DecimalProperty);
        hc.Add(GuidProperty);
        hc.Add(UriProperty);
        hc.Add(DateTimeProperty);
        hc.Add(TimeSpanProperty);
        hc.Add(DateTimeOffsetProperty);
        return hc.ToHashCode();
    }

    public static bool operator ==(Object1 left, Object1 right) => left is null ? right is null : left.Equals(right);

    public static bool operator !=(Object1 left, Object1 right) => !(left == right);
    #endregion

}

[DataContract]
public class ClassDataContract1 : IEquatable<ClassDataContract1>
{
    public ClassDataContract1() { }

    public ClassDataContract1(int i, string s)
    {
        IntProperty = i;
        StringProperty = s;
    }

    [DataMember]
    public int IntProperty { get; set; } = 7;

    [DataMember]
    public string StringProperty { get; set; } = "vm";

    public override string ToString() => "ClassDataContract1";

    public virtual bool Equals(ClassDataContract1? other)
        => other is not null &&
           (ReferenceEquals(this, other) ||
            GetType() == other.GetType()
            && IntProperty == other.IntProperty
            && StringProperty == other.StringProperty
           );

    public override bool Equals(object? obj) => Equals(obj as ClassDataContract1);

    public override int GetHashCode() => HashCode.Combine(IntProperty, StringProperty);

    public static bool operator ==(ClassDataContract1 left, ClassDataContract1 right) => left is null ? right is null : left.Equals(right);

    public static bool operator !=(ClassDataContract1 left, ClassDataContract1 right) => !(left == right);
}

#pragma warning disable IDE0021 // Use expression body for constructor
[DataContract]
public class ClassDataContract2 : ClassDataContract1, IEquatable<ClassDataContract2>
{
    public ClassDataContract2()
    {
    }

    public ClassDataContract2(int i, string s, decimal d) :
        base(i, s)
    {
        DecimalProperty = d;
    }

    [DataMember]
    public decimal DecimalProperty { get; set; } = 17M;

    #region Identity rules implementation.
    #region IEquatable<ClassDataContract2> Members
    public virtual bool Equals(ClassDataContract2? other)
        => other is not null &&
           (ReferenceEquals(this, other) ||
            GetType() == other.GetType()
            && IntProperty == other.IntProperty
            && StringProperty == other.StringProperty
            && DecimalProperty == other.DecimalProperty
           );
    #endregion

    public override bool Equals(object? obj) => Equals(obj as ClassDataContract2);

    public override int GetHashCode() => HashCode.Combine(IntProperty, StringProperty, DecimalProperty);

    public static bool operator ==(ClassDataContract2 left, ClassDataContract2 right) => left is null ? right is null : left.Equals(right);

    public static bool operator !=(ClassDataContract2 left, ClassDataContract2 right) => !(left == right);
    #endregion

    public override string ToString() => "ClassDataContract2";
}
#pragma warning restore IDE0021 // Use expression body for constructor

[Serializable]
public class ClassSerializable1 : IEquatable<ClassSerializable1>
{
    public int IntProperty { get; set; }

    public string StringProperty { get; set; } = "";

    public override string ToString() => "ClassSerializable1";

    public virtual bool Equals(ClassSerializable1? other)
        => other is not null &&
           (ReferenceEquals(this, other) ||
            GetType() == other.GetType()
            && IntProperty == other.IntProperty
            && StringProperty == other.StringProperty
           );

    public override bool Equals(object? obj) => Equals(obj as ClassSerializable1);

    public override int GetHashCode() => HashCode.Combine(IntProperty, StringProperty);

    public static bool operator ==(ClassSerializable1 left, ClassSerializable1 right) => left is null ? right is null : left.Equals(right);

    public static bool operator !=(ClassSerializable1 left, ClassSerializable1 right) => !(left == right);
}

public class ClassNonSerializable(int intProperty, string strProperty)
{
    public int IntProperty { get; set; } = intProperty;

    public string StringProperty { get; set; } = strProperty;

    public override string ToString() => "ClassNonSerializable";
}

[Serializable]
public struct StructSerializable1 : IEquatable<StructSerializable1>
{
    public StructSerializable1() { }

    public int IntProperty { get; set; }

    public string StringProperty { get; set; } = "";

    public readonly bool Equals(StructSerializable1 other)
        => (GetType() == other.GetType()
            && IntProperty == other.IntProperty
            && StringProperty == other.StringProperty
           );

    public override readonly bool Equals(object? obj) => obj is StructSerializable1 s && Equals(s);

    public override readonly int GetHashCode() => HashCode.Combine(IntProperty, StringProperty);

    public static bool operator ==(StructSerializable1 left, StructSerializable1 right) => left.Equals(right);

    public static bool operator !=(StructSerializable1 left, StructSerializable1 right) => !(left == right);
}

[DataContract]
public struct StructDataContract1 : IEquatable<StructDataContract1>
{
    public StructDataContract1() { }
    public StructDataContract1(int i, string s)
    {
        IntProperty = i;
        StringProperty = s;
    }

    [DataMember]
    public int IntProperty { get; set; }

    [DataMember]
    public string StringProperty { get; set; } = "";

    public override readonly string ToString() => "StructDataContract1";

    public readonly bool Equals(StructDataContract1 other)
        => (GetType() == other.GetType()
            && IntProperty == other.IntProperty
            && StringProperty == other.StringProperty
           );

    public override readonly bool Equals(object? obj) => obj is StructDataContract1 s && Equals(s);

    public override readonly int GetHashCode() => HashCode.Combine(IntProperty, StringProperty);

    public static bool operator ==(StructDataContract1 left, StructDataContract1 right) => left.Equals(right);

    public static bool operator !=(StructDataContract1 left, StructDataContract1 right) => !(left == right);
}

[DataContract]
public class A
{
    [DataMember]
    public int _a;

    public static A operator -(A x) => new() { _a = -x._a };

    public static A operator +(A x) => new() { _a = x._a };
}

[DataContract]
public class B
{
    [DataMember]
    public bool _b;

    public static B operator !(B x) => new() { _b = !x._b };
}

[DataContract]
public class D
{
    [DataMember]
    public int _d;

    public static bool operator true(D x) => x._d != 0;

    public static bool operator false(D x) => x._d == 0;
}

#pragma warning disable CS0649
[DataContract]
public class C : A
{
    [DataMember]
    public double _c;
}
#pragma warning restore CS0649

#pragma warning disable IDE0025 // Use expression body for property
[DataContract]
public class TestMethods
{
    public static readonly int S = 42;

    [DataMember]
    public readonly int _a = 3;

    [DataMember]
    public readonly int _b = 11;

    public int A { get => _a; }

    public int B { get => _b; }

    public static int Method1() => 1;

    public static int Method2(int i, string _) => i;

    public int Method3(int i, double _) => i + _a;

    public void Method4(int i, double d) => Console.WriteLine($"Integer: {i}, double: {d}, Integer instance member: {_a}");
}
#pragma warning restore IDE0025 // Use expression body for property

public class Inner
{
    public int IntProperty { get; set; }

    public string StringProperty { get; set; } = "";
}

public class TestMembersInitialized
{
    public int TheOuterIntProperty { get; set; }

    public DateTime Time { get; set; } = new(2024, 4, 14, 22, 48, 34, DateTimeKind.Local);

    public Inner InnerProperty { get; set; } = new();

    public int[] ArrayProperty { get; set; } = [1, 2, 3];

    public IEnumerable<string> EnumerableProperty { get; set; } = ["1", "2", "3"];
}

public class TestMembersInitialized1
{
    public int TheOuterIntProperty { get; set; }

    public DateTime Time { get; set; }

    public Inner InnerProperty { get; set; } = new();

    public int[] ArrayProperty { get; set; } = [];

    public List<Inner> ListProperty { get; set; } = [];

    public int this[int i]
    {
        get => ArrayProperty![i];
        set => ArrayProperty![i] = value;
    }
}

public sealed class DerivedIntList : List<int>;

public static class GenericMethodFixtures
{
    public static int CountArray<T>(T[] items) => items.Length;

    public static int CountList<T>(List<T> items) => items.Count;

    public static int CountStructs<T>(IEnumerable<T> items)
        where T : struct
    {
        var count = 0;

        foreach (var _ in items)
            count++;

        return count;
    }

    public static T EchoByRef<T>(ref T value) => value;
}
