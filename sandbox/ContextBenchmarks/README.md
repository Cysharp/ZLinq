# Context overload benchmarks

This BenchmarkDotNet project compares captured `System.Linq` and ZLinq callbacks with ZLinq callbacks that receive context explicitly. It also compares HonkPerf.NET.RefLinq 1.0.0 in the scenarios supported by its API.

Scenarios supported by all four implementations are grouped in `FourImplementationContextOperatorBenchmarks`: `Select`, `Where`, `All`, `Any`, `Count`, the seeded and unseeded `Aggregate` forms, `MinBy`, `MaxBy`, `TakeWhile`, and `SelectMany` with a sequence selector. The remaining scenarios are in `ZLinqOnlyContextOperatorBenchmarks`. HonkPerf.NET has no `LongCount`, predicate overloads for `First`/`Last`/`Single`, `Aggregate` result-selector overload, or `IEnumerable`-returning `SelectMany`.

`WhereSelectContextShapeBenchmarks` compares a `Where().Select().Sum()` chain across all four implementations for two context shapes: a reference object and a `ValueTuple`, each containing `long`, `double`, and `string` fields. The `System.Linq` and ordinary ZLinq cases capture the context in their lambdas; the explicit-context ZLinq and HonkPerf.NET cases use static lambdas and pass the context as an argument.

Run a quick listing or filter a benchmark from this directory:

```sh
dotnet run -c Release --framework net10.0 -- --list flat
dotnet run -c Release --framework net10.0 -- --filter '*ZLinqWithContext*'
dotnet run -c Release --framework net10.0 -- --filter '*FourImplementationContextOperatorBenchmarks*'
dotnet run -c Release --framework net10.0 -- --filter '*WhereSelectContextShapeBenchmarks*'
```

Each benchmark category uses `Linq_*` as its baseline, `ZLinq_*` captures a local context, and `ZLinqWithContext_*` passes context as an explicit argument. Categories supported by HonkPerf.NET also include `HonkPerf_*`. The suite reports elapsed time and allocations for `N = 100` and `N = 10,000`.
