// <copyright file="TimeSpanModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.StaticConverters;

internal sealed class TimeSpanModelConverter : ModelConverter<TimeSpan>
{
    protected override TimeSpan Read(ref ModelReader reader, Type type, ModelOptions options)
        => new(reader.ReadInt64());

    protected override void Write(ref ModelWriter writer, TimeSpan value, ModelOptions options)
        => writer.Write(value.Ticks);
}
