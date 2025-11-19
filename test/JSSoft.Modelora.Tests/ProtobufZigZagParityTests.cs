// <copyright file="ProtobufZigZagParityTests.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Buffers;
using System.IO;
using System.Numerics;
using Google.Protobuf;
using JSSoft.Modelora;

namespace JSSoft.Modelora.Tests;

public sealed class ProtobufZigZagParityTests
{
    private static byte[] EncodeOur(int v)
    {
        var buf = new ArrayBufferWriter<byte>();
        var w = new ModelWriter(buf);
        w.Write(v);
        return w.ToByteArray();
    }

    private static byte[] EncodeOur(long v)
    {
        var buf = new ArrayBufferWriter<byte>();
        var w = new ModelWriter(buf);
        w.Write(v);
        return w.ToByteArray();
    }

    private static byte[] EncodeOur(BigInteger v)
    {
        var buf = new ArrayBufferWriter<byte>();
        var w = new ModelWriter(buf);
        w.Write(v);
        return w.ToByteArray();
    }

    private static byte[] EncodeProto(int v)
    {
        using var ms = new MemoryStream();
        var cos = new CodedOutputStream(ms);
        cos.WriteSInt32(v);
        cos.Flush();
        return ms.ToArray();
    }

    private static byte[] EncodeProto(long v)
    {
        using var ms = new MemoryStream();
        var cos = new CodedOutputStream(ms);
        cos.WriteSInt64(v);
        cos.Flush();
        return ms.ToArray();
    }

    [Fact]
    public void Int_Parity_With_Protobuf()
    {
        int[] values =
        [
            0, 1, -1,
            63, 64, -64, -65,
            8191, 8192, -8192, -8193,
            1_048_575, 1_048_576, -1_048_576, -1_048_577,
            134_217_727, 134_217_728, -134_217_728, -134_217_729,
            int.MaxValue, int.MinValue
        ];

        foreach (var v in values)
        {
            var ours = EncodeOur(v);
            var proto = EncodeProto(v);
            Assert.True(ours.AsSpan().SequenceEqual(proto), $"Mismatch for {v}");
        }
    }

    [Fact]
    public void Long_Parity_With_Protobuf()
    {
        long[] boundaries = new long[9];
        for (int k = 1; k <= 9; k++)
        {
            boundaries[k - 1] = 1L << ((7 * k) - 1);
        }

        var values = new List<long>() { 0, 1, -1 };
        foreach (var b in boundaries)
        {
            values.AddRange([b - 1, b, -b, -(b + 1)]);
        }

        values.AddRange([long.MaxValue, long.MinValue]);

        foreach (var v in values)
        {
            var ours = EncodeOur(v);
            var proto = EncodeProto(v);
            Assert.True(ours.AsSpan().SequenceEqual(proto), $"Mismatch for {v}");
        }
    }

    [Fact]
    public void BigInteger_Within_LongRange_Parity_With_Protobuf()
    {
        long[] boundaries = new long[9];
        for (int k = 1; k <= 9; k++)
        {
            boundaries[k - 1] = 1L << ((7 * k) - 1);
        }

        var values = new List<long>() { 0, 1, -1 };
        foreach (var b in boundaries)
        {
            values.AddRange(new long[] { b - 1, b, -b, -(b + 1) });
        }

        values.AddRange(new long[] { long.MaxValue, long.MinValue });

        foreach (var lv in values)
        {
            var big = new BigInteger(lv);
            var ours = EncodeOur(big);
            var proto = EncodeProto(lv);
            Assert.True(ours.AsSpan().SequenceEqual(proto), $"Mismatch for {lv}");
        }
    }
}
