// <copyright file="Int32ScalarValue.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Tests;

[ModelScalar("int32_scalar_value", Kind = ModelScalarKind.Int32)]
public readonly partial record struct Int32ScalarValue(int Value)
    : IEquatable<Int32ScalarValue>
{
    public int ToScalarValue() => Value;

    public static Int32ScalarValue FromScalarValue(IServiceProvider serviceProvider, int value)
    {
        _ = serviceProvider.GetService(typeof(object));
        return new(value);
    }
}
