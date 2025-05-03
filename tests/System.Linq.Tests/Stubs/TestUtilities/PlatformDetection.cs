// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Original code: https://github.com/dotnet/runtime/blob/v9.0.3/src/libraries/Common/tests/TestUtilities/System/PlatformDetection.cs

namespace System;

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public static partial class PlatformDetection
{
    public static bool IsRiscV64Process => (int)RuntimeInformation.ProcessArchitecture == 9; // Architecture.RiscV64;
    public static bool IsNativeAot => IsNotMonoRuntime && !IsReflectionEmitSupported;
    public static bool IsDebuggerTypeProxyAttributeSupported => !IsNativeAot;

    public static bool IsMonoRuntime => Type.GetType("Mono.RuntimeStructs") != null;
    public static bool IsNotMonoRuntime => !IsMonoRuntime;

#if NET
    public static bool IsReflectionEmitSupported => RuntimeFeature.IsDynamicCodeSupported;
    public static bool IsNotReflectionEmitSupported => !IsReflectionEmitSupported;
#else
        public static bool IsReflectionEmitSupported => true;
#endif

#if NET10_0_OR_GREATER
    public static bool IsSpeedOptimized => IsLinqSpeedOptimized;

    public static bool IsLinqSpeedOptimized => !IsLinqSizeOptimized;
    public static bool IsLinqSizeOptimized => s_linqIsSizeOptimized.Value;
    private static readonly Lazy<bool> s_linqIsSizeOptimized = new Lazy<bool>(ComputeIsLinqSizeOptimized);
    private static bool ComputeIsLinqSizeOptimized()
    {
#if NET
        var methodInfo = typeof(Enumerable)!.GetMethod("get_IsSizeOptimized", BindingFlags.NonPublic | BindingFlags.Static);
        if (methodInfo == null)
            return false;
        return (bool)methodInfo.Invoke(null, Array.Empty<object>())!;
#else
            return false;
#endif
    }
#else
    public static bool IsLinqSpeedOptimized => IsSpeedOptimized;

    public static bool IsSpeedOptimized => !IsSizeOptimized;
    public static bool IsSizeOptimized => IsBrowser || IsWasi || IsAndroid || IsAppleMobile;
#endif

    public static bool IsBrowser => RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER"));
    public static bool IsNotBrowser => !IsBrowser;
    public static bool IsWasi => RuntimeInformation.IsOSPlatform(OSPlatform.Create("WASI"));
    public static bool IsWasmThreadingSupported => IsBrowser && IsEnvironmentVariableTrue("IsBrowserThreadingSupported");
    public static bool IsThreadingSupported => (!IsWasi && !IsBrowser) || IsWasmThreadingSupported;

    public static bool IsBuiltWithAggressiveTrimming => IsNativeAot || IsAppleMobile;
    public static bool IsNotBuiltWithAggressiveTrimming => !IsBuiltWithAggressiveTrimming;

    public static bool IsAppleMobile => IsMacCatalyst || IsiOS || IstvOS;
    public static bool IsiOS => RuntimeInformation.IsOSPlatform(OSPlatform.Create("IOS"));
    public static bool IstvOS => RuntimeInformation.IsOSPlatform(OSPlatform.Create("TVOS"));
    public static bool IsMacCatalyst => RuntimeInformation.IsOSPlatform(OSPlatform.Create("MACCATALYST"));

    public static bool IsAndroid => RuntimeInformation.IsOSPlatform(OSPlatform.Create("ANDROID"));

    private static bool IsEnvironmentVariableTrue(string variableName)
    {
        if (!IsBrowser)
            return false;

        return Environment.GetEnvironmentVariable(variableName) is "true";
    }
}
