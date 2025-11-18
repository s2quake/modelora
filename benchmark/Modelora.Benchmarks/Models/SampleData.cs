using JSSoft.Modelora;
using MessagePack;

namespace Modelora.Benchmarks.Models;

[Model("Benchmark_SampleData", Version = 1)]
[MessagePackObject]
public sealed class SampleData : IEquatable<SampleData>
{
    [Property(0), Key(0)]
    public int Id { get; init; }

    [Property(1), Key(1)]
    public string Name { get; init; } = string.Empty;

    [Property(2), Key(2)]
    public int[] Scores { get; init; } = Array.Empty<int>();

    [Property(3), Key(3)]
    public DateTimeOffset When { get; init; } = DateTimeOffset.UnixEpoch;

    [Property(4), Key(4)]
    public BigInteger Big { get; init; }

    public static SampleData Create(int seed, int count)
    {
        var rnd = new Random(seed);
        return new SampleData
        {
            Id = rnd.Next(),
            Name = new string(Enumerable.Repeat("abcdefghijklmnopqrstuvwxyz", 1)
                                       .SelectMany(s => s)
                                       .OrderBy(_ => rnd.Next())
                                       .Take(16)
                                       .ToArray()),
            Scores = Enumerable.Range(0, count).Select(_ => rnd.Next()).ToArray(),
            When = DateTimeOffset.UtcNow,
            Big = new BigInteger(Guid.NewGuid().ToByteArray()),
        };
    }

    public bool Equals(SampleData? other)
        => other is not null
        && Id == other.Id
        && Name == other.Name
        && When.Equals(other.When)
        && Big.Equals(other.Big)
        && Scores.SequenceEqual(other.Scores);

    public override bool Equals(object? obj) => Equals(obj as SampleData);

    public override int GetHashCode() => HashCode.Combine(Id, Name, When, Big, Scores.Length);
}
