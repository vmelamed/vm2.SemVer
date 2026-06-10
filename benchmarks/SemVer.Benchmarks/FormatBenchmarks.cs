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

    [Benchmark(Description = "TryFormat(char) pre-release", OperationsPerInvoke=operationsPerInvoke)]
    public bool TryFormat_Char_WithPreRelease()
    {
        var suppressOptimizationDiscard = false;
        Span<char> buffer = stackalloc char[WithPreRelease.Length];

        for (int i = 0; i < operationsPerInvoke; i++)
             suppressOptimizationDiscard |= WithPreRelease.TryFormat(buffer, out _);

        return suppressOptimizationDiscard;
    }

    [Benchmark(Description = "TryFormat(char) full", OperationsPerInvoke=operationsPerInvoke)]
    public bool TryFormat_Char_Full()
    {
        var suppressOptimizationDiscard = false;
        Span<char> buffer = stackalloc char[Full.Length];

        for (int i = 0; i < operationsPerInvoke; i++)
             suppressOptimizationDiscard |= Full.TryFormat(buffer, out _);

        return suppressOptimizationDiscard;
    }

    [Benchmark(Description = "TryFormat(char) huge", OperationsPerInvoke=operationsPerInvoke)]
    public bool TryFormat_Char_HugeNumbers()
    {
        var suppressOptimizationDiscard = false;
        Span<char> buffer = stackalloc char[HugeNumbers.Length];

        for (int i = 0; i < operationsPerInvoke; i++)
             suppressOptimizationDiscard |= HugeNumbers.TryFormat(buffer, out _);

        return suppressOptimizationDiscard;
    }

    // --- TryFormat(Span<byte>) UTF-8 ---

    [Benchmark(Description = "TryFormat(byte) pre-release", OperationsPerInvoke=operationsPerInvoke)]
    public bool TryFormat_Utf8_WithPreRelease()
    {
        var suppressOptimizationDiscard = false;
        Span<byte> buffer = stackalloc byte[WithPreRelease.Length];

        for (int i = 0; i < operationsPerInvoke; i++)
             suppressOptimizationDiscard |= WithPreRelease.TryFormat(buffer, out _);

        return suppressOptimizationDiscard;
    }

    [Benchmark(Description = "TryFormat(byte) full", OperationsPerInvoke=operationsPerInvoke)]
    public bool TryFormat_Utf8_Full()
    {
        var suppressOptimizationDiscard = false;
        Span<byte> buffer = stackalloc byte[Full.Length];

        for (int i = 0; i < operationsPerInvoke; i++)
             suppressOptimizationDiscard |= Full.TryFormat(buffer, out _);

        return suppressOptimizationDiscard;
    }

    [Benchmark(Description = "TryFormat(byte) huge", OperationsPerInvoke=operationsPerInvoke)]
    public bool TryFormat_Utf8_HugeNumbers()
    {
        var suppressOptimizationDiscard = false;
        Span<byte> buffer = stackalloc byte[HugeNumbers.Length];

        for (int i = 0; i < operationsPerInvoke; i++)
             suppressOptimizationDiscard |= HugeNumbers.TryFormat(buffer, out _);

        return suppressOptimizationDiscard;
    }
}
