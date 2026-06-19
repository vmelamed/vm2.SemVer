# vm2.SemVer — Claude Context

@~/.claude/CLAUDE.md
@~/repos/vm2/CLAUDE.md
@.github/CONVENTIONS.md

## Package Identity

- Repo: <https://github.com/vmelamed/vm2.SemVer>
- NuGet: <https://www.nuget.org/packages/vm2.SemVer/>, <https://www.nuget.org/packages/vm2.SemVerTool/>
- Status: stable
- Target: .NET 10.0+

## What This Package Does

- Provides an immutable type that represents a semantic version, adhering to the Semantic Versioning 2.0.0 specification.
- The type is `public readonly struct vm2.SemVer`
- It allows for parsing, comparing, and manipulating semantic version numbers in a type-safe manner.
- The type is System.Text.Json serializable, allowing for easy integration with JSON-based APIs and storage solutions.
- Via the companion package vm2.Serialization.NsJson.SemVer, it provides Newtonsoft.Json converters for the SemVer type, enabling seamless serialization and deserialization of semantic version instances in JSON format.
- The package includes utility methods for incrementing major, minor, and patch versions, as well as for comparing semantic version instances according to the rules defined by the Semantic Versioning 2.0.0 specification.
- The companion dotnet tool package vm2.SemVerTool provides a command-line interface for working with semantic version numbers, allowing for operations such as parsing, comparing, and incrementing semantic version instances directly from the command line.

### Key design decisions

- The SemVer type is immutable, ensuring that semantic version instances cannot be modified after creation, which promotes thread safety and predictable behavior.
- Internally the segments of the semantic version (major, minor, patch) are stored as separate, immutable, strongly typed fields, ensuring that each component can be accessed independently while maintaining the overall immutability of the SemVer instance.
- According to the Semantic Versioning 2.0.0 specification and implemented here, the build metadata is ignored in comparisons, e.g., `1.0.0+build1` is considered equal to `1.0.0+build2` or `1.0.0`.
- A SemVer type instance can be instantiated from a string representation of a semantic version, allowing for easy creation of SemVer instances from textual version identifiers. The string is parsed with a compiled Regular expression according to the Semantic Versioning 2.0.0 specification, and any deviations from the specification result in a parsing error, ensuring that only valid semantic version strings can be used to create SemVer instances.
- To keep the AOT and trimmability of the SemVer type, the implementation does not include serialization via Newtonsoft.Json within the SemVer type itself. Instead, serialization via Newtonsoft.Json is provided through the companion package vm2.Serialization.NsJson.SemVer, ensuring that the core SemVer type remains lightweight and fully compatible with AOT and trimming scenarios.

## Regex abbreviations and conventions

- `RE`: regular expression.
- `Charset`: a set of characters (that is, a character-class fragment).

"Charsets" are strings that contain literal characters (for example, `"abc"`) and/or character ranges (for example, `"A-Z"`). A charset may represent a BNF term that is defined as a set of characters, for example:

```text
letter ::= "A" | "B" | "C" | ... | "Z" | "a" | "b" | "c" | ... | "z"
digit  ::= "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9"
```

Charsets can be concatenated, subject to .NET regex syntax rules, to build larger charsets, for example:

```csharp
const string letterChars = "_A-Za-z";
const string digitChars  = "0-9";
const string alphanumericChars = $"{letterChars}{digitChars}"; // => "_A-Za-z0-9" is a valid
                                                               // character class fragment
```

A charset is not a regular expression by itself; it is typically wrapped in square brackets to form one, for example:

```csharp
const string letter = $"[{letterChars}]"; // real RE that matches a single letter character
```

By convention, non-public charset constants use camelCase and the `Chars` suffix, for example `letterChars`.

Non-public constants in camelCase without a suffix represent regex fragments. Most of these are valid regex patterns on their own, but they are intended for composition rather than standalone use.

**Whitespace rule:**
If a fragment includes readability spaces around operators (for example, around `|`), every Regex instance that includes that fragment must be compiled or generated with `RegexOptions.IgnorePatternWhitespace`.

Rex vs Regex:

- `*Rex` constants are generally unanchored patterns intended for composition or searching within larger strings.
- `*Regex` constants are full-string validation patterns, typically anchored with `^` and `$`.

Only `*Regex` constants get public `Regex` instance producing factory methods. These methods are named after the constant without the `Regex` suffix (for example, `SemVer20()`), and are generated via `GeneratedRegexAttribute` using the corresponding `*Regex` pattern.

Quick convention table:

| Visibility | Suffix    | Naming convention | Description                                               |
|------------|-----------|-------------------|-----------------------------------------------------------|
| Non-public | *Chars    | camelCase         | character-class fragments (not standalone regex patterns) |
| Non-public |           | camelCase         | regex fragments for composition                           |
| Public     | *Rex      | PascalCase        | generally unanchored public patterns                      |
| Public     | *Regex    | PascalCase        | anchored full-string validation patterns                  |
| Public     | *Regex    | PascalCase        | `GeneratedRegex` factories for `*Regex` constants (method name is the constant name without the `Regex` suffix) |

> [!NOTE]
> This style of building the regexes usually requires that the regular expression objects MUST be built with the
> options `RegexOptions.IgnorePatternWhitespace` and `RegexOptions.ExplicitCapture` for correctness, better readability, and
> performance.

## Common Local Commands

```bash
# Build
dotnet build vm2.SemVer.slnx

# Run tests (xUnit v3, MTP v2 — each project is a compiled executable)
dotnet test --project tests/SemVer.Tests/SemVer.Tests.csproj

# Run test executables (xUnit v3, MTP v2 — each project is a compiled to an executable) on Linux:
tests/SemVer.Tests/bin/Debug/net10.0/SemVer.Tests # on Linux
tests/SemVer.Tests/bin/Debug/net10.0/SemVer.Tests.exe # on Windows

# Run a single test by method name (xUnit v3, MTP v2 filter syntax)
dotnet test --project tests/SemVer.Tests/SemVer.Tests.csproj --filter "MethodName_WhenCondition_ShouldOutcome"

# Pack NuGet package
dotnet pack vm2.SemVer.slnx --configuration Release

# Run benchmarks (Release only)
dotnet run --project benchmarks/SemVer.Benchmarks/SemVer.Benchmarks.csproj --configuration Release -- --filter "*"

# If the benchmark tests are already built, you can run the compiled executable directly:
benchmarks/SemVer.Benchmarks/bin/Release/net10.0/SemVer.Benchmarks --help
benchmarks/SemVer.Benchmarks/bin/Release/net10.0/SemVer.Benchmarks --filter "*" # on Linux
benchmarks/SemVer.Benchmarks/bin/Release/net10.0/SemVer.Benchmarks.exe --filter "*" # on Windows
```

## Prompting Notes for This Package

- *Rex* vs *Regex* distinction is load-bearing — don't use a *Rex* constant where a *Regex* is expected (unanchored vs. full-string).
