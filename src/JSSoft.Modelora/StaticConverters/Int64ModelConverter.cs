// <copyright file="Int64ModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.StaticConverters;

internal sealed class Int64ModelConverter : ModelConverter<long>
{
    protected override long Read(ref ModelReader reader, Type type, ModelOptions options)
        => reader.ReadInt64();

    protected override void Write(ref ModelWriter writer, long value, ModelOptions options)
        => writer.Write(value);
}
