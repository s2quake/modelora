// <copyright file="ImmutableArrayModelConverterTests.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Tests.DynamicConverters;

public sealed class ImmutableArrayModelConverterTests
{
    [Fact]
    public void Equals_Test()
    {
        ImmutableArray<int> items1 = [1, 2, 3];
        ImmutableArray<int> items2 = [1, 2, 3];
        var converter = (IModelComparer)ModelResolver.GetConverter(typeof(ImmutableArray<int>));
        Assert.True(converter.Equals(items1, items2, typeof(ImmutableArray<int>)));
        Assert.False(converter.Equals(0, items2, typeof(ImmutableArray<int>)));
        Assert.False(converter.Equals(items1, 0, typeof(ImmutableArray<int>)));

        Assert.False(converter.Equals(items1, default(ImmutableArray<int>), typeof(ImmutableArray<int>)));
        Assert.False(converter.Equals(default(ImmutableArray<int>), items2, typeof(ImmutableArray<int>)));
    }

    [Fact]
    public void GetHashCode_Test()
    {
        ImmutableArray<int> items = [1, 2, 3];
        var converter = (IModelComparer)ModelResolver.GetConverter(typeof(ImmutableArray<int>));
        var hash = converter.GetHashCode(items, typeof(ImmutableArray<int>));
        Assert.NotEqual(0, hash);
        Assert.Equal(0, converter.GetHashCode(default(ImmutableArray<int>), typeof(ImmutableArray<int>)));

        Assert.Throws<NotSupportedException>(
            () => converter.GetHashCode(items, typeof(int)));
    }
}
