// <copyright file="ModelReaderWriterTests.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Buffers;
using System.IO;
using System.Numerics;
using System.Text;
using JSSoft.Modelora;
using Xunit;

namespace JSSoft.Modelora.Tests;

public class ModelReaderWriterTests
{
    private static ReadOnlySequence<byte> ToSequence(ArrayBufferWriter<byte> buffer)
        => new ReadOnlySequence<byte>(buffer.WrittenMemory);

    private static ArrayBufferWriter<byte> CreateBuffer() => new ArrayBufferWriter<byte>();

    [Fact]
    public void WriteRead_Primitives_RoundTrip()
    {
        var buf = CreateBuffer();
        var w = new ModelWriter(buf);
        w.Write(true);
        w.Write((byte)0xAB);
        w.Write((short)-12345);
        w.Write(0);
        w.Write(63);
        w.Write(-64);
        w.Write(1234567890123L);

        var r = new ModelReader(ToSequence(buf));
        Assert.True(r.ReadBoolean());
        Assert.Equal((byte)0xAB, r.ReadByte());
        Assert.Equal((short)-12345, r.ReadInt16());
        Assert.Equal(0, r.ReadInt32());
        Assert.Equal(63, r.ReadInt32());
        Assert.Equal(-64, r.ReadInt32());
        Assert.Equal(1234567890123L, r.ReadInt64());
    }

    [Fact]
    public void WriteRead_Char_RoundTrip()
    {
        var buf = CreateBuffer();
        var w = new ModelWriter(buf);
        w.Write('A');
        w.Write('한');
        var r = new ModelReader(ToSequence(buf));
        Assert.Equal('A', r.ReadChar());
        Assert.Equal('한', r.ReadChar());
    }

    [Fact]
    public void WriteChar_Throws_On_Surrogate()
    {
        var buf = CreateBuffer();
        var w = new ModelWriter(buf);
        var threw1 = false;
        try
        {
            w.Write('\uD800');
        }
        catch (ArgumentException)
        {
            threw1 = true;
        }

        Assert.True(threw1);

        var threw2 = false;
        try
        {
            w.Write('\uDFFF');
        }
        catch (ArgumentException)
        {
            threw2 = true;
        }

        Assert.True(threw2);
    }

    [Theory]
    [InlineData("")]
    [InlineData("hello")]
    [InlineData("한글🙂emoji")]
    public void WriteRead_String_RoundTrip(string s)
    {
        var buf = CreateBuffer();
        var w = new ModelWriter(buf);
        w.Write(s);
        var r = new ModelReader(ToSequence(buf));
        Assert.Equal(s, r.ReadString());
    }

    [Fact]
    public void WriteRead_Bytes_RoundTrip()
    {
        var data = Enumerable.Range(0, 256).Select(i => (byte)i).ToArray();
        var buf = CreateBuffer();
        var w = new ModelWriter(buf);
        w.Write(data);
        var r = new ModelReader(ToSequence(buf));
        var read = r.ReadBytes(data.Length);
        Assert.True(read.SequenceEqual(data));
        var tmp = new byte[256];
        var r2 = new ModelReader(ToSequence(buf));
        r2.ReadBytes(tmp);
        Assert.True(tmp.SequenceEqual(data));
    }

    [Fact]
    public void WriteRead_Guid_RoundTrip()
    {
        var g = Guid.NewGuid();
        var buf = CreateBuffer();
        var w = new ModelWriter(buf);
        w.Write(g);
        var r = new ModelReader(ToSequence(buf));
        Assert.Equal(g, r.ReadGuid());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(127)]
    [InlineData(-128)]
    [InlineData(300)]
    public void WriteRead_BigInteger_RoundTrip_Small(long v)
    {
        var big = new BigInteger(v);
        var buf = CreateBuffer();
        var w = new ModelWriter(buf);
        w.Write(big);
        var r = new ModelReader(ToSequence(buf));
        Assert.Equal(big, r.ReadBigInteger());
    }

    [Fact]
    public void WriteRead_BigInteger_RoundTrip_Large()
    {
        var big = BigInteger.Parse("123456789012345678901234567890");
        var buf = CreateBuffer();
        var w = new ModelWriter(buf);
        w.Write(big);
        var r = new ModelReader(ToSequence(buf));
        Assert.Equal(big, r.ReadBigInteger());
    }

    private enum E32
    {
        A = -1,
        B = 0,
        C = 123456,
    }

    private enum E64 : long
    {
        A = -1,
        B = 0,
        C = 1234567890123,
    }

    [Fact]
    public void WriteRead_Enum_Int32_Underlying()
    {
        var buf = CreateBuffer();
        var w = new ModelWriter(buf);
        w.WriteEnum(E32.C);
        var r = new ModelReader(ToSequence(buf));
        var obj = r.ReadEnum(typeof(E32));
        Assert.Equal(E32.C, (E32)obj);
    }

    [Fact]
    public void WriteRead_Enum_Int64_Underlying()
    {
        var buf = CreateBuffer();
        var w = new ModelWriter(buf);
        w.WriteEnum(E64.C);
        var r = new ModelReader(ToSequence(buf));
        var obj = r.ReadEnum(typeof(E64));
        Assert.Equal(E64.C, (E64)obj);
    }

    private sealed class DummyBufferWriter : IBufferWriter<byte>
    {
        private byte[] _buffer = new byte[256];
        private int _written;

        public void Advance(int count) => _written += count;
        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            if (_buffer.Length - _written < sizeHint)
            {
                Array.Resize(ref _buffer, Math.Max(_buffer.Length * 2, _written + sizeHint));
            }

            return _buffer.AsMemory(_written);
        }

        public Span<byte> GetSpan(int sizeHint = 0) => GetMemory(sizeHint).Span;
    }

    [Fact]
    public void ToByteArray_Throws_When_Not_ArrayBufferWriter()
    {
        var dummy = new DummyBufferWriter();
        var w = new ModelWriter(dummy);
        w.Write((byte)1);
        var threw = false;
        try
        {
            w.ToByteArray();
        }
        catch (InvalidOperationException)
        {
            threw = true;
        }

        Assert.True(threw);
    }

    [Fact]
    public void Reader_Throws_On_EndOfStream()
    {
        var empty = new ReadOnlySequence<byte>(ReadOnlyMemory<byte>.Empty);
        var r = new ModelReader(empty);
        var threw = false;
        try
        {
            r.ReadByte();
        }
        catch (EndOfStreamException)
        {
            threw = true;
        }

        Assert.True(threw);
    }

    [Fact]
    public void Reader_ReadString_Throws_When_Length_TooLarge()
    {
        var buf = CreateBuffer();
        var w = new ModelWriter(buf);

        // Manually write an invalid large length (e.g., 10) with no bytes following
        w.Write(10);
        var r = new ModelReader(ToSequence(buf));
        var threw = false;
        try
        {
            r.ReadString();
        }
        catch (EndOfStreamException)
        {
            threw = true;
        }

        Assert.True(threw);
    }

    [Fact]
    public void Reader_ReadBoolean_Throws_On_EndOfStream()
    {
        var empty = new ReadOnlySequence<byte>(ReadOnlyMemory<byte>.Empty);
        var r = new ModelReader(empty);
        var threw = false;
        try
        {
            r.ReadBoolean();
        }
        catch (EndOfStreamException)
        {
            threw = true;
        }

        Assert.True(threw);
    }

    [Fact]
    public void Reader_ReadInt32_Throws_On_Incomplete_Varint()
    {
        var seq = new ReadOnlySequence<byte>(new byte[] { 0x80 });
        var r = new ModelReader(seq);
        var threw = false;
        try
        {
            r.ReadInt32();
        }
        catch (EndOfStreamException)
        {
            threw = true;
        }

        Assert.True(threw);
    }

    [Fact]
    public void Reader_ReadBigInteger_Throws_On_Incomplete_Varint()
    {
        var seq = new ReadOnlySequence<byte>(new byte[] { 0x80 });
        var r = new ModelReader(seq);
        var threw = false;
        try
        {
            r.ReadBigInteger();
        }
        catch (EndOfStreamException)
        {
            threw = true;
        }

        Assert.True(threw);
    }

    [Fact]
    public void Reader_ReadGuid_Throws_On_Short_Data()
    {
        var data = new byte[15];
        var seq = new ReadOnlySequence<byte>(data);
        var r = new ModelReader(seq);
        var threw = false;
        try
        {
            r.ReadGuid();
        }
        catch (EndOfStreamException)
        {
            threw = true;
        }

        Assert.True(threw);
    }

    [Fact]
    public void Reader_ReadEnum_Throws_On_EndOfStream()
    {
        var empty = new ReadOnlySequence<byte>(ReadOnlyMemory<byte>.Empty);
        var r = new ModelReader(empty);
        var threw = false;
        try
        {
            r.ReadEnum(typeof(E32));
        }
        catch (EndOfStreamException)
        {
            threw = true;
        }

        Assert.True(threw);
    }

    [Fact]
    public void Reader_ReadChar_Throws_On_Incomplete_Varint()
    {
        var seq = new ReadOnlySequence<byte>(new byte[] { 0x80 });
        var r = new ModelReader(seq);
        var threw = false;
        try
        {
            r.ReadChar();
        }
        catch (EndOfStreamException)
        {
            threw = true;
        }

        Assert.True(threw);
    }

    [Fact]
    public void Reader_ReadInt16_Throws_On_Incomplete_Varint()
    {
        var seq = new ReadOnlySequence<byte>(new byte[] { 0x80 });
        var r = new ModelReader(seq);
        var threw = false;
        try
        {
            r.ReadInt16();
        }
        catch (EndOfStreamException)
        {
            threw = true;
        }

        Assert.True(threw);
    }

    [Fact]
    public void Reader_ReadInt64_Throws_On_Incomplete_Varint()
    {
        var seq = new ReadOnlySequence<byte>(new byte[] { 0x80 });
        var r = new ModelReader(seq);
        var threw = false;
        try
        {
            r.ReadInt64();
        }
        catch (EndOfStreamException)
        {
            threw = true;
        }

        Assert.True(threw);
    }

    [Fact]
    public void Reader_ReadBytes_Allows_Empty_Span_NoOp()
    {
        var data = new byte[] { 1, 2, 3 };
        var seq = new ReadOnlySequence<byte>(data);
        var r = new ModelReader(seq);

        r.ReadBytes(Span<byte>.Empty);

        var b = r.ReadByte();
        Assert.Equal((byte)1, b);
    }

    [Fact]
    public void Reader_ReadBytes_Throws_When_Destination_Exceeds_Remaining()
    {
        var data = new byte[] { 1, 2 };
        var seq = new ReadOnlySequence<byte>(data);
        var r = new ModelReader(seq);
        var threw = false;
        try
        {
            Span<byte> tmp = stackalloc byte[3];
            r.ReadBytes(tmp);
        }
        catch (EndOfStreamException)
        {
            threw = true;
        }

        Assert.True(threw);
    }
}
