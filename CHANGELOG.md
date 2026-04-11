# Changelog

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
