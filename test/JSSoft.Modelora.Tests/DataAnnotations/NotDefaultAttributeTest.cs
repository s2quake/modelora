// <copyright file="NotDefaultAttributeTest.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Reflection;
using JSSoft.Modelora.DataAnnotations;

namespace JSSoft.Modelora.Tests.DataAnnotations;

public sealed class NotDefaultAttributeTest
{
    [Fact]
    public void AttributeTest()
    {
        var attr = typeof(NotDefaultAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(AttributeTargets.Property, attr.ValidOn);
        Assert.False(attr.AllowMultiple);
    }

    [Fact]
    public void Validate_Test()
    {
        var obj1 = new TestClass
        {
            Value1 = 1,
            Value2 = HexValue.Parse("a1b2c3d4e5f6789012345678901234567890abcd"),
            Value3 = [1, 2, 3],
        };
        ModelAssert.DoseNotThrow(obj1);
    }

    [Fact]
    public void Validate_Throw()
    {
        string[] propertyNames =
        [
            nameof(TestClass.Value1),
            nameof(TestClass.Value2),
            nameof(TestClass.Value3)
        ];

        var obj1 = new TestClass();
        ModelAssert.ThrowsMany(obj1, propertyNames);
    }

    [Fact]
    public void UnsupportedType_Throw()
    {
        string[] propertyNames =
        [
            nameof(InvalidTestClass.Value1),
            nameof(InvalidTestClass.Value2)
        ];
        var obj1 = new InvalidTestClass();
        ModelAssert.ThrowsMany(obj1, propertyNames);
    }

    private sealed record class TestClass
    {
        [NotDefault]
        public int Value1 { get; init; }

        [NotDefault]
        public HexValue Value2 { get; init; }

        [NotDefault]
        public ImmutableArray<int> Value3 { get; init; }
    }

    private sealed record class InvalidTestClass
    {
        [NotDefault]
        public int? Value1 { get; init; }

        [NotDefault]
        public object Value2 { get; init; } = new object();
    }
}
