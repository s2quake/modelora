using JSSoft.Modelora;
using MessagePack;
using Modelora.Benchmarks.Models;

namespace Modelora.Benchmarks;

internal static class SizePreview
{
    public static void Print()
    {
        var small = SampleData.Create(seed: 1, count: 16);
        var medium = SampleData.Create(seed: 2, count: 128);
        var large = SampleData.Create(seed: 3, count: 1024);

        PrintRow("Small", small);
        PrintRow("Medium", medium);
        PrintRow("Large", large);

        static void PrintRow(string label, SampleData data)
        {
            var options = new ModelOptions
            {
                TypeInfoEmission = TypeInfoEmission.Never,
            };
            var bi = new BigInteger(100000123123123123);
            var modelBytes = ModelSerializer.Serialize(bi, options);
            var modelSize = modelBytes.Length;
            var mp = MessagePackSerializer.Serialize(bi);
            var mpBytes = mp.Length;

            Console.WriteLine($"[Size] {label,-6} | Modelora(bytes): {modelSize,8} | MessagePack: {mpBytes,8}");
        }
    }
}
