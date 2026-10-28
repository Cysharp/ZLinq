namespace System;

public static class SkipReason
{
    /// <summary>
    /// Generic skip message.
    /// </summary>
    public const string NotCompatible = "There is no compatibility with ZLinq";

    /// <summary>
    /// Skip tests because it require too much CPU/memories.
    /// </summary>
    public const string RequiresTooMuchResource = "Test require memory/times to execute.";

    /// <summary>
    /// ZLinq using `ref struct` type. So following limitations are exists.
    /// https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/ref-struct
    /// </summary>
    public const string RefStruct = "There is no compatibility because ZLinq use `ref struct`";

    /// <summary>
    /// ZLinq's Enumerator indicate first element.
    /// </summary>
    public const string EnumeratorBehaviorDifference = "ZLinq's Enumerator property is not compatible with LINQ";

    /// <summary>
    /// ZLinq don't use ICollection::CopyTo() on ToArray()/ToList().
    /// </summary>
    public const string ICollectionCopyTo = "ZLinq don't use ICollection::CopyTo() when calling ToArray()/ToList()";

    /// <summary>
    /// ZLinq don't support Enumerator Reset() API.
    /// </summary>
    public const string EnumeratorReset = "ZLinq don't support Enumerator.Reset() API.";

    /// <summary>
    /// Custom enumerator is not used by ZLinq optimization.
    /// If target collection implements collection interfaces other than IEnumerable.
    /// </summary>
    public const string CustomEnumerator = "Custom enumerator is not used by ZLinq optimization.";

    /// <summary>
    /// ZLinq don't support CreateOrderedEnumerable API
    /// </summary>
    public const string CreateOrderedEnumerable = "There is no compatibility. And it's not expected be supported by ZLinq. " +
                                                  "See: https://github.com/Cysharp/ZLinq/issues/88";
    /// <summary>
    /// Dispose method is not called on some cases.
    /// </summary>
    public const string Issue0081 = "See: https://github.com/Cysharp/ZLinq/issues/81";

    /// <summary>
    /// ZLinq don't enumerate items on Count(), when data source support `TryGetNonEnumeratedCount`.
    /// </summary>
    public const string Issue0082 = "See: https://github.com/Cysharp/ZLinq/issues/82";

    /// <summary>
    /// ZLinq don't support optimization for IEnumerable[T]::Take(range).Skip()
    /// </summary>
    public const string Issue0090 = "ZLinq don't support IEnumerable<T>::Take(range).Skip() optimization. " +
                                    "See: https://github.com/Cysharp/ZLinq/issues/90";

    /// <summary>
    /// ZLinq don't support optimization for IEnumerable[T]::Min() that contains NaN.
    /// Skip these tests because it takes times to complete.
    /// </summary>
    public const string Issue0092 = "ZLinq don't support optimization for IEnumerable<T>::Min() that contains NaN. " +
                                    "See: https://github.com/Cysharp/ZLinq/issues/92";

    /// <summary>
    /// ZLinq don't support Last/LastOrDefault/TakeLast operation for IEnumerable data source.
    /// When using `Enumerable.Range`. It's recommended to use ValueEnumerable.Range
    /// Skip these tests because it takes times to complete.
    /// </summary>
    public const string Issue0094 = "ZLinq don't support Last/LastOrDefault/TakeLast operation for IEnumerable data source. " +
                                    "See: https://github.com/Cysharp/ZLinq/issues/94";

    /// <summary>
    /// ZLinq's query can't re-assign to variable when ValueEnumerable type is different.
    /// </summary>
    public const string Issue0096 = "ZLinq don't support reuse variables if ValueEnumerable type is different. " +
                                    "See: https://github.com/Cysharp/ZLinq/issues/96";

    // Dummy code.
    public const string ZLinq_IssueNNNN = "See: https://github.com/Cysharp/ZLinq/issues/{placeholder}";
}
