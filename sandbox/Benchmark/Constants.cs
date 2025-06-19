using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Benchmark;

public static class Constants
{
#if NET10_0_OR_GREATER
    public static readonly IToolchain DefaultToolchain = CsProjCoreToolchain.NetCoreApp10_0;
#else
    public static readonly IToolchain DefaultToolchain = CsProjCoreToolchain.NetCoreApp90;
#endif

    public static class DefineConstants
    {
        public const string USE_SYSTEM_LINQ = "USE_SYSTEM_LINQ";
        public const string USE_ZLINQ_NUGET_PACKAGE = "USE_ZLINQ_NUGET_PACKAGE";

        public const string ZLINQ_1_2_0_OR_GREATER = "ZLINQ_1_2_0_OR_GREATER";
        public const string ZLINQ_1_3_1_OR_GREATER = "ZLINQ_1_3_1_OR_GREATER";
        public const string ZLINQ_1_4_0_OR_GREATER = "ZLINQ_1_4_0_OR_GREATER";
    }

    public static class CustomBuildConfigurations
    {
        public const string SystemLinq = "SystemLinq";
        public const string UseZLinqNuGetPackage = "UseZLinqNuGetPackage";
    }

    public static class MSBuildProperties
    {
        public const string TargetZLinqVersion = "TargetZLinqVersion";

        public const string ZLinqDefineConstants = "ZLinqDefineConstants";
    }
}
