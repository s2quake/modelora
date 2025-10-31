// <copyright file="ModelSerializerTestBase.Tuple.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using static JSSoft.Randora.RandomUtility;

namespace JSSoft.Modelora.Tests;

public abstract partial class ModelSerializerTestBase<TData>
{
    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    [InlineData(1849913649)]
    [ClassData(typeof(RandomSeedsData))]
    public void TupleProperty_SerializeAndDeserialize_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new RecordClassWithTuple(random);
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<RecordClassWithTuple>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    [InlineData(1849913649)]
    [ClassData(typeof(RandomSeedsData))]
    public void TupleProperty_WithNoTypeInfo_SerializeAndDeserialize_Test(int seed)
    {
        var random = new Random(seed);
        var options = new ModelOptions { TypeInfoEmission = TypeInfoEmission.Never };
        var expectedObject = new RecordClassWithTuple(random);
        var serialized = Serialize(expectedObject, options);
        var actualObject = Deserialize<RecordClassWithTuple>(serialized, options)!;
        Assert.Equal(expectedObject, actualObject);
    }
}

[Model("Libplanet_Serialization_Tests_ModelSerializerTest_RecordClassWithTuple", Version = 1)]
public sealed record class RecordClassWithTuple : IEquatable<RecordClassWithTuple>
{
    public RecordClassWithTuple()
    {
    }

    public RecordClassWithTuple(Random random)
    {
        Value1 = Tuple(random, Int32, Boolean);
        Value2 = NullableTuple(random, Int32, Boolean);
        Value3 = Tuple(random, NullableInt32, NullableBoolean);
        Value4 = NullableTuple(random, NullableInt32, NullableBoolean);
        Value5 = ImmutableArray(random, random => Tuple(random, Int32, Boolean));
        Value6 = NullableImmutableArray(random, random => Tuple(random, Int32, Boolean));
        Value7 = ImmutableArray(random, random => Tuple(random, NullableInt32, NullableBoolean));
        Value8 = NullableImmutableArray(random, random => Tuple(random, NullableInt32, NullableBoolean));
        Value9 = ImmutableArray(random, random => NullableTuple(random, Int32, Boolean));
        Value10 = NullableImmutableArray(random, random => NullableTuple(random, Int32, Boolean));
        Value11 = ImmutableArray(random, random => NullableTuple(random, NullableInt32, NullableBoolean));
        Value12 = NullableImmutableArray(random, random => NullableTuple(random, NullableInt32, NullableBoolean));
    }

    [Property(0)]
    public Tuple<int, bool> Value1 { get; init; } = new(0, false);

    [Property(1)]
    public Tuple<int, bool>? Value2 { get; init; } = new(0, false);

    [Property(2)]
    public Tuple<int?, bool?> Value3 { get; init; } = new(null, null);

    [Property(3)]
    public Tuple<int?, bool?>? Value4 { get; init; } = new(null, null);

    [Property(4)]
    public ImmutableArray<Tuple<int, bool>> Value5 { get; init; } = [];

    [Property(5)]
    public ImmutableArray<Tuple<int, bool>>? Value6 { get; init; } = [];

    [Property(6)]
    public ImmutableArray<Tuple<int?, bool?>> Value7 { get; init; } = [];

    [Property(7)]
    public ImmutableArray<Tuple<int?, bool?>>? Value8 { get; init; } = [];

    [Property(8)]
    public ImmutableArray<Tuple<int, bool>?> Value9 { get; init; } = [];

    [Property(9)]
    public ImmutableArray<Tuple<int, bool>?>? Value10 { get; init; } = [];

    [Property(10)]
    public ImmutableArray<Tuple<int?, bool?>?> Value11 { get; init; } = [];

    [Property(11)]
    public ImmutableArray<Tuple<int?, bool?>?>? Value12 { get; init; } = [];

    public bool Equals(RecordClassWithTuple? other) => ModelResolver.Equals(this, other);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}
