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

## Repository Layout

```text
vm2.SemVer/
├── src/
│   ├── SemVer/           # Core library: SemVer struct, regex, serialization
│   └── SemVerTool/       # CLI tool (dotnet tool): validate, compare, bump
├── test/
│   ├── SemVer.Tests/     # Unit tests for the core library
│   └── SemVerTool.Tests/ # Unit tests for the CLI tool
├── benchmarks/
│   └── SemVer.Benchmarks/  # BenchmarkDotNet performance benchmarks
├── examples/             # Usage examples
├── docs/                 # Additional documentation
├── changelog/            # git-cliff configuration files
└── vm2.SemVer.slnx       # Solution file (all projects)
```

## CI / GitHub Actions — Project Registration

**CRITICAL: When adding a new project, register it in `.github/workflows/CI.yaml`.**

The CI workflow uses JSON arrays in `env:` to drive build, test, benchmark, and pack jobs.
When you add a new test project or packable project, you **must** add its path to the appropriate list:

| Array               | Purpose                      | Example entry                                    |
|---------------------|------------------------------|--------------------------------------------------|
| `BUILD_PROJECTS`    | Solutions/projects to build  | `"vm2.SemVer.slnx"`                             |
| `TEST_PROJECTS`     | Test projects to run         | `"test/SemVerTool.Tests/SemVerTool.Tests.csproj"` |
| `BENCHMARK_PROJECTS`| Benchmark projects to run    | `"benchmarks/SemVer.Benchmarks/SemVer.Benchmarks.csproj"` |
| `PACKAGE_PROJECTS`  | Projects to pack as NuGet    | `"src/SemVerTool/SemVerTool.csproj"`             |

Also add the new project to `vm2.SemVer.slnx` under the appropriate solution folder (`/src/`, `/test/`, `/benchmarks/`).

## SemVer Library — Domain-Specific Guidance

### Core Type: `SemVer` (readonly partial struct)

- Implements SemVer 2.0.0 specification (<https://semver.org/>)
- Immutable value type — all "mutation" returns a new instance
- Uses C# 14 `field` keyword for backing fields with coalescing (`field ?? ""`)
- Regex patterns live in `SemVer.re.cs` with documented naming conventions
- `numericIdentifier` rejects leading zeroes — this is spec-required and validates CompareSpans' length-then-ordinal comparison
- `default(SemVer)` is valid and represents `0.0.0`

### Serialization

- **System.Text.Json**: `SemVerSysConverter` — uses `CopyString()` (not `ValueSpan`) to handle JSON escape sequences like `\u002B`
- **Newtonsoft.Json**: `SemVerNsConverter` — handles null tokens and null-safe WriteJson

### CLI Tool: `SemVerTool`

- Packaged as a .NET global tool (`PackAsTool=true`, command name `semver`)
- Uses `System.CommandLine` with `Recursive = true` for the `--quiet` option to propagate it to all subcommands
- `InternalsVisibleTo` exposes internals to `SemVerTool.Tests`
- Exit codes for `compare`: `0` = equal, `1` = greater than, `255` = less than, `2` = error
- The `Run(args, output)` overload captures console output for testability

### Conversions

- `SemVer → string` (implicit): safe, lossless
- `string → SemVer` (explicit): can throw `FormatException` — must remain `explicit`

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

- When implementing specifications (SemVer 2.0.0, RFC, etc.), cite the source:
  - Include title, URL, and date accessed
  - Example: "Based on Semantic Versioning 2.0.0 (<https://semver.org/>)"
- When using algorithms or concepts from papers/articles, cite them
- When adapting code patterns from other projects, acknowledge them
- Include links to relevant documentation for users to learn more

## Markdown File Generation

### Always follow the Markdown lint default rules or defined in the `.markdownlint.json` file in the repository root

**IMPORTANT: When generating complete Markdown (.md) file content:**

- **ALWAYS** wrap the entire file content in a code fence using **TILDES** (`~~~markdown` ... `~~~`)
- This prevents nesting issues with triple-backtick code blocks inside the Markdown content
- **Inside the Markdown content**, use **4-space indentation** for code blocks (not triple backticks)
- This avoids nested code fence conflicts while maintaining valid Markdown syntax

### Ordered Lists

When creating ordered lists in Markdown documents, use `1.` for all items instead of sequential numbering (1., 2., 3., etc.).

## File Modification Guidelines

**CRITICAL: When updating or modifying existing files:**

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

## Async

- Suffix async methods with `Async`.
- Pass `CancellationToken ct` through all async call chains.
- Avoid fire-and-forget except for explicitly scheduled background operations (document rationale).
- Use `ValueTask` only when it measurably reduces allocations (e.g. hot paths already returning cached results).

## Error Handling / Logging

- Throw domain-specific exceptions for business rule violations; not generic `InvalidOperationException` unless internal invariant.
- Avoid swallowing exceptions — log or rethrow.
- Do not log sensitive data (PII, secrets).
- Prefer `Try...` patterns over broad exception-based control flow where appropriate.

## Testing Conventions

- Framework: xUnit v3.
- Assertions: FluentAssertions (never Assert.* unless framework-specific).
- Test naming:
  - Async: `MethodName_WhenCondition_ShouldOutcome_Async`
  - Sync: `MethodName_WhenCondition_ShouldOutcome`
- Use Arrange / Act / Assert with clear spacing.
- Minimize deep object graphs — use builders or inline records.
- Prefer one logical assertion per test (grouped FluentAssertions chain counts as one).
- Avoid testing implementation details (interactions only when behavior requires).
- Deterministic time: inject clock abstractions (never rely on `DateTime.UtcNow` directly in tests).
- Deterministic IDs: inject or override ID providers when needed.
- For the CLI tool: use `SemVerToolApp.Run(args, output)` with a `StringWriter` to capture console output for assertions.

## Performance / Allocation

- Avoid premature optimization; but:
  - Prefer `ReadOnlySpan<char>` overloads for parsing hot paths.
  - Use `stackalloc` for small buffers (with heap fallback for large inputs).
  - Avoid unnecessary string allocations in formatting/parsing.

## Git / PR Hygiene

- One logical concern per PR.
- PR description: What / Why / How / Risk / Rollback.
- Commit messages: `<scope>: <imperative summary>`
  - Example: `semver: add IUtf8SpanParsable implementation`

## Security

- Never embed secrets — use user secrets or environment variables.
- Validate all external inputs at boundaries.
- Strive to follow the principle of least privilege in all access patterns.

## Copilot Prompting Hints

When asking Copilot for code:

- Specify: context (struct, converter, CLI command, test scenario).
- Specify: required patterns (e.g. "readonly struct with span-based parsing").
- For tests: mention desired patterns, e.g. "use FluentAssertions, TheoryData for parameterized cases".

## Anti-Patterns (Avoid)

- Mutable state in the SemVer struct.
- Allocating strings in hot parsing/formatting paths when spans suffice.
- Static utility god classes.
- Catch-all exception wrappers that rethrow without context.
- Copy/paste validation logic — centralize in validators or regex patterns.

## Tooling / Analyzer Suggestions

- Add analyzers for: async usage, nullability misuse, trim compatibility.
- Treat warnings as build errors (`TreatWarningsAsErrors=True`).
- Enable trim analyzer (`EnableTrimAnalyzer=true`, `IsTrimmable=true`).
