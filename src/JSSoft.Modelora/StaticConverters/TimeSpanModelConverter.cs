// <copyright file="TimeSpanModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;
using JSSoft.Modelora.Extensions;

namespace JSSoft.Modelora.StaticConverters;

internal sealed class TimeSpanModelConverter : ModelConverterBase<TimeSpan>
{
    protected override TimeSpan Read(BinaryReader reader, Type type, ModelOptions options)
        => new(reader.ReadZigZagEncodedInt64());

    protected override void Write(BinaryWriter writer, TimeSpan value, ModelOptions options)
        => writer.WriteZigZagEncodedInt64(value.Ticks);
}
