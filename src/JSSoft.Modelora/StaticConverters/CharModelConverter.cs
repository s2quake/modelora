// <copyright file="CharModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.StaticConverters;

internal sealed class CharModelConverter : ModelConverter<char>
{
    protected override char Read(ref ModelReader reader, Type type, ModelOptions options) => reader.ReadChar();

    protected override void Write(ref ModelWriter writer, char value, ModelOptions options) => writer.Write(value);
}
