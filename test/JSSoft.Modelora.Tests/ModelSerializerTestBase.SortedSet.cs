// <copyright file="ModelSerializerTestBase.SortedSet.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

#pragma warning disable SA1414 // Tuple types in signatures should have element names
using static JSSoft.Randora.RandomUtility;

namespace JSSoft.Modelora.Tests;

public abstract partial class ModelSerializerTestBase<TData>
{
    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    [InlineData(1849913649)]
    [ClassData(typeof(RandomSeedsData))]
    public void SortedSetProperty_SerializeAndDeserialize_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new RecordClassWithSortedSet(random);
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<RecordClassWithSortedSet>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [ClassData(typeof(RandomSeedsData))]
    public void SortedSet_SerializeAndDeserialize_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new RecordClassWithSortedSet(random);
        var properties = ModelResolver.GetProperties(expectedObject.GetType());
        foreach (var property in properties)
        {
            var expectedValue = property.GetValue(expectedObject);
            var serialized1 = Serialize(expectedValue);
            var actualValue1 = Deserialize(serialized1);
            Assert.Equal(expectedValue, actualValue1);

            var options = new ModelOptions { TypeInfoEmission = TypeInfoEmission.Never };
            var serialized2 = Serialize(expectedValue, options);
            var actualValue2 = Deserialize(serialized2, property.PropertyType, options);
            Assert.Equal(expectedValue, actualValue2);

            var serialized3 = Serialize(expectedValue, options);
            Assert.ThrowsAny<ModelException>(() => Deserialize(serialized3, options));
        }
    }
}

[Model("Libplanet_Serialization_Tests_ModelSerializerTest_RecordClassWithSortedSet", Version = 1)]
public sealed record class RecordClassWithSortedSet
    : IEquatable<RecordClassWithSortedSet>
{
    public RecordClassWithSortedSet()
    {
    }

    public RecordClassWithSortedSet(Random random)
    {
        Ints = SortedSet(random, Int32);
        Longs = SortedSet(random, Int64);
        BigIntegers = SortedSet(random, BigInteger);
        Enums = SortedSet(random, Enum<TestEnum>);
        Bools = SortedSet(random, Boolean);
        Strings = SortedSet(random, String);
        DateTimeOffsets = SortedSet(random, DateTimeOffset);
        TimeSpans = SortedSet(random, TimeSpan);
    }

    [Property(0)]
    public SortedSet<int> Ints { get; init; } = [1];

    [Property(1)]
    public SortedSet<long> Longs { get; init; } = [];

    [Property(2)]
    public SortedSet<BigInteger> BigIntegers { get; init; } = [];

    [Property(3)]
    public SortedSet<TestEnum> Enums { get; init; } = [];

    [Property(4)]
    public SortedSet<bool> Bools { get; init; } = [];

    [Property(5)]
    public SortedSet<string> Strings { get; init; } = [];

    [Property(6)]
    public SortedSet<DateTimeOffset> DateTimeOffsets { get; init; } = [];

    [Property(7)]
    public SortedSet<TimeSpan> TimeSpans { get; init; } = [];

    public bool Equals(RecordClassWithSortedSet? other) => ModelResolver.Equals(this, other);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}
