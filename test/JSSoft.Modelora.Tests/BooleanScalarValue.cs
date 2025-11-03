// <copyright file="BooleanScalarValue.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Tests;

[ModelScalar("boolean_scalar_value", Kind = ModelScalarKind.Boolean)]
public readonly partial record struct BooleanScalarValue(bool Value)
    : IEquatable<BooleanScalarValue>, IComparable<BooleanScalarValue>, IComparable
{
    public bool ToScalarValue() => Value;

    public static BooleanScalarValue FromScalarValue(IServiceProvider serviceProvider, bool value) => new(value);

    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        BooleanScalarValue other => CompareTo(other),
        _ => throw new ArgumentException($"Argument {nameof(obj)} is not ${nameof(BooleanScalarValue)}.", nameof(obj)),
    };

    public int CompareTo(BooleanScalarValue other) => Value.CompareTo(other.Value);
}
