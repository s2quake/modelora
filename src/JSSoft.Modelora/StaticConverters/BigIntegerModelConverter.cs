// <copyright file="BigIntegerModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;

namespace JSSoft.Modelora.StaticConverters;

internal sealed class BigIntegerModelConverter : ModelConverterBase<BigInteger>
{
    protected override BigInteger Read(BinaryReader reader, Type type, ModelOptions options)
    {
        var length = reader.ReadInt32();
        return new BigInteger(reader.ReadBytes(length));
    }

    protected override void Write(BinaryWriter writer, BigInteger value, ModelOptions options)
    {
        var bytes = value.ToByteArray();
        writer.Write(bytes.Length);
        writer.Write(bytes, 0, bytes.Length);
    }
}
