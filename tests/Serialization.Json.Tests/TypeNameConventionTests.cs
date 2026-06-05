// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Json;

[ExcludeFromCodeCoverage]
public class TypeNameConventionTests
{
    static readonly Expression<Func<int, int>> _intExpr    = x => x + 1;
    static readonly Expression<Func<string, int>> _strExpr  = s => s.Length;
    // List<int> is not in the vocabulary, so its type name differs across conventions.
    static readonly Expression _listConstExpr = Expression.Constant(new List<int> { 1, 2, 3 });

    static JsonOptions OptionsFor(TypeNameConventions convention) => new() {
        Indent      = false,
        AddComments = false,
        TypeNames   = convention,
    };

    static string ToJson(Expression expression, TypeNameConventions convention)
        => expression.ToJsonString(OptionsFor(convention));

    // -----------------------------------------------------------------------
    // The serialized JSON must contain the type name written according to the
    // configured convention.
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(TypeNameConventions.AssemblyQualifiedName)]
    [InlineData(TypeNameConventions.FullName)]
    [InlineData(TypeNameConventions.Name)]
    public void TypeNameConvention_IsHonoredInOutput(TypeNameConventions convention)
    {
        var json             = ToJson(_strExpr, convention);
        var expectedTypeName = Transform.TypeName(typeof(string), convention);

        json.Should().Contain(expectedTypeName, $"the serialized JSON should contain the type name formatted as {convention}");
    }

    // -----------------------------------------------------------------------
    // Switching the convention must produce a different type name string.
    // -----------------------------------------------------------------------

    [Fact]
    public void AssemblyQualifiedName_And_Name_ProduceDifferentOutput()
    {
        var jsonFull = ToJson(_listConstExpr, TypeNameConventions.AssemblyQualifiedName);
        var jsonName = ToJson(_listConstExpr, TypeNameConventions.Name);

        jsonFull.Should().NotBe(jsonName, "AssemblyQualifiedName and Name conventions must produce different JSON");
    }

    // -----------------------------------------------------------------------
    // Expressions serialized with AssemblyQualifiedName and FullName must
    // round-trip correctly.
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(TypeNameConventions.AssemblyQualifiedName)]
    [InlineData(TypeNameConventions.FullName)]
    public void TypeNameConvention_RoundTrip(TypeNameConventions convention)
    {
        var json      = ToJson(_intExpr, convention);
        var roundTrip = ExpressionJson.FromString(json);

        _intExpr.DeepEquals(roundTrip).Should().BeTrue($"expression serialized with {convention} should round-trip cleanly");
    }

    // -----------------------------------------------------------------------
    // DocumentOptions.TransformTypeName delegates to the configured TypeNames.
    // -----------------------------------------------------------------------

    [Theory]
    [InlineData(TypeNameConventions.AssemblyQualifiedName)]
    [InlineData(TypeNameConventions.FullName)]
    [InlineData(TypeNameConventions.Name)]
    public void DocumentOptions_TransformTypeName_HonorsConvention(TypeNameConventions convention)
    {
        var options  = OptionsFor(convention);
        var expected = Transform.TypeName(typeof(List<int>), convention);

        options.TransformTypeName(typeof(List<int>)).Should().Be(expected, $"DocumentOptions.TransformTypeName should use the {convention} convention");
    }
}
