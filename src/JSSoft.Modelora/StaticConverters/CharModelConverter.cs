// <copyright file="CharModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;

namespace JSSoft.Modelora.StaticConverters;

internal sealed class CharModelConverter : ModelConverterBase<char>
{
    protected override char Read(BinaryReader reader, Type type, ModelOptions options) => reader.ReadChar();

    protected override void Write(BinaryWriter writer, char value, ModelOptions options) => writer.Write(value);
}
