namespace vm2.Linq.Expressions.Serialization.Json.Tests;

[CollectionDefinition("JSON")]
public class TransformLoadDocumentTest(JsonTestsFixture fixture, ITestOutputHelper output) : BaseTests(fixture, output)
{
    protected override string JsonTestFilesPath => Path.Combine(_fixture.TestLoadPath);

    protected override Expression Substitute(string id) => throw new NotImplementedException();

    void ResetReloadSchemas(bool loadSchemas, ValidateExpressionDocuments validate)
        => _fixture.Options = loadSchemas
                                ? new(Path.Combine(_fixture.SchemasPath, "Linq.Expressions.Serialization.json")) {
                                    Indent = true,
                                    IndentSize = 4,
                                    AddComments = true,
                                    AllowTrailingCommas = true,
                                    ValidateInputDocuments = validate,
                                }
                                : new() {
                                    Indent = true,
                                    IndentSize = 4,
                                    AddComments = true,
                                    AllowTrailingCommas = true,
                                    ValidateInputDocuments = validate,
                                };

    public static readonly TheoryData<string, ValidateExpressionDocuments, string, bool, Type?> TransformLoadDocumentData = new()
    {
        { TestLine(), ValidateExpressionDocuments.Always,          "__NullObjectInvalid", false, typeof(InvalidOperationException)       },
        { TestLine(), ValidateExpressionDocuments.Always,          "__NullObjectInvalid", true,  typeof(SchemaValidationErrorsException) },
        { TestLine(), ValidateExpressionDocuments.Never,           "__NullObjectInvalid", false, null                                    },
        { TestLine(), ValidateExpressionDocuments.Never,           "__NullObjectInvalid", true,  null                                    },
        { TestLine(), ValidateExpressionDocuments.IfSchemaPresent, "__NullObjectInvalid", false, null                                    },
        { TestLine(), ValidateExpressionDocuments.IfSchemaPresent, "__NullObjectInvalid", true,  typeof(SchemaValidationErrorsException) },
        { TestLine(), ValidateExpressionDocuments.Always,          "NullObject",          false, typeof(InvalidOperationException)       },
        { TestLine(), ValidateExpressionDocuments.Always,          "NullObject",          true,  null                                    },
        { TestLine(), ValidateExpressionDocuments.Never,           "NullObject",          false, null                                    },
        { TestLine(), ValidateExpressionDocuments.Never,           "NullObject",          true,  null                                    },
        { TestLine(), ValidateExpressionDocuments.IfSchemaPresent, "NullObject",          false, null                                    },
        { TestLine(), ValidateExpressionDocuments.IfSchemaPresent, "NullObject",          true,  null                                    },
    };

    [Theory]
    [MemberData(nameof(TransformLoadDocumentData))]
    public void JsonFileShouldLoadTest(string _, ValidateExpressionDocuments validate, string fileName, bool reloadSchema, Type? exceptionType)
    {
        ResetReloadSchemas(reloadSchema, validate);

        var transform = new ExpressionJsonTransform(_fixture.Options);
        using var stream = new FileStream(Path.Combine(JsonTestFilesPath, fileName+".json"), FileMode.Open, FileAccess.Read);
        var deserialize = () => transform.Deserialize(stream);

        if (exceptionType is null)
            deserialize.Should().NotThrow().Which.Should().NotBeNull();
        else
        if (exceptionType == typeof(SerializationException))
            deserialize.Should().Throw<SerializationException>();
        else
        if (exceptionType == typeof(AggregateException))
            deserialize.Should().Throw<AggregateException>();
        else
        if (exceptionType == typeof(InvalidOperationException))
            deserialize.Should().Throw<InvalidOperationException>();
        else
        if (exceptionType == typeof(SchemaValidationErrorsException))
            deserialize.Should().Throw<SchemaValidationErrorsException>();
        else
            Assert.Fail("Unexpected exception.");
    }
}
