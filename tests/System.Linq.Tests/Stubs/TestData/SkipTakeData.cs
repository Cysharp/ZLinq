// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// Original code: https://github.com/dotnet/runtime/blob/v9.0.3/src/libraries/Common/tests/System/Linq/SkipTakeData.cs

namespace System.Linq.Tests;

public class SkipTakeData
{
    public static TheoryData<int[], int> EnumerableData()
    {
        IEnumerable<int> sourceCounts = [0, 1, 2, 3, 5, 8, 13, 55, 100, 250];

        IEnumerable<int> counts = [1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 100, 250, 500, int.MaxValue];
        counts = counts.Concat(counts.Select(c => -c)).Append(0).Append(int.MinValue);

        var items = from sourceCount in sourceCounts
                    let source = Enumerable.Range(0, sourceCount)
                    from count in counts
                    select new { source, count };

        return new(items.Select(x => (x.source.ToArray(), x.count)));
    }

    public static TheoryData<int> EvaluationBehaviorData()
    {
        return new(Enumerable.Range(-1, 15).Select(count => count).ToArray());
    }
}
