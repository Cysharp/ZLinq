using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ZLinq;

namespace Benchmark;

public partial class LinqPerfBenchmarks
{
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class CountBy00
    {
        const int IterationsCountBy00 = 1000000;
        List<Product> TestData = default!;

        [GlobalSetup]
        public void Setup() => TestData = Product.GetProductList();

#if NET9_0_OR_GREATER
        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        [BenchmarkCategory(Categories.Filters.NET9_0_OR_GREATER)]
        public bool Linq_CountBy_Count()
        {
            List<Product> products = TestData;
            int count = 0;
            for (int i = 0; i < IterationsCountBy00; i++)
            {
                count += products
                    .CountBy(p => p.Category)
                    .Count();
            }

            return (count == 5 * IterationsCountBy00);
        }

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        [BenchmarkCategory(Categories.Filters.NET9_0_OR_GREATER)]
        public bool Linq_AggregateBy_Count()
        {
            List<Product> products = TestData;
            int count = 0;
            for (int i = 0; i < IterationsCountBy00; i++)
            {
                count += products
                    .AggregateBy(p => p.Category, 0, (count, _) => ++count)
                    .Count();
            }

            return (count == 5 * IterationsCountBy00);
        }
#endif

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        public bool Linq_GroupBy_ToDictionary_Count()
        {
            List<Product> products = TestData;
            int count = 0;
            for (int i = 0; i < IterationsCountBy00; i++)
            {
                count += products
                    .GroupBy(p => p.Category)
                    .ToDictionary(c => c, g => g.Count())
                    .Count();
            }

            return (count == 5 * IterationsCountBy00);
        }

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        public bool Linq_ToLookup_ToDictionary_Count()
        {
            List<Product> products = TestData;
            int count = 0;
            for (int i = 0; i < IterationsCountBy00; i++)
            {
                count += products
                    .ToLookup(p => p.Category)
                    .ToDictionary(c => c, g => g.Count())
                    .Count();
            }

            return (count == 5 * IterationsCountBy00);
        }

#if NET9_0_OR_GREATER
        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        [BenchmarkCategory(Categories.Filters.NET9_0_OR_GREATER)]
        public bool ZLinq_CountBy_Count()
        {
            List<Product> products = TestData;
            int count = 0;
            for (int i = 0; i < IterationsCountBy00; i++)
            {
                count += products
                    .AsValueEnumerable()
                    .CountBy(p => p.Category)
                    .Count();
            }

            return (count == 5 * IterationsCountBy00);
        }

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        [BenchmarkCategory(Categories.Filters.NET9_0_OR_GREATER)]
        public bool ZLinq_AggregateBy_Count()
        {
            List<Product> products = TestData;
            int count = 0;
            for (int i = 0; i < IterationsCountBy00; i++)
            {
                count += products
                    .AsValueEnumerable()
                    .AggregateBy(p => p.Category, 0, (count, _) => ++count)
                    .Count();
            }

            return (count == 5 * IterationsCountBy00);
        }
#endif

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        public bool ZLinq_GroupBy_ToDictionary_Count()
        {
            List<Product> products = TestData;
            int count = 0;
            for (int i = 0; i < IterationsCountBy00; i++)
            {
                count += products
                    .AsValueEnumerable()
                    .GroupBy(p => p.Category)
                    .ToDictionary(c => c, g => g.Count())
                    .Count();
            }

            return (count == 5 * IterationsCountBy00);
        }

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        public bool ZLinq_ToLookup_ToDictionary_Count()
        {
            List<Product> products = TestData;
            int count = 0;
            for (int i = 0; i < IterationsCountBy00; i++)
            {
                count += products
                    .AsValueEnumerable()
                    .ToLookup(p => p.Category)
                    .ToDictionary(c => c, g => g.Count())
                    .Count();
            }

            return (count == 5 * IterationsCountBy00);
        }
    }
}
