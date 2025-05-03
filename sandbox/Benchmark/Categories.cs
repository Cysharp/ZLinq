namespace Benchmark;

public static class Categories
{
    #pragma warning disable format 
    public const string LINQ = "LINQ";
    public const string ZLinq = "ZLinq";

    // Categories that is used to filter benchmarks.
    public static class Filters
    {
        public const string ZLinqOnly = "ZLinqOnly";

        public const string ZLINQ_1_2_0_OR_GREATER = Constants.DefineConstants.ZLINQ_1_2_0_OR_GREATER;
        public const string ZLINQ_1_3_1_OR_GREATER = Constants.DefineConstants.ZLINQ_1_3_1_OR_GREATER;
        public const string ZLINQ_1_4_0_OR_GREATER = Constants.DefineConstants.ZLINQ_1_4_0_OR_GREATER;

        public const string NET8_0_OR_GREATER = "NET8_0_OR_GREATER";
        public const string NET9_0_OR_GREATER = "NET9_0_OR_GREATER";
        public const string NET10_0_OR_GREATER = "NET10_0_OR_GREATER";

        /// <summary>
        /// Custom filter for `#if !USE_SYSTEM_LINQ || NET10_0_OR_GREATER`
        /// </summary>
        public const string SystemLinq_NET10_0_OR_GREATER = "SystemLinq_NET10_0_OR_GREATER";
    }

    public static class From
    {
        public const string Default                 = "Default";                         // FromArray
        public const string Array                   = "FromArray";                       // FromArray
        public const string List                    = "FromList";                        // FromList
                                                    
        public const string Enumerable              = "FromEnumerable";                  // FromEnumerable
        public const string EnumerableArray         = "FromEnumerable(Array)";           // FromEnumerable
        public const string EnumerableList          = "FromEnumerable(List)";            // FromEnumerable
        public const string EnumerableIReadOnlyList = "FromEnumerable(IReadOnlyList)";   // FromEnumerable
        public const string EnumerableIList         = "FromEnumerable(IList)";           // FromEnumerable

        public const string ReadOnlyMemory          = "FromReadOnlyMemory";              // FromMemory
        public const string ReadOnlySequence        = "FromReadOnlySequence";            // FromReadOnlySequence
        public const string ReadOnlyCollection      = "FromReadOnlyCollection";          // FromEnumerable

        public const string Dictionary              = "FromDictionary";                  // FromDictionary
        public const string Queue                   = "FromQueue";                       // FromQueue
        public const string Stack                   = "FromStack";                       // FromStack
        public const string LinkedList              = "FromLinkedList";                  // FromLinkedList
        public const string HashSet                 = "FromHashSet";                     // FromHashSet
        public const string ImmutableArray          = "FromImmutableArray";              // FromImmutableArray

// #if NET9_0_OR_GREATER
        public const string Span                    = "FromSpan";                        // FromSpan
        public const string ReadOnlySpan            = "FromReadOnlySpan";                // FromSpan
// #endif

    }

    public static class Methods
    {
        // Unary
        public const string Append                      = "Append";
        public const string Cast                        = "Cast";
        public const string Distinct                    = "Distinct";
        public const string DistinctBy                  = "DistinctBy";
        public const string DefaultIfEmpty              = "DefaultIfEmpty";
        public const string GroupBy                     = "GroupBy";
        public const string OfType                      = "OfType";
        public const string Order                       = "Order";
        public const string OrderBy                     = "OrderBy";
        public const string OrderByDescending           = "OrderByDescending";
        public const string OrderDescending             = "OrderDescending";
        public const string Prepend                     = "Prepend";
        public const string Reverse                     = "Reverse";
        public const string Select                      = "Select";
        public const string SelectMany                  = "SelectMany";
        public const string Shuffle                     = "Shuffle";
        public const string Skip                        = "Skip";
        public const string SkipWhile                   = "SkipWhile";
        public const string SkipLast                    = "SkipLast";
        public const string Take                        = "Take";
        public const string TakeLast                    = "TakeLast";
        public const string TakeWhile                   = "TakeWhile";
        public const string ThenBy                      = "ThenBy";
        public const string ThenByDescending            = "ThenByDescending";
        public const string Where                       = "Where";

        // Binary
        public const string Concat                      = "Concat";
        public const string Except                      = "Except";
        public const string GroupJoin                   = "GroupJoin";
        public const string Intersect                   = "Intersect";
        public const string Join                        = "Join";
        public const string LeftJoin                    = "LeftJoin";
        public const string RightJoin                   = "RightJoin";
        public const string Union                       = "Union";
        public const string Zip                         = "Zip";

        // Sinks
        public const string Aggregate                   = "Aggregate";
        public const string AggregateBy                 = "AggregateBy";
        public const string All                         = "All";
        public const string Average                     = "Average";
        public const string Any                         = "Any";
        public const string Contains                    = "Contains";
        public const string Count                       = "Count";
        public const string CountBy                     = "CountBy";
        public const string ElementAt                   = "ElementAt";
        public const string ElementAtOrDefault          = "ElementAtOrDefault";
        public const string First                       = "First";
        public const string FirstOrDefault              = "FirstOrDefault";
        public const string Last                        = "Last";
        public const string LastOrDefault               = "LastOrDefault";
        public const string LongCount                   = "LongCount";
        public const string Max                         = "Max";
        public const string Min                         = "Min";
        public const string SequenceEqual               = "SequenceEqual";
        public const string Single                      = "Single";
        public const string SingleOrDefault             = "SingleOrDefault";
        public const string Sum                         = "Sum";
        public const string ToArray                     = "ToArray";
        public const string ToDictionary                = "ToDictionary";
        public const string ToHashSet                   = "ToHashSet";
        public const string ToList                      = "ToList";
        public const string ToLookup                    = "ToLookup";

        // ImmutableCollection Sinks
        public const string ToImmutableArray            = "ToImmutableArray";
        public const string ToImmutableList             = "ToImmutableList";
        public const string ToImmutableDictionary       = "ToImmutableDictionary";
        public const string ToImmutableSortedDictionary = "ToImmutableSortedDictionary";
        public const string ToImmutableHashSet          = "ToImmutableHashSet";
        public const string ToImmutableSortedSet        = "ToImmutableSortedSet";

        // FrozenCollection Sinks
        public const string ToFrozenDictionary          = "ToFrozenDictionary";
        public const string ToFrozenSet                 = "ToFrozenSet";

        // Others
        public const string CopyTo                      = "CopyTo";
        public const string ToArrayPool                 = "ToArrayPool";
    }
#pragma warning restore format 
}
