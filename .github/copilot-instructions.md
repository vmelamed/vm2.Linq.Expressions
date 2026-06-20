# Copilot Instructions for vm2.MyPackage

## Shared Conventions

Copilot MUST read and follow [CONVENTIONS.md](CONVENTIONS.md) before suggesting or making changes.

Do not duplicate shared rules here — shared instructions belong in [CONVENTIONS.md](CONVENTIONS.md) so all AI systems
use the same source of truth.

## Package-Specific Guidance

### Package Identity

- Repo: <https://github.com/vmelamed/vm2.Linq.Expressions>
- NuGet packages: `vm2.Linq.Expressions.DeepEquals`, `vm2.Linq.Expressions.Serialization.Abstractions`,
  `vm2.Linq.Expressions.Serialization.Xml`, `vm2.Linq.Expressions.Serialization.Json`
- Status: Published, stable
- Target: .NET 10.0+
- All four packages are versioned in lockstep via a single MinVer tag on this repo.

Throughout chat and internal notes, **LE** means `Linq.Expressions`. Use the full name in code and public documentation.

### What These Packages Do

#### XML/JSON serialization for .NET LINQ expression trees (ASTs)

Key design decisions in serialization:
- `vm2.Linq.Expressions.Serialization.Json.JElement` is a custom class that encapsulates `System.Text.Json.Nodes.JsonNode`and adds some methods and properties that make it similar to `System.Linq.Xml.XElement`. This makes it easier to produce or process JSON in a way that is similar to XML.
- The main idea is that the abstract syntax tree representation of LINQ expressions with a proper transformation process can be mapped to tree-like documents -- XML or JSON. And vice versa: XML or JSON documents that conform to a pre-defined schema can be transformed back into the corresponding LINQ expression trees through the deserialization process.
- The serialization process builds a hierarchical document that mirrors the expression tree:
  - Traverses the expression tree with an instance of `ExpressionTransformVisitor<TElement>` derived from `System.Linq.Expressions.ExpressionVisitor` by visiting all expression nodes in the AST
  - At each expression node the process constructs a `TElement` instance -- either a `System.Linq.Xml.XElement` for XML or`vm2.Linq.Expressions.Serialization.Json.JElement` for JSON -- representing that expression node with all its properties
  - Composes the `TElement` instance into the hierarchical structure of the XML/JSON document in memory
  - Save the hierarchical structure as an XML or JSON document, completing the serialization process
- The deserialization process is the opposite -- builds an expression tree that mirrors the hierarchical structure of the XMLor JSON document:
  - The document structure consisting of `TElement` instances (`System.Linq.Xml.XElement` or `vm2.Linq.ExpressionsSerialization.Json.JElement`) is traversed by visiting each node with an instance of  `ExpressionTransformVisitor<TElement>`
  - At each document node the process constructs a `System.Linq.Expressions.Expression` node instance, including all of its properties
  - The `Expression` nodes are composed into the hierarchical structure of the expression tree, resulting in a fully reconstructed LINQ expression tree from the XML or JSON document.
- The shared package `vm2.Serialization.Abstractions` captures the commonalities between the two transformations from/to LINQexpressions to/from XML or JSON documents and provides the foundational abstractions, conventions, and helpers that enable  consistent serialization and deserialization processes across the different formats.
  - `Conventions` defines general conventions and patterns that are followed within the serialization packages
    - `IdentifierConventions`: defines standard conventions for how to transform the .NET identifiers when stored in the  serialized representation (Camel-case, Pascal-case, Snake-lower, etc.)
    - `TypeNameConventions`: defines standard conventions for how to represent type names in the serialized representation(e.  g., full names, assembly-qualified names, short names, etc.)
    - `Transform.Identifiers`: implements the identifiers' conversions to and from serialized form according to the current  identifier convention
    - `Transform.TypeNames`: implements the type names' conversions to and from serialized form according to the current type   name convention
    - `Vocabulary` defines a set of standard terms used within the serialized representation: standard types, element names  properties, attributes, etc. E.g., `UnsignedLong`, `Private`, `Comment`, `Add`, etc. The vocabulary is common across  different serialization formats
  - `Extensions`: contains extension methods that add functionality to existing `Type`-s within the serialization packages
    - `ReaderWriterLockExtensions.cs`: contains extension methods for `ReaderWriterLockSlim` to simplify usage and improve  readability within the packages' code, particularly when dealing with concurrent access to the schemas.
    - `TypeExtensions.cs`: extends the `Type` class with additional helper methods to simplify common type-related operations within the main packages
  - `DocumentOptions.cs` - is an abstract class that encapsulates common serialization options and should be inherited by the specific serialization format classes (XML or JSON) to provide additional, format-specific options. It also controls whether the input documents should be validated against their file-format specific schema.
  - `ExpressionTransformVisitor<TElement>` is the base class visitor that drives the serialization. Each `Visit...` override pushes its children `TElement`-s onto `_elements`; when the recursion rewinds the parent pops its already fully constructed `TElement` children and composes them into itself. The type parameter `TElement` is either  `XElement` for XML serialization or `JElement` for JSON serialization.
  The stack must be empty (or contain exactly the root - the top-level `TElement` representing the entire expression tree) on completion — this invariant is a core correctness concern.
- The deserialization process **can optionally validate** the input XML or JSON document against its schema before constructing the expression tree. The respective schemas are located in `src/Serialization.Json/Schema/Linq.ExpressionsSerialization.json` and `src/Serialization.Json/Schema/Linq.Expressions.Serialization.json`. Validating JSON documents can be done with two different, third party packages:
  - by default the packages use `Json.Schema` that uses `System.Text.Json.Schema` internally and is faster with smaller memory allocation footprint, but has some minor problems in some complex, recursive schema constructs
  - the `Newtonsoft.Json.Schema` that uses the popular `Newtonsoft.Json` does not have those problems but is generally slower, has heavier memory allocation footprint, and has some licensing limitations. It also requires rebuilding of the package with defined preprocessor directive `NEWTONSOFT_SCHEMA`.

#### Structural Deep Equality Comparison

`DeepEquals` is a standalone package with no dependency outside `System.Linq.Expressions.Expression`.

For deep equality comparison of expression trees:
- **traverses two expression ASTs** node-by-node, comparing them property-by-property, in parallel using the custom `DeepEqualsComparer`
- the `DeepEqualsComparer` is not derived from `System.Linq.Expressions.ExpressionVisitor` as the comparison logic requires parallel traversal of two expression trees simultaneously, which is not supported by the single-tree visitor pattern

#### Hash Code Computation

The hash code computation for expression trees is based on the easiest visitor in these packages -- the `vm2.Linq.Expressions.DeepEquals.HashCodeVisitor`. This visitor traverses an expression AST and computes a hash code that reflects the structure and the properties' values of the nodes in the expression tree, ensuring that structurally identical expression trees produce the same hash code.

### Common Local Commands

```bash
# Build
dotnet build vm2.Linq.Expressions.slnx

# Run all tests (MTP v2 — each project is a compiled executable)
dotnet test --project tests/DeepEquals.Tests/DeepEquals.Tests.csproj
dotnet test --project tests/Serialization.Xml.Tests/Serialization.Xml.Tests.csproj
dotnet test --project tests/Serialization.Json.Tests/Serialization.Json.Tests.csproj
dotnet test --project tests/Serialization.Abstractions.Tests/Serialization.Abstractions.Tests.csproj

# Run a single test by name (MTP v2 filter syntax)
dotnet test --project tests/DeepEquals.Tests/DeepEquals.Tests.csproj --filter "MethodName_WhenCondition_ShouldOutcome"

# Pack all four NuGet packages
dotnet pack vm2.Linq.Expressions.slnx --configuration Release

# Run benchmarks (Release only)
dotnet run --project benchmarks/Linq.Expressions.Benchmarks/Linq.Expressions.Benchmarks.csproj --configuration Release -- --filter "*"

# If the benchmark tests are already built, you can run the compiled executable directly:
benchmarks/Linq.Expressions.Benchmarks/bin/Release/net10.0/Linq.Expressions.Benchmarks --help
benchmarks/Linq.Expressions.Benchmarks/bin/Release/net10.0/Linq.Expressions.Benchmarks --filter "*" # on Linux
benchmarks/Linq.Expressions.Benchmarks/bin/Release/net10.0/Linq.Expressions.Benchmarks.exe --filter "*" # on Windows
```

### Performance Characteristics

- JSON schema validation is ~1000x slower than no validation — use `JsonOptions { ValidateDocument = false }` on hot paths.
- XML schema validation overhead is much smaller (typically acceptable).
- Benchmark project (`SerializationBenchmarks.cs`) runs round-trip validation as a correctness guardrail   (`EnsureValidationSchemas()`) before measuring — strip that check if measuring pure transform speed.

### Known Trade-offs and Design Notes

- **JSON schema**: 2 known false-positive validation failures exist in the JSON schema. They are tracked but not blocking. The `NEWTONSOFT_SCHEMA` preprocessor symbol switches to the Newtonsoft.Json.Schema validator as an alternative.
- **Unsupported expression types**: `DebugInfo`, `Dynamic`, `RuntimeVariables`, and `Extension` node types are not serializable. Their visitor overrides are marked `[ExcludeFromCodeCoverage]`.
- **No tests for Abstractions**: The Abstractions package is tested indirectly via the XML and JSON test projects.   `Serialization.Abstractions.Tests` covers only infrastructure (the `[ExcludeFromCodeCoverage]` attribute on `TransformTests` means its line contribution is zero after exclusions — this is expected).
- **Coverage threshold**: Branch and full-method coverage for this repo is set to 75% (below the usual `vm2` ecosystem default of 80%). Override is applied via the `MIN_BRANCH_COVERAGE_PCT=75` GitHub repository variable. Line coverage remains at 80%.
- **Test data** XML and/or JSON files can be generated by running all tests in the `Serialization.Xml.Tests` and `Serialization.Json.Tests` projects. All tests that are missing their data files will fail but will also generate the necessary XML and/or JSON files in the `tests/Serialization.TestData/TestData/Xml` and/or `tests/Serialization.TestData/TestData/Json` directories. The test will generate a message similar to:

  ```text
  ❌ FAILED: AssignmentToJsonTestAsync(testFileLine: "0019 : ", expressionString: "a += b", fileName: "AddAssign") (15ms)
     The expected JSON does not appear to exist. Saved the actual JSON in the file `/home/valo/repos/vm2/vm2.Linq.Expressions/  tests/Serialization.TestData/TestData/Json/Assignments/AddAssign.json`.
  WARNING: error getting the EXPECTED document from `/home/valo/repos/vm2/vm2.Linq.Expressions/tests/Serialization.TestData/  TestData/Json/Assignments/AddAssign.json`:
  System.IO.FileNotFoundException: Could not find file '/home/valo/repos/vm2/vm2.Linq.Expressions/tests/Serialization.TestData/  TestData/Json/Assignments/AddAssign.json'.
  File name: '/home/valo/repos/vm2/vm2.Linq.Expressions/tests/Serialization.TestData/TestData/Json/Assignments/AddAssign.json'
     at Interop.ThrowExceptionForIoErrno(ErrorInfo errorInfo, String path, Boolean isDirError)
     ...
  Proceeding with creating the file from the actual document...
  ```

  Re-running the tests should succeed, as the necessary JSON files have now been generated.

### Security Guidance

Expression trees represent executable code. Deserializing them carries the same risk class as `BinaryFormatter`.
The serializer does not implement signing or encryption — those are the consumer's responsibility:

- Compose security in the pipeline: `Serialize → Sign → Encrypt → Store` / `Load → Decrypt → Verify → Deserialize`.
- Validate documents against the schema *before* deserialization as a defense-in-depth measure.
- XML: XMLDsig / XMLEnc (W3C standards; .NET provides `SignedXml` and `EncryptedXml`).
- JSON: JOSE RFC 7515–7520 (JWS for signatures, JWE for encryption).

### Active Work / Known Issues

- Keep an eye on the package `Json.Schema` for updates on the known bugs.
- `ReaderWriterLockExtensions.cs` is a very useful abstraction that should be moved to some common library of primitives like it (the proverbial `vm2.Utilities`? 😀)

### Prompting Notes for This Package

- When working on `ExpressionTransformVisitor<TElement>`: the Reversed Polish Notation stack invariant (`_elements`) is the core correctness concern — every `Visit...` override must push exactly one element, and the stack must be empty between top-level `Transform()` calls.
- When adding a new expression node type: add visitor override in the abstract base, implement in both XML and JSON visitors, add test data in `Serialization.TestData`, and cover in both `Xml.Tests` and `Json.Tests`.
- When writing tests: use `Serialization.TestData` shared fixtures; do not duplicate expression definitions across test projects.
- Benchmarks use BenchmarkDotNet; always run in Release configuration. The `EnsureValidationSchemas()` call in `GlobalSetup` is a correctness guard, not part of the measured path.
- `ExpressionXmlTransform` and `ExpressionJsonTransform` are safe to reuse — they create new visitors per `Transform()` call. Document this in any public-facing example.
