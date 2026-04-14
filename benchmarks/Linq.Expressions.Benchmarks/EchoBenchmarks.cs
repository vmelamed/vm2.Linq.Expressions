// SPDX-License-Identifier: MIT

namespace vm2.Linq.Expressions.Benchmarks;

#if SHORT_RUN
[ShortRunJob]
#else
[SimpleJob(RuntimeMoniker.HostProcess)]
#endif
public class EchoBenchmarks
{
    private string _value = "payload";

    [Benchmark]
    public string Echo_Value() => Linq.ExpressionsApi.Echo(_value, "fallback");

    [Benchmark]
    public string Echo_Fallback() => Linq.ExpressionsApi.Echo(null, "fallback");
}
