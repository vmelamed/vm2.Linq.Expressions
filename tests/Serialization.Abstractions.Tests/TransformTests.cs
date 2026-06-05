// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Abstractions;

[ExcludeFromCodeCoverage]
public partial class TransformTests(ITestOutputHelper output) : TestBase(output)
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

    [Theory]
    [MemberData(nameof(TransformTypeNameRoundTripData))]
    public void TransformTypeNameRoundTripTest(string _, Type type, TypeNameConventions convention)
    {
        var name = Transform.TypeName(type, convention);
        var resolved = Transform.GetType(name);

        resolved.Should().Be(type, $"Transform.GetType should round-trip the type serialized with {convention}");
    }
}
