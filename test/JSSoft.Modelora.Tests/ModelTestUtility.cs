// <copyright file="ModelTestUtility.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Reflection;

namespace JSSoft.Modelora.Tests;

public static class ModelTestUtility
{
    public static void AssertProperty<T>(string propertyName, int index)
    {
        var property = typeof(T).GetProperty(propertyName);
        Assert.NotNull(property);
        var attribute = property.GetCustomAttribute<PropertyAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal(index, attribute.Index);
    }
}
