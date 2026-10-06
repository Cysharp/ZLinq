using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;
using System.Text.RegularExpressions;

namespace ZLinq;

[Generator(LanguageNames.CSharp)]
public class DropInGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // AssemblyAttribtue

        var generatorOptions = context.CompilationProvider.Select((compilation, token) =>
        {
            foreach (var attr in compilation.Assembly.GetAttributes())
            {
                if (attr.AttributeClass?.Name is "ZLinqDropIn" or "ZLinqDropInAttribute")
                {
                    // (string generateNamespace, DropInGenerateTypes dropInGenerateTypes)
                    var ctor = attr.ConstructorArguments;

                    var generateNamespace = (string)ctor[0].Value!;
                    var dropInGenerateTypes = (DropInGenerateTypes)ctor[1].Value!;

                    var args = attr.NamedArguments;

                    var generateAsPublic = args.FirstOrDefault(x => x.Key == "GenerateAsPublic").Value.Value as bool? ?? false;
                    var conditionalCompilationSymbols = args.FirstOrDefault(x => x.Key == "ConditionalCompilationSymbols").Value.Value as string;
                    var disableEmitSource = args.FirstOrDefault(x => x.Key == "DisableEmitSource").Value.Value as bool? ?? false;

                    return new ZLinqDropInAttribute(generateNamespace, dropInGenerateTypes)
                    {
                        GenerateAsPublic = generateAsPublic,
                        ConditionalCompilationSymbols = conditionalCompilationSymbols,
                        DisableEmitSource = disableEmitSource
                    };
                }
            }

            return null;
        });

        context.RegisterSourceOutput(generatorOptions, EmitSourceOutput);

        // Extension per type

        var extension = context.SyntaxProvider.ForAttributeWithMetadataName("ZLinq.ZLinqDropInExtensionAttribute",
            (_, _) => true, // Class or Struct
            (x, _) =>
            {
                string? elementName = null;
                string? valueEnumeratorName = null;
                bool isElementGenericType = false;
                bool isValueType = false;
                var namedTypeSymbol = x.TargetSymbol as INamedTypeSymbol;
                if (namedTypeSymbol != null)
                {
                    isElementGenericType = namedTypeSymbol.IsGenericType;

                    foreach (var item in namedTypeSymbol.AllInterfaces)
                    {
                        if (item.MetadataName is "IValueEnumerable`2")
                        {
                            // 0 = TEnumerator, 1 = TSource
                            var typeArg = item.TypeArguments[1];
                            elementName = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            isValueType = typeArg.IsValueType;
                            valueEnumeratorName = item.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            break;
                        }
                        else if (item.MetadataName is "IEnumerable`1")
                        {
                            var typeArg = item.TypeArguments[0];
                            elementName = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                            isValueType = typeArg.IsValueType;
                        }
                    }
                }

                if (namedTypeSymbol == null || elementName == null)
                {
                    var location = x.Attributes[0].ApplicationSyntaxReference?.GetSyntax()?.GetLocation();
                    var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ExtensionNotSupported, location);
                    return new DropInExtension(diagnostic);
                }

                if (isElementGenericType)
                {
                    if (namedTypeSymbol.TypeArguments.Length >= 2)
                    {
                        var location = x.Attributes[0].ApplicationSyntaxReference?.GetSyntax()?.GetLocation();
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.GenericTypeArgumentTooMuch, location);
                        return new DropInExtension(diagnostic);
                    }
                }

                var fullyQualified = x.TargetSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                var fullNamespae = x.TargetSymbol.ContainingNamespace.IsGlobalNamespace ? "" : x.TargetSymbol.ContainingNamespace.ToDisplayString();
                string constraint = namedTypeSymbol.FormatGenericConstraints();
                return new DropInExtension(fullNamespae, x.TargetSymbol.Name, fullyQualified, valueEnumeratorName, elementName, isElementGenericType, isValueType, constraint, x.TargetSymbol.DeclaredAccessibility);
            });

        context.RegisterSourceOutput(extension, EmitDropInExtension);

        // Extension per type from assembly

        var externalExtensions = context.CompilationProvider.SelectMany((compilation, token) =>
        {
            var list = new List<DropInExtension>();
            foreach (var attr in compilation.Assembly.GetAttributes())
            {
                if (attr.AttributeClass?.Name is "ZLinqDropInExternalExtension" or "ZLinqDropInExternalExtensionAttribute")
                {
                    var ctor = attr.ConstructorArguments;
                    if (ctor.Length == 0) continue;

                    // parse from attr
                    var generateNamespace = (string)ctor[0].Value!;
                    var sourceTypeFullyQualifiedMetadataName = (string)ctor[1].Value!;
                    var enumeratorTypeFullyQualifiedMetadataName = (string?)ctor[2].Value!;

                    var args = attr.NamedArguments;
                    var generateAsPublic = args.FirstOrDefault(x => x.Key == "GenerateAsPublic").Value.Value as bool? ?? false;

                    // construct from compilation info

                    var namedTypeSymbol = compilation.GetTypeByMetadataName(sourceTypeFullyQualifiedMetadataName);

                    if (namedTypeSymbol == null)
                    {
                        var diagnostic = Diagnostic.Create(DiagnosticDescriptors.SourceTypeNotFound, attr.GetConstructorParameterLocation(1), sourceTypeFullyQualifiedMetadataName);
                        list.Add(new DropInExtension(diagnostic));
                        continue;
                    }

                    string? elementName = null;
                    string? valueEnumeratorName = null;
                    bool isElementGenericType = namedTypeSymbol.IsGenericType;
                    bool isValueType = false;

                    if (enumeratorTypeFullyQualifiedMetadataName == null)
                    {
                        // try to get IEnumerable<T>
                        foreach (var item in namedTypeSymbol.AllInterfaces)
                        {
                            if (item.MetadataName is "IEnumerable`1")
                            {
                                var typeArg = item.TypeArguments[0];
                                elementName = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                                isValueType = typeArg.IsValueType;
                            }
                        }

                        if (elementName == null)
                        {
                            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ElementNotFoundInSourceType, attr.GetConstructorParameterLocation(1), sourceTypeFullyQualifiedMetadataName);
                            list.Add(new DropInExtension(diagnostic));
                            continue;
                        }
                    }
                    else
                    {
                        // try to get value-enumerator
                        var enumeratorTypeSymbol = compilation.GetTypeByMetadataName(enumeratorTypeFullyQualifiedMetadataName);
                        if (enumeratorTypeSymbol == null)
                        {
                            var location = attr.ApplicationSyntaxReference?.GetSyntax()?.GetLocation();
                            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.EnumeratorTypeNotFound, attr.GetConstructorParameterLocation(2), enumeratorTypeFullyQualifiedMetadataName);
                            list.Add(new DropInExtension(diagnostic));
                            continue;
                        }

                        foreach (var item in enumeratorTypeSymbol.AllInterfaces)
                        {
                            if (item.MetadataName is "IValueEnumerator`1")
                            {
                                // 0 = TEnumerator, 1 = TSource
                                var typeArg = item.TypeArguments[0];
                                elementName = typeArg.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                                isValueType = typeArg.IsValueType;
                                valueEnumeratorName = enumeratorTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                                break;
                            }
                        }

                        if (elementName == null)
                        {
                            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.ElementNotFoundInEnumeratorType, attr.GetConstructorParameterLocation(2), enumeratorTypeFullyQualifiedMetadataName);
                            list.Add(new DropInExtension(diagnostic));
                            continue;
                        }
                    }

                    if (isElementGenericType)
                    {
                        if (namedTypeSymbol.TypeArguments.Length >= 2)
                        {
                            var location = attr.ApplicationSyntaxReference?.GetSyntax()?.GetLocation();
                            var diagnostic = Diagnostic.Create(DiagnosticDescriptors.GenericTypeArgumentTooMuch, location);
                            list.Add(new DropInExtension(diagnostic));
                            continue;
                        }
                    }

                    var fullyQualified = namedTypeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var accessibility = generateAsPublic ? Accessibility.Public : Accessibility.Internal;

                    string constraint = namedTypeSymbol.FormatGenericConstraints();
                    var extension = new DropInExtension(generateNamespace, namedTypeSymbol.Name, fullyQualified, valueEnumeratorName, elementName, isElementGenericType, isValueType, constraint, accessibility);
                    list.Add(extension);
                }
            }

            return list;
        });

        context.RegisterSourceOutput(externalExtensions, EmitDropInExtension);
    }

    void EmitSourceOutput(SourceProductionContext context, ZLinqDropInAttribute? attribute)
    {
        if (attribute is null)
        {
            // In Unity, Source Generators are automatically referenced even in assemblies that don't need them, causing numerous errors.
            // Until an effective countermeasure can be devised, these warnings will be temporarily excluded.
            // context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.AttributeNotFound, null));
            return;
        }

        if (attribute.DisableEmitSource)
        {
            return;
        }

        var thisAsm = typeof(DropInGenerator).Assembly;
        var resourceNames = thisAsm.GetManifestResourceNames();
        foreach (var resourceName in resourceNames)
        {
            var dropinType = FromResourceName(resourceName);
            if (dropinType == DropInGenerateTypes.None || !attribute.DropInGenerateTypes.HasFlag(dropinType))
            {
                continue;
            }

            var sb = new StringBuilder();

            // #if...
            if (attribute.ConditionalCompilationSymbols is not null)
            {
                sb.AppendLine($"#if {attribute.ConditionalCompilationSymbols}");
            }

            if (dropinType == DropInGenerateTypes.Span)
            {
                sb.AppendLine("#if NET9_0_OR_GREATER");
            }

            using (var stream = thisAsm.GetManifestResourceStream(resourceName))
            using (var sr = new StreamReader(stream!))
            {
                var code = sr.ReadToEnd();
                sb.AppendLine(code); // write code
            }

            // replaces...
            if (!string.IsNullOrWhiteSpace(attribute.GenerateNamespace))
            {
                sb.Replace("using ZLinq.Linq;", $$"""
using ZLinq.Linq;
namespace {{attribute.GenerateNamespace}}
{
""");
            }

            if (attribute.GenerateAsPublic)
            {
                sb.Replace("internal static partial class", "public static partial class");
            }


            if (!string.IsNullOrWhiteSpace(attribute.GenerateNamespace))
            {
                sb.AppendLine("}");
            }
            if (attribute.ConditionalCompilationSymbols is not null)
            {
                sb.AppendLine($"#endif");
            }

            if (dropinType == DropInGenerateTypes.Span)
            {
                sb.AppendLine("#endif");
            }

            var hintName = resourceName.Replace("ZLinq.DropInGenerator.ResourceCodes.", "ZLinq.DropIn.").Replace(".cs", ".g.cs");
            context.AddSource($"{hintName}", sb.ToString().ReplaceLineEndings());
        }
    }

    void EmitDropInExtension(SourceProductionContext context, DropInExtension? extension)
    {
        if (extension == null) return;
        if (extension.Error != null)
        {
            context.ReportDiagnostic(extension.Error);
            return;
        }

        var sb = new StringBuilder();
        sb.AppendLine("#define ENABLE_EXTENSION");
        if (extension.IsElementGenericType)
        {
            if (extension.ElementConstraint is "")
            {
                sb.AppendLine("#define ENABLE_SUM_AVERAGE");
                sb.AppendLine("#define ENABLE_NULLABLE_SUM_AVERAGE");
                sb.AppendLine("#define ENABLE_TKEY_TVALUE_TODICTIONARY");
            }
            else if (extension.ElementConstraint == $"where {extension.ElementName} : struct")
            {
                sb.AppendLine("#define ENABLE_SUM_AVERAGE");
            }
            else if (extension.ElementConstraint == $"where {extension.ElementName} : unmanaged")
            {
                sb.AppendLine("#define ENABLE_SUM_AVERAGE");
            }
        }

        var thisAsm = typeof(DropInGenerator).Assembly;
        using (var stream = thisAsm.GetManifestResourceStream("ZLinq.DropInGenerator.ResourceCodes.ForExtension.cs"))
        using (var sr = new StreamReader(stream!))
        {
            var code = sr.ReadToEnd();

            var element = extension.ElementName;

            // fix first for sum and average, todictionary
            if (extension.IsElementGenericType)
            {
                code = code.Replace("Single[]", extension.TypeNameFullyQualified.Replace($"<{element}>", "<Single>"));
                code = code.Replace("Nullable<Single>[]", extension.TypeNameFullyQualified.Replace($"<{element}>", "<Single?>"));
                code = code.Replace("Decimal[]", extension.TypeNameFullyQualified.Replace($"<{element}>", "<Decimal>"));
                code = code.Replace("Nullable<Decimal>[]", extension.TypeNameFullyQualified.Replace($"<{element}>", "<Decimal?>"));
                code = code.Replace("Nullable<TSource>[]", extension.TypeNameFullyQualified.Replace($"<{element}>", "<TSource?>"));
                code = code.Replace("KeyValuePair<TKey, TValue>[]", extension.TypeNameFullyQualified.Replace($"<{element}>", "<KeyValuePair<TKey, TValue>>"));
                code = code.Replace("(TKey Key, TValue Value)[]", extension.TypeNameFullyQualified.Replace($"<{element}>", "<(TKey Key, TValue Value)>"));
            }

            // Repleace this TSource[] ...=> this CustomType
            code = Regex.Replace(code, "this .+\\[\\]", x => "this " + extension.TypeNameFullyQualified); // ToString shows <T> if type is generics
            if (extension.ValueEnumeratorName != null)
            {
                code = Regex.Replace(code, "FromArray<[^>]+>", $"{extension.ValueEnumeratorName}");
            }
            else
            {
                code = Regex.Replace(code, "FromArray<[^>]+>", $"FromEnumerable<{element}>");
            }

            // element type fix
            code = code.Replace("FromEnumerable<TSource>", $"FromEnumerable<{element}>");
            code = code.Replace("IEnumerable<TSource>", $"IEnumerable<{element}>");
            code = code.Replace("ValueEnumerable<TEnumerator2, TSource>", $"ValueEnumerable<TEnumerator2, {element}>");
            code = code.Replace("IValueEnumerator<TSource>", $"IValueEnumerator<{element}>");
            code = code.Replace("IEqualityComparer<TSource>", $"IEqualityComparer<{element}>");
            code = code.Replace("IComparer<TSource>", $"IComparer<{element}>");

            // generic defnition fix
            if (!extension.IsElementGenericType)
            {
                code = code.Replace("<TEnumerator2, TSource>", "<TEnumerator2>");
                code = code.Replace("<TEnumerator2, TOuter, ", "<TEnumerator2, ");
                code = code.Replace("Func<TOuter, TInner", $"Func<{element}, TInner");
                code = code.Replace("Func<TOuter?, TInner", $"Func<{element}{(extension.IsElementValueType ? "" : "?")}, TInner"); // for RightJoin, FullJoin
                code = code.Replace("IGrouping<TOuter, TInner>", $"IGrouping<{element}, TInner>"); // for GroupJoin without resultSelector
                code = code.Replace("(TOuter Outer, ", $"({element} Outer, "); // for Join, LeftJoin without resultSelector
                code = code.Replace("(TOuter? Outer, ", $"({element}{(extension.IsElementValueType ? "" : "?")} Outer, "); // for RightJoin, FullJoin without resultSelector
                code = code.Replace("<TOuter, TInner", "<TInner");
                code = code.Replace("TOuter, ", $"{element}, ");
                code = code.Replace("<TEnumerator2, TFirst", "<TEnumerator2");
                code = code.Replace("<TEnumerator2, TEnumerator3, TFirst", "<TEnumerator2, TEnumerator3");
                code = code.Replace("Func<TFirst, TSecond", $"Func<{element}, TSecond");
                code = code.Replace("<TFirst, TSecond", "<TSecond");
                code = code.Replace("TFirst", $"{element}");
                code = code.Replace("ToList<TSource>", $"ToList");
                code = code.Replace("ToHashSet<TSource>", $"ToHashSet");
                code = code.Replace("Span<TSource>", $"Span<{element}>");
                code = code.Replace("List<TSource>", $"List<{element}>");
                code = code.Replace("HashSet<TSource>", $"HashSet<{element}>");
                code = code.Replace("PooledArray<TSource>", $"PooledArray<{element}>");
                code = code.Replace("<TSource>", "");
                code = Regex.Replace(code, "<(.*)TSource, (.+?)>\\(", x =>
                {
                    if (string.IsNullOrEmpty(x.Groups[1].Value))
                    {
                        return "<" + x.Groups[2].Value + ">(";
                    }
                    else
                    {
                        return "<" + x.Groups[1].Value + x.Groups[2].Value + ">(";
                    }
                });
            }
            else
            {
                // for RightJoin, FullJoin, remove ? annotation where T:struct
                if (extension.ElementConstraint.Contains(": struct") || extension.ElementConstraint.Contains(": unmanaged"))
                {
                    code = code.Replace("Func<TOuter?, TInner", $"Func<{element}{(extension.IsElementValueType ? "" : "?")}, TInner");
                    code = code.Replace("(TOuter? Outer, ", $"({element}{(extension.IsElementValueType ? "" : "?")} Outer, "); // for RightJoin, FullJoin without resultSelector
                }

                code = code.Replace("TOuter", element);
                code = code.Replace("TFirst", element);
            }

            // Replace TSource -> ElementType(for Func, etc...)
            code = Regex.Replace(code, "TSource", extension.ElementName);
            if (!extension.IsElementGenericType)
            {
                if (extension.ValueEnumeratorName != null)
                {
                    code = code.Replace("ArrayWhere", $"Where<{extension.ValueEnumeratorName}, {element}>");
                    code = Regex.Replace(code, "ArraySelect<.+?>", $"Select<{extension.ValueEnumeratorName}, {element}, TResult>");
                }
                else
                {
                    code = code.Replace("ArrayWhere", $"Where<FromEnumerable<{element}>, {element}>");
                    code = Regex.Replace(code, "ArraySelect<.+?>", $"Select<FromEnumerable<{element}>, {element}, TResult>");
                }
            }
            else
            {
                if (extension.ValueEnumeratorName != null)
                {
                    code = Regex.Replace(code, "ArrayWhere<.+?>", $"Where<{extension.ValueEnumeratorName}, {element}>");
                    code = Regex.Replace(code, "ArraySelect<.+?>", $"Select<{extension.ValueEnumeratorName}, {element}, TResult>");
                }
                else
                {
                    code = Regex.Replace(code, "ArrayWhere<.+?>", $"Where<FromEnumerable<{element}>, {element}>");
                    code = Regex.Replace(code, "ArraySelect<.+?>", $"Select<FromEnumerable<{element}>, {element}, TResult>");
                }
            }

            // finally remove constraint
            if (extension.ElementConstraint != "")
            {
                code = code.Replace("=>", $"{extension.ElementConstraint} =>");
                // fix more
                code = code.Replace($"<Single> source) {extension.ElementConstraint}", "<Single> source)");
                code = code.Replace($"<Decimal> source) {extension.ElementConstraint}", "<Decimal> source)");
                code = code.Replace($"{extension.ElementConstraint} => source.AsValueEnumerable().Average();", " => source.AsValueEnumerable().Average();");
                code = code.Replace($"{extension.ElementConstraint} => source.AsValueEnumerable().Sum();", " => source.AsValueEnumerable().Sum();");
                code = code.Replace($"{extension.ElementConstraint} => source.AsValueEnumerable().SumUnchecked();", " => source.AsValueEnumerable().SumUnchecked();");

                if (extension.ElementConstraint.Contains(": unmanaged"))
                {
                    code = code.Replace($"where {element} : struct", $"where {element} : unmanaged");
                }
            }

            sb.AppendLine(code); // write code
        }

        // replace namespace
        if (!string.IsNullOrWhiteSpace(extension.NamespaceName))
        {
            sb.Replace("using ZLinq.Linq;", $$"""
using ZLinq.Linq;
namespace {{extension.NamespaceName}}
{
""");
        }

        // replace accessibility
        sb.Replace("internal static partial class", $"{extension.Accessibility.ToString().ToLower().Replace("and", " ").Replace("or", " ").Replace("friend", "internal")} static partial class");

        // replace typename
        sb.Replace("class ZLinqDropInExtensions", $"class {extension.TypeName}ZLinqDropInExtensions");

        // namespace close
        if (!string.IsNullOrWhiteSpace(extension.NamespaceName))
        {
            sb.AppendLine("}");
        }

        var fullType = extension.TypeNameFullyQualified.Replace("global::", "").Replace("<", "_").Replace(">", "_").Replace("+", "_").Replace(" ", "_");
        var hintName = "ZLinq.DropIn." + fullType + "Extensions.g.cs";
        context.AddSource(hintName, sb.ToString().ReplaceLineEndings());
    }

    DropInGenerateTypes FromResourceName(string name)
    {
        switch (name)
        {
            case "ZLinq.DropInGenerator.ResourceCodes.Array.cs": return DropInGenerateTypes.Array;
            case "ZLinq.DropInGenerator.ResourceCodes.IEnumerable.cs": return DropInGenerateTypes.Enumerable;
            case "ZLinq.DropInGenerator.ResourceCodes.List.cs": return DropInGenerateTypes.List;
            case "ZLinq.DropInGenerator.ResourceCodes.Memory.cs": return DropInGenerateTypes.Memory;
            case "ZLinq.DropInGenerator.ResourceCodes.ReadOnlyMemory.cs": return DropInGenerateTypes.Memory;
            case "ZLinq.DropInGenerator.ResourceCodes.ReadOnlySpan.cs": return DropInGenerateTypes.Span;
            case "ZLinq.DropInGenerator.ResourceCodes.Span.cs": return DropInGenerateTypes.Span;
            default: return DropInGenerateTypes.None;
        }
    }
}

record class DropInExtension(
    string NamespaceName,
    string TypeName,
    string TypeNameFullyQualified, // with generics
    string? ValueEnumeratorName,
    string? ElementName,
    bool IsElementGenericType,
    bool IsElementValueType,
    string ElementConstraint,
    Accessibility Accessibility,
    Diagnostic? Error = null)
{
    public DropInExtension(Diagnostic error)
        : this(null!, null!, null!, null, null, false, false, null!, Accessibility.Public, error)
    {
    }
}
