# Changelog






## v1.0.2-preview.4 - 2026-04-22


### Fixed

- correct invalid prerelease version headers in CHANGELOG






## v1.0.2-preview.3 - 2026-04-22


### Internal

- add shared conventions document for vm2 packages for claude [skip ci]
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
