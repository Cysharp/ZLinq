using ZLinq;

namespace Benchmark;

internal static class ExtensionMethods
{
    /// <summary>
    /// Gets display name of specified type.
    /// </summary>
    public static string GetDisplayName(this Type typeInfo)
    {
        if (!typeInfo.IsGenericType)
            return typeInfo.Name;

        // Resolve generics name
        string name = typeInfo.Name.Substring(0, typeInfo.Name.IndexOf('`'));
        string args = string.Join(", ", typeInfo.GetGenericArguments().Select(GetDisplayName));
        return $"{name}<{args}>";
    }
}
