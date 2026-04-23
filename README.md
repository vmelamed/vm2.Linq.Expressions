# vm2.Linq.Expressions — LINQ Expression Tree Serialization for .NET

[![CI](https://github.com/vmelamed/vm2.Linq.Expressions/actions/workflows/CI.yaml/badge.svg?branch=main)](https://github.com/vmelamed/vm2.Linq.Expressions/actions/workflows/CI.yaml)
[![codecov](https://codecov.io/gh/vmelamed/vm2.Linq.Expressions/branch/main/graph/badge.svg?branch=main)](https://codecov.io/gh/vmelamed/vm2.Linq.Expressions)
[![Release](https://github.com/vmelamed/vm2.Linq.Expressions/actions/workflows/Release.yaml/badge.svg?branch=main)](https://github.com/vmelamed/vm2.Linq.Expressions/actions/workflows/Release.yaml)

[![NuGet Version](https://img.shields.io/nuget/v/vm2.Linq.Expressions.Serialization.Xml)](https://www.nuget.org/packages/vm2.Linq.Expressions.Serialization.Xml/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/vm2.Linq.Expressions.Serialization.Xml.svg)](https://www.nuget.org/packages/vm2.Linq.Expressions.Serialization.Xml/)
[![GitHub License](https://img.shields.io/github/license/vmelamed/vm2.Linq.Expressions)](https://github.com/vmelamed/vm2.Linq.Expressions/blob/main/LICENSE)

<!-- TOC tocDepth:2..4 chapterDepth:2..6 -->

- [vm2.Linq.Expressions — LINQ Expression Tree Serialization for .NET](#vm2linqexpressions--linq-expression-tree-serialization-for-net)
  - [Overview](#overview)
  - [Packages](#packages)
  - [Prerequisites](#prerequisites)
  - [Install the Packages (NuGet)](#install-the-packages-nuget)
  - [Quick Start](#quick-start)
    - [Serialize to XML](#serialize-to-xml)
    - [Serialize to JSON](#serialize-to-json)
    - [Using Transforms to convert to/from XML/JSON Documents](#using-transforms-to-convert-tofrom-xmljson-documents)
    - [Deep Comparison and Deep Hash Code](#deep-comparison-and-deep-hash-code)
  - [Unsupported Expression Types](#unsupported-expression-types)
  - [Configuration Options](#configuration-options)
    - [Common Options](#common-options)
    - [XML-Specific Options](#xml-specific-options)
    - [JSON-Specific Options](#json-specific-options)
  - [Schema Validation](#schema-validation)
  - [Security Considerations](#security-considerations)
    - [Risks](#risks)
    - [Mitigations](#mitigations)
  - [Get the Code](#get-the-code)
  - [Build from the Source Code](#build-from-the-source-code)
  - [Tests](#tests)
  - [Related Packages](#related-packages)
  - [License](#license)
  - [Version History](#version-history)

<!-- /TOC -->

## Overview

This repository provides a set of .NET packages for serializing and deserializing
[LINQ expression trees](https://learn.microsoft.com/dotnet/csharp/advanced-topics/expression-trees/) to and from XML and JSON
documents. Expression trees are in-memory representations of code as data — lambda expressions, method calls, member access,
conditionals, loops, and more. .NET compiles these into executable delegates. These packages let you persist, transmit, and
reconstruct expression trees across process and machine boundaries.

The serialization produces human-readable, schema-backed documents that preserve the full structure of the expression's
abstract syntax tree (AST), including types, parameters, constants, and control flow. Both the XML and JSON formats can
optionally be validated against their respective schemas.

A companion package provides structural deep-equality comparison and hash code computation for expression trees, useful for
caching, deduplication, and testing.

## Packages

| Package | Description |
|---------|-------------|
| **[vm2.Linq.Expressions.Serialization.Xml](https://www.nuget.org/packages/vm2.Linq.Expressions.Serialization.Xml/)** | Serialize and deserialize expression trees to/from XML (`XDocument`). |
| **[vm2.Linq.Expressions.Serialization.Json](https://www.nuget.org/packages/vm2.Linq.Expressions.Serialization.Json/)** | Serialize and deserialize expression trees to/from JSON (`JsonObject`). |
| **[vm2.Linq.Expressions.DeepEquals](https://www.nuget.org/packages/vm2.Linq.Expressions.DeepEquals/)** | Structural deep-equality comparison and hash code computation for expression trees. |

## Prerequisites

- .NET 10.0 or later

## Install the Packages (NuGet)

Install the serialization package for the format you need. Each serialization package transitively references
`Serialization.Abstractions`.

- Using the dotnet CLI:

  ```bash
  dotnet add package vm2.Linq.Expressions.Serialization.Xml
  dotnet add package vm2.Linq.Expressions.Serialization.Json
  dotnet add package vm2.Linq.Expressions.DeepEquals
  ```

- From Visual Studio **Package Manager Console**:

  ```powershell
  Install-Package vm2.Linq.Expressions.Serialization.Xml
  Install-Package vm2.Linq.Expressions.Serialization.Json
  Install-Package vm2.Linq.Expressions.DeepEquals
  ```

## Quick Start

Each serialization package provides extension methods on `Expression` and static deserialization methods for the simplest
possible API. For advanced scenarios (reusing a transform instance, custom options), use `ExpressionXmlTransform` or
`ExpressionJsonTransform` directly — see the [examples](examples/) directory.

### Serialize to XML

```csharp
using System.Linq.Expressions;
using System.Xml.Linq;
using vm2.Linq.Expressions.Serialization.Xml;

Expression<Func<int, int, int>> addExpr = (a, b) => a + b;

// Expression → XML string
string xml = addExpr.ToXmlString();

// Expression → XML file
addExpr.ToXmlFile("expression.xml");

// Expression → XDocument
XDocument doc = addExpr.ToXmlDocument();

// Round-trip back to Expression
Expression roundTrip = doc.ToExpression();

// Deserialize from file / stream / string
Expression fromFile   = ExpressionXml.FromFile("expression.xml");
Expression fromString = ExpressionXml.FromString(xml);
```

### Serialize to JSON

```csharp
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using vm2.Linq.Expressions.Serialization.Json;

Expression<Func<int, int, int>> addExpr = (a, b) => a + b;

// Expression → JSON string
string json = addExpr.ToJsonString();

// Expression → JSON file
addExpr.ToJsonFile("expression.json");

// Expression → JsonObject
JsonObject doc = addExpr.ToJsonDocument();

// Round-trip back to Expression
Expression roundTrip = doc.ToExpression();

// Deserialize from file / stream / string
Expression fromFile   = ExpressionJson.FromFile("expression.json");
Expression fromString = ExpressionJson.FromString(json);
```

All methods accept an optional `XmlOptions` or `JsonOptions` parameter for customization:

```csharp
addExpr.ToXmlFile("expression.xml", new XmlOptions { Indent = true, IndentSize = 4 });
```

Stream and writer overloads are also available (sync and async):

```csharp
using var stream = new MemoryStream();
addExpr.ToXmlStream(stream);

stream.Position = 0;
Expression fromStream = ExpressionXml.FromStream(stream);
```

### Using Transforms to convert to/from XML/JSON Documents

If you need to work with the XML or JSON documents directly (e.g., to embed them in a larger document, to manipulate them before deserialization, or to integrate with other libraries), you can use the `ExpressionXmlTransform` and `ExpressionJsonTransform` classes:

```csharp
using System.Linq.Expressions;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using vm2.Linq.Expressions.Serialization.Xml;
using vm2.Linq.Expressions.Serialization.Json;

Expression<Func<int, int, int>> addExpr = (a, b) => a + b;
var xmlOptions = new XmlOptions { Indent = true };

// Reuse the transform instance for multiple serializations (avoids re-allocating visitors)
var xmlTransform = new ExpressionXmlTransform(xmlOptions);
XDocument xmlDoc  = xmlTransform.Transform(addExpr); // Expression → XDocument
Expression fromXml = xmlTransform.Transform(xmlDoc); // XDocument  → Expression

var jsonTransform  = new ExpressionJsonTransform();
JsonObject jsonDoc  = jsonTransform.Transform(addExpr);  // Expression → JsonObject
Expression fromJson = jsonTransform.Transform(jsonDoc);  // JsonObject → Expression
```

### Deep Comparison and Deep Hash Code

```csharp
using System.Linq.Expressions;
using vm2.Linq.Expressions.DeepEquals;

Expression<Func<int, int>> original  = x => x * 2;
Expression<Func<int, int>> duplicate = x => x * 2;
Expression<Func<int, int>> different = x => x + 2;

Console.WriteLine(original.DeepEquals(duplicate)); // True
Console.WriteLine(original.DeepEquals(different));  // False

// With difference explanation
if (!original.DeepEquals(different, out string difference))
    Console.WriteLine(difference); // describes the first structural difference

// Structural hash code for caching/deduplication
int hash = original.GetDeepHashCode();
```

See also the [toFromDoc.cs](https://github.com/vmelamed/vm2.Linq.Expressions/blob/main/examples/toFromDoc.cs) program in the `examples/` directory for more usage patterns and edge cases.

## Unsupported Expression Types

Both the XML and JSON serializers support virtually all LINQ expression node types — arithmetic, bitwise, logical,
comparison, assignment, increment/decrement, type operations, member access, method calls, invocations, object and collection
creation, lambdas, parameters, blocks, conditionals, loops, switches, try/catch/finally, gotos, labels, constants of all
primitive and complex types, and more.

The following expression types are **not** supported and will throw `NotImplementedExpressionException` if encountered:

| Expression Type | Reason |
|-----------------|--------|
| `DebugInfo` | Compiler-generated debugging metadata; not meaningful outside the debugger. |
| `Dynamic` | DLR dynamic dispatch (C# `dynamic`); depends on runtime binders that cannot be serialized. |
| `RuntimeVariables` | Provides runtime access to variable values; intrinsically tied to the execution context. |
| `Extension` | Custom expression nodes from third-party providers; no universal serialization is possible. |

## Configuration Options

Both `XmlOptions` and `JsonOptions` extend the shared `DocumentOptions` base class:

### Common Options

| Property | Default | Description |
|----------|---------|-------------|
| `Indent` | `true` | Indent the output document for readability. |
| `IndentSize` | `2` | Number of spaces per indentation level. |
| `Identifiers` | `Preserve` | Naming convention for identifiers: `Preserve`, `Camel`, `Pascal`, `SnakeLower`, `SnakeUpper`. |
| `TypeNames` | `FullName` | How types are written: `FullName`, `Name`, `AssemblyQualifiedName`. |
| `AddComments` | `false` | Add explanatory comments to the output document. |
| `AddLambdaTypes` | `false` | Include explicit type annotations on lambda parameters. |
| `ValidateInputDocuments` | `IfSchemaPresent` | When to validate input documents: `Never`, `Always`, `IfSchemaPresent`. |

### XML-Specific Options

| Property | Default | Description |
|----------|---------|-------------|
| `CharacterEncoding` | `"utf-8"` | Document encoding (`ascii`, `utf-8`, `utf-16`, `utf-32`, `iso-8859-1`). |
| `ByteOrderMark` | `false` | Include a BOM in the stream output. |
| `BigEndian` | `false` | Use big-endian byte order for multi-byte encodings. |
| `AddDocumentDeclaration` | `true` | Include the XML declaration. |
| `OmitDuplicateNamespaces` | `true` | Suppress redundant namespace declarations. |
| `AttributesOnNewLine` | `false` | Place each attribute on its own line. |

### JSON-Specific Options

| Property | Default | Description |
|----------|---------|-------------|
| `AllowTrailingCommas` | `false` | Allow trailing commas in input JSON. |

Options can be passed to the transform constructor:

```csharp
var options = new XmlOptions
{
    Indent                 = true,
    IndentSize             = 4,
    Identifiers            = IdentifierConventions.Camel,
    ValidateInputDocuments = ValidateExpressionDocuments.Never,
};

var transform = new ExpressionXmlTransform(options);
```

## Schema Validation

Both the XML and JSON serialization packages ship with embedded schemas that formally describe the structure of their
serialized documents. The XML package includes three XSD schemas
(`Linq.Expressions.Serialization.xsd`, `DataContract.xsd`, `Microsoft.Serialization.xsd`); the JSON package includes a JSON
Schema (`Linq.Expressions.Serialization.json`).

In many scenarios validation is unnecessary. When both the producer and consumer of a serialized expression use the same
version of these packages, the output is guaranteed to conform to the schema. **Turning validation off**
(`ValidateInputDocuments = Never`) eliminates the parsing overhead entirely and is the recommended setting for production
workloads where both ends are under your control. Validation becomes valuable when the serialized document acts as a
**contract boundary** — for example, when documents are stored long-term and may outlive the producing code, when they cross
trust boundaries between independently deployed services, or when third-party tools generate or modify the documents.

> [!WARNING]
> Performance impact can be substantial, especially for JSON deserialization with strict schema validation.
> In benchmark runs, `JSON deserialize + ValidateInputDocuments = Always` is often **orders of magnitude** slower than
> `Never`, while XML validation overhead is typically much smaller. If performance is important, prefer:
>
> 1. `ValidateInputDocuments = Never` for trusted/internal documents.
> 2. XML format when strict validation is required on hot paths.

Validation is demand-driven: schema evaluation only runs when `MustValidate` is true (that is, when validation is enabled
and a schema is available). The schema libraries are still linked into the build, but per-document validation work is not
performed unless requested.

The JSON schema validation uses [JsonSchema.Net](https://github.com/gregsdennis/json-everything). It handles the vast
majority of expression patterns correctly but has known edge cases with deeply nested recursive `$ref` combined with `oneOf`
constructs — currently 2 out of 718 tested expression patterns produce false validation failures on documents that are in
fact valid. If your project requires complete JSON validation fidelity, you can clone the source, define the
`NEWTONSOFT_SCHEMA` preprocessor symbol in the build configuration, and build against
[Newtonsoft.Json.Schema](https://www.newtonsoft.com/jsonschema) (NSJ) instead. NSJ is commercially licensed: the free tier
allows 1,000 validations per hour; a paid license is required for higher throughput. The XML schema validation uses the
built-in `System.Xml.Schema` infrastructure and has no known issues.

## Security Considerations

> [!WARNING]
> Deserializing a LINQ expression tree reconstructs executable code: the resulting `Expression` can be compiled into a delegate
> and invoked. This makes expression documents a potential vector for **remote code execution** if an attacker can supply or
> tamper with the input.

### Risks

- **Arbitrary type instantiation** — the document contains assembly-qualified type names that are resolved via reflection.
  A crafted document could reference types the consumer did not intend to load.
- **Method invocation** — `MethodCall` and `Invocation` nodes can describe calls to any accessible method, including
  process-level operations (`Process.Start`, file I/O, network access).
- **Constant injection** — `Constant` nodes can carry arbitrary literal values, including connection strings, credentials,
  or other sensitive data designed to be captured by a downstream lambda closure.

### Mitigations

1. **Treat serialized expressions as untrusted code.** Apply the same scrutiny you would give to a dynamically loaded
   assembly. Never deserialize expression documents from unauthenticated or unverified sources.

2. **Wrap the document in a signed envelope.** Before transmitting or persisting a serialized expression, embed it inside a
   signed wrapper — for example, a JWS (JSON Web Signature) or XML Digital Signature envelope. The consumer verifies the
   signature before deserializing. This ensures the document has not been tampered with in transit or at rest.

3. **Attach security metadata.** Insert additional properties or elements into the document (or its envelope) that describe
   the permitted execution context — e.g., an allow-list of assemblies and types, a maximum expression depth, or a
   principal/role that is authorized to evaluate the expression. The consumer checks these properties before compiling or
   invoking the result.

4. **Restrict the type universe.** After deserialization, walk the resulting `Expression` tree and verify that every `Type`
   reference and every `MethodInfo` belongs to an approved allow-list. Reject the expression if any node references an
   unexpected type or method.

5. **Run in a sandboxed context.** If you must evaluate expressions from less-trusted sources, compile and invoke them inside
   a restricted environment — a separate process with limited permissions, a container, or an AppDomain-equivalent isolation
   boundary — to contain the blast radius of a malicious expression.

> [!IMPORTANT]
> Schema validation does **not** provide security. A document can be schema-valid yet still contain harmful type references
> or method calls. Security requires authentication, integrity verification, and runtime constraints as described above.

## Get the Code

Clone the [GitHub repository](https://github.com/vmelamed/vm2.Linq.Expressions):

```bash
git clone https://github.com/vmelamed/vm2.Linq.Expressions.git
```

## Build from the Source Code

```bash
dotnet build
```

Or build a specific project:

```bash
dotnet build src/Serialization.Xml/Serialization.Xml.csproj
```

## Tests

The test projects are in the `test/` directory. They use MTP v2 with xUnit v3. Run each project individually:

```bash
dotnet test --project test/DeepEquals.Tests/DeepEquals.Tests.csproj
dotnet test --project test/Serialization.Abstractions.Tests/Serialization.Abstractions.Tests.csproj
dotnet test --project test/Serialization.Json.Tests/Serialization.Json.Tests.csproj
dotnet test --project test/Serialization.Xml.Tests/Serialization.Xml.Tests.csproj
```

## Related Packages

- **[vm2.Glob.Api](https://www.nuget.org/packages/vm2.Glob.Api/)** — Cross-platform glob pattern matching
- **[vm2.Ulid](https://www.nuget.org/packages/vm2.Ulid/)** — ULID generation and parsing
- **[vm2.SemVer](https://www.nuget.org/packages/vm2.SemVer/)** — Semantic versioning

## License

MIT — See [LICENSE](https://github.com/vmelamed/vm2.Linq.Expressions/blob/main/LICENSE)

## Version History

See [CHANGELOG](https://github.com/vmelamed/vm2.Linq.Expressions/blob/main/CHANGELOG.md).
