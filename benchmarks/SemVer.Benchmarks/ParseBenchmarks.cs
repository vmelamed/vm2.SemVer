// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Benchmarks.SemVer;

#pragma warning disable CA1822 // Mark members as static

#if SHORT_RUN
[ShortRunJob]
#else
[SimpleJob(RuntimeMoniker.HostProcess)]
#endif
public class ParseBenchmarks
{
    const int operationsPerInvoke = 1000;

    const string WithPreRelease = "1.2.3-rc.1";
    const string Full = "1.2.3-rc.1+build.7";
    const string HugeNumbers = "2147483647.2147483647.2147483647-alpha.1+meta.2";

    static readonly byte[] WithPreReleaseUtf8 = Encoding.UTF8.GetBytes(WithPreRelease);
    static readonly byte[] FullUtf8 = Encoding.UTF8.GetBytes(Full);
    static readonly byte[] HugeNumbersUtf8 = Encoding.UTF8.GetBytes(HugeNumbers);

    // --- String TryParse ---

    [Benchmark(Description = "TryParse(string) pre-release", OperationsPerInvoke = operationsPerInvoke)]
    public bool TryParse_String_WithPreRelease()
    {
        var suppressOptimizationDiscard = false;
        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard |= vm2.SemVer.TryParse(WithPreRelease, null, out _);
        return suppressOptimizationDiscard;
    }

    [Benchmark(Description = "TryParse(string) full", OperationsPerInvoke = operationsPerInvoke)]
    public bool TryParse_String_Full()
    {
        var suppressOptimizationDiscard = false;
        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard |= vm2.SemVer.TryParse(Full, null, out _);
        return suppressOptimizationDiscard;
    }


    [Benchmark(Description = "TryParse(string) huge", OperationsPerInvoke = operationsPerInvoke)]
    public bool TryParse_String_HugeNumbers()
    {
        var suppressOptimizationDiscard = false;
        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard |= vm2.SemVer.TryParse(HugeNumbers, null, out _);
        return suppressOptimizationDiscard;
    }

    // --- Span<char> TryParse ---

    [Benchmark(Description = "TryParse(span<char>) pre-release", OperationsPerInvoke = operationsPerInvoke)]
    public bool TryParse_CharSpan_WithPreRelease()
    {
        var suppressOptimizationDiscard = false;
        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard |= vm2.SemVer.TryParse(WithPreRelease.AsSpan(), null, out _);
        return suppressOptimizationDiscard;
    }


    [Benchmark(Description = "TryParse(span<char>) full", OperationsPerInvoke = operationsPerInvoke)]
    public bool TryParse_CharSpan_Full()
    {
        var suppressOptimizationDiscard = false;
        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard |= vm2.SemVer.TryParse(Full.AsSpan(), null, out _);
        return suppressOptimizationDiscard;
    }


    [Benchmark(Description = "TryParse(span<char>) huge", OperationsPerInvoke = operationsPerInvoke)]
    public bool TryParse_CharSpan_HugeNumbers()
    {
        var suppressOptimizationDiscard = false;
        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard |= vm2.SemVer.TryParse(HugeNumbers.AsSpan(), null, out _);
        return suppressOptimizationDiscard;
    }


    // --- Span<byte> UTF-8 TryParse ---

    [Benchmark(Description = "TryParse(span<byte>) pre-release", OperationsPerInvoke = operationsPerInvoke)]
    public bool TryParse_Utf8Span_WithPreRelease()
    {
        var suppressOptimizationDiscard = false;
        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard |= vm2.SemVer.TryParse(WithPreReleaseUtf8.AsSpan(), null, out _);
        return suppressOptimizationDiscard;
    }


    [Benchmark(Description = "TryParse(span<byte>) full", OperationsPerInvoke = operationsPerInvoke)]
    public bool TryParse_Utf8Span_Full()
    {
        var suppressOptimizationDiscard = false;
        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard |= vm2.SemVer.TryParse(FullUtf8.AsSpan(), null, out _);
        return suppressOptimizationDiscard;
    }


    [Benchmark(Description = "TryParse(span<byte>) huge", OperationsPerInvoke = operationsPerInvoke)]
    public bool TryParse_Utf8Span_HugeNumbers()
    {
        var suppressOptimizationDiscard = false;
        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard |= vm2.SemVer.TryParse(HugeNumbersUtf8.AsSpan(), null, out _);
        return suppressOptimizationDiscard;
    }

}
