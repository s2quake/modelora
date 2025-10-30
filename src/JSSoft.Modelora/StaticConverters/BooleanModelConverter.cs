// <copyright file="BooleanModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;

namespace JSSoft.Modelora.StaticConverters;

internal sealed class BooleanModelConverter : ModelConverterBase<bool>
{
    protected override bool Read(BinaryReader reader, Type type, ModelOptions options) => reader.ReadBoolean();

    protected override void Write(BinaryWriter writer, bool value, ModelOptions options) => writer.Write(value);
}
