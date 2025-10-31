// <copyright file="ByteUtility.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Buffers;
using System.Globalization;

namespace JSSoft.Modelora;

public static class ByteUtility
{
    private static readonly char[] _hexCharacters =
    [
        '0', '1', '2', '3', '4', '5', '6', '7',
        '8', '9', 'a', 'b', 'c', 'd', 'e', 'f',
    ];

    public static byte[] Parse(string hex)
    {
        if (hex.Length % 2 > 0)
        {
            throw new FormatException("A length of a hexadecimal string must be an even number.");
        }

        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < hex.Length / 2; i++)
        {
            bytes[i] = byte.Parse(hex.AsSpan(i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return bytes;
    }

    public static string Hex(in ImmutableArray<byte> bytes) => Hex(bytes.AsSpan());

    public static string Hex(ReadOnlySpan<byte> bytes)
    {
#if NETSTANDARD2_1_OR_GREATER
        char[] chars = new char[bytes.Length * 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            chars[i * 2] = _hexCharacters[bytes[i] >> 4];
            chars[(i * 2) + 1] = _hexCharacters[bytes[i] & 0xf];
        }

        return new string(chars);
#else
        var length = bytes.Length * 2;
        var chars = ArrayPool<char>.Shared.Rent(length);
        for (int i = 0; i < bytes.Length; i++)
        {
            chars[i * 2] = _hexCharacters[bytes[i] >> 4];
            chars[(i * 2) + 1] = _hexCharacters[bytes[i] & 0xf];
        }

        var result = new string(chars, 0, length);
        ArrayPool<char>.Shared.Return(chars);
        return result;
#endif
    }

    public static int GetHashCode(ReadOnlySpan<byte> bytes)
    {
        var hashCode = 0;
        unchecked
        {
            foreach (var @byte in bytes)
            {
                hashCode = (hashCode * 397) ^ @byte.GetHashCode();
            }
        }

        return hashCode;
    }
}
