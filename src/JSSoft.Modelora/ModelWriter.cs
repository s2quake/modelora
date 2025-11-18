// <copyright file="ModelWriter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Buffers;
using System.Text;

namespace JSSoft.Modelora;

public ref struct ModelWriter(IBufferWriter<byte> bufferWriter)
{
    public readonly void WriteEnum<T>(T value)
        where T : Enum => WriteEnum(value, typeof(T));

    public readonly void WriteEnum(object value, Type enumType)
    {
        var underlyingType = Enum.GetUnderlyingType(enumType);
        if (underlyingType == typeof(long))
        {
            Write((byte)1);
            Write(Convert.ToInt64(value));
        }
        else
        {
            Write((byte)0);
            Write(Convert.ToInt32(value));
        }
    }

    public readonly void Write(ReadOnlySpan<byte> bytes)
    {
        var span = bufferWriter.GetSpan(bytes.Length);
        bytes.CopyTo(span);
        bufferWriter.Advance(bytes.Length);
    }

    public readonly void Write(bool value)
    {
        var span = bufferWriter.GetSpan(1);
        span[0] = (byte)(value ? 1 : 0);
        bufferWriter.Advance(1);
    }

    public readonly void Write(char value)
    {
        if (char.IsHighSurrogate(value) || char.IsLowSurrogate(value))
        {
            throw new ArgumentException("Cannot write surrogate character.", nameof(value));
        }

        ushort v = value;
        while (v > 0x7Fu)
        {
            var span = bufferWriter.GetSpan(1);
            span[0] = (byte)((v & 0x7Fu) | 0x80u);
            bufferWriter.Advance(1);
            v >>= 7;
        }

        var last = bufferWriter.GetSpan(1);
        last[0] = (byte)v;
        bufferWriter.Advance(1);
    }

    public readonly void Write(string value)
    {
        Encoding.UTF8.GetMaxByteCount(value.Length);
        var byteCount = Encoding.UTF8.GetByteCount(value);
        Write(byteCount);

        var span = bufferWriter.GetSpan(byteCount);
        Encoding.UTF8.GetBytes(value, span);
        bufferWriter.Advance(byteCount);
    }

    public readonly void Write(byte value)
    {
        var span = bufferWriter.GetSpan(1);
        span[0] = value;
        bufferWriter.Advance(1);
    }

    public readonly void Write(short value)
    {
        unchecked
        {
            ushort zigzag = (ushort)((value << 1) ^ (value >> 15));

            while (zigzag > 0x7Fu)
            {
                bufferWriter.GetSpan(1)[0] = (byte)((zigzag & 0x7Fu) | 0x80u);
                bufferWriter.Advance(1);
                zigzag >>= 7;
            }

            bufferWriter.GetSpan(1)[0] = (byte)zigzag;
            bufferWriter.Advance(1);
        }
    }

    public readonly void Write(int value)
    {
        unchecked
        {
            uint zigzag = (uint)((value << 1) ^ (value >> 31));

            while (zigzag > 0x7Fu)
            {
                bufferWriter.GetSpan(1)[0] = (byte)((zigzag & 0x7Fu) | 0x80u);
                bufferWriter.Advance(1);
                zigzag >>= 7;
            }

            bufferWriter.GetSpan(1)[0] = (byte)zigzag;
            bufferWriter.Advance(1);
        }
    }

    public readonly void Write(long value)
    {
        unchecked
        {
            ulong zigzag = (ulong)((value << 1) ^ (value >> 63));

            while (zigzag > 0x7Ful)
            {
                bufferWriter.GetSpan(1)[0] = (byte)((zigzag & 0x7Ful) | 0x80u);
                bufferWriter.Advance(1);
                zigzag >>= 7;
            }

            bufferWriter.GetSpan(1)[0] = (byte)zigzag;
            bufferWriter.Advance(1);
        }
    }

    public readonly void Write(BigInteger value)
    {
        var zigzag = value < 0 ? ((-value) << 1) - 1 : value << 1;

        while (zigzag >= 0x80)
        {
            bufferWriter.GetSpan(1)[0] = (byte)((zigzag & 0x7F) | 0x80);
            bufferWriter.Advance(1);
            zigzag >>= 7;
        }

        bufferWriter.GetSpan(1)[0] = (byte)zigzag;
        bufferWriter.Advance(1);
    }

    public readonly void Write(Guid value)
    {
        var span = bufferWriter.GetSpan(16);
        value.TryWriteBytes(span);
        bufferWriter.Advance(16);
    }

    public readonly byte[] ToByteArray()
    {
        if (bufferWriter is ArrayBufferWriter<byte> arrayBufferWriter)
        {
            return arrayBufferWriter.WrittenSpan.ToArray();
        }

        throw new InvalidOperationException("Cannot convert to byte array.");
    }
}
