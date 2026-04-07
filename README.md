# vm2.SemVer

[![CI](https://github.com/vmelamed/vm2.SemVer/actions/workflows/CI.yaml/badge.svg?branch=main)](https://github.com/vmelamed/vm2.SemVer/actions/workflows/CI.yaml)
[![codecov](https://codecov.io/gh/vmelamed/vm2.SemVer/branch/main/graph/badge.svg?branch=main)](https://codecov.io/gh/vmelamed/vm2.SemVer)
[![Release](https://github.com/vmelamed/vm2.SemVer/actions/workflows/Release.yaml/badge.svg?branch=main)](https://github.com/vmelamed/vm2.SemVer/actions/workflows/Release.yaml)

[![NuGet Version](https://img.shields.io/nuget/v/vm2.SemVer)](https://www.nuget.org/packages/vm2.SemVer/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/vm2.SemVer.svg)](https://www.nuget.org/packages/vm2.SemVer/)
[![GitHub License](https://img.shields.io/github/license/vmelamed/vm2.SemVer)](https://github.com/vmelamed/vm2.SemVer/blob/main/LICENSE)

A .NET implementation of the [Semantic Versioning 2.0.0](https://semver.org/spec/v2.0.0.html) specification. Provides parsing, formatting, comparison, and JSON serialization of semantic versions as a lightweight, immutable `readonly struct`.

<!-- TOC tocDepth:2..6 chapterDepth:2..6 -->

- [vm2.SemVer](#vm2semver)
  - [Features](#features)
  - [Installation](#installation)
  - [Quick start](#quick-start)
  - [JSON serialization](#json-serialization)
    - [System.Text.Json](#systemtextjson)
    - [Newtonsoft.Json](#newtonsoftjson)
  - [Building and testing](#building-and-testing)
  - [Package metadata](#package-metadata)
  - [Project structure](#project-structure)
  - [Regex abbreviations and conventions](#regex-abbreviations-and-conventions)

<!-- /TOC -->

## Features

- **Immutable value type** — `readonly partial struct` with value semantics
- **Full SemVer 2.0.0 compliance** — major, minor, patch, pre-release, and build metadata
- **Multiple parsing paths** — `string`, `ReadOnlySpan<char>`, `ReadOnlySpan<byte>` (UTF-8)
- **Multiple formatting paths** — `ToString()`, `TryFormat(Span<char>)`, `TryFormat(Span<byte>)` (UTF-8)
- **Zero-allocation formatting** — span-based `TryFormat` overloads allocate nothing
- **JSON serialization** — built-in converters for both System.Text.Json and Newtonsoft.Json
- **Comparison & equality** — implements `IComparable<SemVer>`, `IEquatable<SemVer>`, and all comparison/equality operators per the spec (build metadata is ignored in precedence)
- **Regex validation** — `GeneratedRegex`-based patterns for SemVer strings, pre-release identifiers, and build metadata identifiers
- **Interface-rich** — implements `ISpanParsable<SemVer>`, `IUtf8SpanParsable<SemVer>`, `ISpanFormattable`, `IUtf8SpanFormattable`, `IFormattable`

## Installation

```bash
dotnet add package vm2.SemVer
```

## Quick start

```csharp
using vm2;

// Parse
var version = SemVer.Parse("1.2.3-rc.1+build.7");

// Construct
var v = new SemVer(1, 2, 3, "rc.1", "build.7");

// Properties
Console.WriteLine(v.Major);         // 1
Console.WriteLine(v.Minor);         // 2
Console.WriteLine(v.Patch);         // 3
Console.WriteLine(v.PreRelease);    // "rc.1"
Console.WriteLine(v.BuildMetadata); // "build.7"
Console.WriteLine(v.IsPreRelease);  // true
Console.WriteLine(v.IsStable);      // false
Console.WriteLine(v.Core);          // 1.2.3

// Format
Console.WriteLine(v.ToString());    // "1.2.3-rc.1+build.7"

// Compare (build metadata is ignored per spec)
var a = SemVer.Parse("1.0.0-alpha");
var b = SemVer.Parse("1.0.0-beta");
Console.WriteLine(a < b);           // true

// Span-based formatting (zero allocation)
Span<char> buffer = stackalloc char[v.Length];
v.TryFormat(buffer, out int charsWritten);

// UTF-8 formatting (zero allocation)
Span<byte> utf8 = stackalloc byte[v.Length];
v.TryFormat(utf8, out int bytesWritten);
```

## JSON serialization

Both converters are applied via attributes on the `SemVer` struct, so serialization works out of the box with no additional configuration.

### System.Text.Json

The `SemVerSysConverter` uses span-based UTF-8 formatting and parsing for minimal allocations.

```csharp
using System.Text.Json;

var v = new SemVer(1, 2, 3, "rc.1", "build.7");
string json = JsonSerializer.Serialize(v);          // "\"1.2.3-rc.1+build.7\""
var parsed = JsonSerializer.Deserialize<SemVer>(json);
```

### Newtonsoft.Json

The `SemVerNsConverter` provides serialization support for Newtonsoft.Json consumers.

```csharp
using Newtonsoft.Json;

var v = new SemVer(1, 2, 3, "rc.1", "build.7");
string json = JsonConvert.SerializeObject(v);       // "\"1.2.3-rc.1+build.7\""
var parsed = JsonConvert.DeserializeObject<SemVer>(json);
```

## Building and testing

- Build:

  ```bash
  dotnet restore
  dotnet build
  ```

- Test:
  - from **CLI**, if it is not built yet (builds on MTP v2):

    ```bash
    dotnet run --project test/SemVer.Tests/SemVer.Tests.csproj
    ```

  - from **CLI**, if it is already built in **CLI** or **VSCode** (MTP v2):

    ```bash
    dotnet test --solution vm2.SemVer.slnx
    ```

- Benchmarks:

  ```bash
  dotnet run --project benchmarks/SemVer.Benchmarks/SemVer.Benchmarks.csproj -c Release -- --filter '*'
  ```

  > [!TIP]
  > In a personal development environment, you can run benchmarks with the `SHORT_RUN` preprocessor directive for faster (less
  accurate) iterations:
  >
  > `dotnet run --project benchmarks/SemVer.Benchmarks/SemVer.Benchmarks.csproj -c Release -p:preprocessor_symbols=SHORT_RUN --
  --filter '*'`

## Package metadata

- Package ID: `vm2.SemVer`
- License: MIT
- Repository: <https://github.com/vmelamed/vm2.SemVer>

## Project structure

- `src/SemVer/` — library source code
- `test/SemVer.Tests/` — xUnit v3 + MTP tests
- `benchmarks/SemVer.Benchmarks/` — BenchmarkDotNet suite (Parse, Format, JSON)
- `examples/SemVer.Example/` — minimal console sample
- `docs/` — documentation
- `changelog/` — git-cliff configs for prerelease/release changelog updates

## Regex abbreviations and conventions

- `RE`: regular expression.
- `Charset`: a set of characters (that is, a character-class fragment).

Charsets are strings that contain literal characters (for example, `"abc"`) and/or character ranges (for example, `"A-Z"`).
A charset may represent a BNF term that is defined as a set of characters, for example:

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

Non-public constants in camelCase without a suffix represent regex fragments. Most of these are valid regex patterns on their
own, but they are intended for composition rather than standalone use.

Whitespace rule:
If a fragment includes readability spaces around operators (for example, around `|`), every Regex instance that includes that
fragment must be compiled or generated with `RegexOptions.IgnorePatternWhitespace`.

Rex vs Regex:

- `*Rex` constants are generally unanchored patterns intended for composition or searching within larger strings.
- `*Regex` constants are full-string validation patterns, typically anchored with `^` and `$`.

Only `*Regex` constants get public `Regex` instance producing factory methods.
These methods are named after the constant without the `Regex` suffix (for example, `SemVer20()`), and are generated via
`GeneratedRegexAttribute` using the corresponding `*Regex` pattern.

Quick convention table:

| Visibility | Suffix    | Naming convention | Description                                               |
|------------|-----------|-------------------|-----------------------------------------------------------|
| Non-public | *Chars    | camelCase         | character-class fragments (not standalone regex patterns) |
| Non-public |           | camelCase         | regex fragments for composition                           |
| Public     | *Rex      | PascalCase        | generally unanchored public patterns                      |
| Public     | *Regex    | PascalCase        | anchored full-string validation patterns                  |
| Public     | *Regex    | PascalCase        | `GeneratedRegex` factories for `*Regex` constants (method name is the constant name without the `Regex` suffix) |
