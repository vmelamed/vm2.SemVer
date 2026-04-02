// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.SemVer.Tests;

public class SemVerApiTests
{
    [Fact]
    public void Echo_returns_value_when_present()
    {
        var result = SemVerApi.Echo("hi", "fallback");
        result.Should().Be("hi");
    }

    [Fact]
    public void Echo_returns_fallback_when_null()
    {
        var result = SemVerApi.Echo(null, "fallback");
        result.Should().Be("fallback");
    }
}
