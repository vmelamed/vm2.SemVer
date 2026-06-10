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
    const int operationsPerInvoke = 1000;

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

    [Benchmark(Description = "STJ Serialize full", OperationsPerInvoke = operationsPerInvoke)]
    public string SysJson_Serialize_Full()
    {
        string suppressOptimizationDiscard = string.Empty;

        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard = System.Text.Json.JsonSerializer.Serialize(FullSemVer);

        return suppressOptimizationDiscard;
    }

    // --- System.Text.Json Deserialize ---

    [Benchmark(Description = "STJ Deserialize full", OperationsPerInvoke = operationsPerInvoke)]
    public vm2.SemVer SysJson_Deserialize_Full()
    {
        vm2.SemVer suppressOptimizationDiscard = default;

        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard = System.Text.Json.JsonSerializer.Deserialize<vm2.SemVer>(FullSemVerStj);

        return suppressOptimizationDiscard;
    }

    // --- Newtonsoft.Json Serialize ---

    [Benchmark(Description = "NSJ Serialize full", OperationsPerInvoke = operationsPerInvoke)]
    public string NsJson_Serialize_Full()
    {
        string suppressOptimizationDiscard = string.Empty;

        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard = Newtonsoft.Json.JsonConvert.SerializeObject(FullSemVer, NsjSettings);

        return suppressOptimizationDiscard;
    }

    // --- Newtonsoft.Json Deserialize ---

    [Benchmark(Description = "NSJ Deserialize full", OperationsPerInvoke = operationsPerInvoke)]
    public vm2.SemVer NsJson_Deserialize_Full()
    {
        vm2.SemVer suppressOptimizationDiscard = default;

        for (int i = 0; i < operationsPerInvoke; i++)
            suppressOptimizationDiscard = Newtonsoft.Json.JsonConvert.DeserializeObject<vm2.SemVer>(FullSemVerNsj, NsjSettings)!;

        return suppressOptimizationDiscard;
    }
}
