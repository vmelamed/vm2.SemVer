# Changelog







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

### Internal

- promote to stable v1.0.0 [skip ci]
- update changelog for v1.0.0 [skip ci]

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
> - commit prefix for git-cliff: `refactor:`, `docs:`, `style:`, `test:`, `chore:`, `ci:`, `build:`
>

## References

This format follows:

- [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)
- [Semantic Versioning](https://semver.org/)
- Version numbers are produced by [MinVer](./ReleaseProcess.md) from Git tags.
