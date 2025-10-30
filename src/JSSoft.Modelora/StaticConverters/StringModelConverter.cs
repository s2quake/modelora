// <copyright file="StringModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;

namespace JSSoft.Modelora.StaticConverters;

internal sealed class StringModelConverter : ModelConverterBase<string>
{
    protected override string? Read(BinaryReader reader, Type type, ModelOptions options)
        => reader.ReadString();

    protected override void Write(BinaryWriter writer, string value, ModelOptions options)
        => writer.Write(value);
}
