# vm2.SemVer

A starter vm2 package scaffold. Customize the code, tests, benchmarks, docs, and workflows as needed.

## Getting started

- Build:

  ```bash
  dotnet restore
  dotnet build
  ```

- Test:
  - from **CLI**, if it is not built yet (builds on MTP v2):

    ```bash
    dotnet run --project test/SemVer.Tests/SemVer.Tests.csproj`
    ```

  - from **CLI**, if it is already built in **CLI** or **VSCode** (MTP v2):
    - any OS or shell:

      ```bash
      dotnet test test/SemVer.Tests/bin/Debug/net10.0/SemVer.Tests.dll`
      ```

    - on Windows **CLI** (already built in **CLI** or **VSCode** - on MTP v2):

      ```batch
      test/SemVer.Tests/bin/Debug/net10.0/SemVer.Tests.exe`
      ```

    - on Linux or MacOS **CLI** (already built in **CLI** or **VSCode** - on MTP v2):

      ```bash
      test/SemVer.Tests/bin/Debug/net10.0/SemVer.Tests`
      ```

  - from Visual Studio:
    - use the Test Explorer to build and run tests (builds on MTP v1)
    - if it is already built in **Visual Studio** (MTP v1), from the **CLI** you can run:

      ```bash
      dotnet test
      ```

- Benchmarks (if included):

  ```bash
  dotnet run --project benchmarks/SemVer.Benchmarks/SemVer.Benchmarks.csproj --configuration Release
  ```

  > [!TIP] in a personal development environment, you can run benchmarks with defined `SHORT_RUN` preprocessor directive. The run will be faster, although less accurate, but still suitable for quick iterations.

## Package metadata

- Package ID: `vm2.SemVer`
- Version: 0.1.0
- License: MIT
- Repository: <https://github.com/vmelamed/vm2.SemVer>

## Structure

- .github/workflows: CI, prerelease, release, clear-cache.
- src/SemVer: the library source code
- test/SemVer.Tests: xUnit + MTP tests, includes testconfig.json
- benchmarks/SemVer.Benchmarks: BenchmarkDotNet suite (optional)
- examples/SemVer.Example: minimal console sample (optional)
- docs/: documentation starter (optional)
- scripts/: bootstrap helpers
- changelog/: git-cliff configs for prerelease/release changelog updates

## Next steps

- create GitHub repository using the generated bootstrap script: `scripts/repo-setup.sh`
- Update README, CHANGELOG, and package metadata.
- Set secrets and variables for workflows:
  - Set required secrets in the new GitHub repo:
    - `CODECOV_TOKEN`
    - `BENCHER_API_TOKEN`
    - `NUGET_API_KEY` must be issued by the selected `NUGET_SERVER` (below)
  - Set required variables:
    - `DOTNET_VERSION`: `10.0.x`: the .NET SDK version to use
    - `CONFIGURATION`: `Release`: the build configuration to use (e.g., Release or Debug)
    - `NUGET_SERVER`: `github`: the NuGet server to publish to (supported values: 'github', 'nuget', or custom URI)
    - `MINVERTAGPREFIX`: `v`: Prefix for git tags to be recognized by MinVer
    - `MINVERDEFAULTPRERELEASEIDENTIFIERS`: `.0`: Prefix for the prerelease tag, e.g. 'preview.0', 'alpha', 'beta', 'rc', etc.
    - `SAVE_PACKAGE_ARTIFACTS`: `false`: Whether to save package artifacts after build/publish
    - `MIN_COVERAGE_PCT`: `80`%: Minimum code coverage percentage required
    - `MAX_REGRESSION_PCT`: `20`%: Maximum allowed regression percentage
    - `RESET_BENCHMARK_THRESHOLDS`: `false`: Whether to reset Bencher thresholds
  - Set debug flags (variables):
    - `ACTIONS_RUNNER_DEBUG`: `false`: Whether to enable GitHub Actions runner debug logging
    - `ACTIONS_STEP_DEBUG`: `false`: Whether to enable GitHub Actions step debug logging

- Protect the `main` branch by enabling required checks and requiring pull requests. Suggested check names:
  - `build` (job id from CI workflow "CI: Build, Test, Benchmark")
  - `test` (job id from CI workflow "CI: Build, Test, Benchmark")
  - `benchmark` (job id from CI workflow "CI: Build, Test, Benchmark")
- In GitHub repo settings, enable Actions PR automation:
  - `Settings` -> `Actions` -> `General` -> `Workflow permissions`
  - enable `Allow GitHub Actions to create and approve pull requests`
  - required for prerelease changelog PR creation.
- Changelog: prerelease workflow appends a prerelease section; release workflow adds a stable header with "See prereleases below." (prerelease sections stay intact).
