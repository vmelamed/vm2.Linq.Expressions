#!/usr/bin/env dotnet

// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

#:property TargetFramework=net10.0
#:project ../src/Serialization.Json/Serialization.Json.csproj
#:project ../src/Serialization.Xml/Serialization.Xml.csproj
#:project ../src/DeepEquals/DeepEquals.csproj

using static System.Console;
using System.Linq.Expressions;

using vm2.Linq.Expressions.Serialization;
using vm2.Linq.Expressions.Serialization.Xml;
using vm2.Linq.Expressions.Serialization.Json;
using vm2.Linq.Expressions.DeepEquals;

WriteLine("""
    --------------------------
    XML round-trip:
    --------------------------
    """);

Expression<Func<int, int, int>> expr1 = (x, y) => x * y + 42;
WriteLine($"Original expression: {expr1}");

// serialize to XML
var expr1Str = expr1.ToXmlString();
WriteLine($"Serialized XML:\n{expr1Str}");
// deserialize from XML
var expr1_1 = (Expression<Func<int, int, int>>)ExpressionXml.FromString(expr1Str);
WriteLine($"Deserialized expression: {expr1_1}");

WriteComparisonResults("XML round-trip", expr1, expr1_1);

WriteLine("""
    --------------------------
    JSON round-trip:
    --------------------------
    """);

// serialize to JSON
expr1Str = expr1.ToJsonString();
WriteLine($"Serialized JSON:\n{expr1Str}");
// deserialize from JSON
expr1_1 = (Expression<Func<int, int, int>>)ExpressionJson.FromString(expr1Str);

WriteComparisonResults("JSON round-trip", expr1, expr1_1);

// advanced usage with transform, options and schema validation
WriteLine("""
    --------------------------
    JSON round-trip (advanced):
    --------------------------
    """);

var jsonOptions = new JsonOptions
{
    Indent = false,
    Identifiers = IdentifierConventions.SnakeLower,
    TypeNames = TypeNameConventions.AssemblyQualifiedName,
    AddLambdaTypes = false,
    ValidateInputDocuments = ValidateExpressionDocuments.Always,
};

bool validate=true;
try
{
    jsonOptions.LoadSchema(
        Environment.ExpandEnvironmentVariables(
            "%VM2_REPOS%/vm2.Linq.Expressions/src/Serialization.Json/Schema/Linq.Expressions.Serialization.json"));
}
catch (IOException x)
{
    WriteLine(x.Message);
    WriteLine("Will not validate the JSON document against the schema. Fix the path directly in toFromDoc.cs or set the environment variable VM2_REPOS to the path of the vm2 repositories.");
    validate = false;
    jsonOptions.ValidateInputDocuments = ValidateExpressionDocuments.Never;
}

var transform = new ExpressionJsonTransform(jsonOptions);
var document = transform.Transform(expr1);

expr1Str = document.ToJsonString(jsonOptions.JsonSerializerOptions);
WriteLine($"Serialized JSON:\n{expr1Str}");

if (validate)
    jsonOptions.Validate(document); // if it does not throw exception - it is valid

WriteComparisonResults("JSON (adv) round-trip", expr1, expr1_1);

void WriteComparisonResults(
    string label,
    Expression<Func<int, int, int>> serialized,
    Expression<Func<int, int, int>> deserialized)
{
    WriteLine($"{label} equality: {serialized.DeepEquals(deserialized)}");

    var hc1 = serialized.GetDeepHashCode();
    var hc2 = deserialized.GetDeepHashCode();

    WriteLine($"{label} hash-code equality: {hc1 == hc2} (hash codes: {hc1} and {hc2})");

    var x = Random.Shared.Next(1, 10);
    var y = Random.Shared.Next(1, 10);

    var result1 = serialized.Compile()(x, y);
    var result1_1 = deserialized.Compile()(x, y);

    WriteLine($"Compiled deserialized expression result: {result1_1} (should be {x}*{y}+42={result1})");
}
