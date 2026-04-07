// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.SemVer;

#pragma warning disable IL2026 // Trim analyzer: Newtonsoft.Json and System.Text.Json reflection-based APIs are safe in test code

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

    public static TheoryData<int, int, int, string, string> SemVerComponentCases => new()
    {
        { 1, 2, 3, "", "" },
        { 1, 2, 3, "alpha", "" },
        { 1, 2, 3, "", "build.7" },
        { 1, 2, 3, "alpha.1", "build.7" },
        { 1, 2, 3, "rc.1", "meta.9" },
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
        var parsed = vm2.SemVer.Parse(input);

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
        var ok = vm2.SemVer.TryParse(input, null, out var parsed);

        ok.Should().BeTrue();
        parsed.Should().Be(new vm2.SemVer(major, minor, patch, preRelease, buildMetadata));
    }

    [Theory]
    [MemberData(nameof(InvalidSemVerCases))]
    public void TryParse_WhenInputIsInvalid_ShouldReturnFalse(string input)
    {
        var ok = vm2.SemVer.TryParse(input, null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }

    [Theory]
    [MemberData(nameof(InvalidSemVerCases))]
    public void Parse_WhenInputIsInvalid_ShouldThrowFormatException(string input)
    {
        Action act = () => vm2.SemVer.Parse(input, null);

        act.Should().Throw<FormatException>();
    }

    [Theory]
    [MemberData(nameof(InvalidSemVerCases))]
    public void CtorString_WhenInputIsInvalid_ShouldThrowFormatException(string input)
    {
        Action act = () => _ = new vm2.SemVer(input);

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void TryParse_WhenInputIsNull_ShouldReturnFalse()
    {
        var ok = vm2.SemVer.TryParse((string?)null, null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }

    [Fact]
    public void Parse_WhenInputIsNull_ShouldThrowFormatException()
    {
        Action act = () => vm2.SemVer.Parse((string)null!);

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Properties_ShouldReflectQualifierState_AndCoreShouldDropQualifiers()
    {
        var preRelease = new vm2.SemVer(1, 2, 3, "rc.1", "build.7");
        var stable = new vm2.SemVer(1, 2, 3);

        preRelease.IsPreRelease.Should().BeTrue();
        preRelease.IsStable.Should().BeFalse();
        preRelease.Core.Should().Be(new vm2.SemVer(1, 2, 3));

        stable.IsPreRelease.Should().BeFalse();
        stable.IsStable.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(CoreCtorArgumentCases))]
    public void Ctor_WhenCoreVersionIsNegative_ShouldThrowArgumentOutOfRangeException(int major, int minor, int patch, string paramName)
    {
        Action act = () => _ = new vm2.SemVer(major, minor, patch);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .Which.ParamName.Should().Be(paramName);
    }

    [Fact]
    public void Ctor_WhenPreReleaseIsInvalid_ShouldThrowArgumentException()
    {
        Action act = () => _ = new vm2.SemVer(1, 2, 3, "01");

        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("preRelease");
    }

    [Fact]
    public void Ctor_WhenBuildMetadataIsInvalid_ShouldThrowArgumentException()
    {
        Action act = () => _ = new vm2.SemVer(1, 2, 3, buildMetadata: "build..7");

        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("buildMetadata");
    }

    public static TheoryData<string, int, int, int, string, string> ValidCtorSemVerCases { get; } = new TheoryData<string, int, int, int, string, string>
    {
        { "1.2.3", 1, 2, 3, "", "" },
        { "1.2.3-alpha", 1, 2, 3, "alpha", "" },
        { "1.2.3+build.7", 1, 2, 3, "", "build.7" },
        { "1.2.3-alpha.1+build.7", 1, 2, 3, "alpha.1", "build.7" },
        { "  1.2.3-rc.1+meta.9  ", 1, 2, 3, "rc.1", "meta.9" },
        { "  123456789.234567890.345678901-rc.1+meta.9  ", 123456789, 234567890, 345678901, "rc.1", "meta.9" },
    };

    [Theory]
    [MemberData(nameof(ValidCtorSemVerCases))]
    public void CtorString_WhenInputIsValid_ShouldCopyParsedComponents(string input, int major, int minor, int patch, string preRelease, string buildMetadata)
    {
        var version = new vm2.SemVer(input);

        version.Should().Be(new vm2.SemVer(major, minor, patch, preRelease, buildMetadata));
    }

    [Theory]
    [MemberData(nameof(CompareCases))]
    public void CompareTo_ShouldMatchSemVerPrecedence(string left, string right, int sign)
    {
        var lhs = vm2.SemVer.Parse(left);
        var rhs = vm2.SemVer.Parse(right);

        Math.Sign(lhs.CompareTo(rhs)).Should().Be(sign);
        Math.Sign(rhs.CompareTo(lhs)).Should().Be(-sign);
    }

    [Fact]
    public void CompareTo_WhenOnlyMinorDiffers_ShouldUseMinorComponent()
    {
        var lhs = vm2.SemVer.Parse("1.2.0");
        var rhs = vm2.SemVer.Parse("1.3.0");

        lhs.CompareTo(rhs).Should().BeNegative();
        rhs.CompareTo(lhs).Should().BePositive();
    }

    [Fact]
    public void CompareTo_WhenOnlyPatchDiffers_ShouldUsePatchComponent()
    {
        var lhs = vm2.SemVer.Parse("1.2.3");
        var rhs = vm2.SemVer.Parse("1.2.4");

        lhs.CompareTo(rhs).Should().BeNegative();
        rhs.CompareTo(lhs).Should().BePositive();
    }

    [Fact]
    public void CompareTo_WhenPreReleaseIdentifiersMatch_ShouldReturnZero()
    {
        var lhs = vm2.SemVer.Parse("1.2.3-alpha.1");
        var rhs = vm2.SemVer.Parse("1.2.3-alpha.1+build.9");

        lhs.CompareTo(rhs).Should().Be(0);
    }

    [Fact]
    public void EqualsObject_WhenObjectIsDifferentType_ShouldReturnFalse()
    {
        var version = vm2.SemVer.Parse("1.2.3");

        version.Equals("1.2.3").Should().BeFalse();
    }

    [Fact]
    public void Equals_WhenOnlyBuildMetadataDiffers_ShouldBeEqual()
    {
        var a = vm2.SemVer.Parse("1.2.3-alpha+build.1");
        var b = vm2.SemVer.Parse("1.2.3-alpha+build.2");

        a.Should().Be(b);
        a.GetHashCode().Should().Be(b.GetHashCode());
    }

    [Fact]
    public void Equals_WhenPreReleaseCaseDiffers_ShouldNotBeEqual()
    {
        var a = vm2.SemVer.Parse("1.2.3-alpha");
        var b = vm2.SemVer.Parse("1.2.3-Alpha");

        a.Should().NotBe(b);
    }

    [Theory]
    [MemberData(nameof(BumpCases))]
    public void BumpMethods_ShouldReturnExpectedVersion(string bump, string source, string expected)
    {
        var version = vm2.SemVer.Parse(source);

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
        var version = vm2.SemVer.Parse(source);

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
        var version = vm2.SemVer.Parse("1.2.3+build.7");

        var changed = version.WithPreRelease("rc.1");

        changed.ToString().Should().Be("1.2.3-rc.1+build.7");
    }

    [Fact]
    public void WithPreRelease_WhenValueIsWhitespace_ShouldClearPreRelease()
    {
        var version = vm2.SemVer.Parse("1.2.3-rc.1+build.7");

        var changed = version.WithPreRelease("   ");

        changed.ToString().Should().Be("1.2.3+build.7");
    }

    [Fact]
    public void WithPreRelease_WhenValueIsInvalid_ShouldThrowArgumentException()
    {
        var version = vm2.SemVer.Parse("1.2.3+build.7");

        Action act = () => _ = version.WithPreRelease("01");

        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("preRelease");
    }

    [Fact]
    public void WithBuildMetadata_ShouldPreservePreRelease()
    {
        var version = vm2.SemVer.Parse("1.2.3-rc.1");

        var changed = version.WithBuildMetadata("build.7");

        changed.ToString().Should().Be("1.2.3-rc.1+build.7");
    }

    [Fact]
    public void WithBuildMetadata_WhenValueIsWhitespace_ShouldClearBuildMetadata()
    {
        var version = vm2.SemVer.Parse("1.2.3-rc.1+build.7");

        var changed = version.WithBuildMetadata("   ");

        changed.ToString().Should().Be("1.2.3-rc.1");
    }

    [Fact]
    public void WithBuildMetadata_WhenValueIsInvalid_ShouldThrowArgumentException()
    {
        var version = vm2.SemVer.Parse("1.2.3-rc.1");

        Action act = () => _ = version.WithBuildMetadata("build..7");

        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("buildMetadata");
    }

    [Fact]
    public void Release_ShouldClearPreReleaseAndBuildMetadata()
    {
        var version = vm2.SemVer.Parse("1.2.3-rc.1+build.7");

        var released = version.Release();

        released.ToString().Should().Be("1.2.3");
    }

    [Fact]
    public void TryFormat_WhenDestinationIsTooSmall_ShouldReturnFalse()
    {
        var version = vm2.SemVer.Parse("1.2.3-rc.1+build.7");
        Span<char> destination = stackalloc char[4];

        var ok = version.TryFormat(destination, out var charsWritten, default, null);

        ok.Should().BeFalse();
        charsWritten.Should().Be(0);
    }

    [Fact]
    public void TryFormat_WhenDestinationHasEnoughRoom_ShouldWriteFullValue()
    {
        var version = vm2.SemVer.Parse("1.2.3-rc.1+build.7");
        Span<char> destination = stackalloc char[64];

        var ok = version.TryFormat(destination, out var charsWritten, default, null);

        ok.Should().BeTrue();
        charsWritten.Should().Be("1.2.3-rc.1+build.7".Length);
        new string(destination[..charsWritten]).Should().Be("1.2.3-rc.1+build.7");
    }

    [Fact]
    public void ParseSpan_WhenInputIsValid_ShouldReturnExpectedVersion()
    {
        var parsed = vm2.SemVer.Parse(" 1.2.3-rc.1+build.7 ".AsSpan(), null);

        parsed.Should().Be(new vm2.SemVer(1, 2, 3, "rc.1", "build.7"));
    }

    [Fact]
    public void ParseSpan_WhenInputIsInvalid_ShouldThrowFormatException()
    {
        Action act = () => vm2.SemVer.Parse("not-semver".AsSpan(), null);

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void TryParseSpan_WhenInputIsValid_ShouldSucceed()
    {
        var ok = vm2.SemVer.TryParse(" 1.2.3-alpha+build.7 ".AsSpan(), null, out var parsed);

        ok.Should().BeTrue();
        parsed.Should().Be(new vm2.SemVer(1, 2, 3, "alpha", "build.7"));
    }

    [Fact]
    public void TryParseSpan_WhenInputIsEmpty_ShouldReturnFalse()
    {
        var ok = vm2.SemVer.TryParse(ReadOnlySpan<char>.Empty, null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }

    [Fact]
    public void TryParseSpan_WhenInputIsWhitespace_ShouldReturnFalse()
    {
        var ok = vm2.SemVer.TryParse("   ".AsSpan(), null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }

    [Fact]
    public void TryParseSpan_WhenInputIsInvalid_ShouldReturnFalse()
    {
        var ok = vm2.SemVer.TryParse("1.2".AsSpan(), null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }

    [Fact]
    public void ToStringFormat_WhenFormatIsNullOrEmpty_ShouldReturnDefaultString()
    {
        var version = vm2.SemVer.Parse("1.2.3-rc.1+build.7");

        version.ToString(null, null).Should().Be("1.2.3-rc.1+build.7");
        version.ToString(string.Empty, null).Should().Be("1.2.3-rc.1+build.7");
    }

    [Fact]
    public void ToStringFormat_WhenFormatIsUnsupported_ShouldThrowFormatException()
    {
        var version = vm2.SemVer.Parse("1.2.3");

        Action act = () => _ = version.ToString("G", null);

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Operators_ShouldDelegateToEqualityAndComparisonSemantics()
    {
        var left = vm2.SemVer.Parse("1.2.3-alpha+build.1");
        var equal = vm2.SemVer.Parse("1.2.3-alpha+build.2");
        var greater = vm2.SemVer.Parse("1.2.3");

        (left == equal).Should().BeTrue();
        (left != greater).Should().BeTrue();
        (left < greater).Should().BeTrue();
        (left <= equal).Should().BeTrue();
        (greater > left).Should().BeTrue();
        (greater >= equal).Should().BeTrue();
    }

    #region Length accuracy
    public static TheoryData<string, int> LengthCases => new()
    {
        { "0.0.0", 5 },
        { "1.2.3", 5 },
        { "1.2.3-a", 7 },
        { "1.2.3+b", 7 },
        { "1.2.3-a+b", 9 },
        { "1.2.3-rc.1+build.7", 18 },
        { "999999999.999999999.999999999", 29 },
    };

    [Theory]
    [MemberData(nameof(LengthCases))]
    public void Length_ShouldMatchActualStringLength(string input, int expectedLength)
    {
        var version = vm2.SemVer.Parse(input);

        version.Length.Should().Be(expectedLength);
        version.ToString().Length.Should().Be(expectedLength);
    }

    [Fact]
    public void Length_WhenConstructedFromString_ShouldBeCorrect()
    {
        var version = new vm2.SemVer("1.2.3-rc.1+build.7");

        version.Length.Should().Be("1.2.3-rc.1+build.7".Length);
    }
    #endregion

    #region IUtf8SpanFormattable
    [Theory]
    [MemberData(nameof(SemVerComponentCases))]
    public void TryFormatUtf8_WhenDestinationHasEnoughRoom_ShouldWriteCorrectBytes(
        int major, int minor, int patch, string preRelease, string buildMetadata)
    {
        var version = new vm2.SemVer(major, minor, patch, preRelease, buildMetadata);
        var expected = version.ToString();
        Span<byte> destination = stackalloc byte[256];

        var ok = version.TryFormat(destination, out var bytesWritten);

        ok.Should().BeTrue();
        Encoding.UTF8.GetString(destination[..bytesWritten]).Should().Be(expected);
    }

    [Fact]
    public void TryFormatUtf8_WhenDestinationIsTooSmall_ShouldReturnFalse()
    {
        var version = vm2.SemVer.Parse("1.2.3-rc.1+build.7");
        Span<byte> destination = stackalloc byte[4];

        var ok = version.TryFormat(destination, out var bytesWritten);

        ok.Should().BeFalse();
        bytesWritten.Should().Be(0);
    }

    [Fact]
    public void TryFormatUtf8_WhenFormatIsNonEmpty_ShouldThrowFormatException()
    {
        var version = vm2.SemVer.Parse("1.2.3");
        var destination = new byte[64];

        Action act = () => version.TryFormat(destination, out _, "G");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void TryFormatUtf8_WhenVersionIsDefault_ShouldWriteZeroZeroZero()
    {
        var version = default(vm2.SemVer);
        Span<byte> destination = stackalloc byte[64];

        var ok = version.TryFormat(destination, out var bytesWritten);

        ok.Should().BeTrue();
        Encoding.UTF8.GetString(destination[..bytesWritten]).Should().Be("0.0.0");
    }
    #endregion

    #region IUtf8SpanParsable
    [Theory]
    [MemberData(nameof(ValidSemVerCases))]
    public void TryParseUtf8_WhenInputIsValid_ShouldSucceed(
        string input, int major, int minor, int patch, string preRelease, string buildMetadata)
    {
        var utf8 = Encoding.UTF8.GetBytes(input);

        var ok = vm2.SemVer.TryParse(utf8, null, out var parsed);

        ok.Should().BeTrue();
        parsed.Should().Be(new vm2.SemVer(major, minor, patch, preRelease, buildMetadata));
    }

    [Theory]
    [MemberData(nameof(InvalidSemVerCases))]
    public void TryParseUtf8_WhenInputIsInvalid_ShouldReturnFalse(string input)
    {
        var utf8 = Encoding.UTF8.GetBytes(input);

        var ok = vm2.SemVer.TryParse(utf8, null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }

    [Fact]
    public void TryParseUtf8_WhenInputIsEmpty_ShouldReturnFalse()
    {
        var ok = vm2.SemVer.TryParse(ReadOnlySpan<byte>.Empty, null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }

    [Fact]
    public void ParseUtf8_WhenInputIsValid_ShouldReturnExpectedVersion()
    {
        var utf8 = Encoding.UTF8.GetBytes("1.2.3-rc.1+build.7");

        var parsed = vm2.SemVer.Parse(utf8, null);

        parsed.Should().Be(new vm2.SemVer(1, 2, 3, "rc.1", "build.7"));
    }

    [Fact]
    public void ParseUtf8_WhenInputIsInvalid_ShouldThrowFormatException()
    {
        var utf8 = Encoding.UTF8.GetBytes("not-semver");

        Action act = () => vm2.SemVer.Parse(utf8, null);

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void TryParseUtf8_WhenInputIsInvalidUtf8_ShouldReturnFalse()
    {
        // 0xFF is not valid UTF-8
        ReadOnlySpan<byte> badUtf8 = [0xFF, 0xFE, 0x31, 0x2E, 0x32, 0x2E, 0x33];

        var ok = vm2.SemVer.TryParse(badUtf8, null, out var parsed);

        ok.Should().BeFalse();
        parsed.Should().Be(default);
    }
    #endregion

    #region UTF-8 round-trip
    [Theory]
    [MemberData(nameof(SemVerComponentCases))]
    public void Utf8RoundTrip_FormatThenParse_ShouldProduceSameVersion(
        int major, int minor, int patch, string preRelease, string buildMetadata)
    {
        var original = new vm2.SemVer(major, minor, patch, preRelease, buildMetadata);
        Span<byte> buffer = stackalloc byte[256];

        original.TryFormat(buffer, out var bytesWritten).Should().BeTrue();

        var ok = vm2.SemVer.TryParse(buffer[..bytesWritten], null, out var parsed);

        ok.Should().BeTrue();
        parsed.Should().Be(original);
    }
    #endregion

    #region TryFormat char-span edge cases
    [Fact]
    public void TryFormatChar_WhenFormatIsNonEmpty_ShouldThrowFormatException()
    {
        var version = vm2.SemVer.Parse("1.2.3");
        var destination = new char[64];

        Action act = () => version.TryFormat(destination, out _, "G");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void TryFormatChar_WhenExactSize_ShouldSucceed()
    {
        var version = vm2.SemVer.Parse("1.2.3-rc.1+build.7");
        Span<char> destination = stackalloc char[version.Length];

        var ok = version.TryFormat(destination, out var charsWritten);

        ok.Should().BeTrue();
        charsWritten.Should().Be(version.Length);
        new string(destination[..charsWritten]).Should().Be("1.2.3-rc.1+build.7");
    }
    #endregion

    #region System.Text.Json converter
    [Theory]
    [MemberData(nameof(SemVerComponentCases))]
    public void SysJson_RoundTrip_ShouldPreserveValue(
        int major, int minor, int patch, string preRelease, string buildMetadata)
    {
        var original = new vm2.SemVer(major, minor, patch, preRelease, buildMetadata);

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<vm2.SemVer>(json);

        deserialized.Should().Be(original);
    }

    [Fact]
    public void SysJson_Serialize_ShouldWriteQuotedString()
    {
        var version = new vm2.SemVer(1, 2, 3, "rc.1", "build.7");

        var json = JsonSerializer.Serialize(version);

        // Utf8JsonWriter escapes '+' as '\u002B' by default
        using var doc = JsonDocument.Parse(json);
        doc.RootElement.GetString().Should().Be("1.2.3-rc.1+build.7");
    }

    [Fact]
    public void SysJson_Serialize_CoreVersion_ShouldWriteQuotedString()
    {
        var version = new vm2.SemVer(1, 2, 3);

        var json = JsonSerializer.Serialize(version);

        json.Should().Be("\"1.2.3\"");
    }

    [Fact]
    public void SysJson_Deserialize_NullableSemVer_WhenJsonIsNull_ShouldReturnNull()
    {
        var result = JsonSerializer.Deserialize<vm2.SemVer?>("null");

        result.Should().BeNull();
    }

    [Fact]
    public void SysJson_Deserialize_WhenJsonIsInvalidSemVer_ShouldThrowJsonException()
    {
        Action act = () => JsonSerializer.Deserialize<vm2.SemVer>("\"not-a-semver\"");

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void SysJson_Deserialize_WhenJsonIsNumber_ShouldThrowJsonException()
    {
        Action act = () => JsonSerializer.Deserialize<vm2.SemVer>("42");

        act.Should().Throw<JsonException>();
    }

    [Fact]
    public void SysJson_RoundTrip_NullableSemVer_WithValue()
    {
        var original = (vm2.SemVer?)new vm2.SemVer(1, 2, 3);

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<vm2.SemVer?>(json);

        deserialized.Should().Be(original);
    }

    [Fact]
    public void SysJson_RoundTrip_NullableSemVer_Null()
    {
        vm2.SemVer? original = null;

        var json = JsonSerializer.Serialize(original);
        var deserialized = JsonSerializer.Deserialize<vm2.SemVer?>(json);

        json.Should().Be("null");
        deserialized.Should().BeNull();
    }

    [Fact]
    public void SysJson_RoundTrip_ObjectWithSemVerProperty()
    {
        var obj = new SysJsonTestModel { Version = new vm2.SemVer(1, 2, 3, "beta.1"), Label = "test" };

        var json = JsonSerializer.Serialize(obj);
        var deserialized = JsonSerializer.Deserialize<SysJsonTestModel>(json);

        deserialized!.Version.Should().Be(obj.Version);
        deserialized.Label.Should().Be("test");
    }

    [Fact]
    public void SysJson_RoundTrip_ObjectWithNullableSemVerProperty_Null()
    {
        var obj = new SysJsonTestModelNullable { Version = null, Label = "test" };

        var json = JsonSerializer.Serialize(obj);
        var deserialized = JsonSerializer.Deserialize<SysJsonTestModelNullable>(json);

        deserialized!.Version.Should().BeNull();
    }

    private sealed class SysJsonTestModel
    {
        public vm2.SemVer Version { get; set; }
        public string Label { get; set; } = "";
    }

    private sealed class SysJsonTestModelNullable
    {
        public vm2.SemVer? Version { get; set; }
        public string Label { get; set; } = "";
    }
    #endregion

    #region Newtonsoft.Json converter
    [Theory]
    [MemberData(nameof(SemVerComponentCases))]
    public void NsJson_RoundTrip_ShouldPreserveValue(
        int major, int minor, int patch, string preRelease, string buildMetadata)
    {
        var original = new vm2.SemVer(major, minor, patch, preRelease, buildMetadata);

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(original);
        var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<vm2.SemVer>(json);

        deserialized.Should().Be(original);
    }

    [Fact]
    public void NsJson_Serialize_ShouldWriteQuotedString()
    {
        var version = new vm2.SemVer(1, 2, 3, "rc.1", "build.7");

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(version);

        json.Should().Be("\"1.2.3-rc.1+build.7\"");
    }

    [Fact]
    public void NsJson_Deserialize_NullableSemVer_WhenJsonIsNull_ShouldReturnNull()
    {
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<vm2.SemVer?>("null");

        result.Should().BeNull();
    }

    [Fact]
    public void NsJson_Deserialize_WhenJsonIsInvalidSemVer_ShouldThrowJsonReaderException()
    {
        Action act = () => Newtonsoft.Json.JsonConvert.DeserializeObject<vm2.SemVer>("\"not-a-semver\"");

        act.Should().Throw<Newtonsoft.Json.JsonReaderException>();
    }

    [Fact]
    public void NsJson_Deserialize_WhenJsonIsNumber_ShouldThrowJsonReaderException()
    {
        Action act = () => Newtonsoft.Json.JsonConvert.DeserializeObject<vm2.SemVer>("42");

        act.Should().Throw<Newtonsoft.Json.JsonReaderException>();
    }

    [Fact]
    public void NsJson_RoundTrip_NullableSemVer_WithValue()
    {
        var original = (vm2.SemVer?)new vm2.SemVer(1, 2, 3);

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(original);
        var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<vm2.SemVer?>(json);

        deserialized.Should().Be(original);
    }

    [Fact]
    public void NsJson_RoundTrip_NullableSemVer_Null()
    {
        vm2.SemVer? original = null;

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(original);
        var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<vm2.SemVer?>(json);

        json.Should().Be("null");
        deserialized.Should().BeNull();
    }

    [Fact]
    public void NsJson_RoundTrip_ObjectWithSemVerProperty()
    {
        var obj = new NsJsonTestModel { Version = new vm2.SemVer(1, 2, 3, "beta.1"), Label = "test" };

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<NsJsonTestModel>(json);

        deserialized!.Version.Should().Be(obj.Version);
        deserialized.Label.Should().Be("test");
    }

    [Fact]
    public void NsJson_RoundTrip_ObjectWithNullableSemVerProperty_Null()
    {
        var obj = new NsJsonTestModelNullable { Version = null, Label = "test" };

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<NsJsonTestModelNullable>(json);

        deserialized!.Version.Should().BeNull();
    }

    private sealed class NsJsonTestModel
    {
        public vm2.SemVer Version { get; set; }
        public string Label { get; set; } = "";
    }

    private sealed class NsJsonTestModelNullable
    {
        public vm2.SemVer? Version { get; set; }
        public string Label { get; set; } = "";
    }
    #endregion

    #region Edge cases
    [Fact]
    public void Default_ShouldBeZeroZeroZero()
    {
        var version = default(vm2.SemVer);

        version.Major.Should().Be(0);
        version.Minor.Should().Be(0);
        version.Patch.Should().Be(0);
        version.PreRelease.Should().Be("");
        version.BuildMetadata.Should().Be("");
        version.Length.Should().Be(5);
        version.ToString().Should().Be("0.0.0");
    }

    [Fact]
    public void ZeroVersion_ShouldBeValidZeroZeroZero()
    {
        var version = new vm2.SemVer(0, 0, 0);

        version.Major.Should().Be(0);
        version.Minor.Should().Be(0);
        version.Patch.Should().Be(0);
        version.PreRelease.Should().Be("");
        version.BuildMetadata.Should().Be("");
        version.Length.Should().Be(5);
        version.ToString().Should().Be("0.0.0");
    }

    [Fact]
    public void HugeVersionNumbers_ShouldFormatAndParsecorrectly()
    {
        var version = new vm2.SemVer(int.MaxValue, int.MaxValue, int.MaxValue);
        var expected = $"{int.MaxValue}.{int.MaxValue}.{int.MaxValue}";

        version.ToString().Should().Be(expected);
        version.Length.Should().Be(expected.Length);

        var parsed = vm2.SemVer.Parse(expected);
        parsed.Should().Be(version);
    }

    [Fact]
    public void HugeVersionNumbers_Utf8RoundTrip()
    {
        var version = new vm2.SemVer(int.MaxValue, int.MaxValue, int.MaxValue);
        var expected = $"{int.MaxValue}.{int.MaxValue}.{int.MaxValue}";
        Span<byte> buffer = stackalloc byte[256];

        version.TryFormat(buffer, out var bytesWritten).Should().BeTrue();
        Encoding.UTF8.GetString(buffer[..bytesWritten]).Should().Be(expected);

        var ok = vm2.SemVer.TryParse(buffer[..bytesWritten], null, out var parsed);
        ok.Should().BeTrue();
        parsed.Should().Be(version);
    }

    [Fact]
    public void SysJson_CrossSerializer_NsJsonSerialized_SysJsonDeserialized()
    {
        var original = new vm2.SemVer(1, 2, 3, "alpha.1", "build.7");

        var json = Newtonsoft.Json.JsonConvert.SerializeObject(original);
        var deserialized = JsonSerializer.Deserialize<vm2.SemVer>(json);

        deserialized.Should().Be(original);
    }

    [Fact]
    public void NsJson_CrossSerializer_SysJsonSerialized_NsJsonDeserialized()
    {
        var original = new vm2.SemVer(1, 2, 3, "alpha.1", "build.7");

        var json = JsonSerializer.Serialize(original);
        var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject<vm2.SemVer>(json);

        deserialized.Should().Be(original);
    }
    #endregion
}
