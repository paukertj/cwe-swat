# Performance of LINQ `First` and `Single` methods

In many cases we seen use of LINQ method [.First(x => ...)](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.first?view=net-8.0#system-linq-enumerable-first-1(system-collections-generic-ienumerable((-0))-system-func((-0-system-boolean)))) or [.Single(x => ...)](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.single?view=net-8.0#system-linq-enumerable-single-1(system-collections-generic-ienumerable((-0)))) with condition inside foreach loop. This is not efficient, as it can iterate through entire collection, contrary to [.First()](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.first?view=net-8.0#system-linq-enumerable-first-1(system-collections-generic-ienumerable((-0)))) or [.Single()](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.single?view=net-8.0#system-linq-enumerable-single-1(system-collections-generic-ienumerable((-0))-system-func((-0-system-boolean)))) without arguments, which will visit only the first item for `First()`, or first two in case of `Single()`.

For example this code have quadratic complexity:

```csharp
foreach (var book in books)
{
    var author = authors.First(a => a.Id == book.Author);
    book.AuthorName = author.Name;
}
```

Better is to use [.Join()](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.join) method, which will join two collections based on the key:

```csharp
foreach (var (book, author) in books.Join(authors, book => book.Author, author => author.Id, Tuple.Create))
{
    book.AuthorName = author.Name;
}
```

To illustrate the performance difference, I did simple benchmark:
![First and Join performance comparison](2024-02-13_JIV_First-Performance-chart.svg)

`.Join()` method is [internally using](https://source.dot.net/#System.Linq/System/Linq/Join.cs,48) [`Lookup`](https://learn.microsoft.com/en-us/dotnet/api/system.linq.lookup-2) and thus have linear complexity. There is some overhead, but after collection size of around 25 elements, difference in time grows quickly.

You can look at [benchmark code with few more variants](https://github.com/paukertj/cwe-swat/tree/main/benchmarks/ForeachFirstBenchmark).

> Found a bug or have additional questions? Let me know in the comments! I created this post on behalf of the CWE [**SWAT Workgroup**](https://wiki.ciklum.net/display/CGNA/SWAT+Workgroup). You can reach me and other group members at swat@ciklum.com.