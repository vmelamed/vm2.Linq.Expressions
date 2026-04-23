# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

@~/.claude/CLAUDE.md
@~/repos/vm2/CLAUDE.md
@.github/CONVENTIONS.md

## Package Identity

- Repo: <https://github.com/vmelamed/vm2.Linq.Expressions>
- NuGet packages: `vm2.Linq.Expressions.DeepEquals`, `vm2.Linq.Expressions.Serialization.Abstractions`,
  `vm2.Linq.Expressions.Serialization.Xml`, `vm2.Linq.Expressions.Serialization.Json`
- Status: Published, stable
- Target: .NET 10.0+
- All four packages are versioned in lockstep via a single MinVer tag on this repo.

Throughout chat and internal notes, **LE** means `Linq.Expressions`. Use the full name in code and public documentation.

## What This Package Does

Provides structural deep equality comparison and XML/JSON serialization for .NET LINQ expression trees (ASTs).

Key design decisions:

- `DeepEquals` is standalone — no dependency on the serialization packages. It traverses two expression ASTs
  node-by-node, value-by-value, using a `DeepEqualsComparer` and a `HashCodeVisitor`.
- Serialization shares a common `Serialization.Abstractions` package (abstract visitor base, transform
  interface, options, identifier vocabulary, schema validation helpers). Published as a NuGet package so
  advanced consumers can build custom formats against it — follows the `Microsoft.Extensions.*.Abstractions` pattern.
- `ExpressionTransformVisitor<TElement>` (in Abstractions) drives serialization using an RPN (reverse Polish
  notation) stack: each `Visit...` override pushes a `TElement` onto `_elements`; the parent pops and assembles.
  The stack must be empty (or contain exactly the root) on completion — this invariant is the core correctness concern.
- Facade classes for consumers: `ExpressionXmlTransform` and `ExpressionJsonTransform`. Reuse instances
  across multiple calls — they allocate internal visitors per `Transform()` call, so the instance itself is
  stateless between calls.

## Repository Layout

```text
vm2.Linq.Expressions/
├── src/
│   ├── DeepEquals/                     → vm2.Linq.Expressions.DeepEquals
│   ├── Serialization.Abstractions/     → vm2.Linq.Expressions.Serialization.Abstractions
│   ├── Serialization.Xml/              → vm2.Linq.Expressions.Serialization.Xml
│   └── Serialization.Json/             → vm2.Linq.Expressions.Serialization.Json
├── test/
│   ├── DeepEquals.Tests/
│   ├── Serialization.Abstractions.Tests/
│   ├── Serialization.Json.Tests/
│   ├── Serialization.TestData/         # Shared test expression fixtures (not a test runner)
│   └── Serialization.Xml.Tests/
├── benchmarks/
│   └── Linq.Expressions.Benchmarks/   # EqualityBenchmarks + SerializationBenchmarks
├── examples/
│   └── toFromDoc.cs
├── changelog/
└── vm2.Linq.Expressions.slnx
```

Dependency graph:

```text
Serialization.Xml  ──┐
                     ├──▶ Serialization.Abstractions
Serialization.Json  ─┘

DeepEquals (standalone)
```

## Common Local Commands

```bash
# Build
dotnet build vm2.Linq.Expressions.slnx

# Run all tests (MTP v2 — each project is a compiled executable)
dotnet test --project test/DeepEquals.Tests/DeepEquals.Tests.csproj
dotnet test --project test/Serialization.Xml.Tests/Serialization.Xml.Tests.csproj
dotnet test --project test/Serialization.Json.Tests/Serialization.Json.Tests.csproj
dotnet test --project test/Serialization.Abstractions.Tests/Serialization.Abstractions.Tests.csproj

# Run a single test by name (MTP v2 filter syntax)
dotnet test --project test/DeepEquals.Tests/DeepEquals.Tests.csproj --filter "MethodName_WhenCondition_ShouldOutcome"

# Pack all four NuGet packages
dotnet pack vm2.Linq.Expressions.slnx --configuration Release

# Run benchmarks (Release only)
dotnet run --project benchmarks/Linq.Expressions.Benchmarks --configuration Release -- --filter "*"
```

Tests use MTP v2 (Microsoft Testing Platform v2) with xUnit v3 — they compile to standalone executables.
Use `dotnet test --project <path>` per project; solution-wide `dotnet test` is not supported with MTP v2.

## Performance Characteristics

- JSON schema validation is ~1000x slower than no validation — use `JsonOptions { ValidateDocument = false }`
  on hot paths.
- XML schema validation overhead is much smaller (typically acceptable).
- Benchmark project (`SerializationBenchmarks.cs`) runs round-trip validation as a correctness guardrail
  (`EnsureValidationSchemas()`) before measuring — strip that check if measuring pure transform speed.

## Known Trade-offs and Design Notes

- **JSON schema**: 2 known false-positive validation failures exist in the JSON schema. They are tracked but
  not blocking. The `NEWTONSOFT_SCHEMA` preprocessor symbol switches to the Newtonsoft.Json.Schema validator
  as an alternative.
- **Unsupported expression types**: `DebugInfo`, `Dynamic`, `RuntimeVariables`, and `Extension` node types
  are not serializable. Their visitor overrides are marked `[ExcludeFromCodeCoverage]`.
- **No tests for Abstractions**: The Abstractions package is tested indirectly via the XML and JSON test projects.
  `Serialization.Abstractions.Tests` covers only infrastructure (the `[ExcludeFromCodeCoverage]` attribute on
  `TransformTests` means its line contribution is zero after exclusions — this is expected).
- **Coverage threshold**: Branch and full-method coverage for this repo is set to 75% (below the ecosystem
  default of 80%). Override is applied via the `MIN_BRANCH_COVERAGE_PCT=75` GitHub repository variable.
  Line coverage remains at 80%.

## Security Guidance

Expression trees represent executable code. Deserializing them carries the same risk class as `BinaryFormatter`.
The serializer does not implement signing or encryption — those are the consumer's responsibility:

- Compose security in the pipeline: `Serialize → Sign → Encrypt → Store` / `Load → Decrypt → Verify → Deserialize`.
- Validate documents against the schema *before* deserialization as a defense-in-depth measure.
- XML: XMLDsig / XMLEnc (W3C standards; .NET provides `SignedXml` and `EncryptedXml`).
- JSON: JOSE RFC 7515–7520 (JWS for signatures, JWE for encryption).

## Active Work / Known Issues

- [Fill in when working on this package]

## Prompting Notes for This Package

- When working on `ExpressionTransformVisitor<TElement>`: the RPN stack invariant (`_elements`) is the
  core correctness concern — every `Visit...` override must push exactly one element, and the stack must
  be empty between top-level `Transform()` calls.
- When adding a new expression node type: add visitor override in the abstract base, implement in both
  XML and JSON visitors, add test data in `Serialization.TestData`, and cover in both `Xml.Tests` and
  `Json.Tests`.
- When writing tests: use `Serialization.TestData` shared fixtures; do not duplicate expression definitions
  across test projects.
- Benchmarks use BenchmarkDotNet; always run in Release configuration. The `EnsureValidationSchemas()`
  call in `GlobalSetup` is a correctness guard, not part of the measured path.
- `ExpressionXmlTransform` and `ExpressionJsonTransform` are safe to reuse — they create new visitors
  per `Transform()` call. Document this in any public-facing example.
