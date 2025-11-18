// <copyright file="DateTimeOffsetModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.StaticConverters;

internal sealed class DateTimeOffsetModelConverter : ModelConverter<DateTimeOffset>
{
    protected override DateTimeOffset Read(ref ModelReader reader, Type type, ModelOptions options)
        => new(reader.ReadInt64(), TimeSpan.Zero);

    protected override void Write(ref ModelWriter writer, DateTimeOffset value, ModelOptions options)
        => writer.Write(value.UtcTicks);
}
