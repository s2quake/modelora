// <copyright file="NotEmptyAttributeTest.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections;
using System.Reflection;
using JSSoft.Modelora.DataAnnotations;

namespace JSSoft.Modelora.Tests.DataAnnotations;

public sealed class NotEmptyAttributeTest
{
    [Fact]
    public void AttributeTest()
    {
        var attr = typeof(NotEmptyAttribute).GetCustomAttribute<AttributeUsageAttribute>();
        Assert.NotNull(attr);
        Assert.Equal(AttributeTargets.Property, attr.ValidOn);
        Assert.False(attr.AllowMultiple);
    }

    [Fact]
    public void Validate_Test()
    {
        var obj1 = new TestClass
        {
            Value1 = [1],
            Value2 = [HexValue.Parse("a1b2c3d4e5f6789012345678901234567890abcd")],
            Value3 = [1, 2, 3],
            Value4 = [HexValue.Parse("a1b2c3d4e5f6789012345678901234567890abcd")],
            Value5 = Enumerable.Range(1, 3),
            Value6 = new TestArray { Items = [1, 2, 3] },
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
            nameof(TestClass.Value3),
            nameof(TestClass.Value4),
            nameof(TestClass.Value5),
            nameof(TestClass.Value6),
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
            nameof(InvalidTestClass.Value2),
            nameof(InvalidTestClass.Value3),
        ];
        var obj1 = new InvalidTestClass();
        ModelAssert.ThrowsMany(obj1, propertyNames);
    }

    private sealed record class TestClass
    {
        [NotEmpty]
        public int[] Value1 { get; init; } = [];

        [NotEmpty]
        public HexValue[] Value2 { get; init; } = [];

        [NotEmpty]
        public ImmutableArray<int> Value3 { get; init; } = [];

        [NotEmpty]
        public ImmutableList<HexValue> Value4 { get; init; } = [];

        [NotEmpty]
        public IEnumerable<int> Value5 { get; init; } = Enumerable.Repeat(0, 0);

        [NotEmpty]
        public TestArray Value6 { get; init; } = new TestArray { Items = [] };
    }

    private sealed record class InvalidTestClass
    {
        [NotEmpty]
        public int Value1 { get; init; }

        [NotEmpty]
        public ImmutableArray<int> Value2 { get; init; }

        [NotEmpty]
        public TestArray Value3 { get; init; }
    }

    private readonly struct TestArray : IEnumerable
    {
        public required int[] Items { get; init; }

        public readonly IEnumerator GetEnumerator()
        {
            foreach (var item in Items)
            {
                yield return item;
            }
        }
    }
}
