## How to run benchmarks

### 1. Visual Studio

1. Open ZLinq solution with Visual Studio.
2. Select `Release` configuration.
3. Edit `Properties/launchSettings.json`.
4. Select launch profile from dropdown.
5. Start benchmark with `Start Without Debugging`(Ctrl+F5`)

### 2. Commandline

1. Open shell at `sandbox/Benchmark`.
2. Build project with following command.
    `dotnet build -c Release`
3. Start benchmark with following command.
    `dotnet run -c Release --framework net9.0 --no-build --no-launch-profile -- --filter "*"`

### 3. GitHub Actions

It can start benchmarks with following command.

```pwsh
$branchName = 'main'

# Run benchmark with `Default` config
gh workflow run benchmark.yaml --repo Cysharp/ZLinq --ref $branchName

# Run benchmark with `Default` config with benchmark filter
gh workflow run benchmark.yaml --repo Cysharp/ZLinq --ref $branchName -f filter=Benchmark.ReadMeBenchmark*

# Run benchmark with `TargetFrameworks` config
gh workflow run benchmark.yaml --repo Cysharp/ZLinq --ref $branchName -f config=TargetFrameworks

# Run benchmark with `SystemLinq` config
gh workflow run benchmark.yaml --repo Cysharp/ZLinq --ref $branchName -f config=SystemLinq
```

> [!NOTE]
> When running .NET 10 benchmarks. It need to add `-f framework=net10.0` input parameter

Benchmark results are written to `GitHub Actions Job Summaries`.
And archived as ZIP artifacts.

## List of BenchmarkConfig

Following custom benchmark configs are provided.
When using custom benchmark config. Pass config key as extra arguments (e.g. `-- InProcess`)

### `Default`

Default benchmark config.
It use `CsProjCoreToolchain.NetCoreApp90` tool chain and run benchmarks on separated process.

### `InProcess`

Benchmark config that use `InProcessEmitToolchain.Instance`.
It's run faster than default config. Because benchmark is executed on same process.

### `Test` (InProcessMonitoring)

Benchmark config that use `InProcessEmitToolchain.Instance` and `RunStrategy.Monitoring`.
It's run faster than other configs. Because it skip overhead evaluating steps.
Therefore, the benchmark results are not accurate when compared to other configurations.

To obtain more accurate results.
It need to adjust `--iterationCount` argument.
If benchmark execution time is less than `100ms`. Warning messages are displayed.

### `ColdStart`
Benchmark config that use `RunStrategy.ColdStart`.
This config is used to measure startup performance.

By default. This config launch process 3 times.
It can be customized with `--launchCount` argument.

### `TargetFrameworks`
Benchmark config to complare multiple target frameworks (.NET8/.NET9)

### `NuGetVersions`
Benchmark config to complare multiple version on NuGet packages and local build.

### `SystemLinq`
Benchmark config to complare ZLinq/SystemLinq performance.
When using this config. Target benchmarks must be defined on `Benchmark.ZLinq.*` namespace.
Benchmarks that defined on other namespaces are filtered by `ZLinqBenchmarkFilter`.
