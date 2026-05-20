# Project Guidance for Copilot & Contributors

## Style Sources (Do Not Duplicate Here)

Refer to:

- .editorconfig (authoritative code style + analyzers)
- Directory.Packages.props (centralized package versions)
- Directory.Build.props / .targets (shared build config)
- Each project's *.csproj
- Per project: usings.cs (global usings)

Keep this file focused on *intent* and *preferences* so Copilot infers patterns.

## Project Structure and Tooling

- **All new solutions should use the new `.slnx` format** (XML-based solution format introduced in Visual Studio 2022)
  - Easier to read and merge in source control
  - Better tooling support for modern .NET workflows
  - Use `dotnet new sln -n solution-name` to create .slnx solutions
- **All new projects should strive to use Central Package Management (CPM)**
  - Define package versions in `Directory.Packages.props`
  - Use `<PackageReference Include="..." />` without `Version` attribute in project files
  - Enable with `<ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>`
  - Benefits: consistent versions across projects, easier updates, reduced merge conflicts
  - See existing `Directory.Packages.props` for reference implementation
- **All new projects should use SDK-style project files**
  - Simplified XML format
  - Implicit package references for common SDKs
  - Easier to read and maintain
- **All new projects should use implicit global usings where applicable**
  - Define common namespaces in `usings.cs` files
  - Reduces boilerplate in individual source files
  - Improves readability
- **All new projects should have the following folder structure:**
  - `.github/` for GitHub workflows and issue templates (optional)
  - `.github/workflows/` for GitHub Actions workflows (optional)
  - `src/` for source code
  - `test/` for test projects (very rarely optional - only for test and other tiny utilities)
  - `benchmarks/` for performance benchmarks (desirable)
  - `examples/` for sample code and usage examples (desirable)
  - `docs/` for documentation (optional)
  - `tools/` for compiled utilities (optional)
  - `scripts/` for build and deployment scripts (optional)

## General Coding Conventions

- Use file-scoped namespaces.
- Prefer `readonly record struct` for small immutable value objects.
- Prefer `internal` over `public` unless part of an intentional API surface.
- Use expression-bodied members when trivial and not harming readability.
- Use `sealed` by default for classes unless extensibility is required.
- Prefer `var` when the type is obvious from the right-hand side; otherwise be explicit.
- Always honor nullable reference types (treat warnings as design feedback).
- Avoid static mutable state.
- Use dependency injection over service locator.
- Prefer guard clauses at method start (throw early, no nested pyramid).
- Prefer pattern matching (`is`, `switch expressions`) over `if`/`else` chains when semantic.
- Do not use curly braces for single-line blocks unless improving readability.
- It's OK to use #region / #endregion for logical grouping in larger files.

## Language and Writing Quality

**IMPORTANT: The project owner is a non-native English speaker.**

- **ALWAYS** check spelling, grammar, and technical English style in all documentation and comments
- **ALWAYS** recommend better wording for:
  - Sentences that could be clearer or more concise
  - Paragraphs with awkward flow or structure
  - Entire documents that could be reorganized for better readability
- Prefer active voice over passive voice
- Use technical terminology correctly and consistently
- When suggesting changes, explain WHY the alternative is better
- Examples of improvements:
  - ❌ "The pattern is being matched by the enumerator"
  - ✅ "The enumerator matches the pattern"
  - ❌ "For doing the search"
  - ✅ "To search" or "For searching"

## Documentation and Attribution

**ALWAYS include proper credits and references:**

- When implementing specifications (POSIX, RFC, etc.), cite the source:
  - Include title, URL, and date accessed
  - Example: "Based on POSIX.2 glob specification (<https://www.man7.org/linux/man-pages/man7/glob.7.html>)"
- When using algorithms or concepts from papers/articles, cite them
- When adapting code patterns from other projects, acknowledge them
- Include links to relevant documentation for users to learn more
- Standard attribution format:

      ## References

      - [POSIX Glob Specification](https://www.man7.org/linux/man-pages/man7/glob.7.html) - The Linux man-pages project
      - [RFC XXXX](https://www.rfc-editor.org/rfc/rfcXXXX) - Title (if applicable)
      - [Article/Blog Title](URL) - Author Name (if used)

## Markdown File Generation

### Always follow the Markdown lint default rules or defined in the `.markdownlint.json` file in the repository root

**IMPORTANT: When generating complete Markdown (.md) file content:**

- **ALWAYS** wrap the entire file content in a code fence using **TILDES** (`~~~markdown` ... `~~~`)
- This prevents nesting issues with triple-backtick code blocks inside the Markdown content
- **Inside the Markdown content**, use **4-space indentation** for code blocks (not triple backticks)
- This avoids nested code fence conflicts while maintaining valid Markdown syntax
- Example format:

      ~~~markdown
      # README Title

      ## Installation

          dotnet tool install -g package-name

      ## Examples

          command "**/*.cs"
          command "*.json" -d ~/config

      ## References

      - [Source Documentation](https://example.com) - Proper attribution
      ~~~

- Benefits:
  - ✅ No nested code fence conflicts (tildes vs backticks)
  - ✅ Copy-paste ready content for users
  - ✅ Valid Markdown syntax (CommonMark compliant)
  - ✅ Works in Visual Studio Copilot pane
  - ✅ Renders correctly on GitHub, VS Code, and other Markdown viewers
- For partial edits or snippets, normal Markdown rendering is acceptable

### Ordered Lists

When creating ordered lists in Markdown documents, use `1.` for all items instead of sequential numbering (1., 2., 3., etc.). This approach provides several benefits:

- Easier to reorder items without renumbering
- Simpler to add new items without adjusting subsequent numbers
- Less maintenance when deleting items
- Markdown renderers automatically number items correctly

**Example:**

    ~~~markdown
    1. First item
    1. Second item
    1. Third item
    ~~~

This renders as:

1. First item
2. Second item
3. Third item

## File Modification Guidelines

### YAML conventions

- Prefer kebab-case in YAML (e.g., `my-setting`) and avoid snake_case unless required by an external schema or by MSBuild/C# interop values.

### General

- **ALWAYS preserve existing comments** - Comments provide context, rationale, and documentation
- Existing comments can be modified if this improves English spelling and phrasing, as well as clarity or correctness.
- Also, existing comments can be modified if they contain inaccuracies or outdated information or they need to reflect modified behavior.
- When adding new code, include appropriate comments explaining:
  - Why the code exists (not just what it does)
  - Non-obvious implementation details
  - Business logic or domain-specific rationale
  - Temporary workarounds or TODOs with context
- Do not remove commented-out code without explicit permission
- Preserve YAML/JSON comments in configuration files (they document intentions and alternatives)
- When refactoring, update affected comments to maintain accuracy
- For workflow files (GitHub Actions, CI/CD):
  - Preserve commented-out alternative implementations
  - Keep notes about disabled features or experimental options
  - Maintain explanatory comments about concurrency, permissions, and environment setup

## CI / GitHub Actions — Project Registration

**CRITICAL: When adding a new project, register it in `.github/workflows/CI.yaml`.**

The CI workflow uses JSON arrays in `env:` to drive build, test, benchmark, and pack jobs.
When you add a new test project or packable project, you **must** add its path to the appropriate list:

| Array               | Purpose                      | Example entry                                    |
|---------------------|------------------------------|--------------------------------------------------|
| `BUILD_PROJECTS`    | Solutions/projects to build  | `"vm2.Linq.Expressions.slnx"`                   |
| `TEST_PROJECTS`     | Test projects to run         | `"test/DeepEquals.Tests/DeepEquals.Tests.csproj"` |
| `BENCHMARK_PROJECTS`| Benchmark projects to run    | `"benchmarks/DeepEquals.Benchmarks/DeepEquals.Benchmarks.csproj"` |
| `PACKAGE_PROJECTS`  | Projects to pack as NuGet    | `"vm2.Linq.Expressions.slnx"` (packs all `IsPackable=true` projects) |

Also add the new project to `vm2.Linq.Expressions.slnx` under the appropriate solution folder (`/src/`, `/test/`, `/benchmarks/`).

---

## Repository Architecture

### Overview

This repository produces **four NuGet packages** from a single solution, versioned in lockstep via MinVer.
The packages cover LINQ expression tree (AST) comparison and serialization.

### Abbreviation Convention

Throughout chat and internal notes, **LE** means `Linq.Expressions`. Use the full name in code, namespaces, and public documentation.

### Packages and Assemblies

| NuGet Package | Project Folder | Namespace | Description |
|---|---|---|---|
| `vm2.Linq.Expressions.DeepEquals` | `src/DeepEquals/` | `vm2.Linq.Expressions.DeepEquals` | Traverses the ASTs of two expression trees and compares them node-by-node, value-by-value. Standalone; no serialization dependency. |
| `vm2.Linq.Expressions.Serialization.Abstractions` | `src/Serialization.Abstractions/` | `vm2.Linq.Expressions.Serialization` | Shared abstractions for the serialization packages: common options, abstract visitor base, transform interface, type-name conventions, identifier vocabulary, and document validation helpers. |
| `vm2.Linq.Expressions.Serialization.Xml` | `src/Serialization.Xml/` | `vm2.Linq.Expressions.Serialization.Xml` | Serializes expression tree ASTs to XML documents with a defined schema. Depends on Serialization.Abstractions. |
| `vm2.Linq.Expressions.Serialization.Json` | `src/Serialization.Json/` | `vm2.Linq.Expressions.Serialization.Json` | Serializes expression tree ASTs to JSON documents with a defined schema. Depends on Serialization.Abstractions. |

### Dependency Graph

```text
Serialization.Xml ──┐
                    ├──▶ Serialization.Abstractions
Serialization.Json ─┘

DeepEquals (standalone, no dependency on serialization)
```

### Repository Layout

```text
vm2.Linq.Expressions/
├── src/
│   ├── DeepEquals/                        → vm2.Linq.Expressions.DeepEquals
│   ├── Serialization.Abstractions/        → vm2.Linq.Expressions.Serialization.Abstractions
│   ├── Serialization.Xml/                 → vm2.Linq.Expressions.Serialization.Xml
│   └── Serialization.Json/                → vm2.Linq.Expressions.Serialization.Json
├── test/
│   ├── DeepEquals.Tests/
│   ├── Serialization.Xml.Tests/
│   └── Serialization.Json.Tests/
├── benchmarks/
│   └── (as needed per package)
├── examples/
├── docs/
├── changelog/
├── vm2.Linq.Expressions.slnx
├── Directory.Build.props
├── Directory.Packages.props
└── ...
```

### Design Decisions

1. **Single repo, multiple packages** — All four assemblies live in one repository and one solution.
   They share `Directory.Build.props`, `Directory.Packages.props`, and release in lockstep from a single MinVer tag.
   This avoids the overhead and DLL-hell risk of coordinating four separate repos.

1. **Abstractions as a public NuGet package** — The shared serialization assembly is published as
   `vm2.Linq.Expressions.Serialization.Abstractions` (not internal-only). Advanced consumers who build
   custom serialization formats can reference it directly. This follows the `Microsoft.Extensions.*.Abstractions` pattern.

1. **No tests for Abstractions** — The Abstractions package contains primarily interfaces, abstract base classes,
   enums, options records, and vocabulary constants. These are tested indirectly through the XML and JSON test projects.

1. **DeepEquals is standalone** — Although it originated from unit-test necessities, it solves a general problem
   (structural equality of expression trees) and has no dependency on the serialization packages.

1. **Pack via solution** — CI uses `dotnet pack vm2.Linq.Expressions.slnx` to produce all four packages in one
   invocation. Test and benchmark projects set `<IsPackable>false</IsPackable>`.

### Security Guidance

**Expression trees represent executable code.** Deserializing expression trees carries the same class of risk
as `BinaryFormatter` or unrestricted XML deserialization.

- **Document signing and encryption are cross-cutting concerns** — the serializer does not implement them.
- Consumers should compose security in their pipeline: `Serialize → Sign → Encrypt → Store` and
  `Load → Decrypt → Verify → Deserialize`.
- Prefer `Stream`-based APIs so consumers can layer `CryptoStream` transparently.
- Validate documents against the schema *before* deserialization as a defense-in-depth measure.
- Recommended standards for securing serialized documents:
  - **XML**: XMLDsig (W3C XML Signature) and XMLEnc (W3C XML Encryption) — .NET provides `SignedXml` and `EncryptedXml`.
  - **JSON**: JOSE (RFC 7515–7520) — JWS for signatures, JWE for encryption.
  - **CBOR** (if applicable in the future): COSE (RFC 9052/9053).
- Include a prominent security warning in README and XML doc comments.

### Serialization.Abstractions — Key Contents

The shared abstractions package provides:

- **Common options** — Serialization/deserialization configuration
- **Abstract visitor base** — For traversing expression tree nodes
- **Transform interface** — Common contract for serialization transforms
- **Document validation** — Schema validation helpers
- **Identifier vocabulary** — Shared naming conventions for serialized elements/properties
- **Type-name conventions** — Consistent type representation across formats
- **Dictionaries** — Shared lookup tables for node types, member types, etc.

## Pre-PR Quality Checks

**Before pushing a branch or creating a PR, proactively review for correctness, performance, and edge cases.**

- **Review the diff.** Ask Copilot: *"Review the diff on this branch for correctness, performance, and edge cases"* (`git diff main...HEAD`). This is the mode where issues are most likely to be caught — focused review with full context.
- **Review non-trivial files as you go.** When finishing a complex file, ask *"Anything wrong here?"* while context is fresh. Cheaper than a full-branch review later.
- **Verify runtime behavior for reflection-heavy code.** Static analysis (including Copilot review) often gets reflection wrong — e.g., `Enumerable.Cast<T>()` short-circuiting on inputs that already implement `IEnumerable<T>`, `Task<T>` inheriting from `Task`, LINQ to XML auto-cloning parented nodes. When writing such code, verify assumptions with a quick test or explicit question.
- **Check test coverage on new/changed paths.** Run tests with coverage and inspect uncovered branches in new code. An untested code path is an unverified assumption.
- **Treat Copilot auto-review as a noisy first pass.** It will have false positives — dismiss those quickly. The real value is the true positives it catches that you might miss.
