# Changelog

## v2.1.0-preview.2 - 2026-06-11

### Internal

- changed comments and UI for clarity [skip ci]

## v2.1.0-preview.1 - 2026-06-10

### Added

- add max generation collection thresholds to CI environment variables [skip ci]

### Internal

- promote to stable v2.0.2 [skip ci]
- update changelog for v2.0.2 [skip ci]
- Bump the minor-and-patch group with 1 update
- update vm2.TestUtilities to version 2.1.0
- update vm2.TestUtilities to version 2.1.0
- enhance parameter validation in JsonConverter methods
- streamline error handling and null checks in WriteJson method
- simplify null checks in WriteJson method of SemVerConverter
- update Microsoft.NET.ILLink.Tasks version to 10.0.9 in packages.lock.json
- add OperationsPerInvoke to Newtonsoft.Json benchmark methods
- implement operationsPerInvoke in benchmark methods for performance consistency
- add OperationsPerInvoke to System.Text.Json benchmark methods for consistency
- update Microsoft.DotNet.ILCompiler and Microsoft.NET.ILLink.Tasks versions to 10.0.9 in packages.lock.json
- update vm2.TestUtilities to version 2.1.1
- re-add OperationsPerInvoke
- rename suppress optimization discard variables in benchmark methods

## v2.0.2 - 2026-06-05

See prereleases below.

## v2.0.2-preview.1 - 2026-06-05

### Fixed

- streamline the dev. environment for multi-OS/multi-IDE and for consistent configuration of AI [skip ci]
- update commit prefix for git-cliff to include 'tests' and adjust documentation
- remove trailing newline from file header template

### Internal

- update changelog for v2.0.1 [skip ci]

## v2.0.1 - 2026-06-04

See prereleases below.

## v2.0.1-preview.1 - 2026-06-03

### Internal

- promote to stable v2.0.0 [skip ci]
- update changelog for v2.0.0 [skip ci]
- add default solution setting for .NET in VSCode configuration

## v2.0.0 - 2026-05-30

See prereleases below.

## v2.0.0-preview.1 - 2026-05-30

### Fixed

- ensure .cs files are normalized with LF line endings in .gitattributes [skip ci]
- **BREAKING:** correct order of attributes for .cs file normalization in .gitattributes

### Internal

- **BREAKING:** separated NSJ converter in its own assembly to keep the main package AOT compatible

## v1.2.1-preview.1 - 2026-05-30

### Fixed

- disable AoT, refactor Directory.Build.props
- correct path for test projects in CI configuration
- Update project configurations and benchmarks for AOT compatibility and namespace adjustments
- added JsonSerializerSettings
- clarify comment for text file normalization in .gitattributes

### Internal

- promote to stable v1.2.0 [skip ci]
- update changelog for v1.2.0 [skip ci]
- sync with diff-shared.sh
- update vm2.TestUtilities to version 1.5.1
- update vm2.TestUtilities to version 1.5.1 and clean up unused dependencies
- fix typo in conventions for merge or copy action description
- improve wording in CI warning message and conventions documentation
- added OutputType Exe
- following conventions
- diff-shared.sh
- change "test/" to "tests/"
- update dependencies

## v1.2.0 - 2026-05-20

See prereleases below.

## v1.2.0-preview.1 - 2026-05-20

### Added

- add telemetry opt-out and first-time experience skip for .NET CLI [skip ci]
- add NSubstitute package references to test projects

### Internal

- sync with diff-shared [skip ci]
- Bump the minor-and-patch group with 15 updates
- update dependencies
- add Copilot instructions and project guidance documentation
- update *.lock.json
- update Copilot instructions to reflect correct package name

## v1.1.1-preview.7 - 2026-04-30

### Internal

DevOps changes only.

## v1.1.1-preview.6 - 2026-04-30

### Fixed

- commit prefix

## v1.1.1-preview.5 - 2026-04-30

### Internal

- update vm2.TestUtilities to version 1.4.6

### deps

- Bump the minor-and-patch group with 1 update

## v1.1.1-preview.4 - 2026-04-24

### Fixed

- adjust trim setting in changelog templates for consistency

## v1.1.1-preview.3 - 2026-04-22

### Internal

- clean up whitespace in CHANGELOG

## v1.1.1-preview.2 - 2026-04-22

### Fixed

- correct invalid prerelease version headers in CHANGELOG

## v1.1.1-preview.1 - 2026-04-22

### Internal

- add shared conventions document for vm2 packages for claude [skip ci]
- diff-shared

## v1.1.0 - 2026-04-14

See prereleases below.

## v1.1.0-preview.3 - 2026-04-14

### Internal

- update vm2.TestUtilities to 1.4.3 and refine changelog template

## v1.1.0-preview.2 - 2026-04-14

### Internal

- update .gitattributes and add .slnx support; enhance changelog reminders in workflows; bump vm2.TestUtilities to 1.4.1; refine changelog commit parsers; add .gitmessage template
- bump vm2.TestUtilities to 1.4.2 and align changelog parser
- update tag pattern in changelog configuration for semantic versioning compliance
- refine documentation commit parser regex and add pull request template

### deps

- Bump the minor-and-patch group with 1 update

## v1.1.0-preview.1 - 2026-04-11

### Added

- add practical overloads for the TryParse functions. Add example program.
- add explicit and implicit string conversions to SemVer
- add an example program
- add an example program
- add dotnet tool SemVer
- add JsonExporter and MemoryDiagnoser attributes to benchmark classes
- add regex printing helper and improve SemVer validation tests. Addressed Copilot review comments.

### Fixed

- no indentation in block scalars
- update environment variables to provide default values and remove deprecated attributes in benchmarks
- conditionally disable TestingPlatformServerCapability for Visual Studio builds
- curate CHANGELOG and fix git-cliff template for v2.x
- add missing newlines in CHANGELOG and cliff.prerelease.toml for better readability
- update vm2.TestUtilities version to 1.3.1 in package locks
- update project references and improve README for SemVerTool; added DefaultValueFactory for the Tool's parameter version; Added README.md for the Tool;
- the comment in Program.cs regarding explicit string cast was wrong
- remove unnecessary blank lines in CHANGELOG.md
- adjust default bump value to "patch" in the command. Update vm2.TestUtilities to version 1.4.0 and inherit all test classes from TestBase
- update settings for cSpell and improve Codecov configuration. Adjust coverage settings for better source matching.
- add missing newline at end of packages.lock.json files

### Performance

- trim redundant benchmarks - remove ToString (wraps TryFormat), replace Parse with TryParse, drop CoreOnly JSON payloads
- remove CoreOnly benchmarks to reduce alert noise

## v1.0.0 - 2026-04-07

See prereleases below.

## v1.0.0-preview.1 - 2026-04-07

### Added

- Initial implementation of SemVer parsing and formatting
- Regular expression-based SemVer validation
- `Newtonsoft.Json` and `System.Text.Json` converters for SemVer serialization
- Explicit and implicit string conversions
- Benchmarks for parsing, formatting, and JSON serialization
- Comprehensive unit tests

### Internal

- Scaffold from vm2pkg template with CI/CD workflows

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
> - commit prefix for git-cliff: `refactor:`, `docs:`, `style:`, `test:`, `tests:`, `chore:`, `ci:`, `build:`
>

## References

This format follows:

- [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)
- [Semantic Versioning](https://semver.org/)
- Version numbers are produced by [MinVer](./ReleaseProcess.md) from Git tags.
