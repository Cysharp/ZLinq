using BenchmarkDotNet.Toolchains;
using BenchmarkDotNet.Toolchains.CsProj;

namespace Benchmark;

public static class Constants
{
    public static readonly IToolchain DefaultToolchain = CsProjCoreToolchain.NetCoreApp90;

    public static class DefineConstants
    {
        public const string USE_SYSTEM_LINQ = "USE_SYSTEM_LINQ";

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
