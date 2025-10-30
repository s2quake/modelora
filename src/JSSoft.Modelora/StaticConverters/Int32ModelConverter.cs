// <copyright file="Int32ModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;
using JSSoft.Modelora.Extensions;

namespace JSSoft.Modelora.StaticConverters;

internal sealed class Int32ModelConverter : ModelConverterBase<int>
{
    protected override int Read(BinaryReader reader, Type type, ModelOptions options)
        => reader.ReadZigZagEncodedInt32();

    protected override void Write(BinaryWriter writer, int value, ModelOptions options)
        => writer.WriteZigZagEncodedInt32(value);
}
