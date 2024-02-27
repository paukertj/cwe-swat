using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Text;

namespace StringConcatenationBenchmark
{
    [SimpleJob(RuntimeMoniker.Net70)]
    [SimpleJob(RuntimeMoniker.Net80)]
    public class Bechmarks
    {
        private const string Fragment = "Abc";

        [Params(10, 50, 100, 1000, 5000, 10000, 100000, 500000, 1000000)]
        public int Size { get; set; }

        private List<string> _fragments = new List<string>();

        [IterationSetup]
        public void Setup()
        {
            _fragments = new List<string>();

            for (int i = 0; i < Size; i++)
            {
                _fragments.Add(Fragment);
            }
        }

        [Benchmark]
        public string StringConcat()
        {
            string result = string.Empty;
            
            foreach (string fragment in _fragments)
            {
                result += fragment;
            }

            return result;
        }

        [Benchmark]
        public string StringBuilder()
        {
            var result = new StringBuilder();

            foreach (string fragment in _fragments)
            {
                result.Append(fragment);
            }

            return result.ToString();
        }

        [Benchmark]
        public string StringBuilderWithFixSize()
        {
            long size = _fragments.Count * Fragment.Length;
            
            int sbSize = size <= int.MaxValue 
                ? (int)size 
                : int.MaxValue;

            var result = new StringBuilder(sbSize);

            foreach (string fragment in _fragments)
            {
                result.Append(fragment);
            }

            return result.ToString();
        }
    }
}
