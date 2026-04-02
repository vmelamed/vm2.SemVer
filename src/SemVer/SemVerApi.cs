// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.SemVer;

/// <summary>
/// A minimal sample API for vm2.SemVer.
/// </summary>
public static class SemVerApi
{
    /// <summary>
    /// Returns the input string or a default fallback when null.
    /// </summary>
    /// <param name="value">Input value.</param>
    /// <param name="fallback">Optional fallback when <paramref name="value"/> is null.</param>
    /// <returns>The original value when provided; otherwise the fallback.</returns>
    public static string Echo(string? value, string fallback = "default")
    {
        return value ?? fallback;
    }
}
