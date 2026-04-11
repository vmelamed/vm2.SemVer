// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.SemVerBenchmarks;

#pragma warning disable CA1822 // Mark members as static

#if SHORT_RUN
[ShortRunJob]
#else
[SimpleJob(RuntimeMoniker.HostProcess)]
#endif
public class ParseBenchmarks
{
    const string WithPreRelease = "1.2.3-rc.1";
    const string Full = "1.2.3-rc.1+build.7";
    const string HugeNumbers = "2147483647.2147483647.2147483647-alpha.1+meta.2";

    static readonly byte[] WithPreReleaseUtf8 = Encoding.UTF8.GetBytes(WithPreRelease);
    static readonly byte[] FullUtf8 = Encoding.UTF8.GetBytes(Full);
    static readonly byte[] HugeNumbersUtf8 = Encoding.UTF8.GetBytes(HugeNumbers);

    // --- String TryParse ---

    [Benchmark(Description = "TryParse(string) pre-release")]
    public bool TryParse_String_WithPreRelease() => SemVer.TryParse(WithPreRelease, null, out _);

    [Benchmark(Description = "TryParse(string) full")]
    public bool TryParse_String_Full() => SemVer.TryParse(Full, null, out _);

    [Benchmark(Description = "TryParse(string) huge")]
    public bool TryParse_String_HugeNumbers() => SemVer.TryParse(HugeNumbers, null, out _);

    // --- Span<char> TryParse ---

    [Benchmark(Description = "TryParse(span<char>) pre-release")]
    public bool TryParse_CharSpan_WithPreRelease() => SemVer.TryParse(WithPreRelease.AsSpan(), null, out _);

    [Benchmark(Description = "TryParse(span<char>) full")]
    public bool TryParse_CharSpan_Full() => SemVer.TryParse(Full.AsSpan(), null, out _);

    [Benchmark(Description = "TryParse(span<char>) huge")]
    public bool TryParse_CharSpan_HugeNumbers() => SemVer.TryParse(HugeNumbers.AsSpan(), null, out _);

    // --- Span<byte> UTF-8 TryParse ---

    [Benchmark(Description = "TryParse(span<byte>) pre-release")]
    public bool TryParse_Utf8Span_WithPreRelease() => SemVer.TryParse(WithPreReleaseUtf8.AsSpan(), null, out _);

    [Benchmark(Description = "TryParse(span<byte>) full")]
    public bool TryParse_Utf8Span_Full() => SemVer.TryParse(FullUtf8.AsSpan(), null, out _);

    [Benchmark(Description = "TryParse(span<byte>) huge")]
    public bool TryParse_Utf8Span_HugeNumbers() => SemVer.TryParse(HugeNumbersUtf8.AsSpan(), null, out _);
}
