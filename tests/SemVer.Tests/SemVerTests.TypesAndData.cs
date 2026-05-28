// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Tests.SemVer;

using vm2;

public partial class SemVerTests
{
    private static SemVer MakeLargeSemVer()
    {
        // Build metadata long enough to push Length > 1024
        var longMeta = new string('a', 1100);
        return new SemVer(1, 2, 3, "", longMeta);
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

    public static TheoryData<string?, bool> IsValidCases => new()
    {
        { "1.2.3", true },
        { "1.2.3-alpha.1+build.7", true },
        { "1.2.3-rc.1+meta.09", true },
        { "1.2.a3-rc.1+meta.9", false },
        { "some string", false },
        { "", false },
        { "   ", false },
        { null, false },
    };

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

    private sealed class SysJsonTestModel
    {
        public SemVer Version { get; set; }
        public string Label { get; set; } = "";
    }

    private sealed class SysJsonTestModelNullable
    {
        public SemVer? Version { get; set; }
        public string Label { get; set; } = "";
    }
}
