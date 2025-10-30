// <copyright file="GuidModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;

namespace JSSoft.Modelora.StaticConverters;

internal sealed class GuidModelConverter : ModelConverterBase<Guid>
{
    protected override Guid Read(BinaryReader reader, Type type, ModelOptions options)
        => new(reader.ReadBytes(16));

    protected override void Write(BinaryWriter writer, Guid value, ModelOptions options)
        => writer.Write(value.ToByteArray());
}
