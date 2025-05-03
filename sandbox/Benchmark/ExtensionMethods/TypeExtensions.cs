namespace Benchmark;

internal static class TypeExtensions
{
    /// <summary>
    /// Gets C# display name of type.
    /// </summary>
    public static string GetDisplayName(this Type type)
    {
        if (type.IsGenericType)
        {
            if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
                return $"{GetDisplayName(type.GetGenericArguments()[0])}?";

            var genericTypeName = type.GetGenericTypeDefinition().Name;
            var typeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
            var genericArgs = string.Join(", ", type.GetGenericArguments().Select(GetDisplayName));
            return $"{typeName}<{genericArgs}>";
        }

        if (type.IsArray)
            return $"{type.GetElementType()!.GetDisplayName()}[]";

        if (type == typeof(object))
            return "object";

        if (type.IsEnum)
            return type.Name;

        return Type.GetTypeCode(type) switch
        {
#pragma warning disable format
            TypeCode.Boolean => "bool",
            TypeCode.Char    => "char",
            TypeCode.SByte   => "sbyte",
            TypeCode.Byte    => "byte",
            TypeCode.Int16   => "short",
            TypeCode.UInt16  => "ushort",
            TypeCode.Int32   => "int",
            TypeCode.UInt32  => "uint",
            TypeCode.Int64   => "long",
            TypeCode.UInt64  => "ulong",
            TypeCode.Single  => "float",
            TypeCode.Double  => "double",
            TypeCode.Decimal => "decimal",
            TypeCode.String  => "string",
            _                => type.Name
#pragma warning restore format
        };
    }
}
