// <copyright file="BinaryReaderExtensions.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;

namespace JSSoft.Modelora.Extensions;

public static class BinaryReaderExtensions
{
    public static T ReadEnum<T>(this BinaryReader @this)
        where T : Enum => (T)ReadEnum(@this, typeof(T));

    public static object ReadEnum(this BinaryReader @this, Type enumType)
    {
        var isLong = @this.ReadByte() == 1;
        var bytes = new byte[isLong ? sizeof(long) : sizeof(int)];
        if (@this.Read(bytes, 0, bytes.Length) != bytes.Length)
        {
            throw new EndOfStreamException("Failed to read enum from stream.");
        }

        return isLong
            ? Enum.ToObject(enumType, BitConverter.ToInt64(bytes))
            : Enum.ToObject(enumType, BitConverter.ToInt32(bytes));
    }

    public static int ReadZigZagEncodedInt32(this BinaryReader @this)
    {
        uint raw = ReadUnsignedVarint32(@this);
        return (int)((raw >> 1) ^ (uint)-(int)(raw & 1u));
    }

    public static long ReadZigZagEncodedInt64(this BinaryReader @this)
    {
        ulong raw = ReadUnsignedVarint64(@this);
        return (long)((raw >> 1) ^ (ulong)-(long)(raw & 1ul));
    }

    private static uint ReadUnsignedVarint32(BinaryReader @this)
    {
        uint result = 0;
        int shift = 0;
        for (int i = 0; i < 5; i++)
        {
            byte b = @this.ReadByte();
            result |= (uint)(b & 0x7F) << shift;
            if ((b & 0x80) == 0)
            {
                return result;
            }

            shift += 7;
        }

        throw new FormatException("Bad 7-bit unsigned varint for UInt32.");
    }

    private static ulong ReadUnsignedVarint64(BinaryReader @this)
    {
        ulong result = 0;
        int shift = 0;
        for (int i = 0; i < 10; i++)
        {
            byte b = @this.ReadByte();
            result |= (ulong)(b & 0x7F) << shift;
            if ((b & 0x80) == 0)
            {
                return result;
            }

            shift += 7;
        }

        throw new FormatException("Bad 7-bit unsigned varint for UInt64.");
    }
}
