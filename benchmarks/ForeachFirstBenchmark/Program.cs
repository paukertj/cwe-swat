using System;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Author { get; set; }
    public string AuthorName { get; set; }
}

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class Pairing
{
    //[Params(100,1000)]
    //[Params(10,50,100,1000)]
    [Params(10,15,20,25,30,40,50,60,70,80,90,100)]
    public int N { get; set; }

    public Book[] books { get; set; }
    private Author[] authors { get; set; }
    private Dictionary<int, Author> authorDictionary { get; set; }

    [GlobalSetup]
    public void Setup() {
        books = Enumerable.Range(0, N).Select(i => new Book { Id = i, Title = $"Book {i}", Author = i }).ToArray();
        authors = Enumerable.Range(0, N).Select(i => new Author { Id = i, Name = $"Author {i}" }).ToArray();
        authorDictionary = authors.ToDictionary(a => a.Id);
    }

    [Benchmark(Baseline = true)]
    public void First()
    {
        foreach (var book in books)
        {
            var author = authors.First(a => a.Id == book.Author);
            book.AuthorName = author.Name;
        }
    }

    [Benchmark]
    public void Manual()
    {
        foreach (var book in books)
        {
            for (int i = 0; i < authors.Length; i++)
            {
                if (authors[i].Id == book.Author)
                {
                    book.AuthorName = authors[i].Name;
                    break;
                }
            }
        }
    }

    [Benchmark]
    public void Dict() {
        var authorDictionary = authors.ToDictionary(a => a.Id);

        foreach (var book in books)
        {
            var author = authorDictionary[book.Author];
            book.AuthorName = author.Name;
        }
    }

    [Benchmark]
    public void DictCached() {
        foreach (var book in books)
        {
            var author = authorDictionary[book.Author];
            book.AuthorName = author.Name;
        }
    }

    [Benchmark]
    public void Join() {
        foreach (var (book, author) in books.Join(authors, book => book.Author, author => author.Id, Tuple.Create))
        {
            book.AuthorName = author.Name;
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Pairing>();
    }
}