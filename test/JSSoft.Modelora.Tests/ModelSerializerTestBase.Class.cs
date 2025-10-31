// <copyright file="ModelSerializerTestBase.Class.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using static JSSoft.Randora.RandomUtility;

namespace JSSoft.Modelora.Tests;

public abstract partial class ModelSerializerTestBase<TData>
    where TData : notnull
{
    [Fact]
    public void ObjectClass_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ObjectClass();
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<ObjectClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    [ClassData(typeof(RandomSeedsData))]
    public void ObjectClass_SerializeAndDeserialize_Seed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ObjectClass(random);
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<ObjectClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void ArrayClass_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ArrayClass();
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<ArrayClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    [ClassData(typeof(RandomSeedsData))]
    public void ArrayClass_SerializeAndDeserialize_Seed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayClass(random);
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<ArrayClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void MixedClass_SerializeAndDeserialize_Test()
    {
        var expectedObject = new MixedClass();
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<MixedClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    [ClassData(typeof(RandomSeedsData))]
    public void MixedClass_SerializeAndDeserialize_Seed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new MixedClass(random);
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<MixedClass>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }
}

[Model("Libplanet_Serialization_Tests_ModelSerializerTest_ObjectClass", Version = 1)]
public sealed class ObjectClass : IEquatable<ObjectClass>
{
    public ObjectClass()
    {
    }

    public ObjectClass(Random random)
    {
        Int = Int32(random);
        Long = Int64(random);
        BigInteger = BigInteger(random);
        Enum = Enum<TestEnum>(random);
        Bool = Boolean(random);
        String = String(random);
        DateTimeOffset = DateTimeOffset(random);
        TimeSpan = TimeSpan(random);
        Byte = Byte(random);
    }

    [Property(0)]
    public int Int { get; init; }

    [Property(1)]
    public long Long { get; init; }

    [Property(2)]
    public BigInteger BigInteger { get; init; }

    [Property(3)]
    public TestEnum Enum { get; init; }

    [Property(4)]
    public bool Bool { get; init; }

    [Property(5)]
    public string String { get; init; } = string.Empty;

    [Property(6)]
    public DateTimeOffset DateTimeOffset { get; init; } = DateTimeOffset.UnixEpoch;

    [Property(7)]
    public TimeSpan TimeSpan { get; init; }

    [Property(8)]
    public byte Byte { get; init; }

    public bool Equals(ObjectClass? other) => ModelResolver.Equals(this, other);

    public override bool Equals(object? obj) => Equals(obj as ObjectClass);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}

[Model("Libplanet_Serialization_Tests_ModelSerializerTest_ArrayClass", Version = 1)]
public sealed class ArrayClass : IEquatable<ArrayClass>
{
    public ArrayClass()
    {
    }

    public ArrayClass(Random random)
    {
        Ints = Array(random, Int32);
        Longs = Array(random, Int64);
        BigIntegers = Array(random, BigInteger);
        Enums = Array(random, Enum<TestEnum>);
        Bools = Array(random, Boolean);
        Strings = Array(random, String);
        DateTimeOffsets = Array(random, DateTimeOffset);
        TimeSpans = Array(random, TimeSpan);
    }

    [Property(0)]
    public int[] Ints { get; init; } = [];

    [Property(1)]
    public long[] Longs { get; init; } = [];

    [Property(2)]
    public BigInteger[] BigIntegers { get; init; } = [];

    [Property(3)]
    public TestEnum[] Enums { get; init; } = [];

    [Property(4)]
    public bool[] Bools { get; init; } = [];

    [Property(5)]
    public string[] Strings { get; init; } = [];

    [Property(6)]
    public DateTimeOffset[] DateTimeOffsets { get; init; } = [];

    [Property(7)]
    public TimeSpan[] TimeSpans { get; init; } = [];

    public bool Equals(ArrayClass? other) => ModelResolver.Equals(this, other);

    public override bool Equals(object? obj) => Equals(obj as ArrayClass);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}

[Model("Libplanet_Serialization_Tests_ModelSerializerTest_MixedClass", Version = 1)]
public sealed class MixedClass : IEquatable<MixedClass>
{
    public MixedClass()
    {
        Object = new ObjectClass();
    }

    public MixedClass(Random random)
    {
        Object = new ObjectClass(random);
        Objects = Array(random, random => new ObjectClass(random));
    }

    [Property(0)]
    public ObjectClass Object { get; init; }

    [Property(1)]
    public ObjectClass[] Objects { get; init; } = [];

    public bool Equals(MixedClass? other) => ModelResolver.Equals(this, other);

    public override bool Equals(object? obj) => Equals(obj as MixedClass);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}
