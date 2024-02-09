# Performance of LINQ `.First()` method

In many cases we seen use of LINQ method `.First(x => ...)` or `.Single(x => ...)` with condition inside `foreach` loop. This is not efficient, as it can iterate through entire collection, contrary to `.First()` or `.Single()` without arguments, which will stop at the first match. 

For example this code have quadratic complexity:

```csharp
foreach (var book in books)
{
    var author = authors.First(a => a.Id == book.Author);
    book.AuthorName = author.Name;
}
```

Better is to use `.Join()` method, which will join two collections based on the key:

```csharp
foreach (var (book, author) in books.Join(authors, book => book.Author, author => author.Id, Tuple.Create))
{
    book.AuthorName = author.Name;
}
```

To illustrate the performance difference, I did simple benchmark:
![First and Join performance comparison](2024-02-13_JIV_First-Performance-chart.svg)

`.Join()` method is [using `Lookup` internally](https://source.dot.net/#System.Linq/System/Linq/Join.cs,48) and thus have linear complexity. There is some overhead, but after collection size of around 25 elements, difference in time grows quickly.

You can look at benchmark code with few more variants [here](https://github.com/paukertj/cwe-swat/tree/main/benchmarks/ForeachFirstBenchmark).

> Found a bug or have additional questions? Let me know in the comments! I created this post on behalf of the CWE [**SWAT Workgroup**](https://wiki.ciklum.net/display/CGNA/SWAT+Workgroup). You can reach me and other group members at swat@ciklum.com.