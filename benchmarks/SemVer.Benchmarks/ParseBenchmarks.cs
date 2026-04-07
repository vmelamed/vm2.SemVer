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
public class ParseBenchmarks
{
    const string CoreOnly = "1.2.3";
    const string WithPreRelease = "1.2.3-rc.1";
    const string Full = "1.2.3-rc.1+build.7";
    const string HugeNumbers = "2147483647.2147483647.2147483647-alpha.1+meta.2";

    static readonly byte[] CoreOnlyUtf8 = Encoding.UTF8.GetBytes(CoreOnly);
    static readonly byte[] WithPreReleaseUtf8 = Encoding.UTF8.GetBytes(WithPreRelease);
    static readonly byte[] FullUtf8 = Encoding.UTF8.GetBytes(Full);
    static readonly byte[] HugeNumbersUtf8 = Encoding.UTF8.GetBytes(HugeNumbers);

    // --- String Parse ---

    [Benchmark(Description = "Parse(string) core")]
    public SemVer Parse_String_CoreOnly() => SemVer.Parse(CoreOnly, null);

    [Benchmark(Description = "Parse(string) pre-release")]
    public SemVer Parse_String_WithPreRelease() => SemVer.Parse(WithPreRelease, null);

    [Benchmark(Description = "Parse(string) full")]
    public SemVer Parse_String_Full() => SemVer.Parse(Full, null);

    [Benchmark(Description = "Parse(string) huge")]
    public SemVer Parse_String_HugeNumbers() => SemVer.Parse(HugeNumbers, null);

    // --- Span<char> Parse ---

    [Benchmark(Description = "Parse(span<char>) core")]
    public SemVer Parse_CharSpan_CoreOnly() => SemVer.Parse(CoreOnly.AsSpan(), null);

    [Benchmark(Description = "Parse(span<char>) pre-release")]
    public SemVer Parse_CharSpan_WithPreRelease() => SemVer.Parse(WithPreRelease.AsSpan(), null);

    [Benchmark(Description = "Parse(span<char>) full")]
    public SemVer Parse_CharSpan_Full() => SemVer.Parse(Full.AsSpan(), null);

    [Benchmark(Description = "Parse(span<char>) huge")]
    public SemVer Parse_CharSpan_HugeNumbers() => SemVer.Parse(HugeNumbers.AsSpan(), null);

    // --- Span<byte> UTF-8 Parse ---

    [Benchmark(Description = "Parse(span<byte>) core")]
    public SemVer Parse_Utf8Span_CoreOnly() => SemVer.Parse(CoreOnlyUtf8.AsSpan(), null);

    [Benchmark(Description = "Parse(span<byte>) pre-release")]
    public SemVer Parse_Utf8Span_WithPreRelease() => SemVer.Parse(WithPreReleaseUtf8.AsSpan(), null);

    [Benchmark(Description = "Parse(span<byte>) full")]
    public SemVer Parse_Utf8Span_Full() => SemVer.Parse(FullUtf8.AsSpan(), null);

    [Benchmark(Description = "Parse(span<byte>) huge")]
    public SemVer Parse_Utf8Span_HugeNumbers() => SemVer.Parse(HugeNumbersUtf8.AsSpan(), null);
}
