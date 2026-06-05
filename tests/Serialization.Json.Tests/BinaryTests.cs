// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Json;

[CollectionDefinition("JSON")]
public partial class BinaryTests(JsonTestsFixture fixture, ITestOutputHelper output) : BaseTests(fixture, output)
{
    protected override string JsonTestFilesPath => Path.Combine(_fixture.TestFilesPath, "Binary");

    [Theory]
    [MemberData(nameof(BinaryTestData.Data), MemberType = typeof(BinaryTestData))]
    public async Task BinaryToJsonTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.ToJsonTestAsync(testFileLine, expressionString, fileName);

    [Theory]
    [MemberData(nameof(BinaryTestData.Data), MemberType = typeof(BinaryTestData))]
    public async Task BinaryFromJsonTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.FromJsonTestAsync(testFileLine, expressionString, fileName);

    protected override Expression Substitute(string id) => BinaryTestData.GetExpression(id);
}
