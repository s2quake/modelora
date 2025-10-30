// <copyright file="Int64ModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;
using JSSoft.Modelora.Extensions;

namespace JSSoft.Modelora.StaticConverters;

internal sealed class Int64ModelConverter : ModelConverterBase<long>
{
    protected override long Read(BinaryReader reader, Type type, ModelOptions options)
        => reader.ReadZigZagEncodedInt64();

    protected override void Write(BinaryWriter writer, long value, ModelOptions options)
        => writer.WriteZigZagEncodedInt64(value);
}
