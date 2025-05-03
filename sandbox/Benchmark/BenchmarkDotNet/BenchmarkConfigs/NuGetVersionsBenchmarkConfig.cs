using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Jobs;
using System.Text;
using System.Text.Json.Nodes;

namespace Benchmark;

/// <summary>
/// BenchmarkConfig that run benchmarks to compare performance between LocalBuild/NuGet version.
/// </summary>
public class NuGetVersionsBenchmarkConfig : BaseBenchmarkConfig
{
    public NuGetVersionsBenchmarkConfig() : base()
    {
        var baseJobConfig = GetBaseJobConfig()
                                .WithToolchain(Constants.DefaultToolchain)
                                .Freeze();

        string[] targetNuGetVersions = [GetCurrentZLinqVersion()];

        // Note: Enable following code when comparing multiple ZLinq versions.
        // targetNuGetVersions = GetTargetZlinqVersions();

        // 1. Add jobs that use ZLinq NuGet package with specified versions
        foreach (var targetVersion in targetNuGetVersions)
        {
            var job = baseJobConfig
               .WithCustomBuildConfiguration(GetCustomBuildConfigurationName(targetVersion))
               .WithArguments(
                [
                   new MsBuildArgument($"/p:{Constants.MSBuildProperties.TargetZLinqVersion}={targetVersion}"),
                   new MsBuildArgument($"/p:{Constants.MSBuildProperties.ZLinqDefineConstants}={GetDefineConstantsValue(targetVersion)}"),
                   // TODO: Enable following code and remove settings from csproj after .NET SDK issue is resolved. See:https://github.com/dotnet/sdk/issues/45638
                   // new MsBuildArgument($"/p:DefineConstants={GetDefineConstantsValue(targetVersion)}"),
                ])
               .WithId($"v{targetVersion}");

            bool isBaseline = targetVersion == targetNuGetVersions.First();
            if (isBaseline)
                AddJob(job.AsBaseline());
            else
                AddJob(job);
        }

        // 2. Add LocalBuild job.
        if (targetNuGetVersions.Length == 1)
            AddJob(baseJobConfig.WithId("vLocalBuild")); // Add `v` prefix to change display order.

        // Configure additional settings
        AddConfigurations();
    }

    protected override void AddColumnHidingRules()
    {
        HideColumns(Column.Arguments);
        HideColumns(Column.NuGetReferences);
        HideColumns(Column.BuildConfiguration);
    }

    /// <summary>
    /// Gets DefineConstants settings for using target ZLinq version.
    /// </summary>
    private static string GetDefineConstantsValue(string versionText)
    {
        // Currently it need to escape MSBuild special characters (https://learn.microsoft.com/en-us/visualstudio/msbuild/msbuild-special-characters)
        // Because MSBuildArgument is passed as commandline parameter.
        // See: https://github.com/dotnet/BenchmarkDotNet/issues/2719
        const string ListSeparator = "%3B"; // Escapec semicolon char.

        StringBuilder sb = new();

        // Add target package version symbol
        sb.Append($"{ListSeparator}ZLINQ_{versionText.Replace('.', '_')}");

        // v1.2.0 or later supports Immutable/Frozen collection.
        var version = Version.Parse(versionText);
        if (version >= new Version(1, 2, 0))
            sb.Append($"{ListSeparator}{Constants.DefineConstants.ZLINQ_1_2_0_OR_GREATER}");

        // v1.3.1 contains following breaking changes
        // - WhereArray signature is changed to ArrayWhere.
        // - ArraySelect/ListSelect optimization for Select from Array.
        if (version >= new Version(1, 3, 1))
            sb.Append($"{ListSeparator}{Constants.DefineConstants.ZLINQ_1_3_1_OR_GREATER}");

        // v1.4.0 changes the return value of ToArrayPool to PooledArray<T>,
        if (version >= new Version(1, 4, 0))
            sb.Append($"{ListSeparator}{Constants.DefineConstants.ZLINQ_1_4_0_OR_GREATER}");

        return sb.ToString();
    }

    private static string GetCurrentZLinqVersion()
    {
        DirectoryInfo? dirInfo = new DirectoryInfo(AppContext.BaseDirectory);
        while (true)
        {
            if (dirInfo.GetFiles().Any(x => x.Name == "ZLinq.slnx"))
                break;

            dirInfo = dirInfo.Parent;
            if (dirInfo == null)
                throw new FileNotFoundException("ZLinq.slnx is not found.");
        }

        var solutionDir = dirInfo.FullName;
        var resolvedPath = Path.Combine(solutionDir, "src/ZLinq.Unity/Assets/ZLinq.Unity/package.json");
        if (!File.Exists(resolvedPath) && !Directory.Exists(resolvedPath))
            throw new FileNotFoundException($"File is not found. path: {resolvedPath}");

        var json = File.ReadAllText(resolvedPath);
        var latestVersion = JsonNode.Parse(json)!["version"]!.GetValue<string>();
        return latestVersion;
    }

    private static string[] GetTargetZlinqVersions()
    {
        // Currently multi NuGet versions benchmark is not supported.
        // It require following BenchmarkDotNet feature.
        // https://github.com/dotnet/BenchmarkDotNet/pull/2676
        throw new NotSupportedException("Currently multi NuGet versions benchmark is not supported.");

        // Available package versions: https://api.nuget.org/v3-flatcontainer/ZLinq/index.json
        ////return
        ////[
        ////    "1.0.0",
        ////    "1.1.0",
        ////    "1.2.0",
        ////    "1.3.0",
        ////    "1.3.1",
        ////    "1.4.0",
        ////    "1.4.1",
        ////    "1.4.2",
        ////];
    }

    private static string GetCustomBuildConfigurationName(string targetVersion)
        => $"{Constants.CustomBuildConfigurations.UseZLinqNuGetPackage}_{targetVersion.Replace(".", "_")}";
}
