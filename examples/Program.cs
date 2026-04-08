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

var version3 = new SemVer("1.2.3-alpha.1+2026.04.07");
CompareSemVer(version1, version3);
CompareSemVer(version2, version3);

if (SemVer.TryParse("1.2.3-alpha.1+2026.04.08", out SemVer version4))
{
    CompareSemVer(version1, version4);
    CompareSemVer(version2, version4);
    CompareSemVer(version3, version4);
}

int CompareSemVer(SemVer version1, SemVer version2)
{
    var relationship = version1.CompareTo(version2);

    switch(relationship)
    {
        case 0:
            WriteLine($"Version '{version1}' is equal to version '{version2}'");
            break;
        case < 0:
            WriteLine($"Version '{version1}' is less than version '{version2}'");
            break;
        case > 0:
            WriteLine($"Version '{version1}' is greater than version '{version2}'");
            break;
    }

    return relationship;
}
