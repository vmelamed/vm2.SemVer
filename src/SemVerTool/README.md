# vm2.SemVerTool

[![NuGet Version](https://img.shields.io/nuget/v/vm2.SemVerTool)](https://www.nuget.org/packages/vm2.SemVerTool/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/vm2.SemVerTool.svg)](https://www.nuget.org/packages/vm2.SemVerTool/)
[![GitHub License](https://img.shields.io/github/license/vmelamed/vm2.SemVer)](https://github.com/vmelamed/vm2.SemVer/blob/main/LICENSE)

A .NET command-line tool for validating, comparing, and bumping [Semantic Versioning 2.0.0](https://semver.org/spec/v2.0.0.html) strings.
Built on top of [vm2.SemVer](https://www.nuget.org/packages/vm2.SemVer/).

## Installation

```bash
dotnet tool install --global vm2.SemVerTool
```

## Commands

### validate

Validate a string against the SemVer 2.0.0 specification.

```bash
semver validate 1.2.3-preview.1+build.42
```

```text
'1.2.3-preview.1+build.42' is a VALID SemVer 2.0.0:
    Major: 1
    Minor: 2
    Patch: 3
    Version core: 1.2.3
    Pre-release identifiers: preview.1
    Build metadata identifiers: build.42
```

Invalid strings return exit code 1:

```bash
semver validate "not-a-version"
# 'not-a-version' is NOT VALID SemVer 2.0.0.
```

### compare

Compare two SemVer strings. Exit codes indicate the result:

| Exit Code | Meaning |
| :-------: | :------ |
| 0         | Equal   |
| 1         | First is greater |
| 255       | First is less |

```bash
semver compare 1.2.3 2.0.0
# '1.2.3' is less than '2.0.0'.
```

### bump

Bump the major, minor, or patch version. Optionally add pre-release and build metadata identifiers.

```bash
semver bump 1.2.3 --part minor
# Bumped minor: 1.3.0

semver bump 1.2.3 --part major --pre-release alpha.1
# Bumped major: 2.0.0-alpha.1

semver bump 1.2.3 -p p -r beta.1 -b build.99
# Bumped patch: 1.2.4-beta.1+build.99
```

**Options:**

| Option | Short | Description | Default |
| :----- | :---- | :---------- | :------ |
| `--part` | `-p` | Part to bump: `major`/`j`, `minor`/`n`, `patch`/`p` | `patch` |
| `--pre-release` | `-r` | Pre-release identifier to append | _(none)_ |
| `--build-metadata` | `-b` | Build metadata identifier to append | _(none)_ |

## Global Options

| Option | Short | Description |
| :----- | :---- | :---------- |
| `--quiet` | `-q` | Suppress verbose output. Only print the bare result. |

Quiet mode is useful for scripting:

```bash
bumped=$(semver bump 1.2.3 -p minor -q)
echo "$bumped"   # 1.3.0
```

## License

[MIT](../../LICENSE)
