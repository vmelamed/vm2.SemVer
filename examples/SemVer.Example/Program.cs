#!/usr/bin/env dotnet

// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

#:property TargetFramework=net10.0
#:project ../../src/SemVer/SemVer.csproj

using static System.Console;
using static System.Text.Encoding;

using vm2.SemVer;

using static vm2.SemVer.SemVerApi;

Console.WriteLine("SemVer example");

Console.WriteLine(Echo("hello", "fallback"));
Console.WriteLine(Echo(null, "fallback"));
