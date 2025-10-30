// <copyright file="BinaryWriterZigZagExtensionsTest.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;
using JSSoft.Modelora.Extensions;

namespace JSSoft.Modelora.Tests.Extensions;

public sealed class BinaryWriterZigZagExtensionsTest
{
    public sealed class Int32KnownPatternsData : TheoryData<int, byte[]>
    {
        public Int32KnownPatternsData()
        {
            // Basic smalls
            Add(0, [0x00]);
            Add(1, [0x02]);
            Add(-1, [0x01]);
            Add(2, [0x04]);
            Add(-2, [0x03]);
            Add(127, [0xFE, 0x01]);
            Add(-128, [0xFF, 0x01]);
            Add(300, [0xD8, 0x04]); // 300 zigzag -> 600 -> 0xD8 0x04

            // ZigZag varint byte-size boundaries for int32
            Add(-64, [0x7F]); // last 1-byte
            Add(64, [0x80, 0x01]); // first 2-byte
            Add(-8192, [0xFF, 0x7F]); // last 2-byte
            Add(8192, [0x80, 0x80, 0x01]); // first 3-byte
            Add(-1048576, [0xFF, 0xFF, 0x7F]); // last 3-byte
            Add(1048576, [0x80, 0x80, 0x80, 0x01]); // first 4-byte
            Add(-134217728, [0xFF, 0xFF, 0xFF, 0x7F]); // last 4-byte
            Add(134217728, [0x80, 0x80, 0x80, 0x80, 0x01]); // first 5-byte
            Add(int.MaxValue, [0xFE, 0xFF, 0xFF, 0xFF, 0x0F]);
            Add(int.MinValue, [0xFF, 0xFF, 0xFF, 0xFF, 0x0F]);
        }
    }

    public sealed class Int64KnownPatternsData : TheoryData<long, byte[]>
    {
        public Int64KnownPatternsData()
        {
            Add(0L, [0x00]);
            Add(1L, [0x02]);
            Add(-1L, [0x01]);
            Add(127L, [0xFE, 0x01]);
            Add(-128L, [0xFF, 0x01]);
            Add(300L, [0xD8, 0x04]);

            // ZigZag varint byte-size boundaries for int64
            Add(-64L, [0x7F]); // last 1-byte
            Add(64L, [0x80, 0x01]); // first 2-byte
            Add(-8192L, [0xFF, 0x7F]); // last 2-byte
            Add(8192L, [0x80, 0x80, 0x01]); // first 3-byte
            Add(-1_048_576L, [0xFF, 0xFF, 0x7F]); // last 3-byte
            Add(1_048_576L, [0x80, 0x80, 0x80, 0x01]); // first 4-byte
            Add(-134_217_728L, [0xFF, 0xFF, 0xFF, 0x7F]); // last 4-byte
            Add(134_217_728L, [0x80, 0x80, 0x80, 0x80, 0x01]); // first 5-byte
            Add(-17_179_869_184L, [0xFF, 0xFF, 0xFF, 0xFF, 0x7F]); // last 5-byte
            Add(17_179_869_184L, [0x80, 0x80, 0x80, 0x80, 0x80, 0x01]); // first 6-byte
            Add(-2_199_023_255_552L, [0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F]); // last 6-byte
            Add(2_199_023_255_552L, [0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01]); // first 7-byte
            Add(-281_474_976_710_656L, [0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F]); // last 7-byte
            Add(281_474_976_710_656L, [0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01]); // first 8-byte
            Add(-36_028_797_018_963_968L, [0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F]); // last 8-byte
            Add(36_028_797_018_963_968L, [0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01]); // first 9-byte
            Add(-4_611_686_018_427_387_904L, [0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F]); // last 9-byte
            Add(4_611_686_018_427_387_904L, [0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01]); // first 10-byte
            Add(long.MinValue, [0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01]);  // long.MinValue (ZigZag -> 0xFFFFFFFFFFFFFFFF)
            Add(long.MaxValue, [0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01]);  // long.MaxValue (ZigZag -> 0xFFFFFFFFFFFFFFFE)
        }
    }

    [Theory]
    [ClassData(typeof(Int32KnownPatternsData))]
    public void WriteZigZagEncodedInt32_KnownPatterns(int value, byte[] expected)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);

        bw.WriteZigZagEncodedInt32(value);
        var actual = ms.ToArray();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(127)]
    [InlineData(-128)]
    [InlineData(300)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public void WriteRead_ZigZag_Int32_RoundTrip(int value)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        bw.WriteZigZagEncodedInt32(value);

        ms.Position = 0;
        using var br = new BinaryReader(ms);
        var read = br.ReadZigZagEncodedInt32();

        Assert.Equal(value, read);
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(-2, 1)]
    [InlineData(-127, 2)]
    [InlineData(-128, 2)]
    [InlineData(-1000, 2)]
    public void WriteZigZagEncodedInt32_Negatives_AreCompact(int value, int expectedLength)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        bw.WriteZigZagEncodedInt32(value);
        Assert.Equal(expectedLength, (int)ms.Length);
    }

    [Theory]
    [ClassData(typeof(Int64KnownPatternsData))]
    public void WriteZigZagEncodedInt64_KnownPatterns(long value, byte[] expected)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        bw.WriteZigZagEncodedInt64(value);
        var actual = ms.ToArray();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(Int32KnownPatternsData))]
    public void WriteRead_ZigZag_Int32_RoundTrip_FromKnownPatterns(int value, byte[] expectedBytes)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        bw.WriteZigZagEncodedInt32(value);

        ms.Position = 0;
        using var br = new BinaryReader(ms);
        var read = br.ReadZigZagEncodedInt32();

        Assert.Equal(value, read);

        // Also verify bytes match the known pattern for completeness
        var actualBytes = ms.ToArray();
        Assert.Equal(expectedBytes, actualBytes);
    }

    [Theory]
    [ClassData(typeof(Int64KnownPatternsData))]
    public void WriteRead_ZigZag_Int64_RoundTrip_FromKnownPatterns(long value, byte[] expectedBytes)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        bw.WriteZigZagEncodedInt64(value);

        ms.Position = 0;
        using var br = new BinaryReader(ms);
        var read = br.ReadZigZagEncodedInt64();

        Assert.Equal(value, read);

        // Also verify bytes match the known pattern for completeness
        var actualBytes = ms.ToArray();
        Assert.Equal(expectedBytes, actualBytes);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(-1L)]
    [InlineData(127L)]
    [InlineData(-128L)]
    [InlineData(300L)]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue)]
    public void WriteRead_ZigZag_Int64_RoundTrip(long value)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        bw.WriteZigZagEncodedInt64(value);

        ms.Position = 0;
        using var br = new BinaryReader(ms);
        var read = br.ReadZigZagEncodedInt64();

        Assert.Equal(value, read);
    }

    [Theory]
    [InlineData(-1L, 1)]
    [InlineData(-2L, 1)]
    [InlineData(-127L, 2)]
    [InlineData(-128L, 2)]
    [InlineData(-1000L, 2)]
    public void WriteZigZagEncodedInt64_Negatives_AreCompact(long value, int expectedLength)
    {
        using var ms = new MemoryStream();
        using var bw = new BinaryWriter(ms);
        bw.WriteZigZagEncodedInt64(value);
        Assert.Equal(expectedLength, (int)ms.Length);
    }
}
