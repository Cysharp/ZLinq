using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using ZLinq;

namespace Benchmark
{
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    public class CastOfType
    {
        private readonly Message[] _testArray = Enumerable
            .Range(0, 16)
            .Select<int, Message>(i => i % 2 == 0 ? new TextMessage() : new IntMessage())
            .ToArray();

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        public object ZLinqCastToArray()
        {
            return _testArray.AsValueEnumerable().Where(o => o.Type == MessageType.Text).Cast<TextMessage>().ToArray();
        }

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        public object LinqCastToArray()
        {
            return _testArray.Where(o => o.Type == MessageType.Text).Cast<TextMessage>().ToArray();
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(Categories.ZLinq)]
        public object ZLinqOfTypeToArray()
        {
            return _testArray.AsValueEnumerable().OfType<TextMessage>().ToArray();
        }

        [Benchmark(Baseline = true)]
        [BenchmarkCategory(Categories.LINQ)]
        public object LinqOfTypeToArray()
        {
            return _testArray.OfType<TextMessage>().ToArray();
        }

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        public object ZLinqWhereOnlyToArray()
        {

            return _testArray.AsValueEnumerable().Where(o => o.Type == MessageType.Text).ToArray();
        }

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        public object LinqWhereOnlyToArray()
        {
            return _testArray.Where(o => o.Type == MessageType.Text).ToArray();
        }

        [Benchmark]
        [BenchmarkCategory(Categories.ZLinq)]
        public void ZLinqWhereOnlyForeach()
        {
            foreach (var _ in _testArray.AsValueEnumerable().Where(o => o.Type == MessageType.Text)) { }
        }

        [Benchmark]
        [BenchmarkCategory(Categories.LINQ)]
        public void LinqWhereOnlyForeach()
        {
            foreach (var _ in _testArray.Where(o => o.Type == MessageType.Text)) { }
        }

        public enum MessageType : byte
        {
            Text,
            Int
        }

        public abstract class Message
        {
            public MessageType Type { get; }

            public Message(MessageType messageType)
            {
                Type = messageType;
            }
        }

        public sealed class TextMessage() : Message(MessageType.Text)
        {
            public string Msg => "123";

            public override string ToString()
            {
                return $"{Type}: {Msg}";
            }
        }

        public sealed class IntMessage() : Message(MessageType.Int)
        {
            public int Num => 1;

            public override string ToString()
            {
                return $"{Type}: {Num}";
            }
        }
    }
}
