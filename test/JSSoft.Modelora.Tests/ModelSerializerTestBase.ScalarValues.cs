// <copyright file="ModelSerializerTestBase.ScalarValues.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using static JSSoft.Randora.RandomUtility;

namespace JSSoft.Modelora.Tests;

public abstract partial class ModelSerializerTestBase<TData>
{
    [Fact]
    public void ScalarValuesClass_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ScalarValuesClass();
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<ScalarValuesClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    [ClassData(typeof(RandomSeedsData))]
    public void ScalarValuesClass_SerializeAndDeserialize_Seed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ScalarValuesClass(random);
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<ScalarValuesClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }
}

[Model("Libplanet_Serialization_Tests_ModelSerializerTest_ScalarValuesClass", Version = 1)]
public sealed class ScalarValuesClass : IEquatable<ScalarValuesClass>
{
    public ScalarValuesClass()
    {
    }

    public ScalarValuesClass(Random random)
    {
        Int32 = new Int32ScalarValue(Int32(random));
        Int64 = new Int64ScalarValue(Int64(random));
        String = new StringScalarValue(String(random));
        Boolean = new BooleanScalarValue(Boolean(random));
        Hex = new HexScalarValue(Array(random, Byte));
        HexInt32 = new HexScalarValue<int>(Int32(random));
        HexString = new HexScalarValue<string>(Word(random));
    }

    [Property(0)]
    public Int32ScalarValue Int32 { get; init; }

    [Property(1)]
    public Int64ScalarValue Int64 { get; init; }

    [Property(2)]
    public StringScalarValue String { get; init; }

    [Property(3)]
    public BooleanScalarValue Boolean { get; init; }

    [Property(4)]
    public HexScalarValue Hex { get; init; }

    [Property(5)]
    public HexScalarValue<int> HexInt32 { get; init; }

    [Property(6)]
    public HexScalarValue<string> HexString { get; init; }

    public bool Equals(ScalarValuesClass? other) => ModelResolver.Equals(this, other);

    public override bool Equals(object? obj) => Equals(obj as ScalarValuesClass);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}

public abstract partial class ModelSerializerTestBase<TData>
{
    [Fact]
    public void UnsupportedHexDBNullClass_Serialize_ShouldThrow()
    {
        var obj = new UnsupportedHexDBNullClass();
        Assert.ThrowsAny<ModelException>(() => Serialize(obj));
    }

    [Fact]
    public void HexScalarValueDBNull_Serialize_ShouldThrow()
    {
        HexScalarValue<DBNull> hexDBNull = default;
        Assert.ThrowsAny<ModelException>(() => Serialize(hexDBNull));
    }
}

[Model("Libplanet_Serialization_Tests_ModelSerializerTest_UnsupportedHexDBNullClass", Version = 1)]
public sealed class UnsupportedHexDBNullClass : IEquatable<UnsupportedHexDBNullClass>
{
    public UnsupportedHexDBNullClass()
    {
    }

    [Property(0)]
    public HexScalarValue<DBNull> HexLong { get; init; }

    public bool Equals(UnsupportedHexDBNullClass? other) => ModelResolver.Equals(this, other);

    public override bool Equals(object? obj) => Equals(obj as UnsupportedHexDBNullClass);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}
