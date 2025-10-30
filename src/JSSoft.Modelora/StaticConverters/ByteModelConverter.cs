// <copyright file="ByteModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;

namespace JSSoft.Modelora.StaticConverters;

internal sealed class ByteModelConverter : ModelConverterBase<byte>
{
    protected override byte Read(BinaryReader reader, Type type, ModelOptions options) => reader.ReadByte();

    protected override void Write(BinaryWriter writer, byte value, ModelOptions options) => writer.Write(value);
}
