// <copyright file="ModelSerializerTestBase.ValueTuple.cs" company="JSSoft">
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
    public void ValueTupleProperty_SerializeAndDeserialize_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new RecordClassWithValueTuple(random);
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<RecordClassWithValueTuple>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }
}

[Model("Libplanet_Serialization_Tests_ModelSerializerTest_RecordClassWithValueTuple", Version = 1)]
public sealed record class RecordClassWithValueTuple : IEquatable<RecordClassWithValueTuple>
{
    public RecordClassWithValueTuple()
    {
    }

    public RecordClassWithValueTuple(Random random)
    {
        Value1 = ValueTuple(random, Int32, Boolean);
        Value2 = NullableValueTuple(random, Int32, Boolean);
        Value3 = ValueTuple(random, NullableInt32, NullableBoolean);
        Value4 = NullableValueTuple(random, NullableInt32, NullableBoolean);
        Value5 = ImmutableArray(random, random => ValueTuple(random, Int32, Boolean));
        Value6 = NullableImmutableArray(random, random => ValueTuple(random, Int32, Boolean));
        Value7 = ImmutableArray(random, random => ValueTuple(random, NullableInt32, NullableBoolean));
        Value8 = NullableImmutableArray(random, random => ValueTuple(random, NullableInt32, NullableBoolean));
        Value9 = ImmutableArray(random, random => NullableValueTuple(random, Int32, Boolean));
        Value10 = NullableImmutableArray(random, random => NullableValueTuple(random, Int32, Boolean));
        Value11 = ImmutableArray(random, random => NullableValueTuple(random, NullableInt32, NullableBoolean));
        Value12 = NullableImmutableArray(random, random => NullableValueTuple(random, NullableInt32, NullableBoolean));
    }

    [Property(0)]
    public (int, bool) Value1 { get; init; }

    [Property(1)]
    public (int, bool)? Value2 { get; init; }

    [Property(2)]
    public (int?, bool?) Value3 { get; init; }

    [Property(3)]
    public (int?, bool?)? Value4 { get; init; }

    [Property(4)]
    public ImmutableArray<(int, bool)> Value5 { get; init; } = [];

    [Property(5)]
    public ImmutableArray<(int, bool)>? Value6 { get; init; } = [];

    [Property(6)]
    public ImmutableArray<(int?, bool?)> Value7 { get; init; } = [];

    [Property(7)]
    public ImmutableArray<(int?, bool?)>? Value8 { get; init; } = [];

    [Property(8)]
    public ImmutableArray<(int, bool)?> Value9 { get; init; } = [];

    [Property(9)]
    public ImmutableArray<(int, bool)?>? Value10 { get; init; } = [];

    [Property(10)]
    public ImmutableArray<(int?, bool?)?> Value11 { get; init; } = [];

    [Property(11)]
    public ImmutableArray<(int?, bool?)?>? Value12 { get; init; } = [];

    public bool Equals(RecordClassWithValueTuple? other) => ModelResolver.Equals(this, other);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}
