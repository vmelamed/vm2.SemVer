// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.SemVerBenchmarks;

#pragma warning disable CA1822 // Mark members as static

#if SHORT_RUN
[ShortRunJob]
#else
[SimpleJob(RuntimeMoniker.HostProcess)]
#endif
[JsonExporter]
[MarkdownExporter]
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class FormatBenchmarks
{
    static readonly SemVer CoreOnly = new(1, 2, 3);
    static readonly SemVer WithPreRelease = new(1, 2, 3, "rc.1");
    static readonly SemVer Full = new(1, 2, 3, "rc.1", "build.7");
    static readonly SemVer HugeNumbers = new(int.MaxValue, int.MaxValue, int.MaxValue, "alpha.1", "meta.2");

    // --- ToString (allocating) ---

    [Benchmark(Description = "ToString() core")]
    public string ToString_CoreOnly() => CoreOnly.ToString();

    [Benchmark(Description = "ToString() pre-release")]
    public string ToString_WithPreRelease() => WithPreRelease.ToString();

    [Benchmark(Description = "ToString() full")]
    public string ToString_Full() => Full.ToString();

    [Benchmark(Description = "ToString() huge")]
    public string ToString_HugeNumbers() => HugeNumbers.ToString();

    // --- TryFormat(Span<char>) ---

    [Benchmark(Description = "TryFormat(char) core")]
    public bool TryFormat_Char_CoreOnly()
    {
        Span<char> buffer = stackalloc char[CoreOnly.Length];
        return CoreOnly.TryFormat(buffer, out _);
    }

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

    [Benchmark(Description = "TryFormat(byte) core")]
    public bool TryFormat_Utf8_CoreOnly()
    {
        Span<byte> buffer = stackalloc byte[CoreOnly.Length];
        return CoreOnly.TryFormat(buffer, out _);
    }

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
