// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Json;

[CollectionDefinition("JSON")]
public class HashCodeRoundTripTests(JsonTestsFixture fixture, ITestOutputHelper output) : BaseTests(fixture, output)
{
    protected override string JsonTestFilesPath => _fixture.TestFilesPath;

    public static readonly TheoryData<string, string> HashCodeRoundTripData = new()
    {
        { TestLine(), "() => new" },
        { TestLine(), "(a,b) => { ... }" },
        { TestLine(), "(f,a) => f(a)" },
        { TestLine(), "accessMemberMember" },
        { TestLine(), "array[index]" },
        { TestLine(), "b => b ? 1 : 3" },
        { TestLine(), "Console.WriteLine" },
        { TestLine(), "goto1" },
        { TestLine(), "goto4" },
        { TestLine(), "indexObject1" },
        { TestLine(), "genericArrayCall" },
        { TestLine(), "genericByRefCall" },
        { TestLine(), "loop" },
        { TestLine(), "linqCall" },
        { TestLine(), "newArrayItems" },
        { TestLine(), "newDictionaryInit" },
        { TestLine(), "newListInit" },
        { TestLine(), "newMembersInit" },
        { TestLine(), "return1" },
        { TestLine(), "switch(a){ ... }" },
        { TestLine(), "throw" },
        { TestLine(), "try1" },
        { TestLine(), "try4" },
        { TestLine(), "try6" },
#if NEWTONSOFT_SCHEMA
        { TestLine(), "newMembersInit1" },
        { TestLine(), "newMembersInit2" },
#endif
    };

    [Theory]
    [MemberData(nameof(HashCodeRoundTripData))]
    public void JsonRoundTrip_ShouldPreserveDeepHashCode(string _, string expressionId)
    {
        var expression = ResolveExpression(expressionId);

        var options = new JsonOptions {
            ValidateInputDocuments = ValidateExpressionDocuments.Never,
        };
        var transform = new ExpressionJsonTransform(options);
        var doc = transform.Transform(expression);
        var roundTrip = transform.Transform(doc);

        expression.DeepEquals(roundTrip).Should().BeTrue($"round-trip expression `{expressionId}` should be deep-equal");
        expression.GetDeepHashCode().Should().Be(roundTrip.GetDeepHashCode(), $"round-trip expression `{expressionId}` should keep deep hash code");
    }

    static Expression ResolveExpression(string id)
    {
        if (ConstantTestData.GetExpression(id) is Expression c)
            return c;

        if (StatementTestData.GetExpression(id) is Expression s)
            return s;

        try
        {
            return ConstantTestDataNs.GetExpression(id);
        }
        catch (KeyNotFoundException)
        {
            // fall through
        }

        try
        {
            return StatementTestDataNs.GetExpression(id);
        }
        catch (KeyNotFoundException)
        {
            // fall through
        }

        throw new InvalidOperationException($"No expression substitute was found for id `{id}`.");
    }
}
