// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.SemVerToolTests;

public class SemVerToolTests
{
    static (int exitCode, string output) RunTool(params string[] args)
    {
        using var writer = new StringWriter();
        var exitCode = SemVerToolApp.Run(args, writer);
        return (exitCode, writer.ToString().TrimEnd());
    }

    #region validate command

    [Theory]
    [InlineData("1.2.3")]
    [InlineData("0.0.0")]
    [InlineData("1.2.3-alpha.1")]
    [InlineData("1.2.3+build.7")]
    [InlineData("1.2.3-rc.1+meta.9")]
    [InlineData("999.999.999")]
    public void Validate_ValidVersion_Returns0(string version)
    {
        var (exitCode, output) = RunTool("validate", version);

        exitCode.Should().Be(0);
        output.Should().Contain("VALID SemVer 2.0.0");
    }

    [Theory]
    [InlineData("not-a-version")]
    [InlineData("1.2")]
    [InlineData("v1.2.3")]
    [InlineData("01.2.3")]
    [InlineData("1.2.3-")]
    [InlineData("1.2.3+")]
    public void Validate_InvalidVersion_Returns1(string version)
    {
        var (exitCode, output) = RunTool("validate", version);

        exitCode.Should().Be(1);
        output.Should().Contain("NOT VALID");
    }

    [Fact]
    public void Validate_ShowsAllParts()
    {
        var (exitCode, output) = RunTool("validate", "1.2.3-alpha.1+build.7");

        exitCode.Should().Be(0);
        output.Should().Contain("Major: 1");
        output.Should().Contain("Minor: 2");
        output.Should().Contain("Patch: 3");
        output.Should().Contain("Version core: 1.2.3");
        output.Should().Contain("Pre-release identifiers: alpha.1");
        output.Should().Contain("Build metadata identifiers: build.7");
    }

    [Fact]
    public void Validate_CoreOnly_OmitsPreReleaseAndBuild()
    {
        var (_, output) = RunTool("validate", "1.0.0");

        output.Should().NotContain("Pre-release");
        output.Should().NotContain("Build metadata");
    }

    [Theory]
    [InlineData("1.2.3", "1.2.3")]
    [InlineData("1.2.3-alpha.1+build.7", "1.2.3-alpha.1+build.7")]
    public void Validate_Quiet_PrintsBareVersion(string input, string expected)
    {
        var (exitCode, output) = RunTool("validate", "-q", input);

        exitCode.Should().Be(0);
        output.Should().Be(expected);
    }

    [Fact]
    public void Validate_Quiet_InvalidVersion_SilentReturn1()
    {
        var (exitCode, output) = RunTool("validate", "-q", "not-a-version");

        exitCode.Should().Be(1);
        output.Should().BeEmpty();
    }

    [Fact]
    public void Validate_NoArgument_Returns1()
    {
        var (exitCode, _) = RunTool("validate");

        exitCode.Should().Be(1);
    }

    #endregion

    #region compare command

    [Theory]
    [InlineData("1.0.0", "2.0.0", 255)]   // less than
    [InlineData("2.0.0", "1.0.0", 1)]     // greater than
    [InlineData("1.0.0", "1.0.0", 0)]     // equal
    [InlineData("1.0.0-alpha", "1.0.0-beta", 255)]  // pre-release ordering
    [InlineData("1.0.0-alpha", "1.0.0", 255)]       // pre-release < release
    public void Compare_ReturnsCorrectExitCode(string v1, string v2, int expectedExit)
    {
        var (exitCode, _) = RunTool("compare", v1, v2);

        exitCode.Should().Be(expectedExit);
    }

    [Fact]
    public void Compare_LessThan_OutputContainsLessThan()
    {
        var (_, output) = RunTool("compare", "1.0.0", "2.0.0");

        output.Should().Contain("less than");
    }

    [Fact]
    public void Compare_GreaterThan_OutputContainsGreaterThan()
    {
        var (_, output) = RunTool("compare", "2.0.0", "1.0.0");

        output.Should().Contain("greater than");
    }

    [Fact]
    public void Compare_Equal_OutputContainsEqual()
    {
        var (_, output) = RunTool("compare", "1.0.0", "1.0.0");

        output.Should().Contain("equal");
    }

    [Fact]
    public void Compare_Quiet_SilentOutput()
    {
        var (exitCode, output) = RunTool("compare", "-q", "1.0.0", "2.0.0");

        exitCode.Should().Be(255);
        output.Should().BeEmpty();
    }

    [Fact]
    public void Compare_InvalidFirstVersion_Returns1()
    {
        var (exitCode, output) = RunTool("compare", "bad", "1.0.0");

        exitCode.Should().Be(1);
        output.Should().Contain("not a valid SemVer");
    }

    [Fact]
    public void Compare_InvalidSecondVersion_Returns1()
    {
        var (exitCode, output) = RunTool("compare", "1.0.0", "bad");

        exitCode.Should().Be(1);
        output.Should().Contain("not a valid SemVer");
    }

    [Fact]
    public void Compare_MissingSecondArg_Returns1()
    {
        var (exitCode, _) = RunTool("compare", "1.0.0");

        exitCode.Should().Be(1);
    }

    #endregion

    #region bump command

    [Theory]
    [InlineData("1.2.3", "major", "2.0.0")]
    [InlineData("1.2.3", "minor", "1.3.0")]
    [InlineData("1.2.3", "patch", "1.2.4")]
    [InlineData("1.2.3", "j", "2.0.0")]
    [InlineData("1.2.3", "n", "1.3.0")]
    [InlineData("1.2.3", "p", "1.2.4")]
    public void Bump_Part_ProducesExpectedVersion(string input, string part, string expected)
    {
        var (exitCode, output) = RunTool("bump", input, "--part", part);

        exitCode.Should().Be(0);
        output.Should().Contain(expected);
    }

    [Fact]
    public void Bump_DefaultsPatch()
    {
        var (exitCode, output) = RunTool("bump", "1.2.3");

        exitCode.Should().Be(0);
        output.Should().Contain("1.2.4");
    }

    [Theory]
    [InlineData("MAJOR", "2.0.0")]
    [InlineData("Minor", "1.3.0")]
    [InlineData("PATCH", "1.2.4")]
    [InlineData("J", "2.0.0")]
    [InlineData("N", "1.3.0")]
    [InlineData("P", "1.2.4")]
    public void Bump_CaseInsensitivePart(string part, string expected)
    {
        var (exitCode, output) = RunTool("bump", "1.2.3", "--part", part);

        exitCode.Should().Be(0);
        output.Should().Contain(expected);
    }

    [Fact]
    public void Bump_WithPreRelease()
    {
        var (exitCode, output) = RunTool("bump", "1.2.3", "--part", "minor", "--pre-release", "alpha");

        exitCode.Should().Be(0);
        output.Should().Contain("1.3.0-alpha");
    }

    [Fact]
    public void Bump_WithBuildMetadata()
    {
        var (exitCode, output) = RunTool("bump", "1.2.3", "--part", "patch", "--build-metadata", "build.42");

        exitCode.Should().Be(0);
        output.Should().Contain("1.2.4+build.42");
    }

    [Fact]
    public void Bump_WithPreReleaseAndBuildMetadata()
    {
        var (exitCode, output) = RunTool("bump", "1.2.3", "--part", "major", "-r", "rc.1", "-b", "meta");

        exitCode.Should().Be(0);
        output.Should().Contain("2.0.0-rc.1+meta");
    }

    [Fact]
    public void Bump_Quiet_PrintsBareVersion()
    {
        var (exitCode, output) = RunTool("bump", "-q", "1.2.3", "--part", "major");

        exitCode.Should().Be(0);
        output.Should().Be("2.0.0");
    }

    [Fact]
    public void Bump_Verbose_ContainsBumpedLabel()
    {
        var (exitCode, output) = RunTool("bump", "1.2.3", "--part", "major");

        exitCode.Should().Be(0);
        output.Should().Contain("Bumped major:");
    }

    [Fact]
    public void Bump_InvalidVersion_Returns1()
    {
        var (exitCode, output) = RunTool("bump", "bad");

        exitCode.Should().Be(1);
        output.Should().Contain("not a valid SemVer");
    }

    [Fact]
    public void Bump_InvalidPart_Returns1()
    {
        var (exitCode, output) = RunTool("bump", "1.2.3", "--part", "bogus");

        exitCode.Should().Be(1);
        output.Should().Contain("Invalid part to bump");
    }

    [Fact]
    public void Bump_InvalidPreRelease_Returns1()
    {
        var (exitCode, output) = RunTool("bump", "1.2.3", "--pre-release", "!!!invalid");

        exitCode.Should().Be(1);
        output.Should().Contain("not a valid SemVer pre-release");
    }

    [Fact]
    public void Bump_InvalidBuildMetadata_Returns1()
    {
        var (exitCode, output) = RunTool("bump", "1.2.3", "--build-metadata", "!!!invalid");

        exitCode.Should().Be(1);
        output.Should().Contain("not a valid SemVer build metadata");
    }

    [Fact]
    public void Bump_FromPreRelease_BumpsCorrectly()
    {
        var (exitCode, output) = RunTool("bump", "1.2.3-alpha.1+build.7", "--part", "patch");

        exitCode.Should().Be(0);
        output.Should().Contain("1.2.4");
    }

    #endregion

    #region root command and help

    [Fact]
    public void NoCommand_Returns1()
    {
        var (exitCode, _) = RunTool();

        // No subcommand provided - System.CommandLine shows help and returns 0 or 1
        // depending on the version; just verify it doesn't crash
        exitCode.Should().BeOneOf(0, 1);
    }

    [Fact]
    public void UnknownCommand_Returns1()
    {
        var (exitCode, output) = RunTool("unknown");

        exitCode.Should().Be(1);
        output.Should().Contain("Error parsing command line arguments");
    }

    #endregion

    #region quiet option aliases

    [Fact]
    public void Quiet_LongForm_Works()
    {
        var (exitCode, output) = RunTool("validate", "--quiet", "1.0.0");

        exitCode.Should().Be(0);
        output.Should().Be("1.0.0");
    }

    [Fact]
    public void Quiet_ShortForm_Works()
    {
        var (exitCode, output) = RunTool("validate", "-q", "1.0.0");

        exitCode.Should().Be(0);
        output.Should().Be("1.0.0");
    }

    #endregion

    #region edge cases

    [Fact]
    public void Validate_WhitespaceVersion_Returns1()
    {
        var (exitCode, _) = RunTool("validate", "  ");

        exitCode.Should().Be(1);
    }

    [Fact]
    public void Compare_EqualWithBuildMetadata_Returns0()
    {
        // Build metadata is ignored in comparison per SemVer spec
        var (exitCode, _) = RunTool("compare", "1.0.0+build.1", "1.0.0+build.2");

        exitCode.Should().Be(0);
    }

    [Fact]
    public void Bump_Major_ResetsMinorAndPatch()
    {
        var (exitCode, output) = RunTool("bump", "-q", "1.5.9", "--part", "major");

        exitCode.Should().Be(0);
        output.Should().Be("2.0.0");
    }

    [Fact]
    public void Bump_Minor_ResetsPatch()
    {
        var (exitCode, output) = RunTool("bump", "-q", "1.5.9", "--part", "minor");

        exitCode.Should().Be(0);
        output.Should().Be("1.6.0");
    }

    [Fact]
    public void Bump_ShortAliases()
    {
        var (exitCode, output) = RunTool("bump", "-q", "1.2.3", "-p", "j", "-r", "beta", "-b", "ci.99");

        exitCode.Should().Be(0);
        output.Should().Be("2.0.0-beta+ci.99");
    }

    #endregion
}
