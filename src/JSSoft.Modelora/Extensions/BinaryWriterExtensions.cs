// <copyright file="BinaryWriterExtensions.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;

namespace JSSoft.Modelora.Extensions;

public static class BinaryWriterExtensions
{
    public static void WriteEnum<T>(this BinaryWriter @this, T value)
        where T : Enum => WriteEnum(@this, value, typeof(T));

    public static void WriteEnum(this BinaryWriter @this, object value, Type enumType)
    {
        var underlyingType = Enum.GetUnderlyingType(enumType);
        if (underlyingType == typeof(long))
        {
            @this.Write((byte)1);
            WriteZigZagEncodedInt64(@this, Convert.ToInt64(value));
        }
        else
        {
            @this.Write((byte)0);
            WriteZigZagEncodedInt32(@this, Convert.ToInt32(value));
        }
    }

    public static void WriteZigZagEncodedInt32(this BinaryWriter @this, int value)
    {
        unchecked
        {
            uint zigzag = (uint)((value << 1) ^ (value >> 31));

            while (zigzag > 0x7Fu)
            {
                @this.Write((byte)((zigzag & 0x7Fu) | 0x80u));
                zigzag >>= 7;
            }

            @this.Write((byte)zigzag);
        }
    }

    public static void WriteZigZagEncodedInt64(this BinaryWriter @this, long value)
    {
        unchecked
        {
            ulong zigzag = (ulong)((value << 1) ^ (value >> 63));

            while (zigzag > 0x7Ful)
            {
                @this.Write((byte)((zigzag & 0x7Ful) | 0x80u));
                zigzag >>= 7;
            }

            @this.Write((byte)zigzag);
        }
    }

    public static void WriteZigZagEncodedBigInteger(this BinaryWriter @this, BigInteger value)
    {
        var zigzag = value < 0 ? ((-value) << 1) - 1 : value << 1;

        while (zigzag >= 0x80)
        {
            @this.Write((byte)(((int)(zigzag & 0x7F)) | 0x80));
            zigzag >>= 7;
        }

        @this.Write((byte)zigzag);
    }
}
