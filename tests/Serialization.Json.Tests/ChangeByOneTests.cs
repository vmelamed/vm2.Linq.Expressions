// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Json;

[CollectionDefinition("JSON")]
public partial class ChangeByOneTests(JsonTestsFixture fixture, ITestOutputHelper output) : BaseTests(fixture, output)
{
    protected override string JsonTestFilesPath => Path.Combine(_fixture.TestFilesPath, "ChangeByOne");

    [Theory]
    [MemberData(nameof(ChangeByOneTestData.Data), MemberType = typeof(ChangeByOneTestData))]
    public async Task ChangeByOneToJsonTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.ToJsonTestAsync(testFileLine, expressionString, fileName);

    [Theory]
    [MemberData(nameof(ChangeByOneTestData.Data), MemberType = typeof(ChangeByOneTestData))]
    public async Task ChangeByOneFromJsonTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.FromJsonTestAsync(testFileLine, expressionString, fileName);

    protected override Expression Substitute(string id) => ChangeByOneTestData.GetExpression(id);
}
