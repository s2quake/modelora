// <copyright file="ModelSerializerTestBase.ScalarValue.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

#pragma warning disable SA1414 // Tuple types in signatures should have element names

namespace JSSoft.Modelora.Tests;

public abstract partial class ModelSerializerTestBase<TData>
{
    [Fact]
    public void ScalarValueDefaultProperty_SerializeAndDeserialize_Test()
    {
        var expectedObject = new RecordClassWithScalarValue();
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<RecordClassWithScalarValue>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [ClassData(typeof(RandomSeedsData))]
    public void ScalarValueProperty_SerializeAndDeserialize_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new RecordClassWithScalarValue(random);
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<RecordClassWithScalarValue>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [ClassData(typeof(RandomSeedsData))]
    public void ScalarValue_SerializeAndDeserialize_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new RecordClassWithScalarValue(random);
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

[Model("Libplanet_Serialization_Tests_ModelSerializerTest_RecordClassWithScalarValue", Version = 1)]
public sealed record class RecordClassWithScalarValue : IEquatable<RecordClassWithScalarValue>
{
    public RecordClassWithScalarValue()
    {
    }

    public RecordClassWithScalarValue(Random random)
    {
        StringClass = new StringScalarClass(R.String(random));
        BooleanClass = new BooleanScalarClass(R.Boolean(random));
        Int32Class = new Int32ScalarClass(R.Int32(random));
        Int64Class = new Int64ScalarClass(R.Int64(random));
        HexClass = new HexScalarClass(R.Array(random, R.Byte));
        StringGenericClass = new GenericScalarClass<string>(R.String(random));
        Int32GenericClass = new GenericScalarClass<int>(R.String(random));
        StringStruct = new StringScalarStruct(R.String(random));
        BooleanStruct = new BooleanScalarStruct(R.Boolean(random));
        Int32Struct = new Int32ScalarStruct(R.Int32(random));
        Int64Struct = new Int64ScalarStruct(R.Int64(random));
        HexStruct = new HexScalarStruct(R.Array(random, R.Byte));
        StringGenericStruct = new GenericScalarStruct<string>(R.String(random));
        Int32GenericStruct = new GenericScalarStruct<int>(R.String(random));
    }

    [Property(0)]
    public StringScalarClass StringClass { get; init; } = new(string.Empty);

    [Property(1)]
    public BooleanScalarClass BooleanClass { get; init; } = new(false);

    [Property(2)]
    public Int32ScalarClass Int32Class { get; init; } = new(0);

    [Property(3)]
    public Int64ScalarClass Int64Class { get; init; } = new(0L);

    [Property(4)]
    public HexScalarClass HexClass { get; init; } = new([]);

    [Property(5)]
    public GenericScalarClass<string> StringGenericClass { get; init; } = new(string.Empty);

    [Property(6)]
    public GenericScalarClass<int> Int32GenericClass { get; init; } = new(string.Empty);

    [Property(7)]
    public StringScalarStruct StringStruct { get; init; }

    [Property(8)]
    public BooleanScalarStruct BooleanStruct { get; init; }

    [Property(9)]
    public Int32ScalarStruct Int32Struct { get; init; }

    [Property(10)]
    public Int64ScalarStruct Int64Struct { get; init; }

    [Property(11)]
    public HexScalarStruct HexStruct { get; init; }

    [Property(12)]
    public GenericScalarStruct<string> StringGenericStruct { get; init; }

    [Property(13)]
    public GenericScalarStruct<int> Int32GenericStruct { get; init; }

    public bool Equals(RecordClassWithScalarValue? other) => ModelResolver.Equals(this, other);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}

[ModelScalar("Libplanet_Serialization_Tests_ModelSerializerTest_StringScalarClass", Kind = ModelScalarKind.String)]
public sealed record class StringScalarClass
{
    public string Value { get; }

    public StringScalarClass(string value)
    {
        Value = value;
    }

    internal static StringScalarClass FromScalarValue(IServiceProvider serviceProvider, string value) => new(value);

    internal string ToScalarValue() => Value;
}

[ModelScalar("Libplanet_Serialization_Tests_ModelSerializerTest_StringScalarStruct", Kind = ModelScalarKind.String)]
public readonly record struct StringScalarStruct
{
    public string Value { get; }

    public StringScalarStruct(string value)
    {
        Value = value;
    }

    internal static StringScalarStruct FromScalarValue(IServiceProvider serviceProvider, string value) => new(value);

    internal string ToScalarValue() => Value;
}

[ModelScalar("Libplanet_Serialization_Tests_ModelSerializerTest_BooleanScalarClass", Kind = ModelScalarKind.Boolean)]
public sealed record class BooleanScalarClass
{
    public bool Value { get; }

    public BooleanScalarClass(bool value)
    {
        Value = value;
    }

    internal static BooleanScalarClass FromScalarValue(IServiceProvider serviceProvider, bool value) => new(value);

    internal bool ToScalarValue() => Value;
}

[ModelScalar("Libplanet_Serialization_Tests_ModelSerializerTest_BooleanScalarStruct", Kind = ModelScalarKind.Boolean)]
public readonly record struct BooleanScalarStruct
{
    public bool Value { get; }

    public BooleanScalarStruct(bool value)
    {
        Value = value;
    }

    internal static BooleanScalarStruct FromScalarValue(IServiceProvider serviceProvider, bool value) => new(value);

    internal bool ToScalarValue() => Value;
}

[ModelScalar("Libplanet_Serialization_Tests_ModelSerializerTest_Int32ScalarClass", Kind = ModelScalarKind.Int32)]
public sealed record class Int32ScalarClass
{
    public int Value { get; }

    public Int32ScalarClass(int value)
    {
        Value = value;
    }

    internal static Int32ScalarClass FromScalarValue(IServiceProvider serviceProvider, int value) => new(value);

    internal int ToScalarValue() => Value;
}

[ModelScalar("Libplanet_Serialization_Tests_ModelSerializerTest_Int32ScalarStruct", Kind = ModelScalarKind.Int32)]
public readonly record struct Int32ScalarStruct
{
    public int Value { get; }

    public Int32ScalarStruct(int value)
    {
        Value = value;
    }

    internal static Int32ScalarStruct FromScalarValue(IServiceProvider serviceProvider, int value) => new(value);

    internal int ToScalarValue() => Value;
}

[ModelScalar("Libplanet_Serialization_Tests_ModelSerializerTest_Int64ScalarClass", Kind = ModelScalarKind.Int64)]
public sealed record class Int64ScalarClass
{
    public long Value { get; }

    public Int64ScalarClass(long value)
    {
        Value = value;
    }

    internal static Int64ScalarClass FromScalarValue(IServiceProvider serviceProvider, long value) => new(value);

    internal long ToScalarValue() => Value;
}

[ModelScalar("Libplanet_Serialization_Tests_ModelSerializerTest_Int64ScalarStruct", Kind = ModelScalarKind.Int64)]
public readonly record struct Int64ScalarStruct
{
    public long Value { get; }

    public Int64ScalarStruct(long value)
    {
        Value = value;
    }

    internal static Int64ScalarStruct FromScalarValue(IServiceProvider serviceProvider, long value) => new(value);

    internal long ToScalarValue() => Value;
}

[ModelScalar("Libplanet_Serialization_Tests_ModelSerializerTest_HexScalarClass", Kind = ModelScalarKind.Hex)]
public sealed record class HexScalarClass : IEquatable<HexScalarClass>
{
    public byte[] Value { get; }

    public HexScalarClass(byte[] value)
    {
        Value = value;
    }

    internal static HexScalarClass FromScalarValue(IServiceProvider serviceProvider, byte[] value) => new(value);

    internal byte[] ToScalarValue() => Value;

    public bool Equals(HexScalarClass? other) => other is not null && Value.SequenceEqual(other.Value);

    public override int GetHashCode() => Value.GetHashCode();
}

[ModelScalar("Libplanet_Serialization_Tests_ModelSerializerTest_HexScalarStruct", Kind = ModelScalarKind.Hex)]
public readonly record struct HexScalarStruct : IEquatable<HexScalarStruct>
{
    public byte[] Value { get; }

    public HexScalarStruct(byte[] value)
    {
        Value = value;
    }

    internal static HexScalarStruct FromScalarValue(IServiceProvider serviceProvider, byte[] value) => new(value);

    internal byte[] ToScalarValue() => Value;

    public bool Equals(HexScalarStruct other)
    {
        if (other.Value == other.Value)
        {
            return true;
        }

        if (other.Value is null || Value is null)
        {
            return false;
        }

        return Value.SequenceEqual(other.Value);
    }

    public override int GetHashCode() => Value.GetHashCode();
}

[ModelScalar(
    "Libplanet_Serialization_Tests_ModelSerializerTest_GenericScalarClass<>",
    Kind = ModelScalarKind.String)]
public sealed record class GenericScalarClass<T>(string Value)
{
    public string Value { get; } = Value;

    internal static GenericScalarClass<T> FromScalarValue(IServiceProvider serviceProvider, string value) => new(value);

    internal string ToScalarValue() => Value;
}

[ModelScalar(
    "Libplanet_Serialization_Tests_ModelSerializerTest_GenericScalarStruct<>",
    Kind = ModelScalarKind.String)]
public readonly record struct GenericScalarStruct<T>(string Value)
{
    public string Value { get; } = Value;

    internal static GenericScalarStruct<T> FromScalarValue(IServiceProvider serviceProvider, string value) => new(value);

    internal string ToScalarValue() => Value;
}
