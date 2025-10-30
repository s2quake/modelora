// <copyright file="HexValue.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Tests;

[ModelScalar("hex_value", Kind = ModelScalarKind.Hex)]
public readonly partial record struct HexValue(in ImmutableArray<byte> Bytes)
    : IEquatable<HexValue>, IComparable<HexValue>, IComparable
{
    private static readonly ImmutableArray<byte> _defaultByteArray = ImmutableArray.Create(Array.Empty<byte>());

    private readonly ImmutableArray<byte> _bytes = Bytes;

    public HexValue(ReadOnlySpan<byte> bytes)
        : this(bytes.ToImmutableArray())
    {
    }

    public ImmutableArray<byte> Bytes => _bytes.IsDefault ? _defaultByteArray : _bytes;

    public static HexValue Parse(string hex)
    {
        ImmutableArray<byte> bytes = [.. ByteUtility.Parse(hex)];
        return new HexValue(bytes);
    }

    public static bool TryParse(string hex, out HexValue? hexValue)
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

    public bool Equals(HexValue other) => Bytes.SequenceEqual(other.Bytes);

    public override int GetHashCode() => ByteUtility.GetHashCode(Bytes.AsSpan());

    public override string ToString() => ByteUtility.Hex(Bytes.AsSpan());

    public int CompareTo(HexValue other)
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
        HexValue other => CompareTo(other),
        _ => throw new ArgumentException($"Argument {nameof(obj)} is not ${nameof(HexValue)}.", nameof(obj)),
    };

    internal byte[] ToScalarValue() => [.. Bytes];

    internal static HexValue FromScalarValue(IServiceProvider serviceProvider, byte[] value)
        => new(value.ToImmutableArray());
}
