using BenchmarkDotNet.Running;

namespace Modelora.Benchmarks;

public static class Program
{
    public static void Main(string[] args)
    {
        // Optional one-shot size preview before running the full benchmark
        SizePreview.Print();

        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly)
                         .Run(args);
    }
}