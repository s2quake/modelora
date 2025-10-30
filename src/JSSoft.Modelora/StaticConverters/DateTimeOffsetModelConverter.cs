// <copyright file="DateTimeOffsetModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;
using JSSoft.Modelora.Extensions;

namespace JSSoft.Modelora.StaticConverters;

internal sealed class DateTimeOffsetModelConverter : ModelConverterBase<DateTimeOffset>
{
    protected override DateTimeOffset Read(BinaryReader reader, Type type, ModelOptions options)
        => new(reader.ReadZigZagEncodedInt64(), TimeSpan.Zero);

    protected override void Write(BinaryWriter writer, DateTimeOffset value, ModelOptions options)
        => writer.WriteZigZagEncodedInt64(value.UtcTicks);
}
