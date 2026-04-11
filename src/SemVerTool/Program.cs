// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

return SemVerToolApp.Run(args);

internal static class SemVerToolApp
{
    internal static readonly Option<bool> QuietOption = new("--quiet", "-q")
    {
        Description = "Suppress verbose output. Only print the bare result.",
        Required = false,
        Recursive = true,
        Arity = ArgumentArity.Zero,
        DefaultValueFactory = _ => false,
    };

    internal static int Run(string[] args, TextWriter? output = null)
    {
        TextWriter? originalOut = Console.Out;
        RootCommand rootCommand = CreateRootCommand();
        var parseResult = rootCommand.Parse(args);

        if (output is not null)
            Console.SetOut(output);

        try
        {
            if (parseResult.Errors.Count > 0)
            {
                Console.WriteLine("Error parsing command line arguments:");
                foreach (var error in parseResult.Errors)
                    Console.WriteLine($"  {error.Message}");
                return 1;
            }

            return parseResult.Invoke();
        }
        finally
        {
            if (originalOut is not null)
                Console.SetOut(originalOut);
        }
    }

    internal static Command CreateValidateCommand()
    {
        var version = new Argument<string>("version")
        {
            HelpName = "version",
            Description = "The SemVer to validate.",
            Arity = ArgumentArity.ExactlyOne,
        };

        var validateCommand = new Command("validate", "Validate a string against the Semantic Versioning (SemVer) 2.0.0 specification.")
        {
            version
        };
        validateCommand.SetAction(
            parseResult =>
            {
#pragma warning disable CA1031 // Do not catch general exception types - it's OK here - this is the highest UI level and we want to catch any unexpected exceptions and report them as errors instead of crashing the tool.
                try
                {
                    var semver = parseResult.GetValue(version);
                    var quiet = parseResult.GetValue(QuietOption);

                    if (!string.IsNullOrWhiteSpace(semver) && SemVer.IsValid(semver))
                    {
                        SemVer semVer = SemVer.Parse(semver);

                        if (quiet)
                        {
                            Console.WriteLine(semVer);
                        }
                        else
                        {
                            Console.WriteLine($"'{semver}' is a VALID SemVer 2.0.0:");
                            Console.WriteLine($"    Major: {semVer.Major}");
                            Console.WriteLine($"    Minor: {semVer.Minor}");
                            Console.WriteLine($"    Patch: {semVer.Patch}");
                            Console.WriteLine($"    Version core: {semVer.Core}");
                            if (semVer.PreRelease is not "")
                                Console.WriteLine($"    Pre-release identifiers: {semVer.PreRelease}");
                            if (semVer.BuildMetadata is not "")
                                Console.WriteLine($"    Build metadata identifiers: {semVer.BuildMetadata}");
                        }
                        return 0;
                    }
                    else
                    {
                        if (!quiet)
                            Console.WriteLine($"'{semver}' is NOT VALID SemVer 2.0.0.");
                        return 1;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error validating SemVer string: {ex.Message}");
                    return 128;
                }
#pragma warning restore CA1031
            });

        return validateCommand;
    }

    internal static Command CreateCompareCommand()
    {
        Command compareCommand = new("compare", "Compare two SemVer 2.0.0 strings.");
        Argument<string> version1Argument = new("version1")
        {
            HelpName = "version1",
            Description = "The first SemVer 2.0.0 string to compare.",
            Arity = ArgumentArity.ExactlyOne,
            Validators =
            {
                result =>
                {
                    var version = result.GetValueOrDefault<string>();
                    if (string.IsNullOrWhiteSpace(version) || !SemVer.IsValid(version))
                        result.AddError($"'{version}' is not a valid SemVer 2.0.0 string.");
                }
            }
        };
        Argument<string> version2Argument = new("version2")
        {
            HelpName = "version2",
            Description = "The second SemVer 2.0.0 string to compare.",
            Arity = ArgumentArity.ExactlyOne,
            Validators =
            {
                result =>
                {
                    var version = result.GetValueOrDefault<string>();
                    if (string.IsNullOrWhiteSpace(version) || !SemVer.IsValid(version))
                        result.AddError($"'{version}' is not a valid SemVer 2.0.0 string.");
                }
            }
        };
        compareCommand.Add(version1Argument);
        compareCommand.Add(version2Argument);
        compareCommand.SetAction(
            int (ParseResult parseResult) =>
            {
#pragma warning disable CA1031 // Do not catch general exception types - it's OK here - this is the highest UI level and we want to catch any unexpected exceptions and report them as errors instead of crashing the tool.
                try
                {
                    var version1 = parseResult.GetValue(version1Argument);
                    var version2 = parseResult.GetValue(version2Argument);

                    Debug.Assert(version1 is not null &&
                                 version2 is not null,
                                 "We validated this already, so it should not be null.");

                    SemVer semVer1 = SemVer.Parse(version1);
                    SemVer semVer2 = SemVer.Parse(version2);

                    var quiet = parseResult.GetValue(QuietOption);

                    int comparison = semVer1.CompareTo(semVer2);
                    if (comparison < 0)
                    {
                        if (!quiet)
                            Console.WriteLine($"'{version1}' is less than '{version2}'.");
                        return 255;
                    }
                    else if (comparison > 0)
                    {
                        if (!quiet)
                            Console.WriteLine($"'{version1}' is greater than '{version2}'.");
                        return 1;
                    }
                    else
                    {
                        if (!quiet)
                            Console.WriteLine($"'{version1}' is equal to '{version2}'.");
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error comparing SemVer strings: {ex.Message}");
                    return 128;
                }
#pragma warning restore CA1031
            });

        return compareCommand;
    }

    static readonly string[] _bumpParts = ["major", "minor", "patch", "j", "n", "p"];

    internal static Command CreateBumpCommand()
    {
        Command bumpCommand = new("bump", "Bump the major, minor, or patch version of a SemVer 2.0.0 string.");
        Argument<string> versionArgument = new("version")
        {
            HelpName = "version",
            Description = "The SemVer 2.0.0 string to bump.",
            Arity = ArgumentArity.ExactlyOne,
            DefaultValueFactory = _ => "",
            Validators =
            {
                result =>
                {
                    var version = result.GetValueOrDefault<string>();
                    if (string.IsNullOrWhiteSpace(version) || !SemVer.IsValid(version))
                        result.AddError($"'{version}' is not a valid SemVer 2.0.0 string.");
                }
            }
        };

        Option<string> partOption = new("--part", "-p")
        {
            HelpName = "part",
            Description = "The part of the version to bump: major or j, minor or n, or patch or p.",
            Required = false,
            Arity = ArgumentArity.ExactlyOne,
            DefaultValueFactory = _ => "",
            Validators =
            {
                result =>
                {
                    var part = result.GetValueOrDefault<string>().Trim().ToLower();
                    if (!_bumpParts.Contains(part))
                        result.AddError($"Invalid part to bump: '{part}'. Must be one of: {string.Join(", ", _bumpParts)}.");
                }
            }
        };

        Option<string> preReleaseOption = new("--pre-release", "-r")
        {
            HelpName = "pre-release",
            Description = "The pre-release identifier to add to the bumped version. Optional.",
            Required = false,
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = _ => "",
            Validators =
            {
                result =>
                {
                    var p = result.GetValueOrDefault<string>();
                    var preRelease = !string.IsNullOrWhiteSpace(p) ? p.Trim() : "";
                    if (preRelease != ""  &&  !SemVer.PreReleaseIdentifier().IsMatch(preRelease))
                        result.AddError($"'{preRelease}' is not a valid SemVer pre-release identifier.");
                }
            }
        };

        Option<string> buildMetadataOption = new("--build-metadata", "-b")
        {
            HelpName = "build-metadata",
            Description = "The build metadata identifier to add to the bumped version. Optional.",
            Required = false,
            Arity = ArgumentArity.ZeroOrOne,
            DefaultValueFactory = _ => "",
            Validators =
            {
                result =>
                {
                    var m = result.GetValueOrDefault<string>();
                    var buildMetadata = !string.IsNullOrEmpty(m) ? m.Trim() : "";
                    if (buildMetadata != ""  &&  !SemVer.BuildIdentifier().IsMatch(buildMetadata))
                        result.AddError($"'{buildMetadata}' is not a valid SemVer build metadata identifier.");
                }
            }
        };

        bumpCommand.Add(versionArgument);
        bumpCommand.Add(partOption);
        bumpCommand.Add(preReleaseOption);
        bumpCommand.Add(buildMetadataOption);

        bumpCommand.SetAction(
            int (ParseResult parseResult) =>
            {
#pragma warning disable CA1031 // Do not catch general exception types - it's OK here - this is the highest UI level and we want to catch any unexpected exceptions and report them as errors instead of crashing the tool.
                try
                {
                    var version = parseResult.GetValue(versionArgument);
                    var part = parseResult.GetValue(partOption)?.Trim().ToLower();
                    var preRelease = parseResult.GetValue(preReleaseOption);
                    var buildMetadata = parseResult.GetValue(buildMetadataOption);

                    Debug.Assert(version is not null, "We validated this already, so it should not be null.");
                    Debug.Assert(part is not null, "We validated this already, so it should not be null.");
                    Debug.Assert(preRelease is not null, "We validated this already, so it should not be null.");
                    Debug.Assert(buildMetadata is not null, "We validated this already, so it should not be null.");

                    var quiet = parseResult.GetValue(QuietOption);
                    var semVer = SemVer.Parse(version);

                    switch (part)
                    {
                        case "major":
                        case "j":
                            semVer = semVer.BumpMajor(preRelease, buildMetadata);
                            break;
                        case "minor":
                        case "n":
                            semVer = semVer.BumpMinor(preRelease, buildMetadata);
                            break;
                        case "patch":
                        case "p":
                            semVer = semVer.BumpPatch(preRelease, buildMetadata);
                            break;
                    }

                    if (quiet)
                        Console.WriteLine(semVer);
                    else
                        Console.WriteLine($"Bumped {part}: {semVer}");

                   return 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error bumping SemVer string: {ex.Message}");
                    return 255;
                }
#pragma warning restore CA1031
            });

        return bumpCommand;
    }

    internal static RootCommand CreateRootCommand()
    {
        RootCommand rootCommand = new("A tool for working with Semantic Versioning (SemVer) 2.0.0 strings.")
        {
            Options =
            {
                QuietOption
            },
            Subcommands =
            {
                CreateValidateCommand(),
                CreateCompareCommand(),
                CreateBumpCommand(),
            }
        };

        return rootCommand;
    }
}
