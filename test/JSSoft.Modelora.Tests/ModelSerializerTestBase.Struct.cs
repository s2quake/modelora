// <copyright file="ModelSerializerTestBase.Struct.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using static JSSoft.Randora.RandomUtility;

namespace JSSoft.Modelora.Tests;

public abstract partial class ModelSerializerTestBase<TData>
{
    [Fact]
    public void ObjectStruct_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ObjectStruct();
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<ObjectStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    [ClassData(typeof(RandomSeedsData))]
    public void ObjectStruct_SerializeAndDeserialize_Seed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ObjectStruct(random);
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<ObjectStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void ArrayStruct_SerializeAndDeserialize_Test()
    {
        var expectedObject = new ArrayStruct();
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<ArrayStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    [ClassData(typeof(RandomSeedsData))]
    public void ArrayStruct_SerializeAndDeserialize_Seed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new ArrayStruct(random);
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<ArrayStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Fact]
    public void MixedStruct_SerializeAndDeserialize_Test()
    {
        var expectedObject = new MixedStruct();
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<MixedStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1074183504)]
    [ClassData(typeof(RandomSeedsData))]
    public void MixedStruct_SerializeAndDeserialize_Seed_Test(int seed)
    {
        var random = new Random(seed);
        var expectedObject = new MixedStruct(random);
        var serialized = Serialize(expectedObject);
        var actualObject = Deserialize<MixedStruct>(serialized)!;
        Assert.Equal(expectedObject, actualObject);
    }
}

[Model("Libplanet_Serialization_Tests_ModelSerializerTest_ObjectStruct", Version = 1)]
public readonly struct ObjectStruct : IEquatable<ObjectStruct>
{
    public ObjectStruct()
    {
    }

    public ObjectStruct(Random random)
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

    public static bool operator ==(ObjectStruct left, ObjectStruct right) => left.Equals(right);

    public static bool operator !=(ObjectStruct left, ObjectStruct right) => !(left == right);

    public bool Equals(ObjectStruct other) => ModelResolver.Equals(this, other);

    public override bool Equals(object? obj) => obj is ObjectStruct @struct && Equals(@struct);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}

[Model("Libplanet_Serialization_Tests_ModelSerializerTest_ArrayStruct", Version = 1)]
public readonly struct ArrayStruct : IEquatable<ArrayStruct>
{
    public ArrayStruct()
    {
    }

    public ArrayStruct(Random random)
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

    public static bool operator ==(ArrayStruct left, ArrayStruct right) => left.Equals(right);

    public static bool operator !=(ArrayStruct left, ArrayStruct right) => !(left == right);

    public bool Equals(ArrayStruct other) => ModelResolver.Equals(this, other);

    public override bool Equals(object? obj) => obj is ArrayStruct @struct && Equals(@struct);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}

[Model("Libplanet_Serialization_Tests_ModelSerializerTest_MixedStruct", Version = 1)]
public readonly struct MixedStruct : IEquatable<MixedStruct>
{
    public MixedStruct()
    {
        Object = new ObjectStruct();
    }

    public MixedStruct(Random random)
    {
        Object = new ObjectStruct(random);
        Objects = Array(random, random => new ObjectStruct(random));
    }

    [Property(0)]
    public ObjectStruct Object { get; init; }

    [Property(1)]
    public ObjectStruct[] Objects { get; init; } = [];

    public static bool operator ==(MixedStruct left, MixedStruct right) => left.Equals(right);

    public static bool operator !=(MixedStruct left, MixedStruct right) => !(left == right);

    public bool Equals(MixedStruct other) => ModelResolver.Equals(this, other);

    public override bool Equals(object? obj) => obj is MixedStruct @struct && Equals(@struct);

    public override int GetHashCode() => ModelResolver.GetHashCode(this);
}
