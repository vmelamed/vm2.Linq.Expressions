// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Xml;

[CollectionDefinition("XML")]
public class TransformLoadDocumentTest(XmlTestsFixture fixture, ITestOutputHelper output) : BaseTests(fixture, output)
{
    protected override string XmlTestFilesPath => "";

    void ResetReloadSchemas(bool loadSchemas)
    {
        if (loadSchemas)
            XmlOptions.SetSchemasLocations(
                new Dictionary<string, string?> {
                    [XmlOptions.Ser] = Path.Combine(_fixture.SchemasPath, "Microsoft.Serialization.xsd"),
                    [XmlOptions.Dcs] = Path.Combine(_fixture.SchemasPath, "DataContract.xsd"),
                    [XmlOptions.Exs] = Path.Combine(_fixture.SchemasPath, "Linq.Expressions.Serialization.xsd"),
                }, true);
        else
            XmlOptions.ResetSchemas();
    }

    public static readonly TheoryData<string, ValidateExpressionDocuments, string, bool, Type?> TransformLoadDocumentData = new()
    {
        { TestLine(), ValidateExpressionDocuments.Always, "__NullObjectInvalid", false, typeof(InvalidOperationException) },
        { TestLine(), ValidateExpressionDocuments.Always, "__NullObjectInvalid", true, typeof(SchemaValidationErrorsException) },
        { TestLine(), ValidateExpressionDocuments.Always, "NullObject", false, typeof(InvalidOperationException) },
        { TestLine(), ValidateExpressionDocuments.Always, "NullObject", true, null },
        { TestLine(), ValidateExpressionDocuments.Never, "__NullObjectInvalid", false, null },
        { TestLine(), ValidateExpressionDocuments.Never, "__NullObjectInvalid", true, null },
        { TestLine(), ValidateExpressionDocuments.Never, "NullObject", false, null },
        { TestLine(), ValidateExpressionDocuments.Never, "NullObject", true, null },
        { TestLine(), ValidateExpressionDocuments.IfSchemaPresent, "__NullObjectInvalid", false, null },
        { TestLine(), ValidateExpressionDocuments.IfSchemaPresent, "__NullObjectInvalid", true, typeof(SchemaValidationErrorsException) },
        { TestLine(), ValidateExpressionDocuments.IfSchemaPresent, "NullObject", false, null },
        { TestLine(), ValidateExpressionDocuments.IfSchemaPresent, "NullObject", true, null },
    };

    [Theory]
    [MemberData(nameof(TransformLoadDocumentData))]
    public void XmlFileShouldLoadTest(string _, ValidateExpressionDocuments validate, string fileName, bool loadSchemas, Type? exceptionType)
    {
        var options = new XmlOptions() { ValidateInputDocuments = validate };

        ResetReloadSchemas(loadSchemas);

        var transform = new ExpressionXmlTransform(options);
        using var stream = new FileStream(Path.Combine(_fixture.TestLoadPath, fileName+".xml"), FileMode.Open, FileAccess.Read);
        var deserialize = () => transform.Deserialize(stream);

        switch (exceptionType)
        {
             case null:
                deserialize.Should().NotThrow().Which.Should().NotBeNull();
                break;
             case Type t when t == typeof(SchemaValidationErrorsException):
                deserialize.Should().Throw<SchemaValidationErrorsException>();
                break;
            case Type t when t == typeof(AggregateException):
                deserialize.Should().Throw<AggregateException>();
                break;
            case Type t when t == typeof(InvalidOperationException):
                deserialize.Should().Throw<InvalidOperationException>();
                break;
            default:
                Assert.Fail("Unexpected exception.");
                break;
        }
    }

    [Theory]
    [MemberData(nameof(TransformLoadDocumentData))]
    public async Task XmlFileShouldLoadTestAsync(string _, ValidateExpressionDocuments validate, string fileName, bool loadSchemas, Type? exceptionType)
    {
        var options = new XmlOptions() { ValidateInputDocuments = validate };

        ResetReloadSchemas(loadSchemas);

        var transform = new ExpressionXmlTransform(options);
        using var stream = new FileStream(Path.Combine(_fixture.TestLoadPath, fileName+".xml"), FileMode.Open, FileAccess.Read);
        var deserialize = async () => await transform.DeserializeAsync(stream);

        switch (exceptionType)
        {
             case null:
                (await deserialize.Should().NotThrowAsync()).Which.Should().NotBeNull();
                break;
             case Type t when t == typeof(SchemaValidationErrorsException):
                await deserialize.Should().ThrowAsync<SchemaValidationErrorsException>();
                break;
            case Type t when t == typeof(AggregateException):
                await deserialize.Should().ThrowAsync<AggregateException>();
                break;
            case Type t when t == typeof(InvalidOperationException):
                await deserialize.Should().ThrowAsync<InvalidOperationException>();
                break;
            default:
                Assert.Fail("Unexpected exception.");
                break;
        }
    }
}
