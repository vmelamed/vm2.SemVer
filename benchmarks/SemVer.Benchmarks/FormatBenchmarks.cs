// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.SemVerBenchmarks;

#pragma warning disable CA1822 // Mark members as static

#if SHORT_RUN
[ShortRunJob]
#else
[SimpleJob(RuntimeMoniker.HostProcess)]
#endif
public class FormatBenchmarks
{
    static readonly SemVer WithPreRelease = new(1, 2, 3, "rc.1");
    static readonly SemVer Full = new(1, 2, 3, "rc.1", "build.7");
    static readonly SemVer HugeNumbers = new(int.MaxValue, int.MaxValue, int.MaxValue, "alpha.1", "meta.2");

    // --- TryFormat(Span<char>) ---

    [Benchmark(Description = "TryFormat(char) pre-release")]
    public bool TryFormat_Char_WithPreRelease()
    {
        Span<char> buffer = stackalloc char[WithPreRelease.Length];
        return WithPreRelease.TryFormat(buffer, out _);
    }

    [Benchmark(Description = "TryFormat(char) full")]
    public bool TryFormat_Char_Full()
    {
        Span<char> buffer = stackalloc char[Full.Length];
        return Full.TryFormat(buffer, out _);
    }

    [Benchmark(Description = "TryFormat(char) huge")]
    public bool TryFormat_Char_HugeNumbers()
    {
        Span<char> buffer = stackalloc char[HugeNumbers.Length];
        return HugeNumbers.TryFormat(buffer, out _);
    }

    // --- TryFormat(Span<byte>) UTF-8 ---

    [Benchmark(Description = "TryFormat(byte) pre-release")]
    public bool TryFormat_Utf8_WithPreRelease()
    {
        Span<byte> buffer = stackalloc byte[WithPreRelease.Length];
        return WithPreRelease.TryFormat(buffer, out _);
    }

    [Benchmark(Description = "TryFormat(byte) full")]
    public bool TryFormat_Utf8_Full()
    {
        Span<byte> buffer = stackalloc byte[Full.Length];
        return Full.TryFormat(buffer, out _);
    }

    [Benchmark(Description = "TryFormat(byte) huge")]
    public bool TryFormat_Utf8_HugeNumbers()
    {
        Span<byte> buffer = stackalloc byte[HugeNumbers.Length];
        return HugeNumbers.TryFormat(buffer, out _);
    }
}
