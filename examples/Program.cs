#!/usr/bin/env dotnet

// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

#:property TargetFramework=net10.0
#:project ../src/SemVer/SemVer.csproj

using static System.Console;

using vm2;

var version1 = new SemVer("1.2.3");
var version2 = new SemVer(1, 2, 4);

CompareSemVer(version1, version2);

var versionString3 = "1.2.3-alpha.1+2026.04.07";

if (SemVer.IsValid(versionString3))
{
    WriteLine($"Version string '{versionString3}' is a valid SemVer 2.0.0 version.");

    var version3 = new SemVer(versionString3);

    CompareSemVer(version1, version3);
    CompareSemVer(version2, version3);

    if (SemVer.TryParse("1.2.3-alpha.1+2026.04.08", out SemVer version4))
    {
        CompareSemVer(version1, version4);
        CompareSemVer(version2, version4);
        // Note: Build metadata is ignored in SemVer precedence — version3 and version4 compare as equal.
        CompareSemVer(version3, version4);

        if (version3.ToString() != version4.ToString())
            WriteLine($"Although equal as SemVer the version string '{version3}' is not equal to the version string '{version4}'");
        if (version3 != "1.2.3-alpha.1+2026.04.08")
            WriteLine($"Although equal as SemVer the version '{version3}' implicitly converted to string is not equal to the version string '1.2.3-alpha.1+2026.04.08'");

        var v3 = version3.BumpPatch();
        var v4 = version4.BumpPatch();

        if ((string)v3 == (string)v4) // note the explicit cast to string
            WriteLine($"After bumping patch, the version string '{v3}' is equal to the version string '{v4}'");
        // now even the strings are equal, because the Bump* methods remove pre-release and build metadata, unless they are specified explicitly in the methods.
        CompareSemVer(v3, v4);
    }

    WriteLine($"Regular expressions used by SemVer for parsing and validation:");
    SemVer.PrintREs();
}

void CompareSemVer(SemVer left, SemVer right)
{
    var relationship = left.CompareTo(right);

    switch (relationship)
    {
        case 0:
            WriteLine($"Version '{left}' is equal to version '{right}'");
            break;
        case < 0:
            WriteLine($"Version '{left}' is less than version '{right}'");
            break;
        case > 0:
            WriteLine($"Version '{left}' is greater than version '{right}'");
            break;
    }
}
