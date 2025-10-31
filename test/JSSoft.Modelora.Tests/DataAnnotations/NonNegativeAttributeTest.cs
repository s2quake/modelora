// <copyright file="NonNegativeAttributeTest.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Reflection;
using JSSoft.Modelora.DataAnnotations;

namespace JSSoft.Modelora.Tests.DataAnnotations;

public sealed class NonNegativeAttributeTest
{
    [Fact]
    public void AttributeTest()
    {
        var attr = typeof(NonNegativeAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(AttributeTargets.Property, attr.ValidOn);
        Assert.False(attr.AllowMultiple);
    }

    [Fact]
    public void Compare()
    {
        var obj1 = new TestClass
        {
            Value1 = 1,
            Value2 = 1,
            Value3 = 1,
            Value4 = 1L,
            Value5 = 1f,
            Value6 = 1d,
            Value7 = 1m,
            Value8 = (BigInteger)1,
        };
        ModelAssert.DoseNotThrow(obj1);

        var obj2 = new TestClass
        {
            Value1 = 0,
            Value2 = 0,
            Value3 = 0,
            Value4 = 0L,
            Value5 = 0f,
            Value6 = 0d,
            Value7 = 0m,
            Value8 = (BigInteger)0,
        };
        ModelAssert.DoseNotThrow(obj2);
    }

    [Fact]
    public void Compare_To_Constant_Throw()
    {
        var propertyNames = new[]
        {
            nameof(TestClass.Value1),
            nameof(TestClass.Value2),
            nameof(TestClass.Value3),
            nameof(TestClass.Value4),
            nameof(TestClass.Value5),
            nameof(TestClass.Value6),
            nameof(TestClass.Value7),
            nameof(TestClass.Value8),
        };

        var obj1 = new TestClass
        {
            Value1 = -1,
            Value2 = -1,
            Value3 = -1,
            Value4 = -1L,
            Value5 = -1f,
            Value6 = -1d,
            Value7 = -1m,
            Value8 = (BigInteger)(-1),
        };
        ModelAssert.ThrowsMany(obj1, propertyNames);
    }

    [Fact]
    public void UnsupportedType_Throw()
    {
        var obj1 = new InvalidTestClass();
        ModelAssert.Throws(obj1, nameof(InvalidTestClass.Value1));
    }

    private sealed record class TestClass
    {
        [NonNegative]
        public sbyte Value1 { get; init; }

        [NonNegative]
        public short Value2 { get; init; }

        [NonNegative]
        public int Value3 { get; init; }

        [NonNegative]
        public long Value4 { get; init; }

        [NonNegative]
        public float Value5 { get; init; }

        [NonNegative]
        public double Value6 { get; init; }

        [NonNegative]
        public decimal Value7 { get; init; }

        [NonNegative]
        public BigInteger Value8 { get; init; }
    }

    private sealed record class InvalidTestClass
    {
        [NonNegative]
        public uint Value1 { get; init; }
    }
}
