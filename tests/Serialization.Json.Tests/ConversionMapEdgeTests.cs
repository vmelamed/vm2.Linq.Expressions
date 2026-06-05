// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.Linq.Expressions.Serialization.Json;

public class ConversionMapEdgeTests
{
    public static readonly TheoryData<string, string, string, string> InvalidConstantData = new()
    {
        { TestLine(), "char", "\"\"", "char" },
        { TestLine(), "long", "true", "long" },
        { TestLine(), "unsignedLong", "[]", "unsigned long" },
        { TestLine(), "intPtr", "\"   \"", "IntPtr" },
        { TestLine(), "unsignedIntPtr", "\"   \"", "IntPtr" },
        { TestLine(), "dateTime", "42", "DateTime" },
        { TestLine(), "decimal", "\" \"", "Decimal" },
        { TestLine(), "guid", "42", "Guid" },
        { TestLine(), "uri", "\" \"", "Uri" },
        { TestLine(), "duration", "\"Nope\"", "TimeSpan" },
    };

    [Theory]
    [MemberData(nameof(InvalidConstantData))]
    public void FromJson_InvalidConstantValue_ShouldThrowSerializationException(string _, string typeName, string rawValue, string messageFragment)
    {
        var json = BuildConstantJson(typeName, rawValue);
        var node = JsonNode.Parse(json);
        node.Should().NotBeNull();

        var options = new JsonOptions {
            ValidateInputDocuments = ValidateExpressionDocuments.Never,
        };
        var transform = new ExpressionJsonTransform(options);

        Action act = () => transform.Transform(node!.AsObject());

        act.Should().Throw<SerializationException>().WithMessage($"*{messageFragment}*");
    }

    static string BuildConstantJson(string typeName, string rawValue)
        =>
            $$"""
            {
              "$schema": "urn:schemas-vm-com:Linq-Expressions-Serialization-Json",
              "expression": {
                "constant": {
                  "{{typeName}}": {{rawValue}}
                }
              }
            }
            """;
}
