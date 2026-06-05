// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Json;

[CollectionDefinition("JSON")]
public partial class UnaryTests(JsonTestsFixture fixture, ITestOutputHelper output) : BaseTests(fixture, output)
{
    protected override string JsonTestFilesPath => Path.Combine(_fixture.TestFilesPath, "Unary");

    [Theory]
    [MemberData(nameof(UnaryTestData.Data), MemberType = typeof(UnaryTestData))]
    public async Task UnaryToJsonTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.ToJsonTestAsync(testFileLine, expressionString, fileName);

    [Theory]
    [MemberData(nameof(UnaryTestData.Data), MemberType = typeof(UnaryTestData))]
    public async Task UnaryFromJsonTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.FromJsonTestAsync(testFileLine, expressionString, fileName);

    protected override Expression Substitute(string id) => UnaryTestData.GetExpression(id);
}
