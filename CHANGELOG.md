# Changelog

## v3.0.1 - 2026-06-11

See prereleases below.

## v3.0.1-preview.3 - 2026-06-11

### Internal

- changed comments and UI for clarity [skip ci]

## v3.0.1-preview.2 - 2026-06-11

### Fixed

- add operationsPerInvoke to benchmark methods for consistency
- update serialization methods to remove nullable types

## v3.0.1-preview.1 - 2026-06-09

### Internal

- promote to stable v3.0.0 [skip ci]
- update changelog for v3.0.0 [skip ci]
- Bump the minor-and-patch group with 1 update
- update vm2.TestUtilities to version 2.1.0
- update vm2.TestUtilities to version 2.1.0 across all projects

## v3.0.0 - 2026-06-08

See prereleases below.

## v3.0.0-preview.1 - 2026-06-08

### Added

- add max generation collection thresholds to CI environment variables [skip ci]

### Fixed

- fix spelling and remove duplicate rows
- streamline the dev. environment for multi-OS/multi-IDE and for consistent configuration of AI
- add SPDX
- correct type for unsignedIntPtr in ConversionMapEdgeTests
- update commit prefix for git-cliff to include 'tests' and adjust documentation
- custom types must be version 1.0.0.0
- update version numbers in nullable enum and struct test data

### Internal

- diff-shared changes
- update changelog for v2.0.0 [skip ci]
- add ExpressionTransformVisitorTests for stack operations and generic visit behavior
- add new tests for JSON and XML serialization, including hash code round-trip and conversion edge cases
- regenerated test data
- add test data
- simplify exception handling in XML deserialization tests
- serialization methods and add null checks
- JSON and XML serialization code for improved null handling and argument validation
- **BREAKING:** lower the public interface surface by change accessibility of XNodeExtensions, ToJsonTransformVisitor, and ToXmlTransformVisitor to internal; small changes after copilot comments
- **BREAKING:** lower the public interface surface by making FromJsonTransformVisitor, JElement, and JsonNodeExtensions internal. Add InternalsVisibleTo for Serialization.Json.Tests

## v2.0.0 - 2026-06-04

See prereleases below.

## v2.0.0-preview.1 - 2026-06-04

### Fixed

- Explicitly specify the OutputType
- update vm2.TestUtilities package version to 2.0.2 in multiple test projects
- change test settings to accommodate Visual Studio 2026
- Change the end-of-lines settings to get the native conventions: on Win - CRLF, and on Unix-like: just LF
- **BREAKING:** make the transform culture invariant
- Remove unnecessary dependencies from package lock files
- Explicitly specify the OutputType
- disable AoT, refactor Directory.Build.props
- add missing files and directories to the solution; merge changes to the lockfiles
- resolve conflicts with main
- clean up package lock files and remove unused test dependencies
- regenerated the test data; removed the test folder (tests is the valid one)

### Internal

- promote to stable v1.1.0
- update changelog for v1.1.0
- Bump the minor-and-patch group with 1 update
- rename test/ to tests
- finished renaming test/ to tests/, sync by diff-shared
- Simplify .NET type names in serialization and snapshots
- Update package management and project references
- **BREAKING:** following conventions
- rename test/ to tests
- finished renaming test/ to tests/, sync by diff-shared
- Update package management and project references
- minor changes

## v1.1.0 - 2026-05-21

See prereleases below.

## v1.1.0-preview.1 - 2026-05-21

### Added

- add telemetry opt-out and first-time experience skip for .NET CLI [skip ci]
- add NSubstitute package references to test projects
- add skip flags

### Fixed

- commit prefix
- change the dependabot's commit message prefix
- description of skip-packages [skip ci]
- correct wording in conventions for merge or copy action
- improve wording in CI warning message and conventions documentation

### Internal

- promote to stable v1.0.2 [skip ci]
- update changelog for v1.0.2 [skip ci]
- improve changelog formatting for clarity and consistency
- address copilot comments
- copy CONVENTIONS.md; cosmetic changes in the yaml
- Bump the minor-and-patch group with 15 updates
- Update package references to version 10.0.8 for Microsoft.Extensions libraries and bump vm2.TestUtilities to 1.5.0 in Serialization tests
- Add Copilot instructions and project guidance documentation
- regenerate *.lock.json
- sync with diff-shared.sh
- update vm2.TestUtilities to version 1.5.1
- Update vm2.TestUtilities to version 1.5.1 and remove unused dependencies in package.lock files across Serialization.Abstractions.Tests, Serialization.Json.Tests, Serialization.TestData, and Serialization.Xml.Tests.

### deps

- Bump the minor-and-patch group with 1 update

## v1.0.2 - 2026-04-24

See prereleases below.

## v1.0.2-preview.11 - 2026-04-24

### Internal

- clean up changelog formatting and improve consistency

## v1.0.2-preview.6 - 2026-04-23

### Fixed

- update regex patterns for source exclusions in coverage settings for Windows and Linux compatibility

### Internal

- update README code examples for completeness and clarity

## v1.0.2-preview.4 - 2026-04-22

### Fixed

- correct invalid prerelease version headers in CHANGELOG

## v1.0.2-preview.3 - 2026-04-22

### Internal

- add shared conventions document for vm2 packages for claude
- diff-shared

## v1.0.2-preview.2 - 2026-04-22

### Internal

- clean up changelog formatting and improve readability
- upgrade dependencies
- remove packages.lock.json file
- update SDK version to 10.0.203 in global.json and in the lock files
- remove deprecated package references and clean up lock files
- update dependency and the git-cliff *.toml files

## v1.0.2-preview.1 - 2026-04-21

### Fixed

- restore changelog content and resolve merge artifacts

### Internal

- clean up changelog by removing duplicate section.

## v1.0.1 - 2026-04-20

See prereleases below.

## v1.0.0 - 2026-04-20

See prereleases below.

## v1.0.0-preview.1 - 2026-04-20

### Added

- LINQ expressions XML serializer and schema
- added facade static classes for ease of use. Changed the README.md

### Fixed

- restore optional NEWTONSOFT_SCHEMA path and fix benchmark schema setup
- Update package references and compile and run example expressions after round trip in toFromDoc.cs
- Addressed GH Copilot review comments, e.g. Improve null handling in HashCodeVisitor and update Uri transformation in FromXmlDataTransform.Maps
- the packaged projects [skip ci]

### Internal

- initial scaffold
- XML serialization tests
- add ExcludeFromCodeCoverage attribute and XmlOptions tests
- add unary operators and XML test data for new type D
- Update package references to version 10.0.6 for Microsoft.Extensions libraries in packages.lock.json
- update SDK version to 10.0.202 in global.json
- clean up code structure and remove unused code blocks
- add tests for the JSON serializer
- added more tests for coverage
- namespaces and update documentation
- Add JSON and XML serialization tests for expression facades
- changed dependency version System.Security.Cryptography.Xml 10.0.6
- Enhance method resolution for generic types in JSON and XML serialization
- Update package dependencies and remove obsolete Newtonsoft.Json.Schema
- Update package dependencies to include JsonSchema.Net and related transitive dependencies
- Add performance warning for JSON deserialization with strict schema validation
- Add pull request template to GitHub folder
- fix/copilot comments (#2)

### Performance

- Update benchmarks to precompute serialized payloads and improve setup validation

## Usage Notes

> [!TIP] Be disciplined with your commit messages and let git-cliff do the work of updating this file.
>
> **Added:**
>
> - add new features
> - commit prefix for git-cliff: `feat:`
>
> **Changed:**
>
> - add behavior changes
> - commit prefix for git-cliff: `refactor:`
>
> **Fixed:**
>
> - add bug fixes
> - commit prefix for git-cliff: `fix:`
>
> **Performance**
>
> - add performance improvements
> - commit prefix for git-cliff: `perf:`
>
> **Security**
>
> - add security-related changes
> - commit prefix for git-cliff: `security:`
>
> **Removed**
>
> - add removed/obsolete items
> - commit prefix for git-cliff: `revert:` or `remove:`
>
> **Internal**
>
> - add internal changes
> - commit prefix for git-cliff: `refactor:`, `doc:`, `docs:`, `style:`, `test:`, `chore:`, `ci:`, `build:`
>

## References

This format follows:

- [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)
- [Semantic Versioning](https://semver.org/)
- Version numbers are produced by [MinVer](./ReleaseProcess.md) from Git tags.
