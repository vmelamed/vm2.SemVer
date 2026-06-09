// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Benchmarks.SemVer;

#pragma warning disable CA1822 // Mark members as static

#if SHORT_RUN
[ShortRunJob]
#else
[SimpleJob(RuntimeMoniker.HostProcess)]
#endif
public class FormatBenchmarks
{
    const int operationsPerInvoke = 1000;

    static readonly vm2.SemVer WithPreRelease = new(1, 2, 3, "rc.1");
    static readonly vm2.SemVer Full = new(1, 2, 3, "rc.1", "build.7");
    static readonly vm2.SemVer HugeNumbers = new(int.MaxValue, int.MaxValue, int.MaxValue, "alpha.1", "meta.2");

    // --- TryFormat(Span<char>) ---

    [Benchmark(Description = "TryFormat(char) pre-release")]
    public bool TryFormat_Char_WithPreRelease()
    {
        var f = false;
        Span<char> buffer = stackalloc char[WithPreRelease.Length];

        for (int i = 0; i < operationsPerInvoke; i++)
             f |= WithPreRelease.TryFormat(buffer, out _);

        return f;
    }

    [Benchmark(Description = "TryFormat(char) full")]
    public bool TryFormat_Char_Full()
    {
        var f = false;
        Span<char> buffer = stackalloc char[Full.Length];

        for (int i = 0; i < operationsPerInvoke; i++)
             f |= Full.TryFormat(buffer, out _);

        return f;
    }

    [Benchmark(Description = "TryFormat(char) huge")]
    public bool TryFormat_Char_HugeNumbers()
    {
        var f = false;
        Span<char> buffer = stackalloc char[HugeNumbers.Length];

        for (int i = 0; i < operationsPerInvoke; i++)
             f |= HugeNumbers.TryFormat(buffer, out _);

        return f;
    }

    // --- TryFormat(Span<byte>) UTF-8 ---

    [Benchmark(Description = "TryFormat(byte) pre-release")]
    public bool TryFormat_Utf8_WithPreRelease()
    {
        var f = false;
        Span<byte> buffer = stackalloc byte[WithPreRelease.Length];

        for (int i = 0; i < operationsPerInvoke; i++)
             f |= WithPreRelease.TryFormat(buffer, out _);

        return f;
    }

    [Benchmark(Description = "TryFormat(byte) full")]
    public bool TryFormat_Utf8_Full()
    {
        var f = false;
        Span<byte> buffer = stackalloc byte[Full.Length];

        for (int i = 0; i < operationsPerInvoke; i++)
             f |= Full.TryFormat(buffer, out _);

        return f;
    }

    [Benchmark(Description = "TryFormat(byte) huge")]
    public bool TryFormat_Utf8_HugeNumbers()
    {
        var f = false;
        Span<byte> buffer = stackalloc byte[HugeNumbers.Length];

        for (int i = 0; i < operationsPerInvoke; i++)
             f |= HugeNumbers.TryFormat(buffer, out _);

        return f;
    }
}
