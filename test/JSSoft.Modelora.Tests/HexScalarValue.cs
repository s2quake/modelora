// <copyright file="HexScalarValue.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Tests;

[ModelScalar("hex_scalar_value", Kind = ModelScalarKind.Hex)]
public readonly partial record struct HexScalarValue(in ImmutableArray<byte> Bytes)
    : IEquatable<HexScalarValue>, IComparable<HexScalarValue>, IComparable
{
    private static readonly ImmutableArray<byte> _defaultByteArray = ImmutableArray.Create(Array.Empty<byte>());

    private readonly ImmutableArray<byte> _bytes = Bytes;

    public HexScalarValue(ReadOnlySpan<byte> bytes)
        : this(bytes.ToImmutableArray())
    {
    }

    public ImmutableArray<byte> Bytes => _bytes.IsDefault ? _defaultByteArray : _bytes;

    public static HexScalarValue Parse(string hex)
    {
        ImmutableArray<byte> bytes = [.. ByteUtility.Parse(hex)];
        return new HexScalarValue(bytes);
    }

    public static bool TryParse(string hex, out HexScalarValue? hexValue)
    {
        try
        {
            hexValue = Parse(hex);
            return true;
        }
        catch (Exception)
        {
            hexValue = null;
            return false;
        }
    }

    public bool Equals(HexScalarValue other) => Bytes.SequenceEqual(other.Bytes);

    public override int GetHashCode() => ByteUtility.GetHashCode(Bytes);

    public override string ToString() => ByteUtility.Hex(Bytes);

    public int CompareTo(HexScalarValue other)
    {
        for (var i = 0; i < Bytes.Length; ++i)
        {
            var cmp = Bytes[i].CompareTo(other.Bytes[i]);
            if (cmp != 0)
            {
                return cmp;
            }
        }

        return 0;
    }

    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        HexScalarValue other => CompareTo(other),
        _ => throw new ArgumentException($"Argument {nameof(obj)} is not ${nameof(HexScalarValue)}.", nameof(obj)),
    };

    internal byte[] ToScalarValue() => [.. Bytes];

    internal static HexScalarValue FromScalarValue(IServiceProvider serviceProvider, byte[] value)
        => new(value.ToImmutableArray());
}

[ModelScalarKnownType(typeof(HexScalarValue<int>), "hex_scalar_value_int")]
[ModelScalarKnownType(typeof(HexScalarValue<string>), "hex_scalar_value_string")]
[ModelScalar("hex_scalar_value<>", Kind = ModelScalarKind.Hex)]
public readonly partial record struct HexScalarValue<T>(in ImmutableArray<byte> Bytes)
    : IEquatable<HexScalarValue<T>>
    where T : notnull, IConvertible
{
    private readonly ImmutableArray<byte> _bytes = Bytes;

    public HexScalarValue(T value)
        : this(GetBytes(value))
    {
    }

    public ImmutableArray<byte> Bytes => _bytes.IsDefault ? [] : _bytes;

    public override int GetHashCode() => HashCode.Combine(Bytes);

    public bool Equals(HexScalarValue<T> other) => Bytes.SequenceEqual(other.Bytes);

    internal byte[] ToScalarValue() => [.. Bytes];

    internal static HexScalarValue<T> FromScalarValue(IServiceProvider serviceProvider, byte[] value)
        => new([.. value]);

    private static ImmutableArray<byte> GetBytes(T value)
    {
        if (value is int @int)
        {
            return [.. BitConverter.GetBytes(@int)];
        }
        else if (value is string @string)
        {
            return [.. System.Text.Encoding.UTF8.GetBytes(@string)];
        }
        else
        {
            throw new NotSupportedException("Unsupported type.");
        }
    }
}

[ModelScalarKnownType(typeof(int), "invalid_hex_scalar_value_int")]
[ModelScalar("invalid_hex_scalar_value<>", Kind = ModelScalarKind.Hex)]
public readonly partial record struct InvalidHexScalarValue<T>(in ImmutableArray<byte> Bytes)
    : IEquatable<InvalidHexScalarValue<T>>
    where T : notnull, IConvertible
{
    private readonly ImmutableArray<byte> _bytes = Bytes;

    public ImmutableArray<byte> Bytes => _bytes.IsDefault ? [] : _bytes;

    internal byte[] ToScalarValue() => throw new NotSupportedException("Cannot convert to scalar value.");

    internal static InvalidHexScalarValue<T> FromScalarValue(IServiceProvider serviceProvider, byte[] value)
        => throw new NotSupportedException("Cannot convert from scalar value.");
}
