namespace vm2.Linq.Expressions.Serialization.Tests;

public partial class TransformTests
{
    [Theory]
    [MemberData(nameof(TransformIdentifiersData))]
    public void TransformIdentifiersTest(string _, string input, string expected, IdentifierConventions convention, bool throws)
    {
        var call = () => Transform.Identifier(input, convention);
        if (throws)
        {
            call.Should().Throw<InternalTransformErrorException>();
            return;
        }

        call().Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(TransformTypeNamesData))]
    public void TransformTypeNamesTest(string _, Type input, string expected, TypeNameConventions convention, bool throws)
    {
        var call = () => Transform.TypeName(input, convention);
        if (throws)
        {
            call.Should().Throw<InternalTransformErrorException>();
            return;
        }

        call().Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(TransformAnonymousTypeNamesLocalData))]
    public void TransformTypeNamesAnonymousTest(string _, string expected, TypeNameConventions convention, bool throws)
    {
        var test = new
        {
            Abc = 123,
            Xyz = "xyz",
        };
        var input = test.GetType();

        var call = () => Transform.TypeName(input, convention);
        if (throws)
        {
            call.Should().Throw<InternalTransformErrorException>();
            return;
        }

        call().Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(TransformGenericTypeNamesLocalData))]
    public void TransformTypeNamesDictionaryTest(string _, string expected, TypeNameConventions convention, bool throws)
    {
        var input = typeof(Dictionary<int, string>);

        var call = () => Transform.TypeName(input, convention);
        if (throws)
        {
            call.Should().Throw<InternalTransformErrorException>();
            return;
        }

        call().Should().Be(expected);
    }
}
