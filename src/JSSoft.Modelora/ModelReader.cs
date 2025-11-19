// <copyright file="ModelReader.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Buffers;
using System.IO;
using System.Text;

namespace JSSoft.Modelora;

public ref struct ModelReader(scoped in ReadOnlySequence<byte> sequence)
{
    private SequenceReader<byte> _sequence = new(sequence);

    public object ReadEnum(Type enumType)
    {
        var underlyingType = Enum.GetUnderlyingType(enumType);
        if (underlyingType == typeof(long))
        {
            long value = ReadInt64();
            return Enum.ToObject(enumType, value);
        }
        else
        {
            int value = ReadInt32();
            return Enum.ToObject(enumType, value);
        }
    }

    public void ReadBytes(scoped Span<byte> destination)
    {
        if (!_sequence.TryCopyTo(destination))
        {
            throw new EndOfStreamException();
        }

        _sequence.Advance(destination.Length);
    }

    public ReadOnlySpan<byte> ReadBytes(int length)
    {
        if (_sequence.Remaining < length)
        {
            throw new EndOfStreamException();
        }

        var span = _sequence.UnreadSpan[..length];
        _sequence.Advance(length);
        return span;
    }

    public bool ReadBoolean()
    {
        if (!_sequence.TryRead(out byte value))
        {
            throw new EndOfStreamException();
        }

        return value != 0;
    }

    public char ReadChar()
    {
        uint result = 0;
        int shift = 0;
        byte b;
        do
        {
            if (!_sequence.TryRead(out b))
            {
                throw new EndOfStreamException();
            }

            result |= (b & 0x7Fu) << shift;
            shift += 7;
        }
        while ((b & 0x80u) != 0);

        return (char)result;
    }

    public string ReadString()
    {
        int byteCount = ReadInt32();
        if (_sequence.Remaining < byteCount)
        {
            throw new EndOfStreamException();
        }

        var span = _sequence.UnreadSpan[..byteCount];
        _sequence.Advance(byteCount);
        return Encoding.UTF8.GetString(span);
    }

    public byte ReadByte()
    {
        if (!_sequence.TryRead(out byte value))
        {
            throw new EndOfStreamException();
        }

        return value;
    }

    public short ReadInt16()
    {
        uint result = 0;
        int shift = 0;
        byte b;
        do
        {
            if (!_sequence.TryRead(out b))
            {
                throw new EndOfStreamException();
            }

            result |= (b & 0x7Fu) << shift;
            shift += 7;
        }
        while ((b & 0x80u) != 0);

        return (short)((result >> 1) ^ -(result & 1));
    }

    public int ReadInt32()
    {
        uint result = 0;
        int shift = 0;
        byte b;
        do
        {
            if (!_sequence.TryRead(out b))
            {
                throw new EndOfStreamException();
            }

            result |= (b & 0x7Fu) << shift;
            shift += 7;
        }
        while ((b & 0x80u) != 0);

        return (int)((result >> 1) ^ -(result & 1));
    }

    public long ReadInt64()
    {
        ulong result = 0;
        int shift = 0;
        byte b;
        do
        {
            if (!_sequence.TryRead(out b))
            {
                throw new EndOfStreamException();
            }

            result |= (ulong)(b & 0x7Fu) << shift;
            shift += 7;
        }
        while ((b & 0x80u) != 0);

        return (long)((result >> 1) ^ (ulong)-(long)(result & 1));
    }

    public BigInteger ReadBigInteger()
    {
        BigInteger result = 0;
        int shift = 0;
        byte b;
        do
        {
            if (!_sequence.TryRead(out b))
            {
                throw new EndOfStreamException();
            }

            result |= (BigInteger)(b & 0x7Fu) << shift;
            shift += 7;
        }
        while ((b & 0x80u) != 0);

        return (result & 1) == 0 ? result >> 1 : -((result + 1) >> 1);
    }

    public Guid ReadGuid()
    {
        Span<byte> buffer = stackalloc byte[16];
        ReadBytes(16).CopyTo(buffer);
        return new Guid(buffer);
    }
}
