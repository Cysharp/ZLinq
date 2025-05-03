namespace Xunit;

internal static class ConditionalUtils
{
    public static bool IsEnable(Type type, string key)
    {
        if (type == typeof(TestEnvironment))
        {
            switch (key)
            {
                case "IsStressModeEnabled" when TestEnvironment.IsStressModeEnabled:
                    return true;
                default:
                    return false;
            }
        }

        if (type == typeof(PlatformDetection))
        {
            switch (key)
            {
                case "IsThreadingSupported" when PlatformDetection.IsThreadingSupported:
                case "IsDebuggerTypeProxyAttributeSupported" when PlatformDetection.IsDebuggerTypeProxyAttributeSupported:
                case "IsNotBuiltWithAggressiveTrimming" when PlatformDetection.IsNotBuiltWithAggressiveTrimming:

                case "IsLinqSpeedOptimized" when PlatformDetection.IsLinqSpeedOptimized:
                case "IsSpeedOptimized" when PlatformDetection.IsSpeedOptimized:
                    return true;
                default:
                    return false;
            }
        }

        return false;
    }
}
