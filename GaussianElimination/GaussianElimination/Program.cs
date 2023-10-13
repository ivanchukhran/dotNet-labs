// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Solution;

var maxThreads = 2;
var sizes = new[]
{
    16, 64, 128, 256, 512, 1024, 10000
};
var random = new Random();
foreach (var size in sizes)
{
    var a = Matrix.Random(size, size);
    var b = new double[size];
    for (var i = 0; i < size; i++)
    {
        b[i] = random.NextDouble();
    }
    Console.WriteLine($"Size: {size}");
    Console.WriteLine("Sequential");
    var watch = Stopwatch.StartNew();
    var (l, u) = LU.Decompose(a);
    watch.Stop();
    var decompositionTime = watch.ElapsedMilliseconds;
    Console.WriteLine($"Decomposition time: {decompositionTime} ms");
    watch = Stopwatch.StartNew();
    var solution = LU.Eliminate(b, l, u);
    watch.Stop();
    var solutionTime = watch.ElapsedMilliseconds;
    Console.WriteLine($"Solution time: {solutionTime} ms");
    Console.WriteLine($"Total time: {decompositionTime + solutionTime} ms");
    Console.WriteLine($"Parallel: {maxThreads} threads");
    watch = Stopwatch.StartNew();
    (l, u) = LU.DecomposeParallel(a,  maxThreads);
    watch.Stop();
    decompositionTime = watch.ElapsedMilliseconds;
    Console.WriteLine($"Decomposition time: {decompositionTime} ms");
    watch = Stopwatch.StartNew();
    solution = LU.EliminateParallel(b, l, u, maxThreads);
    watch.Stop();
    solutionTime = watch.ElapsedMilliseconds;
    Console.WriteLine($"Solution time: {solutionTime} ms");
    Console.WriteLine($"Total time: {decompositionTime + solutionTime} ms");
    
}