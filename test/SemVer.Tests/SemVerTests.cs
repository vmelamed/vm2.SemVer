// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.SemVer;

public class SemVerTests
{
    public static TheoryData<string, int, int, int, string, string> ValidSemVerCases => new()
    {
        { "1.2.3", 1, 2, 3, "", "" },
        { "1.2.3-alpha", 1, 2, 3, "alpha", "" },
        { "1.2.3+build.7", 1, 2, 3, "", "build.7" },
        { "1.2.3-alpha.1+build.7", 1, 2, 3, "alpha.1", "build.7" },
        { "  1.2.3-rc.1+meta.9  ", 1, 2, 3, "rc.1", "meta.9" },
    };

    public static TheoryData<string> InvalidSemVerCases => new()
    {
        "",
        " ",
        "1",
        "1.2",
        "v1.2.3",
        "01.2.3",
        "1.02.3",
        "1.2.03",
        "1.2.3-",
        "1.2.3+",
        "1.2.3-01",
        "1.2.3-..",
    };

    public static TheoryData<string, string, int> CompareCases => new()
    {
        { "1.0.0", "2.0.0", -1 },
        { "2.1.0", "2.1.0", 0 },
        { "1.0.0-alpha", "1.0.0", -1 },
        { "1.0.0-alpha", "1.0.0-alpha.1", -1 },
        { "1.0.0-alpha.1", "1.0.0-alpha.beta", -1 },
        { "1.0.0-beta", "1.0.0-beta.2", -1 },
        { "1.0.0-beta.2", "1.0.0-beta.11", -1 },
        { "1.0.0-beta.11", "1.0.0-rc.1", -1 },
        { "1.0.0-rc.1", "1.0.0", -1 },
        { "1.0.0+build.1", "1.0.0+build.2", 0 },
        { "1.0.0-18446744073709551615", "1.0.0-2", 1 },
    };

    public static TheoryData<string, string, string> BumpCases => new()
    {
        { "major", "1.2.3-alpha+build.1", "2.0.0" },
        { "minor", "1.2.3-alpha+build.1", "1.3.0" },
        { "patch", "1.2.3-alpha+build.1", "1.2.4" },
    };

    public static TheoryData<string, string, string, string, string> BumpWithQualifiersCases => new()
    {
        { "major", "1.2.3-alpha+build.1", "rc.1", "build.7", "2.0.0-rc.1+build.7" },
        { "minor", "1.2.3-alpha+build.1", "beta", "meta.9", "1.3.0-beta+meta.9" },
        { "patch", "1.2.3-alpha+build.1", "preview.2", "ci.42", "1.2.4-preview.2+ci.42" },
    };

    public static TheoryData<int, int, int, string> CoreCtorArgumentCases => new()
    {
        { -1, 0, 0, "major" },
        { 0, -1, 0, "minor" },
        { 0, 0, -1, "patch" },
    };

    public static TheoryData<string?, string> InvalidQualifierCases => new()
    {
        { "01", "preRelease" },
        { "alpha..1", "preRelease" },
        { ".build", "buildMetadata" },
        { "build..7", "buildMetadata" },
    };

    [Theory]
    [MemberData(nameof(ValidSemVerCases))]
    public void Parse_WhenInputIsValid_ShouldReturnExpectedParts(
        string input,
        int major,
        int minor,
        int patch,
        string preRelease,
        string buildMetadata)
    {
        var parsed = global::vm2.SemVer.Parse(input);

        parsed.Major.Should().Be(major);
        parsed.Minor.Should().Be(minor);
        parsed.Patch.Should().Be(patch);
        parsed.PreRelease.Should().Be(preRelease);
        parsed.BuildMetadata.Should().Be(buildMetadata);
    }

    [Theory]
    [MemberData(nameof(ValidSemVerCases))]
    public void TryParse_WhenInputIsValid_ShouldSucceed(
        string input,
        int major,
        int minor,
        int patch,
        string preRelease,
        string buildMetadata)
    {
        var ok = global::vm2.SemVer.TryParse(input, null, out var parsed);

        ok.Should().BeTrue();
        parsed.Should().Be(new global::vm2.SemVer(major, minor, patch, preRelease, buildMetadata));
    }

    [Theory]
    [MemberData(nameof(InvalidSemVerCases))]
    public void TryParse_WhenInputIsInvalid_ShouldReturnFalse(string input)
    {
        var ok = global::vm2.SemVer.TryParse(input, null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }

    [Theory]
    [MemberData(nameof(InvalidSemVerCases))]
    public void Parse_WhenInputIsInvalid_ShouldThrowFormatException(string input)
    {
        Action act = () => global::vm2.SemVer.Parse(input, null);

        act.Should().Throw<FormatException>();
    }

    [Theory]
    [MemberData(nameof(InvalidSemVerCases))]
    public void CtorString_WhenInputIsInvalid_ShouldThrowFormatException(string input)
    {
        Action act = () => _ = new global::vm2.SemVer(input);

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void TryParse_WhenInputIsNull_ShouldReturnFalse()
    {
        var ok = global::vm2.SemVer.TryParse((string?)null, null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }

    [Fact]
    public void Parse_WhenInputIsNull_ShouldThrowFormatException()
    {
        Action act = () => global::vm2.SemVer.Parse((string)null!);

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Properties_ShouldReflectQualifierState_AndCoreShouldDropQualifiers()
    {
        var preRelease = new global::vm2.SemVer(1, 2, 3, "rc.1", "build.7");
        var stable = new global::vm2.SemVer(1, 2, 3);

        preRelease.IsPreRelease.Should().BeTrue();
        preRelease.IsStable.Should().BeFalse();
        preRelease.Core.Should().Be(new global::vm2.SemVer(1, 2, 3));

        stable.IsPreRelease.Should().BeFalse();
        stable.IsStable.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(CoreCtorArgumentCases))]
    public void Ctor_WhenCoreVersionIsNegative_ShouldThrowArgumentOutOfRangeException(int major, int minor, int patch, string paramName)
    {
        Action act = () => _ = new global::vm2.SemVer(major, minor, patch);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .Which.ParamName.Should().Be(paramName);
    }

    [Fact]
    public void Ctor_WhenPreReleaseIsInvalid_ShouldThrowArgumentException()
    {
        Action act = () => _ = new global::vm2.SemVer(1, 2, 3, "01");

        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("preRelease");
    }

    [Fact]
    public void Ctor_WhenBuildMetadataIsInvalid_ShouldThrowArgumentException()
    {
        Action act = () => _ = new global::vm2.SemVer(1, 2, 3, buildMetadata: "build..7");

        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("buildMetadata");
    }

    [Fact]
    public void CtorString_WhenInputIsValid_ShouldCopyParsedComponents()
    {
        var version = new global::vm2.SemVer("1.2.3-rc.1+build.7");

        version.Should().Be(new global::vm2.SemVer(1, 2, 3, "rc.1", "build.7"));
    }

    [Theory]
    [MemberData(nameof(CompareCases))]
    public void CompareTo_ShouldMatchSemVerPrecedence(string left, string right, int sign)
    {
        var lhs = global::vm2.SemVer.Parse(left);
        var rhs = global::vm2.SemVer.Parse(right);

        Math.Sign(lhs.CompareTo(rhs)).Should().Be(sign);
        Math.Sign(rhs.CompareTo(lhs)).Should().Be(-sign);
    }

    [Fact]
    public void CompareTo_WhenOnlyMinorDiffers_ShouldUseMinorComponent()
    {
        var lhs = global::vm2.SemVer.Parse("1.2.0");
        var rhs = global::vm2.SemVer.Parse("1.3.0");

        lhs.CompareTo(rhs).Should().BeNegative();
        rhs.CompareTo(lhs).Should().BePositive();
    }

    [Fact]
    public void CompareTo_WhenOnlyPatchDiffers_ShouldUsePatchComponent()
    {
        var lhs = global::vm2.SemVer.Parse("1.2.3");
        var rhs = global::vm2.SemVer.Parse("1.2.4");

        lhs.CompareTo(rhs).Should().BeNegative();
        rhs.CompareTo(lhs).Should().BePositive();
    }

    [Fact]
    public void CompareTo_WhenPreReleaseIdentifiersMatch_ShouldReturnZero()
    {
        var lhs = global::vm2.SemVer.Parse("1.2.3-alpha.1");
        var rhs = global::vm2.SemVer.Parse("1.2.3-alpha.1+build.9");

        lhs.CompareTo(rhs).Should().Be(0);
    }

    [Fact]
    public void EqualsObject_WhenObjectIsDifferentType_ShouldReturnFalse()
    {
        var version = global::vm2.SemVer.Parse("1.2.3");

        version.Equals("1.2.3").Should().BeFalse();
    }

    [Fact]
    public void Equals_WhenOnlyBuildMetadataDiffers_ShouldBeEqual()
    {
        var a = global::vm2.SemVer.Parse("1.2.3-alpha+build.1");
        var b = global::vm2.SemVer.Parse("1.2.3-alpha+build.2");

        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Equals_WhenPreReleaseCaseDiffers_ShouldNotBeEqual()
    {
        var a = global::vm2.SemVer.Parse("1.2.3-alpha");
        var b = global::vm2.SemVer.Parse("1.2.3-Alpha");

        a.Should().NotBe(b);
    }

    [Theory]
    [MemberData(nameof(BumpCases))]
    public void BumpMethods_ShouldReturnExpectedVersion(string bump, string source, string expected)
    {
        var version = global::vm2.SemVer.Parse(source);

        var next = bump switch
        {
            "major" => version.BumpMajor(),
            "minor" => version.BumpMinor(),
            "patch" => version.BumpPatch(),
            _ => throw new InvalidOperationException("Unsupported bump kind."),
        };

        next.ToString().Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(BumpWithQualifiersCases))]
    public void BumpMethods_WhenQualifiersProvided_ShouldApplyThem(
        string bump,
        string source,
        string preRelease,
        string buildMetadata,
        string expected)
    {
        var version = global::vm2.SemVer.Parse(source);

        var next = bump switch
        {
            "major" => version.BumpMajor(preRelease, buildMetadata),
            "minor" => version.BumpMinor(preRelease, buildMetadata),
            "patch" => version.BumpPatch(preRelease, buildMetadata),
            _ => throw new InvalidOperationException("Unsupported bump kind."),
        };

        next.ToString().Should().Be(expected);
    }

    [Fact]
    public void WithPreRelease_ShouldPreserveBuildMetadata()
    {
        var version = global::vm2.SemVer.Parse("1.2.3+build.7");

        var changed = version.WithPreRelease("rc.1");

        changed.ToString().Should().Be("1.2.3-rc.1+build.7");
    }

    [Fact]
    public void WithPreRelease_WhenValueIsWhitespace_ShouldClearPreRelease()
    {
        var version = global::vm2.SemVer.Parse("1.2.3-rc.1+build.7");

        var changed = version.WithPreRelease("   ");

        changed.ToString().Should().Be("1.2.3+build.7");
    }

    [Fact]
    public void WithPreRelease_WhenValueIsInvalid_ShouldThrowArgumentException()
    {
        var version = global::vm2.SemVer.Parse("1.2.3+build.7");

        Action act = () => _ = version.WithPreRelease("01");

        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("preRelease");
    }

    [Fact]
    public void WithBuildMetadata_ShouldPreservePreRelease()
    {
        var version = global::vm2.SemVer.Parse("1.2.3-rc.1");

        var changed = version.WithBuildMetadata("build.7");

        changed.ToString().Should().Be("1.2.3-rc.1+build.7");
    }

    [Fact]
    public void WithBuildMetadata_WhenValueIsWhitespace_ShouldClearBuildMetadata()
    {
        var version = global::vm2.SemVer.Parse("1.2.3-rc.1+build.7");

        var changed = version.WithBuildMetadata("   ");

        changed.ToString().Should().Be("1.2.3-rc.1");
    }

    [Fact]
    public void WithBuildMetadata_WhenValueIsInvalid_ShouldThrowArgumentException()
    {
        var version = global::vm2.SemVer.Parse("1.2.3-rc.1");

        Action act = () => _ = version.WithBuildMetadata("build..7");

        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("buildMetadata");
    }

    [Fact]
    public void Release_ShouldClearPreReleaseAndBuildMetadata()
    {
        var version = global::vm2.SemVer.Parse("1.2.3-rc.1+build.7");

        var released = version.Release();

        released.ToString().Should().Be("1.2.3");
    }

    [Fact]
    public void TryFormat_WhenDestinationIsTooSmall_ShouldReturnFalse()
    {
        var version = global::vm2.SemVer.Parse("1.2.3-rc.1+build.7");
        Span<char> destination = stackalloc char[4];

        var ok = version.TryFormat(destination, out var charsWritten, default, null);

        ok.Should().BeFalse();
        charsWritten.Should().Be(0);
    }

    [Fact]
    public void TryFormat_WhenDestinationHasEnoughRoom_ShouldWriteFullValue()
    {
        var version = global::vm2.SemVer.Parse("1.2.3-rc.1+build.7");
        Span<char> destination = stackalloc char[64];

        var ok = version.TryFormat(destination, out var charsWritten, default, null);

        ok.Should().BeTrue();
        charsWritten.Should().Be("1.2.3-rc.1+build.7".Length);
        new string(destination[..charsWritten]).Should().Be("1.2.3-rc.1+build.7");
    }

    [Fact]
    public void ParseSpan_WhenInputIsValid_ShouldReturnExpectedVersion()
    {
        var parsed = global::vm2.SemVer.Parse(" 1.2.3-rc.1+build.7 ".AsSpan(), null);

        parsed.Should().Be(new global::vm2.SemVer(1, 2, 3, "rc.1", "build.7"));
    }

    [Fact]
    public void ParseSpan_WhenInputIsInvalid_ShouldThrowFormatException()
    {
        Action act = () => global::vm2.SemVer.Parse("not-semver".AsSpan(), null);

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void TryParseSpan_WhenInputIsValid_ShouldSucceed()
    {
        var ok = global::vm2.SemVer.TryParse(" 1.2.3-alpha+build.7 ".AsSpan(), null, out var parsed);

        ok.Should().BeTrue();
        parsed.Should().Be(new global::vm2.SemVer(1, 2, 3, "alpha", "build.7"));
    }

    [Fact]
    public void TryParseSpan_WhenInputIsEmpty_ShouldReturnFalse()
    {
        var ok = global::vm2.SemVer.TryParse(ReadOnlySpan<char>.Empty, null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }

    [Fact]
    public void TryParseSpan_WhenInputIsWhitespace_ShouldReturnFalse()
    {
        var ok = global::vm2.SemVer.TryParse("   ".AsSpan(), null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }

    [Fact]
    public void TryParseSpan_WhenInputIsInvalid_ShouldReturnFalse()
    {
        var ok = global::vm2.SemVer.TryParse("1.2".AsSpan(), null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }

    [Fact]
    public void ToStringFormat_WhenFormatIsNullOrEmpty_ShouldReturnDefaultString()
    {
        var version = global::vm2.SemVer.Parse("1.2.3-rc.1+build.7");

        version.ToString(null, null).Should().Be("1.2.3-rc.1+build.7");
        version.ToString(string.Empty, null).Should().Be("1.2.3-rc.1+build.7");
    }

    [Fact]
    public void ToStringFormat_WhenFormatIsUnsupported_ShouldThrowFormatException()
    {
        var version = global::vm2.SemVer.Parse("1.2.3");

        Action act = () => _ = version.ToString("G", null);

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Operators_ShouldDelegateToEqualityAndComparisonSemantics()
    {
        var left = global::vm2.SemVer.Parse("1.2.3-alpha+build.1");
        var equal = global::vm2.SemVer.Parse("1.2.3-alpha+build.2");
        var greater = global::vm2.SemVer.Parse("1.2.3");

        (left == equal).Should().BeTrue();
        (left != greater).Should().BeTrue();
        (left < greater).Should().BeTrue();
        (left <= equal).Should().BeTrue();
        (greater > left).Should().BeTrue();
        (greater >= equal).Should().BeTrue();
    }
}
