using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ZLinq;

namespace Benchmark;

public partial class LinqPerfBenchmarks
{
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class AggregateBy00
    {
        int IterationsAggregateBy00 = 1000000;
        List<Product> TestData = default!;

        [GlobalSetup]
        public void Setup() => TestData = Product.GetProductList();

#if NET9_0_OR_GREATER
        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        [BenchmarkCategory(Categories.Filters.NET9_0_OR_GREATER)]
        public bool Linq_AggregateBy_Sum()
        {
            List<Product> products = TestData;
            decimal sum = 0;
            for (int i = 0; i < IterationsAggregateBy00; i++)
            {
                sum += products
                    .AggregateBy(p => p.Category, decimal.Zero, (total, p) => total + p.UnitsInStock * p.UnitPrice)
                    .Sum(kvp => kvp.Value);
            }

            return (sum == 5 * IterationsAggregateBy00);
        }
#endif

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        public bool Linq_GroupBy_ToDictionary_Sum()
        {
            List<Product> products = TestData;
            decimal count = 0;
            for (int i = 0; i < IterationsAggregateBy00; i++)
            {
                count += products
                    .GroupBy(p => p.Category)
                    .ToDictionary(c => c, g => g.Aggregate(decimal.Zero, (total, p) => total + p.UnitsInStock * p.UnitPrice))
                    .Sum(kvp => kvp.Value);
            }

            return (count == 5 * IterationsAggregateBy00);
        }

#if NET9_0_OR_GREATER
        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        [BenchmarkCategory(Categories.Filters.NET9_0_OR_GREATER)]
        public bool ZLinq_AggregateBy_Sum()
        {
            List<Product> products = TestData;
            decimal sum = 0;
            for (int i = 0; i < IterationsAggregateBy00; i++)
            {
                sum += products
                    .AsValueEnumerable()
                    .AggregateBy(p => p.Category, decimal.Zero, (total, p) => total + p.UnitsInStock * p.UnitPrice)
                    .Sum(kvp => kvp.Value);
            }

            return (sum == 5 * IterationsAggregateBy00);
        }
#endif

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        public bool ZLinq_GroupBy_ToDictionary_Sum()
        {
            List<Product> products = TestData;
            decimal count = 0;
            for (int i = 0; i < IterationsAggregateBy00; i++)
            {
                count += products
                    .AsValueEnumerable()
                    .GroupBy(p => p.Category)
                    .ToDictionary(c => c, g => g.Aggregate(decimal.Zero, (total, p) => total + p.UnitsInStock * p.UnitPrice))
                    .Sum(kvp => kvp.Value);
            }

            return (count == 5 * IterationsAggregateBy00);
        }
    }
}
