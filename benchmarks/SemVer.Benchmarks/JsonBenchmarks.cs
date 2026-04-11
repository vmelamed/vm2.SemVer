// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.SemVerBenchmarks;

#pragma warning disable CA1822  // Mark members as static
#pragma warning disable IL2026  // Trim analyzer: serialization uses reflection

#if SHORT_RUN
[ShortRunJob]
#else
[SimpleJob(RuntimeMoniker.HostProcess)]
#endif
public class JsonBenchmarks
{
    static readonly SemVer Full = new(1, 2, 3, "rc.1", "build.7");

    // Pre-serialized JSON strings for deserialization benchmarks.
    static readonly string FullSysJson = System.Text.Json.JsonSerializer.Serialize(Full);
    static readonly string FullNsJson = JsonConvert.SerializeObject(Full);

    // --- System.Text.Json Serialize ---

    [Benchmark(Description = "STJ Serialize full")]
    public string SysJson_Serialize_Full() => System.Text.Json.JsonSerializer.Serialize(Full);

    // --- System.Text.Json Deserialize ---

    [Benchmark(Description = "STJ Deserialize full")]
    public SemVer SysJson_Deserialize_Full() => System.Text.Json.JsonSerializer.Deserialize<SemVer>(FullSysJson);

    // --- Newtonsoft.Json Serialize ---

    [Benchmark(Description = "NSJ Serialize full")]
    public string NsJson_Serialize_Full() => JsonConvert.SerializeObject(Full);

    // --- Newtonsoft.Json Deserialize ---

    [Benchmark(Description = "NSJ Deserialize full")]
    public SemVer NsJson_Deserialize_Full() => JsonConvert.DeserializeObject<SemVer>(FullNsJson);
}
