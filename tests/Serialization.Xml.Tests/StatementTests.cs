// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Xml;

[CollectionDefinition("XML")]
public partial class StatementTests(XmlTestsFixture fixture, ITestOutputHelper output) : BaseTests(fixture, output)
{
    protected override string XmlTestFilesPath => Path.Combine(_fixture.TestFilesPath, "Statements");

    [Theory]
    [MemberData(nameof(StatementTestData.Data), MemberType = typeof(StatementTestData))]
    [MemberData(nameof(StatementTestDataNs.Data), MemberType = typeof(StatementTestDataNs))]
    public async Task StatementToXmlTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.ToXmlTestAsync(testFileLine, expressionString, fileName);

    [Theory]
    [MemberData(nameof(StatementTestData.Data), MemberType = typeof(StatementTestData))]
    [MemberData(nameof(StatementTestDataNs.Data), MemberType = typeof(StatementTestDataNs))]
    public async Task StatementFromXmlTestAsync(string testFileLine, string expressionString, string fileName)
        => await base.FromXmlTestAsync(testFileLine, expressionString, fileName);

    protected override Expression Substitute(string id) => StatementTestData.GetExpression(id) ?? StatementTestDataNs.GetExpression(id);
}