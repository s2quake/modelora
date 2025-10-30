// <copyright file="ProtobufZigZagCompatibilityTest.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;
using Google.Protobuf;
using JSSoft.Modelora.Extensions;

namespace JSSoft.Modelora.Tests.Extensions;

public sealed class ProtobufZigZagCompatibilityTest
{
    // int32: our bytes == protobuf bytes
    [Theory]
    [ClassData(typeof(BinaryWriterZigZagExtensionsTest.Int32KnownPatternsData))]
    public void Int32_Bytes_Match_Protobuf(int value, byte[] expectedBytes)
    {
        // ours
        using var ms1 = new MemoryStream();
        using (var bw = new BinaryWriter(ms1, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            bw.WriteZigZagEncodedInt32(value);
            bw.Flush();
        }

        var ourBytes = ms1.ToArray();

        // protobuf
        using var ms2 = new MemoryStream();
        var cos = new CodedOutputStream(ms2);
        cos.WriteSInt32(value);
        cos.Flush();
        var protobufBytes = ms2.ToArray();

        Assert.Equal(expectedBytes, ourBytes);    // matches our known pattern
        Assert.Equal(ourBytes, protobufBytes);    // matches protobuf bytes
    }

    // int32: our reader can read protobuf bytes
    [Theory]
    [ClassData(typeof(BinaryWriterZigZagExtensionsTest.Int32KnownPatternsData))]
    public void Int32_Read_ProtobufBytes_With_Ours(int value, byte[] expectedBytes)
    {
        using var ms = new MemoryStream();
        var cos = new CodedOutputStream(ms);
        cos.WriteSInt32(value);
        cos.Flush();

        ms.Position = 0;
        using var br = new BinaryReader(ms);
        var read = br.ReadZigZagEncodedInt32();
        Assert.Equal(value, read);

        // protobuf bytes should match our known pattern as well
        var protobufBytes = ms.ToArray();
        Assert.Equal(expectedBytes, protobufBytes);
    }

    // int32: protobuf reader can read our bytes
    [Theory]
    [ClassData(typeof(BinaryWriterZigZagExtensionsTest.Int32KnownPatternsData))]
    public void Int32_Read_OurBytes_With_Protobuf(int value, byte[] expectedBytes)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            bw.WriteZigZagEncodedInt32(value);
            bw.Flush();
        }

        // our bytes should match known pattern
        var ourBytes = ms.ToArray();
        Assert.Equal(expectedBytes, ourBytes);

        ms.Position = 0;
        var cis = new CodedInputStream(ms);
        var read = cis.ReadSInt32();
        Assert.Equal(value, read);
    }

    // int64: our bytes == protobuf bytes
    [Theory]
    [ClassData(typeof(BinaryWriterZigZagExtensionsTest.Int64KnownPatternsData))]
    public void Int64_Bytes_Match_Protobuf(long value, byte[] expectedBytes)
    {
        // ours
        using var ms1 = new MemoryStream();
        using (var bw = new BinaryWriter(ms1, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            bw.WriteZigZagEncodedInt64(value);
            bw.Flush();
        }

        var ourBytes = ms1.ToArray();

        // protobuf
        using var ms2 = new MemoryStream();
        var cos = new CodedOutputStream(ms2);
        cos.WriteSInt64(value);
        cos.Flush();
        var protobufBytes = ms2.ToArray();

        Assert.Equal(expectedBytes, ourBytes);    // matches our known pattern
        Assert.Equal(ourBytes, protobufBytes);    // matches protobuf bytes
    }

    // int64: our reader can read protobuf bytes
    [Theory]
    [ClassData(typeof(BinaryWriterZigZagExtensionsTest.Int64KnownPatternsData))]
    public void Int64_Read_ProtobufBytes_With_Ours(long value, byte[] expectedBytes)
    {
        using var ms = new MemoryStream();
        var cos = new CodedOutputStream(ms);
        cos.WriteSInt64(value);
        cos.Flush();

        ms.Position = 0;
        using var br = new BinaryReader(ms);
        var read = br.ReadZigZagEncodedInt64();
        Assert.Equal(value, read);

        // protobuf bytes should match our known pattern as well
        var protobufBytes = ms.ToArray();
        Assert.Equal(expectedBytes, protobufBytes);
    }

    // int64: protobuf reader can read our bytes
    [Theory]
    [ClassData(typeof(BinaryWriterZigZagExtensionsTest.Int64KnownPatternsData))]
    public void Int64_Read_OurBytes_With_Protobuf(long value, byte[] expectedBytes)
    {
        using var ms = new MemoryStream();
        using (var bw = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
        {
            bw.WriteZigZagEncodedInt64(value);
            bw.Flush();
        }

        // our bytes should match known pattern
        var ourBytes = ms.ToArray();
        Assert.Equal(expectedBytes, ourBytes);

        ms.Position = 0;
        var cis = new CodedInputStream(ms);
        var read = cis.ReadSInt64();
        Assert.Equal(value, read);
    }
}
