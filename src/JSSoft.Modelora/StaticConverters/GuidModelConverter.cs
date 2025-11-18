// <copyright file="GuidModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.StaticConverters;

internal sealed class GuidModelConverter : ModelConverter<Guid>
{
    protected override Guid Read(ref ModelReader reader, Type type, ModelOptions options)
        => reader.ReadGuid();

    protected override void Write(ref ModelWriter writer, Guid value, ModelOptions options)
        => writer.Write(value);
}
