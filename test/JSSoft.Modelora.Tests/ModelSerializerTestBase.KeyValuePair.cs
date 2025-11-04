// <copyright file="ModelSerializerTestBase.KeyValuePair.cs" company="JSSoft">
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
    public void KeyValuePairProperty_SerializeAndDeserialize_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new RecordClassWithKeyValuePair(random);
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<RecordClassWithKeyValuePair>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    [InlineData(1849913649)]
    [ClassData(typeof(RandomSeedsData))]
    public void KeyValuePairProperty_WithNoTypeInfo_SerializeAndDeserialize_Test(int seed)
    {
        var random = new Random(seed);
        var options = new ModelOptions { TypeInfoEmission = TypeInfoEmission.Never };
        var expectedObject = new RecordClassWithKeyValuePair(random);
        var serialized = Serialize(expectedObject, options);
        var actualObject = Deserialize<RecordClassWithKeyValuePair>(serialized, options)!;
        Assert.Equal(expectedObject, actualObject);
    }
}

[Model("JSSoft_Modelora_Tests_ModelSerializerTest_RecordClassWithKeyValuePair", Version = 1)]
public sealed record class RecordClassWithKeyValuePair : IEquatable<RecordClassWithKeyValuePair>
{
    public RecordClassWithKeyValuePair()
    {
    }

    public RecordClassWithKeyValuePair(Random random)
    {
        // Single values
        Value1 = new KeyValuePair<int, string>(Int32(random), String(random));
        Value2 = new KeyValuePair<int, string>(Int32(random), String(random));
        Value3 = new KeyValuePair<int, string?>(Int32(random), NullableObject(random, String));
        Value4 = Nullable(random, r => new KeyValuePair<int, string?>(Int32(r), NullableObject(r, String)));

        // Arrays (non-nullable element/value)
        Value5 = ImmutableArray(random, r => new KeyValuePair<int, string>(Int32(r), String(r)));
        Value6 = NullableImmutableArray(random, r => new KeyValuePair<int, string>(Int32(r), String(r)));

        // Arrays (nullable value part)
        Value7 = ImmutableArray(random, r => new KeyValuePair<int, string?>(Int32(r), NullableObject(r, String)));
        Value8 = NullableImmutableArray(random, r => new KeyValuePair<int, string?>(Int32(r), NullableObject(r, String)));

        // Arrays (nullable element)
        Value9 = ImmutableArray(random, r => Nullable(r, r => new KeyValuePair<int, string>(Int32(r), String(r))));
        Value10 = NullableImmutableArray(random, r => Nullable(r, r => new KeyValuePair<int, string>(Int32(r), String(r))));

        // Arrays (nullable element + nullable value)
        Value11 = ImmutableArray(random, r => Nullable(r, r => new KeyValuePair<int, string?>(Int32(r), NullableObject(r, String))));
        Value12 = NullableImmutableArray(random, r => Nullable(r, r => new KeyValuePair<int, string?>(Int32(r), NullableObject(r, String))));
    }

    [Property(0)]
    public KeyValuePair<int, string> Value1 { get; init; } = new(0, string.Empty);

    [Property(1)]
    public KeyValuePair<int, string>? Value2 { get; init; } = new(0, string.Empty);

    [Property(2)]
    public KeyValuePair<int, string?> Value3 { get; init; } = new(0, null);

    [Property(3)]
    public KeyValuePair<int, string?>? Value4 { get; init; } = new(0, null);

    [Property(4)]
    public ImmutableArray<KeyValuePair<int, string>> Value5 { get; init; } = [];

    [Property(5)]
    public ImmutableArray<KeyValuePair<int, string>>? Value6 { get; init; } = [];

    [Property(6)]
    public ImmutableArray<KeyValuePair<int, string?>> Value7 { get; init; } = [];

    [Property(7)]
    public ImmutableArray<KeyValuePair<int, string?>>? Value8 { get; init; } = [];

    [Property(8)]
    public ImmutableArray<KeyValuePair<int, string>?> Value9 { get; init; } = [];

    [Property(9)]
    public ImmutableArray<KeyValuePair<int, string>?>? Value10 { get; init; } = [];

    [Property(10)]
    public ImmutableArray<KeyValuePair<int, string?>?> Value11 { get; init; } = [];

    [Property(11)]
    public ImmutableArray<KeyValuePair<int, string?>?>? Value12 { get; init; } = [];

    public bool Equals(RecordClassWithKeyValuePair? other) => ModelResolver.Equals(this, other);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}
