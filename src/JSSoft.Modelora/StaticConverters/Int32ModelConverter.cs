// <copyright file="Int32ModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.StaticConverters;

internal sealed class Int32ModelConverter : ModelConverter<int>
{
    protected override int Read(ref ModelReader reader, Type type, ModelOptions options)
        => reader.ReadInt32();

    protected override void Write(ref ModelWriter writer, int value, ModelOptions options)
        => writer.Write(value);
}
