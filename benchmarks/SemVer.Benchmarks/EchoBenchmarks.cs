// SPDX-License-Identifier: MIT
// Copyright (c) 2025-2026 Val Melamed

namespace vm2.SemVer.Benchmarks;

#if SHORT_RUN
[ShortRunJob]
#else
[SimpleJob(RuntimeMoniker.HostProcess)]
#endif
[MemoryDiagnoser]
[JsonExporter]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class EchoBenchmarks
{
    // private string _value = "payload";

    // [Benchmark]
    // public string Echo_Value() => SemVerApi.Echo(_value, "fallback");

    // [Benchmark]
    // public string Echo_Fallback() => SemVerApi.Echo(null, "fallback");
}
