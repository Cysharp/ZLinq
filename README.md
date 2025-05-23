ZLinq
===
[![CI](https://github.com/Cysharp/ZLinq/actions/workflows/build-debug.yaml/badge.svg)](https://github.com/Cysharp/ZLinq/actions/workflows/build-debug.yml)
[![Benchmark](https://github.com/Cysharp/ZLinq/actions/workflows/benchmark.yaml/badge.svg)](https://github.com/Cysharp/ZLinq/actions/workflows/benchmark.yml)
[![NuGet](https://img.shields.io/nuget/v/ZLinq)](https://www.nuget.org/packages/ZLinq)
[![DeepWiki](https://img.shields.io/badge/Chat%20with-DeepWiki%20🤖-blue)](https://deepwiki.com/Cysharp/ZLinq)

Zero allocation LINQ with LINQ to Span, LINQ to SIMD, and LINQ to Tree (FileSystem, JSON, GameObject, etc.) for all .NET platforms(netstandard2.0, 2.1, net8, net9) and Unity, Godot.

![](img/benchmarkhead.jpg)

Unlike regular LINQ, ZLinq doesn't increase allocations when adding more method chains, and it also has higher basic performance. You can check various benchmark patterns at [GitHub Actions/Benchmark](https://github.com/Cysharp/ZLinq/actions/runs/14569138271). ZLinq shows high performance in almost all patterns, with some benchmarks showing overwhelming differences.

As a bonus, LINQ operators and optimizations equivalent to .NET 10 can be used in .NET Framework 4.8 (netstandard2.0) and Unity (netstandard2.1).

```bash
dotnet add package ZLinq
```

```csharp
using ZLinq;

var seq = source
    .AsValueEnumerable() // only add this line
    .Where(x => x % 2 == 0)
    .Select(x => x * 3);

foreach (var item in seq) { }
```

* **99% compatibility** with .NET 10's LINQ (including new `Shuffle`, `RightJoin`, `LeftJoin` operators)
* **Zero allocation** for method chains through struct-based Enumerable via `ValueEnumerable`
* **LINQ to Span** to full support LINQ operations on `Span<T>` using .NET 9/C# 13's `allows ref struct`
* **LINQ to Tree** to extend tree-structured objects (built-in support for FileSystem, JSON, GameObject)
* **LINQ to SIMD** to automatic application of SIMD where possible and customizable arbitrary operations
* Optional **Drop-in replacement** Source Generator to automatically accelerate all LINQ methods

In ZLinq, we have proven high compatibility and performance by running [dotnet/runtime's System.Linq.Tests](https://github.com/Cysharp/ZLinq/tree/main/tests/System.Linq.Tests) as a drop-in replacement, passing 9000 tests.

![](img/testrun.png)

Previously, value type-based LINQ implementations were often experimental, but ZLinq fully implements all methods to completely replace standard LINQ in production use, delivering high performance suitable even for demanding applications like games. The performance aspects are based on my experience with previous LINQ implementations ([linq.js](https://github.com/neuecc/linq.js/), [SimdLinq](https://github.com/Cysharp/SimdLinq/), [UniRx](https://github.com/neuecc/UniRx), [R3](https://github.com/Cysharp/R3)), zero-allocation implementations ([ZString](https://github.com/Cysharp/ZString), [ZLogger](https://github.com/Cysharp/ZLogger)), and high-performance serializers ([MessagePack-CSharp](https://github.com/MessagePack-CSharp/MessagePack-CSharp/), [MemoryPack](https://github.com/Cysharp/MemoryPack)).

ZLinq achieves zero-allocation LINQ implementation using the following structs and interfaces.

```csharp
public readonly ref struct ValueEnumerable<TEnumerator, T>(TEnumerator enumerator)
    where TEnumerator : struct, IValueEnumerator<T>, allows ref struct
{
    public readonly TEnumerator Enumerator = enumerator;
}

public interface IValueEnumerator<T> : IDisposable
{
    bool TryGetNext(out T current); // as MoveNext + Current

    // Optimization helper
    bool TryGetNonEnumeratedCount(out int count);
    bool TryGetSpan(out ReadOnlySpan<T> span);
    bool TryCopyTo(scoped Span<T> destination, Index offset);
}
```

Besides changing to a struct-based approach, we've integrated MoveNext and Current to reduce the number of iterator calls. Also, some operators don't need to hold Current, which allows minimizing the struct size. Additionally, being struct-based, we efficiently separate internal state by copying the Enumerator instead of using GetEnumerator. With .NET 9/C# 13 or later, `allows ref struct` enables natural integration of `Span<T>` into LINQ.

```csharp
public static ValueEnumerable<Where<TEnumerator, TSource>, TSource> Where<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, Boolean> predicate)
    where TEnumerator : struct, IValueEnumerator<TSource>, allows ref struct
````

Operators have this method signature. C# cannot infer types from generic constraints([dotnet/csharplang#6930](https://github.com/dotnet/csharplang/discussions/6930)). Therefore, the traditional Struct LINQ approach required implementing all operator combinations as instance methods, resulting in [100,000+ methods and massive assembly sizes](https://kevinmontrose.com/2018/01/17/linqaf-replacing-linq-and-not-allocating/). However, in ZLinq, we've successfully avoided all the boilerplate method implementations by devising an approach that properly conveys types to C# compiler.

Additionally, `TryGetNonEnumeratedCount(out int count)`, `TryGetSpan(out ReadOnlySpan<T> span)`, and `TryCopyTo(Span<T> destination, Index offset)` defined in the interface itself enable flexible optimizations. To minimize assembly size, we've designed the library to achieve maximum optimization with minimal method additions. For example, `TryCopyTo` works efficiently with methods like `ToArray` when combined with `TryGetNonEnumeratedCount`. However, it also allows copying to smaller-sized destinations. By combining this with Index, we can optimize `First`, `Last`, and `ElementAt` using just `TryCopyTo` by passing a single-element Span along with an Index.

If you're interested in architecture, please read my blog post [**"ZLinq", a Zero-Allocation LINQ Library for .NET**](https://neuecc.medium.com/zlinq-a-zero-allocation-linq-library-for-net-1bb0a3e5c749) where I wrote the details.

Getting Started
---
You can install package from [NuGet/ZLinq](https://www.nuget.org/packages/ZLinq). For Unity usage, refer to the [Unity section](#unity). For Godot usage, refer to the [Godot section](#godot).

```bash
dotnet add package ZLinq
```

Use `using ZLinq;` and call `AsValueEnumerable()` on any iterable type to use ZLinq's zero-allocation LINQ.

```csharp
using ZLinq;

var source = new int[] { 1, 2, 3, 4, 5 };

// Call AsValueEnumerable to apply ZLinq
var seq1 = source.AsValueEnumerable().Where(x => x % 2 == 0);

// Can also be applied to Span (only in .NET 9/C# 13 environments that support allows ref struct)
Span<int> span = stackalloc int[5] { 1, 2, 3, 4, 5 };
var seq2 = span.AsValueEnumerable().Select(x => x * x);
```

Even if it's netstandard 2.0 or below .NET 10, all operators up to .NET 10 are available.

You can method chain and foreach like regular LINQ, but there are some limitations. Please see [Difference and Limitation](#difference-and-limitation) for details. ZLinq has drop-in replacements that apply ZLinq without needing to call `AsValueEnumerable()`. For more information, see [Drop-in replacement](#drop-in-replacement). Detailed information about [LINQ to Tree](#linq-to-tree) for LINQ-ifying tree structures (FileSystems and JSON) and [LINQ to SIMD](#linq-to-simd) for expanding SIMD application range can be found in their respective sections.

Additional Operators
---
In ZLinq, we prioritize compatibility, so we try to minimize adding custom operators. However, the following methods have been added to enable efficient processing with zero allocation:

### `AsValueEnumerable()`

Converts existing collections to a type that can be chained with ZLinq. Any `IEnumerable<T>` can be converted, but for the following types, conversion is done with zero allocation without `IEnumerable<T>.GetEnumerator()` allocation. Standard supported types are `T[]`, `List<T>`, `ArraySegment<T>`, `Memory<T>`, `ReadOnlyMemory<T>`, `ReadOnlySequence<T>`, `Dictionary<TKey, TValue>`, `Queue<T>`, `Stack<T>`, `LinkedList<T>`, `HashSet<T>`, `ImmutableArray<T>`, `Span<T>`, `ReadOnlySpan<T>`. However, conversion from `ImmutableArray<T>` requires `.NET 8` or higher, and conversion from `Span<T>`, `ReadOnlySpan<T>` requires `.NET 9` or higher.

When a type is declared as `IEnumerable<T>` or `ICollection<T>` rather than concrete types like `T[]` or `List<T>`, generally additional allocations occur when using foreach. In `ZLinq`, even when these interfaces are declared, if the actual type is `T[]` or `List<T>`, processing is performed with zero allocation.

Convert from `System.Collections.IEnumerable` is also supported. In that case, using `AsValueEnumerable()` without specifying a type converts to `ValueEnumerable<, object>`, but you can also cast it simultaneously by `AsValueEnumerable<T>()`.

```csharp
IEnumerable nonGenericCollection = default!;
nonGenericCollection.AsValueEnumerable(); // ValueEnumerable<, object>
nonGenericCollection.AsValueEnumerable<int>(); // ValueEnumerable<, int>
```

### `ValueEnumerable.Range()`, `ValueEnumerable.Repeat()`, `ValueEnumerable.Empty()`

`ValueEnumerable.Range` operates more efficiently when handling with `ZLinq` than `Enumerable.Range().AsValueEnumerable()`. The same applies to `Repeat` and `Empty`. The Range can also handle `System.Range`, step increments, `IAdditionOperators<T>`, `DateTime`, and more. Please refer to the [Range](#range) section for details.

### `Average() : where INumber<T>`, `Sum() : where INumber<T>`

System.Linq's `Average` and `Sum` are limited to certain primitive types, but ZLinq extends them to all `INumber<T>` types. In `.NET 8` or higher, where constraints are included, but for others (netstandard2.0, 2.1), runtime errors will occur when called with non-primitive target types.

### `SumUnchecked()`

`Sum` is `checked`, but checking for overflow during SIMD execution creates performance overhead. `SumUnchecked` skips overflow checking to achieve maximum SIMD aggregation performance. Note that this requires `.NET 8` or higher, and SIMD-supported types are `sbyte`, `short`, `int`, `long`, `byte`, `ushort`, `uint`, `ulong`, `double`, and the source must be able to get a Span (`TryGetSpan` returns true).

### `AggregateBy`, `CountBy` constraints

.NET 9 `AggregateBy` and `CountBy` has `TKey : notnull` constraints. However, this is due to internal implementation considerations, and it lacks consistency with traditional operators such as Lookup and Join. Therefore, in ZLinq, the notnull constraint was removed.

### `int CopyTo(Span<T> destination)`, `void CopyTo(List<T> list)`

`CopyTo` can be used to avoid allocation of the return collection unlike `ToArray` or `ToList`. `int CopyTo(Span<T> destination)` allows the destination to be smaller than the source, returning the number of elements copied. `void CopyTo(List<T> list)` clears the list and then fills it with elements from the source, so the destination size is list.Count.

### `PooledArray<TSource> ToArrayPool()`

The returned array is rented from `ArrayPool<TSource>.Shared`. `PooledArray<TSource>` defines `.Span`, `.Memory`, `.AsEnumerable()` and other methods. These allow you to pass a `ValueEnumerable` to another method while minimizing allocations. Additionally, through `.AsValueEnumerable()`, you can call `ZLinq` methods, which is useful for temporarily materializing computationally expensive operations. Being `IDisposable`, you can return the borrowed array to `ArrayPool<TSource>.Shared` using the `using` statement.

```csharp
using var array = ValueEnumerable.Range(1, 1000).ToArrayPool();

var size = array.Size; // same as Length/Count in other types
var span = array.Span;
var memory = array.Memory;
var arraySegment = array.ArraySegment;
var enumerable = array.AsEnumerable();
var valueEnumerable = array.AsValueEnumerable();
```

For performance reasons to reduce allocations, `PooledArray<TSource>` is a `struct`. This creates a risk of returning the same array multiple times due to boxing or copying. Also, ArrayPool is not suitable for long-term array storage. It is recommended to simply use `ToArrayPool()` with `using` and keep the lifetime short.

If you absolutely need the raw internal array, you can `Deconstruct` it to `(T[] Array, int Size)`. After deconstructing, ownership is considered transferred, and all methods of `PooledArray<TSource>` become unavailable.

### `JoinToString(char|string seperator)`

Since `ZLinq` is not `IEnumerable<T>`, it cannot be passed to `String.Join`. `JoinToString` provides the same functionality as `String.Join`, returning a string joined with the separator.

Range
---
`Range` is not only compatible with System.Linq's `Range(int start, int count)` but also has many additional overloads such as `System.Range` and `DateTime`.

```csharp
// 95, 96, 97, 98, 99
var range1 = ValueEnumerable.Range(95..100);

// 95, 96, 97, 98, 99, 100
var range2 = ValueEnumerable.Range(95..100, RightBound.Inclusive);

// 10, 12, 14, 16, 18
var step = ValueEnumerable.Range(start: 10, count: 5, step: 2);

// 10, 9, 8, 7, 6
var reverse = ValueEnumerable.Range(start: 10, count: 5, step: -1);

// 10, 9, 8, 7, 6, 5
var downTo = ValueEnumerable.Range(start: 10, end: 5, RightBound.Inclusive);

// 0, 1,.........
var infinite = ValueEnumerable.Range(..);

// a, b, c,..., z
var alphabets = ValueEnumerable.Range(start: 'a', end: 'z', RightBound.Inclusive);

// 5/13, 5/14, 5/15, 5/16, 5/17, 5/18, 5/19
var daysOfweek = ValueEnumerable.Range(DateTime.Now, 7, TimeSpan.FromDays(1)); ;

// 5/1, 5/2,...,5/31
var now = DateTime.Now;
var calendarOfThisMonth = ValueEnumerable.Range(new DateTime(now.Year, now.Month, 1), DateTime.DaysInMonth(now.Year, now.Month), TimeSpan.FromDays(1));
```

Passing `..` as Range creates an infinite stream. Range is Exclusive by default, but you can also run it as Inclusive by specifying `RightBound.Inclusive/Exclusive`. Also, in .NET 8 or later, it supports `IAdditionOperators<T>`, allowing you to generate not only int but also `char`, `float`, etc. In addition, it supports more generic generation with not only count but also `T end` specification and `TStep step`.

It supports `DateTime`, `DateTimeOffset` + `TimeSpan` for all platforms. [Unfortunately, `DateTime` and `DateTimeOffset` do not support Generic Math](https://github.com/dotnet/runtime/issues/76225), but we have prepared our own implementation that provides functionality equivalent to `IAdditionOperators<T>` support. This makes it easy to generate date sequences.

The complete list of Range APIs is as follows.

```csharp
public enum RightBound
{
    Inclusive,
    Exclusive
}

public static partial class ValueEnumerable
{
    public static ValueEnumerable<FromRange, int> Range(int start, int count)

    public static ValueEnumerable<FromRange2, int> Range(Range range, RightBound rightBound = RightBound.Exclusive)

#if NET8_0_OR_GREATER

    public static ValueEnumerable<FromRange<T, T>, T> Range<T>(T start, int count)
        where T : INumberBase<T>

    public static ValueEnumerable<FromRange<T, TStep>, T> Range<T, TStep>(T start, int count, TStep step)
        where T : IAdditionOperators<T, TStep, T>

    public static ValueEnumerable<FromRangeTo<T, T>, T> Range<T>(T start, T end, RightBound rightBound)
        where T : INumberBase<T>, IComparisonOperators<T, T, bool>

    public static ValueEnumerable<FromRangeTo<T, TStep>, T> Range<T, TStep>(T start, T end, TStep step, RightBound rightBound)
        where T : IAdditionOperators<T, TStep, T>, IComparisonOperators<T, T, bool>

#endif

    public static ValueEnumerable<FromRangeDateTime, DateTime> Range(DateTime start, int count, TimeSpan step)
    public static ValueEnumerable<FromRangeDateTimeTo, DateTime> Range(DateTime start, DateTime end, TimeSpan step, RightBound rightBound)
    public static ValueEnumerable<FromRangeDateTimeOffset, DateTimeOffset> Range(DateTimeOffset start, int count, TimeSpan step)
    public static ValueEnumerable<FromRangeDateTimeOffsetTo, DateTimeOffset> Range(DateTimeOffset start, DateTimeOffset end, TimeSpan step, RightBound rightBound)
}
```

Difference and Limitation
---
For .NET 9 and above, `ValueEnumerable<T>` is a `ref struct` and cannot be converted to `IEnumerable<T>`. To ensure compatibility when upgrading, `AsEnumerable` is not provided by default even for versions prior to .NET 9.

Since `ValueEnumerable<T>` is not an `IEnumerable<T>`, it cannot be passed to methods that require `IEnumerable<T>`. It's also difficult to pass it to other methods due to the complex type signatures required by generics (implementation is explained in the [Custom Extensions](#custom-extensions) section). Using `ToArray()` is one solution, but this can cause unnecessary allocations in some cases. For temporary use, you can call `ToArrayPool` to pass to methods that require `IEnumerable<T>` without allocations. However, be careful that this `IEnumerable<T>` will be returned within the using scope, so you must ensure it doesn't leak outside the scope (storing it in a field is not allowed).

`String.Join` has overloads for both `IEnumerable<string>` and `params object[]`. Passing `ValueEnumerable<T>` directly will select the `object[]` overload, which may not give the desired result. In this case, use the `JoinToString` operator instead.

`ValueEnumerable<T>` is a struct, and its size increases slightly with each method chain. With many chained methods, copy costs can become significant. When iterating over small collections, these copy costs can outweigh the benefits, causing performance to be worse than standard LINQ. However, this is only an issue with extremely long method chains and small iteration counts, so it's rarely a practical concern.

`ValueEnumerable<T>` is `ref struct` in .NET 9 or above, this means that it cannot span across yield or await. Using yield or await inside foreach also fails and shows compilation errors. If enumeration is needed, please materialize the data using methods like [ToArrayPool](#pooledarraytsource-toarraypool).

Each chain operation returns a different type, so you cannot reassign to the same variable. For example, code that continuously reassigns `Concat` in a for loop cannot be implemented.

In .NET 8 and above, the `Sum` and `Average` methods for `double` use SIMD processing, which performs parallel processing based on SIMD width. This results in calculation errors that differ from normal ones due to the different order of addition.

Drop-in replacement
---
When introducing `ZLinq.DropInGenerator`, you can automatically use ZLinq for all LINQ methods without calling `AsValueEnumerable()`.

```bash
dotnet add package ZLinq.DropInGenerator
```

![](img/dropin.jpg)

It works by using a Source Generator to add extension methods for each type that take priority, making `ZLinq` methods be selected instead of System.Linq when the same name and arguments are used.
After installing the package, you need to configure it with an assembly attribute.

```csharp
[assembly: ZLinq.ZLinqDropInAttribute("ZLinq", ZLinq.DropInGenerateTypes.Array)]
```

`generateNamespace` is the namespace for the generated code, and `DropInGenerateTypes` selects the target types.
`DropInGenerateTypes` allows you to choose from `Array`, `Span` (Span/ReadOnlySpan), `Memory` (Memory/ReadOnlyMemory), `List`, and `Enumerable` (IEnumerable).
These are Flags, so you can combine them, such as `DropInGenerateTypes.Array | DropInGenerateTypes.Span`.
There are also predefined combinations: `Collection = Array | Span | Memory | List` and `Everything = Array | Span | Memory | List | Enumerable`.

When using `DropInGenerateTypes.Enumerable`, which generates extension methods for `IEnumerable<T>`, you need to make `generateNamespace` global as a namespace priority.
For example:

```csharp
[assembly: ZLinq.ZLinqDropInAttribute("", ZLinq.DropInGenerateTypes.Everything)]
```

This is the most aggressive configuration, causing all LINQ methods to be processed by ZLinq, and making it impossible to use normal LINQ methods (if Enumerable is not included, you can call AsEnumerable() to execute with System.Linq).

It's better to use application's default namespace rather than globally, as this allows you to switch between normal LINQ using namespaces. This approach is recommended when you need to target `Enumerable`.

```csharp
using ZLinq;

[assembly: ZLinqDropInAttribute("MyApp", DropInGenerateTypes.Everything)]

// namespace under MyApp
namespace MyApp.Foo
{
    public class Bar
    {
        public static void Foo(IEnumerable<int> source)
        {
            // ZLinq ValueEnumerable<T>
            var seq = source.Select(x => x * 2).Shuffle();
            using var e = seq.Enumerator;
            while (e.TryGetNext(out var current))
            {
                Console.WriteLine(current);
            }
        }
    }
}

// not under MyApp namespace
namespace NotMyApp
{
    public class Baz
    {
        public static void Foo(IEnumerable<int> source)
        {
            // IEnumerable<T>
            var seq = source.Select(x => x * 2); // .Shuffle();
            using var e = seq.GetEnumerator();
            while (e.MoveNext())
            {
                Console.WriteLine(e.Current);
            }
        }
    }
}
```

ZLinq is powerful and in many cases it performs better than regular LINQ, but it also has its limitations. For more information, please refer to [Difference and Limitation](#difference-and-limitation). When you are not familiar with it, we recommend that you use `DropInGenerateTypes.Collection` instead of `DropInGenerateTypes.Everything`.

Other options for `ZLinqDropInAttribute` include `GenerateAsPublic`, `ConditionalCompilationSymbols`, and `DisableEmitSource`.

```csharp
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
public sealed class ZLinqDropInAttribute : Attribute
{
    /// <summary>
    /// Gets the namespace where the generated LINQ implementations will be placed.
    /// If empty, the implementations will be generated in the global namespace.
    /// </summary>
    public string GenerateNamespace { get; }

    /// <summary>
    /// Gets the types of collections for which LINQ implementations should be generated.
    /// </summary>
    public DropInGenerateTypes DropInGenerateTypes { get; }

    /// <summary>
    /// Gets whether the generated LINQ implementations should be public.
    /// When true, the implementations will be generated with public visibility.
    /// When false (default), the implementations will be generated with internal visibility.
    /// </summary>
    public bool GenerateAsPublic { get; set; }

    /// <summary>
    /// Gets or sets the conditional compilation symbols to wrap the generated code with #if directives.
    /// If specified, the generated code will be wrapped in #if/#endif directives using these symbols.
    /// </summary>
    public string? ConditionalCompilationSymbols { get; set; }

    /// <summary>
    /// Gets or sets whether to disable source generation in emitted code.
    /// When true, the source code comments will not be included in the generated code.
    /// When false (default), source code comments will be included in the generated code.
    /// </summary>
    public bool DisableEmitSource { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ZLinqDropInAttribute"/> class.
    /// </summary>
    /// <param name="generateNamespace">The namespace where the generated LINQ implementations will be placed. If empty, place to global.</param>
    /// <param name="dropInGenerateTypes">The types of collections for which LINQ implementations should be generated.</param>
    public ZLinqDropInAttribute(string generateNamespace, DropInGenerateTypes dropInGenerateTypes)
    {
        GenerateNamespace = generateNamespace;
        DropInGenerateTypes = dropInGenerateTypes;
    }
}
```

To support DropIn types other than `DropInGenerateTypes`, you can use `ZLinqDropInExternalExtensionAttribute`. This attribute allows you to generate DropIn for any type by specifying its fully qualified name. For example, to add support for `IReadOnlyCollection<T>` and `IReadOnlyList<T>`, write:

```csharp
// T must be written as `1 (metadata-name). For nested types, connect with +
[assembly: ZLinqDropInExternalExtension("ZLinq", "System.Collections.Generic.IReadOnlyCollection`1")]
[assembly: ZLinqDropInExternalExtension("ZLinq", "System.Collections.Generic.IReadOnlyList`1")]
```

For types that support `IValueEnumerator<T>` through `AsValueEnumerable()`, specify the ValueEnumerator type name as the second argument. For example, with `ImmutableArray<T>`:

```csharp
[assembly: ZLinqDropInExternalExtension("ZLinq", "System.Collections.Immutable.ImmutableArray`1", "ZLinq.Linq.FromImmutableArray`1")]
```

This allows all operators to be processed by ZLinq using an optimized type.

If you want to make your custom collection types DropIn compatible, you can embed them in your assembly using `[ZLinqDropInExtension]`.

```csharp
[ZLinqDropInExtension]
public class AddOnlyIntList : IEnumerable<int>
{
    List<int> list = new List<int>();

    public void Add(int x) => list.Add(x);

    public IEnumerator<int> GetEnumerator() => list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
}
```

This generates a `public static partial class AddOnlyIntListZLinqDropInExtensions` in the same namespace, overriding all LINQ operators with ZLinq. This works with generic types as well:

```csharp
[ZLinqDropInExtension]
public class AddOnlyList<T> : IEnumerable<T>
```

While `[ZLinqDropInExtension]` works with classes implementing `IEnumerable<T>`, implementing `IValueEnumerable<TEnumerator, T>` provides zero-allocation optimization for ZLinq:

```csharp
[ZLinqDropInExtension]
public class AddOnlyIntList2 : IValueEnumerable<AddOnlyIntList2.Enumerator, int>
{
    List<int> list = new List<int>();

    public void Add(int x) => list.Add(x);

    public ValueEnumerable<FromValueEnumerable<Enumerator, int>, int> AsValueEnumerable()
    {
        // you need to write new(new(new())) magic.
        return new(new(new(list)));
    }

    // `public` struct enumerator
    public struct Enumerator(List<int> source) : IValueEnumerator<int>
    {
        int index;

        public bool TryGetNonEnumeratedCount(out int count)
        {
            count = source.Count;
            return true;
        }

        public bool TryGetSpan(out ReadOnlySpan<int> span)
        {
            span = CollectionsMarshal.AsSpan(source);
            return true;
        }

        public bool TryCopyTo(scoped Span<int> destination, Index offset)
        {
            // Optional path: if you can not write this, always return false is ok.
            ReadOnlySpan<int> span = CollectionsMarshal.AsSpan(source);
            if (ZLinq.Internal.EnumeratorHelper.TryGetSlice(span, offset, destination.Length, out var slice))
            {
                slice.CopyTo(destination);
                return true;

            }
            return false;
        }

        public bool TryGetNext(out int current)
        {
            if (index < source.Count)
            {
                current = source[index];
                index++;
                return true;
            }

            current = default;
            return false;
        }

        public void Dispose() { }
    }
}
```

In this case, implementing `IEnumerable<T>` is not necessary. If a collection implements both `IEnumerable<T>` and `IValueEnumerable<TEnumerator, T>`, the latter takes precedence.

LINQ to Tree
---
LINQ to XML introduced the concept of querying around axes to C#. Even if you don't use XML, similar APIs are incorporated into Roslyn and effectively used for exploring SyntaxTrees. ZLinq extends this concept to make it applicable to anything that can be considered a Tree, allowing `Ancestors`, `Children`, `Descendants`, `BeforeSelf`, and `AfterSelf` to be applied.

![](img/axis.jpg)

Specifically, by defining a struct that implements the following interface, it becomes iterable:

```csharp
public interface ITraverser<TTraverser, T> : IDisposable
    where TTraverser : struct, ITraverser<TTraverser, T> // self
{
    T Origin { get; }
    TTraverser ConvertToTraverser(T next); // for Descendants
    bool TryGetHasChild(out bool hasChild); // optional: optimize use for Descendants
    bool TryGetChildCount(out int count);   // optional: optimize use for Children
    bool TryGetParent(out T parent); // for Ancestors
    bool TryGetNextChild(out T child); // for Children | Descendants
    bool TryGetNextSibling(out T next); // for AfterSelf
    bool TryGetPreviousSibling(out T previous); // BeforeSelf
}
```

Standard packages are available for FileSystemInfo and JsonNode. For Unity, it's applicable to GameObject and Transform.

### FileSystem

```bash
dotnet add package ZLinq.FileSystem
```

```csharp
using ZLinq;

var root = new DirectoryInfo("C:\\Program Files (x86)\\Steam");

// FileSystemInfo(FileInfo/DirectoryInfo) can call `Ancestors`, `Children`, `Descendants`, `BeforeSelf`, `AfterSelf`
var allDlls = root
    .Descendants()
    .OfType<FileInfo>()
    .Where(x => x.Extension == ".dll");

var grouped = allDlls
    .GroupBy(x => x.Name)
    .Select(x => new { FileName = x.Key, Count = x.Count() })
    .OrderByDescending(x => x.Count);

foreach (var item in grouped)
{
    Console.WriteLine(item);
}
```

### JSON(System.Text.Json)

```bash
dotnet add package ZLinq.Json
```

```csharp
using ZLinq;

// System.Text.Json's JsonNode is the target of LINQ to JSON(not JsonDocument/JsonElement).
var json = JsonNode.Parse("""
{
    "nesting": {
      "level1": {
        "description": "First level of nesting",
        "value": 100,
        "level2": {
          "description": "Second level of nesting",
          "flags": [true, false, true],
          "level3": {
            "description": "Third level of nesting",
            "coordinates": {
              "x": 10.5,
              "y": 20.75,
              "z": -5.0
            },
            "level4": {
              "description": "Fourth level of nesting",
              "metadata": {
                "created": "2025-02-15T14:30:00Z",
                "modified": null,
                "version": 2.1
              },
              "level5": {
                "description": "Fifth level of nesting",
                "settings": {
                  "enabled": true,
                  "threshold": 0.85,
                  "options": ["fast", "accurate", "balanced"],
                  "config": {
                    "timeout": 30000,
                    "retries": 3,
                    "deepSetting": {
                      "algorithm": "advanced",
                      "parameters": [1, 1, 2, 3, 5, 8, 13]
                    }
                  }
                }
              }
            }
          }
        }
      }
    }
}
""");

// JsonNode
var origin = json!["nesting"]!["level1"]!["level2"]!;

// JsonNode axis, Children, Descendants, Anestors, BeforeSelf, AfterSelf and ***Self.
foreach (var item in origin.Descendants().Select(x => x.Node).OfType<JsonArray>())
{
    // [true, false, true], ["fast", "accurate", "balanced"], [1, 1, 2, 3, 5, 8, 13]
    Console.WriteLine(item.ToJsonString(JsonSerializerOptions.Web));
}
```

### GameObject/Transform(Unity)

see: [unity](#unity) section.

LINQ to SIMD
---
In .NET 8 and above, there are operators that apply SIMD when `ValueEnumerable<T>.TryGetSpan` returns true. The scope of application is wider than in regular System.Linq.

* **Range** to ToArray/ToList/CopyTo/etc...
* **Repeat** for `unmanaged struct` and `size is power of 2` to ToArray/ToList/CopyTo/etc...
* **Sum** for `sbyte`, `short`, `int`, `long`, `byte`, `ushort`, `uint`, `ulong`, `double`
* **SumUnchecked** for `sbyte`, `short`, `int`, `long`, `byte`, `ushort`, `uint`, `ulong`, `double`
* **Average** for `sbyte`, `short`, `int`, `long`, `byte`, `ushort`, `uint`, `ulong`, `double`
* **Max** for `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `nint`, `nuint`, `Int128`, `UInt128`
* **Min** for `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `nint`, `nuint`, `Int128`, `UInt128`
* **Contains** for `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `bool`, `char`, `nint`, `nuint`
* **SequenceEqual** for `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `bool`, `char`, `nint`, `nuint`

`Sum` performs calculations as checked, but if you don't need to worry about overflow, using `SumUnchecked` is faster.

| Method            | N     | Mean          | Allocated |
|------------------ |------ |--------------:|----------:|
| ForLoop           | 16384 | 25,198.556 ns |         - |
| SystemLinqSum     | 16384 |  1,402.259 ns |         - |
| ZLinqSum          | 16384 |  1,351.449 ns |         - |
| ZLinqSumUnchecked | 16384 |    721.832 ns |         - |

By using `ZLinq.Simd` in your using statements, you can call `.AsVectorizable()` on `T[]` or `Span<T>` or `ReadOnlySpan<T>`, which allows you to use `Sum`, `SumUnchecked`, `Average`, `Max`, `Min`, `Contains`, and `SequenceEqual`. This explicitly indicates execution with SIMD regardless of the LINQ chain state (though type checking is ambiguous so processing might occur in a normal loop, and if `Vector.IsHardwareAccelerated && Vector<T>.IsSupported` is false, normal loop processing will be used).

From `int[]` or `Span<int>`, you can call `VectorizedFillRange`. This is equivalent to `ValueEunmerable.Range().CopyTo()` and allows you to quickly generate sequential numbers through SIMD processing.

| Method | Mean       | Allocated |
|------- |-----------:|----------:|
| Range  |   540.0 ns |         - |
| For    | 6,228.9 ns |         - |

### `VectorizedUpdate`

In ZLinq, you can perform relatively flexible vectorized loop processing using `Func`. With `T[]` and `Span<T>`, you can use the `VectorizedUpdate` method. By writing two lambda expressions - `Func<Vector<T>, Vector<T>> vectorFunc` for vector operations and `Func<T, T> func` for handling remainder elements - you can perform loop update processing at SIMD width.

```csharp
using ZLinq.Simd; // needs using

int[] source = Enumerable.Range(0, 10000).ToArray();

[Benchmark]
public void For()
{
    for (int i = 0; i < source.Length; i++)
    {
        source[i] = source[i] * 10;
    }
}

[Benchmark]
public void VectorizedUpdate()
{
    // arg1: Vector<int> => Vector<int>
    // arg2: int => int
    source.VectorizedUpdate(static x => x * 10, static x => x * 10);
}
```

| Method           | N     | Mean       | Error    | StdDev  | Allocated |
|----------------- |------ |-----------:|---------:|--------:|----------:|
| For              | 10000 | 4,560.5 ns | 67.24 ns | 3.69 ns |         - |
| VectorizedUpdate | 10000 |   558.9 ns |  6.42 ns | 0.35 ns |         - |

There is delegate overhead when compared to writing everything inline, but processing can be faster than using for-loops. However, this varies case by case, so please take measurements in advance based on your data volume and method content. Of course, if you're seeking the best possible performance, you should write code inline.

### Vectorizable Methods

You can convert from `T[]` or `Span<T>` or `ReadOnlySpan<T>` to `Vectorizable<T>` using `AsVectorizable()`, which allows you to use `Aggregate`, `All`, `Any`, `Count`, `Select`, and `Zip` methods that accept a `Func` as an argument.

* `Aggregate`

```csharp
source.AsVectorizable().Aggregate((x, y) => Vector.Min(x, y), (x, y) => Math.Min(x, y))
```

* `All`

```csharp
source.AsVectorizable().All(x => Vector.GreaterThanAll(x, new(5000)), x => x > 5000);
```

* `Any`

```csharp
source.AsVectorizable().Any(x => Vector.LessThanAll(x, new(5000)), x => x < 5000);
```

* `Count`

```csharp
source.AsVectorizable().Count(x => Vector.GreaterThan(x, new(5000)), x => x > 5000);
```

| Method            | Mean        | Error    | StdDev  | Allocated |
|------------------ |------------:|---------:|--------:|----------:|
| VectorizableCount |  1,048.4 ns | 39.39 ns | 2.16 ns |         - |
| LinqCount         | 10,909.3 ns | 54.79 ns | 3.00 ns |         - |

* `Select` -> `ToArray` or `CopyTo`

```csharp
source.AsVectorizable().Select(x => x * 3, x => x * 3).ToArray();
source.AsVectorizable().Select(x => x * 3, x => x * 3).CopyTo(destination);
```

* `Zip` -> `ToArray` or `CopyTo`

```csharp
// Zip2
array1.AsVectorizable().Zip(array2, (x, y) => x + y, (x, y) => x + y).CopyTo(destination);
array1.AsVectorizable().Zip(array2, (x, y) => x + y, (x, y) => x + y).ToArray();

// Zip3
array1.AsVectorizable().Zip(array2, array3, (x, y, z) => x + y + z, (x, y, z) => x + y + z).CopyTo(destination);
array1.AsVectorizable().Zip(array2, array3, (x, y, z) => x + y + z, (x, y, z) => x + y + z).ToArray();
```

| Method                      | Mean      |
|---------------------------- |----------:|
| ZLinqVectorizableZipCopyTo  |  24.17 μs |
| ZLinqVectorizableZip3CopyTo |  29.26 μs |
| ZLinqZipCopyTo              | 329.43 μs |
| ZLinqZip3CopyTo             | 584.69 μs |


Unity
---
There are two installation steps required to use it in Unity.

1. Install `ZLinq` from NuGet using [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity)
Open Window from NuGet -> Manage NuGet Packages, Search "ZLinq" and Press Install.

2. Install the `ZLinq.Unity` package by referencing the git URL

```bash
https://github.com/Cysharp/ZLinq.git?path=src/ZLinq.Unity/Assets/ZLinq.Unity
```

With the help of the Unity package, in addition to the standard ZLinq, LINQ to GameObject functionality becomes available for exploring GameObject/Transform.

![](img/axis.jpg)

```csharp
using ZLinq;

public class SampleScript : MonoBehaviour
{
    public Transform Origin;

    void Start()
    {
        Debug.Log("Ancestors--------------");  // Container, Root
        foreach (var item in Origin.Ancestors()) Debug.Log(item.name);

        Debug.Log("Children--------------"); // Sphere_A, Sphere_B, Group, Sphere_A, Sphere_B
        foreach (var item in Origin.Children()) Debug.Log(item.name);

        Debug.Log("Descendants--------------"); // Sphere_A, Sphere_B, Group, P1, Group, Sphere_B, P2, Sphere_A, Sphere_B
        foreach (var item in Origin.Descendants()) Debug.Log(item.name);

        Debug.Log("BeforeSelf--------------"); // C1, C2
        foreach (var item in Origin.BeforeSelf()) Debug.Log(item.name);

        Debug.Log("AfterSelf--------------");  // C3, C4
        foreach (var item in Origin.AfterSelf()) Debug.Log(item.name);
    }
}
```

You can chain query(LINQ to Objects). Also, you can filter by component using the `OfComponent<T>` helper.

```csharp
// all filtered(tag == "foobar") objects
var foobars = root.Descendants().Where(x => x.tag == "foobar");

// get FooScript under self childer objects and self
var fooScripts = root.ChildrenAndSelf().OfComponent<FooScript>();
```

UI Toolkit VisualElements are also supported allowing more advanced queries

```csharp
public class SampleScript : MonoBehaviour
{
    private UIDocument Document;

    private void Start()
    {
        var noTextButtons = Document
            .rootVisualElement
            .Descendants()
            .OfType<Button>()
            .Where(btn => string.IsNullOrEmpty(btn.text));

        foreach (var btn in noTextButtons) Debug.Log(btn.name);
    }
}
```

NOTE: In Unity, since .NET Standard 2.1 is referenced, SIMD is not utilized.

In .NET 9, `ValueEnumerable` is a `ref struct`, so it cannot be converted to `IEnumerable<T>`. However, in Unity it's a regular `struct`, making it possible to convert to `IEnumerable<T>`. You can improve interoperability by preparing an extension method like this:

```csharp
public static class ZLinqExtensions
{
    public static IEnumerable<T> AsEnumerable<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> valueEnumerable)
        where TEnumerator : struct, IValueEnumerator<T>
    {
        using (var e = valueEnumerable.Enumerator)
        {
            while (e.TryGetNext(out var current))
            {
                yield return current;
            }
        }
    }
}
```

In Unity, you can convert `NativeArray`, `NativeSlice` using `AsEnumerable()` to write queries with ZLinq. If Unity Collections(`com.unity.collections`) package version is `2.1.1` or above,  `NativeQueue`, `NativeHashSet`, `NativeText`, `FixedList32Bytes`, `FixedList64Bytes`, `FixedList128Bytes`, `FixedList512Bytes`, `FixedList4096Bytes`, `FixedString32Bytes`, `FixedString64Bytes`, `FixedString128Bytes`, `FixedString512Bytes`, and `FixedString4096Bytes` support `AsValueEnumerable()`.

You can also use drop-in replacement. Add `ZLinq.DropInGenerator` from NuGetForUnity. If you want to use DropInGenerator, the minimum supported Unity version will be `2022.3.12f1`, as it is necessary to support C# Incremental Source Generator(Compiler Version, 4.3.0).

Assembly attributes need to be set for each asmdef. For example, place a `.cs` file like the following in each asmdef. The DropInGenerator is defined in the assembly attributes.

```csharp
// AssemblyAttributes.cs
using ZLinq;
[assembly: ZLinqDropIn("MyApp", DropInGenerateTypes.Array | DropInGenerateTypes.List)]
```

For more details about DropInGenerator, please refer to the [Drop-in replacement](#drop-in-replacement) section.

To support Native Collections in addition to regular DropIn types, you can use `ZLinqDropInExternalExtension` as follows:

```csharp
[assembly: ZLinqDropInExternalExtension("ZLinq", "Unity.Collections.NativeArray`1", "ZLinq.Linq.FromNativeArray`1")]
[assembly: ZLinqDropInExternalExtension("ZLinq", "Unity.Collections.NativeArray`1+ReadOnly", "ZLinq.Linq.FromNativeArray`1")]
[assembly: ZLinqDropInExternalExtension("ZLinq", "Unity.Collections.NativeSlice`1", "ZLinq.Linq.FromNativeSlice`1")]
[assembly: ZLinqDropInExternalExtension("ZLinq", "Unity.Collections.NativeList`1", "ZLinq.Linq.FromNativeList`1")]
```

This is not just about Unity, but using `AsValueEnumerable()` even if only for foreach on `IEnumerable<T>` can sometimes reduce allocations. If the actual implementation of `IEnumerable<T>` is a `T[]` or `List<T>`, ZLinq will process it appropriately without allocations.

![](img/unityforeach.png)

```csharp
void IterateNormal(IEnumerable<int> source)
{
    // Normally there's an allocation when getting IEnumerator<T>.
    foreach (var item in source)
    {

    }
}

void IterateZLinq(IEnumerable<int> source)
{
    // Adding AsValueEnumerable results in 0 allocation.
    // However, zero alloc only works when the actual implementation of IEnumerable<T> is an array [] or List<T>
    foreach (var item in source.AsValueEnumerable())
    {

    }
}
```

Godot
---
The minimum supported Godot version will be `4.0.0`.
You can install ZLinq.Godot package via NuGet.

```bash
dotnet add package ZLinq.Godot
```

In addition to the standard ZLinq, LINQ to Node functionality is available.

![](img/godot.jpg)

```csharp
using Godot;
using ZLinq;

public partial class SampleScript : Node2D
{
    public override void _Ready()
    {
        var origin = GetNode<Node2D>("Container/Origin");

        GD.Print("Ancestors--------------"); // Container, Root, root (Root Window)
        foreach (var item in origin.Ancestors()) GD.Print(item.Name);

        GD.Print("Children--------------"); // Sphere_A, Sphere_B, Group, Sphere_A, Sphere_B
        foreach (var item in origin.Children()) GD.Print(item.Name);

        GD.Print("Descendants--------------"); // Sphere_A, Sphere_B, Group, P1, Group, Sphere_B, P2, Sphere_A, Sphere_B
        foreach (var item in origin.Descendants()) GD.Print(item.Name);

        GD.Print("BeforeSelf--------------"); // C1, C2
        foreach (var item in origin.BeforeSelf()) GD.Print(item.Name);

        GD.Print("AfterSelf--------------"); // C3, C4
        foreach (var item in origin.AfterSelf()) GD.Print(item.Name);
    }
}

```

You can chain query(LINQ to Objects). Also, you can filter by node type using the `OfType()`.

```csharp
// get ancestors under a Window
var ancestors = root.Ancestors().TakeWhile(x => x is not Window);
// get FooScript under self childer objects and self
var fooScripts = root.ChildrenAndSelf().OfType<FooScript>();
```

Custom Extensions
---

Implementing extension methods for `IEnumerable<T>` is common. There are two types of operators: consuming operators like `Count` and `Sum`, and chainable operators like `Select` and `Where`. This section explains how to implement them.

#### Consume Operator

The method signature is slightly more complex compared to `IEnumerable<T>`, requiring constraints on `TEnumerator`. For .NET 9 or later, `allows ref struct` is also needed.

```csharp
public static class MyExtensions
{
    public static void Consume<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = source.Enumerator; // using Enumerator

        while (e.TryGetNext(out var current)) // MoveNext + Current
        {
        }
    }
}
```

Instead of `GetEnumerator()`, use `Enumerator`, and instead of `MoveNext + Current`, use `TryGetNext(out)` to consume the iterator. The Enumerator must be used with `using`.

Consumers can call the Enumerator's optimization methods: `TryGetNonEnumeratedCount`, `TryGetSpan`, and `TryCopyTo`. For example, getting a Span like this is faster than normal iteration with TryGetNext:

```csharp
public static class MyExtensions
{
    public static void ForEach<TEnumerator, TSource>(this ValueEnumerable<TEnumerator, TSource> source, Action<TSource> action)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = source.Enumerator;

        if (e.TryGetSpan(out var span))
        {
            // faster iteration
            foreach (var item in span)
            {
                action(item);
            }
        }
        else
        {
            while (e.TryGetNext(out var item))
            {
                action(item);
            }
        }
    }

    public static ImmutableArray<T> ToImmutableArray<TEnumerator, T>(this ValueEnumerable<TEnumerator, T> source)
        where TEnumerator : struct, IValueEnumerator<T>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        using var e = source.Enumerator;

        if (e.TryGetSpan(out var span))
        {
            return ImmutableArray.Create(span);
        }
        else
        {
            // set capacity if can
            var builder = e.TryGetNonEnumeratedCount(out var count)
                ? ImmutableArray.CreateBuilder<T>(count)
                : ImmutableArray.CreateBuilder<T>();

            while (e.TryGetNext(out var current))
            {
                builder.Add(current);
            }

            return builder.ToImmutable();
        }
    }
}
```

Since the enumerator's state changes, you cannot call other methods after calling `TryGetNext`. Also, you cannot call `TryGetNext` after `TryCopyTo` or `TryGetSpan` returns `true`.

#### Custom Operator

Unlike `IEnumerable<T>`, you can't use `yield return`, so everything must be implemented by hand, making it more difficult than Consume operators. A simple `Select` implementation looks like this. For .NET 9 or later, `IValueEnumerator<T>` must be implemented as a `ref struct`. Also, the accessibility must be `public` or `internal`.

```csharp
public static class MyExtensions
{
    public static ValueEnumerable<SimpleSelect<TEnumerator, TSource, TResult>, TResult> SimpleSelect<TEnumerator, TSource, TResult>(this ValueEnumerable<TEnumerator, TSource> source, Func<TSource, TResult> selector)
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
    {
        // ValueEnumerable is only a wrapper so unwrapping to enumerator immediately is ok.
        // `new(new())` is `new ValueEnumerable(new SimpleSelect())`, wrap enumerator to ValueEnumerable.
        return new(new(source.Enumerator, selector));
    }
}

#if NET9_0_OR_GREATER
public ref struct
#else
public struct
#endif
    SimpleSelect<TEnumerator, TSource, TResult>(TEnumerator source, Func<TSource, TResult> selector) : IValueEnumerator<TResult>
        where TEnumerator : struct, IValueEnumerator<TSource>
#if NET9_0_OR_GREATER
        , allows ref struct
#endif
{
    TEnumerator source = source; // need to store source enumerator in field explicitly (ref struct limitation)

    public bool TryGetNonEnumeratedCount(out int count)
    {
        // If source count is not changed, return count.
        // Select count is same as source.
        return source.TryGetNonEnumeratedCount(out count);
    }

    public bool TryGetSpan(out ReadOnlySpan<TResult> span)
    {
        // For example, Take or Skip could return a Slice
        span = default;
        return false;
    }

    public bool TryCopyTo(scoped Span<TResult> destination, Index offset)
    {
        // TryCopyTo implementation needs to consider Index calculations, so it's quite complex.
        // Also, destination can be smaller than the source size.
        // Helper methods for calculations are available in ZLinq.Internal.EnumeratorHelper,
        // such as TryGetSliceRange, TryGetSlice, TryGetSliceRange, TryConsumeGetAt, etc.
        return false;
    }

    // This is the main body of the normal processing
    public bool TryGetNext(out TResult current)
    {
        while (source.TryGetNext(out var value))
        {
            current = selector(value);
            return true;
        }

        current = default!;
        return false;
    }

    public void Dispose()
    {
        // Always dispose the source
        source.Dispose();
    }
}
```

For `TryGetNonEnumeratedCount`, `TryGetSpan`, and `TryCopyTo`, it's fine to return `false` if implementation is difficult. If state is needed (for example, Take needs to keep track of the number of calls), place it in a field, but note that you should not initialize reference types or structs containing reference types in the constructor. This is because in method chains, Enumerators are passed by copy, so reference types would share references. If you need to hold reference types, they must be initialized when `TryGetNext` is first called.

Acknowledgement
---
Since the preview version release, we have received multiple ideas for fundamental interface revisions leading to performance improvements from [@Akeit0](https://github.com/Akeit0), and test and benchmark infrastructure from [@filzrev](https://github.com/filzrev). We are grateful for their many contributions.

License
---
This library is under MIT License.
