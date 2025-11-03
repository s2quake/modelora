// <copyright file="Int32ScalarValue.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Tests;

[ModelScalar("int32_scalar_value", Kind = ModelScalarKind.Int32)]
public readonly partial record struct Int32ScalarValue(int Value)
    : IEquatable<Int32ScalarValue>, IComparable<Int32ScalarValue>, IComparable
{
    public int ToScalarValue() => Value;

    public static Int32ScalarValue FromScalarValue(IServiceProvider serviceProvider, int value) => new(value);

    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        Int32ScalarValue other => CompareTo(other),
        _ => throw new ArgumentException($"Argument {nameof(obj)} is not ${nameof(Int32ScalarValue)}.", nameof(obj)),
    };

    public int CompareTo(Int32ScalarValue other) => Value.CompareTo(other.Value);
}
