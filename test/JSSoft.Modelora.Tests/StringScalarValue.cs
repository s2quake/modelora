// <copyright file="StringScalarValue.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Tests;

[ModelScalar("string_scalar_value", Kind = ModelScalarKind.String)]
public readonly partial record struct StringScalarValue(string Value)
    : IEquatable<StringScalarValue>, IComparable<StringScalarValue>, IComparable
{
    public string ToScalarValue() => Value;

    public static StringScalarValue FromScalarValue(IServiceProvider serviceProvider, string value) => new(value);

    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        StringScalarValue other => CompareTo(other),
        _ => throw new ArgumentException($"Argument {nameof(obj)} is not ${nameof(StringScalarValue)}.", nameof(obj)),
    };

    public int CompareTo(StringScalarValue other) => string.Compare(Value, other.Value, StringComparison.Ordinal);
}
