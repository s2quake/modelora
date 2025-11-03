// <copyright file="ByteUtilityTest.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.Tests;

public sealed class ByteUtilityTest
{
    [Fact]
    public void GetHashCode_Test()
    {
        var bytes1 = new byte[] { 0x01, 0x02, 0x03 };
        var bytes2 = bytes1.ToImmutableArray();
        var hashCode1 = ByteUtility.GetHashCode(bytes1);
        var hashCode2 = ByteUtility.GetHashCode(bytes2);
        Assert.NotEqual(0, hashCode1);
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void Hex_Test()
    {
        var bytes1 = new byte[] { 0x01, 0x02, 0x03 };
        var bytes2 = bytes1.ToImmutableArray();
        var hex1 = ByteUtility.Hex(bytes1);
        var hex2 = ByteUtility.Hex(bytes2);
        Assert.Equal("010203", hex1);
        Assert.Equal(hex1, hex2);
    }

    [Fact]
    public void Parse_Test()
    {
        var hex = "010203";
        var bytes = ByteUtility.Parse(hex);
        Assert.Equal(new byte[] { 0x01, 0x02, 0x03 }, bytes);
    }

    [Fact]
    public void Parse_InvalidLength_Test()
    {
        var hex = "01020";
        Assert.Throws<FormatException>(() => ByteUtility.Parse(hex));
    }
}
