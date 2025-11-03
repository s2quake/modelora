// <copyright file="ArrayModelConverterTests.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using JSSoft.Modelora.DynamicConverters;

namespace JSSoft.Modelora.Tests.DynamicConverters;

public sealed class ArrayModelConverterTests
{
    [Fact]
    public void CanConvert_Detects_Array_Types()
    {
        Assert.True(ModelResolver.TryGetConverter(typeof(int[]), out var conv));
        Assert.True(conv.CanConvert(typeof(int[])));
        Assert.True(conv.CanConvert(typeof(string[])));
        Assert.True(conv.CanConvert(typeof(object[])));
        Assert.True(conv.CanConvert(typeof(Array)));

        Assert.False(conv.CanConvert(typeof(List<int>)));
        Assert.False(conv.CanConvert(typeof(int)));
    }

    [Fact]
    public void Equals_Test()
    {
        var items1 = new int[] { 1, 2, 3 };
        var items2 = new int[] { 1, 2, 3 };
        var items3 = new int[] { 1, 2, 4 };
        var converter = (IModelComparer)ModelResolver.GetConverter(typeof(int[]));
        Assert.True(converter.Equals(items1, items2, typeof(int[])));
        Assert.False(converter.Equals(0, items2, typeof(int[])));
        Assert.False(converter.Equals(items1, 0, typeof(int[])));

        Assert.False(converter.Equals(items1, items3, typeof(int[])));
    }

    [Fact]
    public void GenericTypeDefinition_ThrowTest()
    {
        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        var converter = ModelResolver.GetConverter(typeof(int[]));
        var propertyInfo = converter.GetType().GetProperty("GenericTypeDefinition", bindingFlags)!;
        var e = Assert.Throws<TargetInvocationException>(() => propertyInfo.GetValue(converter));
        Assert.Equal("UnreachableException", e.InnerException?.GetType().Name);
    }

    [Fact]
    public void IsDictionary_Test()
    {
        var converter = ModelResolver.GetConverter(typeof(int[]));
        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        var propertyInfo = converter.GetType().GetProperty("IsDictionary", bindingFlags)!;
        var value = propertyInfo.GetValue(converter);
        Assert.False((bool)value!);
    }

    [Fact]
    public void GetElementType_Test()
    {
        var converter = ModelResolver.GetConverter(typeof(int[]));
        var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
        var methodInfo = converter.GetType().GetMethod("GetElementType", bindingFlags)!;
        var elementType = methodInfo.Invoke(converter, [typeof(int[])]);
        Assert.Equal(typeof(int), elementType);

        var e = Assert.Throws<TargetInvocationException>(() => methodInfo.Invoke(converter, [typeof(int)]));
        Assert.IsType<ArgumentException>(e.InnerException);
    }
}
