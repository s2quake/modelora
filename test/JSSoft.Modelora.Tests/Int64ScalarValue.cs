// <copyright file="Int64ScalarValue.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Tests;

[ModelScalar("int64_scalar_value", Kind = ModelScalarKind.Int64)]
public readonly partial record struct Int64ScalarValue(long Value)
    : IEquatable<Int64ScalarValue>, IComparable<Int64ScalarValue>, IComparable
{
    public long ToScalarValue() => Value;

    public static Int64ScalarValue FromScalarValue(IServiceProvider serviceProvider, long value) => new(value);

    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        Int64ScalarValue other => CompareTo(other),
        _ => throw new ArgumentException($"Argument {nameof(obj)} is not ${nameof(Int64ScalarValue)}.", nameof(obj)),
    };

    public int CompareTo(Int64ScalarValue other) => Value.CompareTo(other.Value);
}
