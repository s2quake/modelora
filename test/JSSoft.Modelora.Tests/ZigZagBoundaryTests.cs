// <copyright file="ZigZagBoundaryTests.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Buffers;
using System.Numerics;
using JSSoft.Modelora;

namespace JSSoft.Modelora.Tests;

public sealed class ZigZagBoundaryTests
{
    private static int EncodedLength(short v)
    {
        var buf = new ArrayBufferWriter<byte>();
        var w = new ModelWriter(buf);
        w.Write(v);
        return w.ToByteArray().Length;
    }

    private static int EncodedLength(int v)
    {
        var buf = new ArrayBufferWriter<byte>();
        var w = new ModelWriter(buf);
        w.Write(v);
        return w.ToByteArray().Length;
    }

    private static int EncodedLength(long v)
    {
        var buf = new ArrayBufferWriter<byte>();
        var w = new ModelWriter(buf);
        w.Write(v);
        return w.ToByteArray().Length;
    }

    private static int EncodedLength(BigInteger v)
    {
        var buf = new ArrayBufferWriter<byte>();
        var w = new ModelWriter(buf);
        w.Write(v);
        return w.ToByteArray().Length;
    }

    [Fact]
    public void Short_Boundaries_And_Samples()
    {
        var boundaries = new short[] { 1 << 6, 1 << 13 };
        Assert.Equal(2, boundaries.Length);

        for (int i = 0; i < boundaries.Length; i++)
        {
            int expectedBefore = i + 1;
            int expectedAt = i + 2;
            short b = boundaries[i];

            Assert.Equal(expectedBefore, EncodedLength((short)(b - 1)));
            Assert.Equal(expectedAt, EncodedLength(b));
            Assert.Equal(expectedBefore, EncodedLength((short)-b));
            Assert.Equal(expectedAt, EncodedLength((short)-(b + 1)));
        }
    }

    [Fact]
    public void Int_Boundaries_And_Samples()
    {
        var boundaries = new int[] { 1 << 6, 1 << 13, 1 << 20, 1 << 27 };
        Assert.Equal(4, boundaries.Length);

        for (int i = 0; i < boundaries.Length; i++)
        {
            int expectedBefore = i + 1;
            int expectedAt = i + 2;
            int b = boundaries[i];

            Assert.Equal(expectedBefore, EncodedLength(b - 1));
            Assert.Equal(expectedAt, EncodedLength(b));
            Assert.Equal(expectedBefore, EncodedLength(-b));
            Assert.Equal(expectedAt, EncodedLength(-(b + 1)));
        }
    }

    [Fact]
    public void Long_Boundaries_And_Samples()
    {
        var boundaries = new long[9];
        for (int k = 1; k <= 9; k++)
        {
            boundaries[k - 1] = 1L << ((7 * k) - 1);
        }

        Assert.Equal(9, boundaries.Length);

        for (int i = 0; i < boundaries.Length; i++)
        {
            int expectedBefore = i + 1;
            int expectedAt = i + 2;
            long b = boundaries[i];

            Assert.Equal(expectedBefore, EncodedLength(b - 1));
            Assert.Equal(expectedAt, EncodedLength(b));
            Assert.Equal(expectedBefore, EncodedLength(-b));
            Assert.Equal(expectedAt, EncodedLength(-(b + 1)));
        }
    }

    [Fact]
    public void BigInteger_LongRange_Boundaries_Match_Long()
    {
        for (int k = 1; k <= 9; k++)
        {
            long b = 1L << ((7 * k) - 1);
            int expectedBefore = k;
            int expectedAt = k + 1;

            Assert.Equal(expectedBefore, EncodedLength(new BigInteger(b - 1)));
            Assert.Equal(expectedAt, EncodedLength(new BigInteger(b)));
            Assert.Equal(expectedBefore, EncodedLength(new BigInteger(-b)));
            Assert.Equal(expectedAt, EncodedLength(new BigInteger(-(b + 1))));
        }
    }
}
