// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.Benchmarks.SemVer;

#pragma warning disable CA1822  // Mark members as static

#if SHORT_RUN
[ShortRunJob]
#else
[SimpleJob(RuntimeMoniker.HostProcess)]
#endif
public class JsonBenchmarks
{
    internal static readonly vm2.SemVer FullSemVer;

    // Pre-serialized JSON strings for deserialization benchmarks.
    internal static readonly string FullSemVerStj;
    internal static readonly string FullSemVerNsj;
    internal static readonly Newtonsoft.Json.JsonSerializerSettings NsjSettings;

    static JsonBenchmarks()
    {
        FullSemVer = new(1, 2, 3, "rc.1", "build.7");

        FullSemVerStj = System.Text.Json.JsonSerializer.Serialize(FullSemVer);

        NsjSettings = new() { Converters = { new SemVerConverter() } };
        FullSemVerNsj = Newtonsoft.Json.JsonConvert.SerializeObject(FullSemVer, NsjSettings);
    }
    // --- System.Text.Json Serialize ---

    [Benchmark(Description = "STJ Serialize full")]
    public string SysJson_Serialize_Full() => System.Text.Json.JsonSerializer.Serialize(FullSemVer);

    // --- System.Text.Json Deserialize ---

    [Benchmark(Description = "STJ Deserialize full")]
    public vm2.SemVer SysJson_Deserialize_Full() => System.Text.Json.JsonSerializer.Deserialize<vm2.SemVer>(FullSemVerStj);

    // --- Newtonsoft.Json Serialize ---

    [Benchmark(Description = "NSJ Serialize full", OperationsPerInvoke = 1000)]
    public string NsJson_Serialize_Full() => Newtonsoft.Json.JsonConvert.SerializeObject(FullSemVer, NsjSettings);

    // --- Newtonsoft.Json Deserialize ---

    [Benchmark(Description = "NSJ Deserialize full", OperationsPerInvoke = 1000)]
    public vm2.SemVer NsJson_Deserialize_Full() => Newtonsoft.Json.JsonConvert.DeserializeObject<vm2.SemVer>(FullSemVerNsj, NsjSettings)!;
}
