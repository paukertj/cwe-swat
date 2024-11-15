Many times I have encountered with statement like the following:

Never return `null` collections!
```
public IEnumerable<TransactionDto> GetTransactions(Guid userId)
{
    Transactions[] transactions = DatabaseCtx.Transactions.ForUser(userId);

    if (transactions.Any())
    {
        return null;
    }

    return transactions.ToDto();
}
```
Do this instead!
```
public IEnumerable<TransactionDto> GetTransactions(Guid userId)
{
    Transactions[] transactions = DatabaseCtx.Transactions.ForUser(userId);

    if (transactions.Any())
    {
        return Enumerable.Empty<TransactionDto>();
    }

    return transactions.ToDto();
}
```

The motivation behind this approach is obvious: when you call `GetTransactions`, you can safely work with the result without needing to check if it is `null`. I personally agree with this motivation; however, it's important to remember that such an advantage is not always "free" — it depends on how things are implemented.

There are multiple ways to return an "empty" collection, each with a different memory footprint.

In the case of `Enumerable.Empty<TransactionDto>()`, there is actually no [heap](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals#the-managed-heap) footprint. This is because an empty enumerable is represented by [class that has nothing enumerate](https://github.com/dotnet/runtime/blob/main/src/coreclr/nativeaot/Common/src/System/Collections/Generic/Empty.cs#L27). However, you must be cautious with how you work with it because you can easily introduce additional footprint through later enumeration, such as by calling [`ToList()`](https://github.com/microsoft/referencesource/blob/master/System.Core/System/Linq/Enumerable.cs#L329) to inspect the [`Count`](https://github.com/microsoft/referencesource/blob/master/mscorlib/system/collections/generic/list.cs#L137) property. If you just want to ensure, that result is empty, you can use [`Any()`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq/src/System/Linq/AnyAll.cs#L11) or  [`Count()`](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq/src/System/Linq/Count.cs#L11) and check, if the result is `0`. Both options will have zero memory footprint on heap in the case of empty `IEnumerable`.

This brings us to the second option: returning an empty list, such as `new List<TransactionDto>()`. Here, it’s easy to check the [`Count`](https://github.com/microsoft/referencesource/blob/master/mscorlib/system/collections/generic/list.cs#L137) property, but returning an empty list will allocate a small amount of space on the heap, which will eventually need garbage collection.

The last option is to return an empty array, like `Array.Empty<TransactionDto>`. This [implementation](https://github.com/dotnet/runtime/blob/main/src/libraries/System.Private.CoreLib/src/System/Array.cs#L1065) will allocate memory only once per application lifetime and then reuse it. In this case, you can check the `Length` without any additional memory footprint.

Lets check it in numbers, in benchmark `T as int`!
```
| Method                   | Size   | Gen0     | Allocated |
|------------------------- |------- |---------:|----------:|
| null                     | 1000   | -        | -         |
| null                     | 10000  | -        | -         |
| null                     | 100000 | -        | -         |
| Enumerable.Empty<T>()    | 1000   | -        | -         |
| Enumerable.Empty<T>()    | 10000  | -        | -         |
| Enumerable.Empty<T>()    | 100000 | -        | -         |
| new List<T>()            | 1000   |   2.5482 |   32000 B |
| new List<T>()            | 10000  |  25.4822 |  320000 B |
| new List<T>()            | 100000 | 254.8828 | 3200002 B |
| Array.Empty<T>()         | 1000   | -        | -         |
| Array.Empty<T>()         | 10000  | -        | -         |
| Array.Empty<T>()         | 100000 | -        | -         |
```
Gen0 referring to [Generation 0](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/fundamentals#generations) in Garbage Collector. [Read how to interpret the values.](https://adamsitnik.com/the-new-Memory-Diagnoser/#how-to-read-the-results)