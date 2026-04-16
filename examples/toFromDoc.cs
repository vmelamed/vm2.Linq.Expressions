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

Expression<Func<int, int, int>> expr1 = (x, y) => x * y + 2;

var expr1Str = expr1.ToXmlString();
WriteLine($"Serialized XML:\n{expr1Str}");

var expr1_1 = ExpressionXml.FromString(expr1Str);
WriteLine($"Deserialized expression: {expr1_1}");

WriteLine($"Round-trip equality: {expr1.DeepEquals(expr1_1)}");

WriteLine("""
    --------------------------
    JSON round-trip:
    --------------------------
    """);

expr1Str = expr1.ToJsonString();
WriteLine($"Serialized JSON:\n{expr1Str}");

expr1_1 = ExpressionJson.FromString(expr1Str);
WriteLine($"Deserialized expression: {expr1_1}");
WriteLine($"Round-trip equality: {expr1.DeepEquals(expr1_1)}");

WriteLine("""
    --------------------------
    JSON advanced round-trip:
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
catch (System.IO.IOException x)
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

expr1_1 = transform.Transform(document);
WriteLine($"Deserialized expression: {expr1_1}");
WriteLine($"Round-trip equality: {expr1.DeepEquals(expr1_1)}");
