// <copyright file="ModelSerializerTestBase.Clone.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Tests;

public abstract partial class ModelSerializerTestBase<TData>
    where TData : notnull
{
    [Fact]
    public void Clone_ObjectClass_Test()
    {
        var source = new ObjectClass();
        var options = new ModelOptions();
        var cloned = Clone(source, options);
        Assert.Equal(source, cloned);
    }
}
