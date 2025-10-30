// <copyright file="EnumModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;
using JSSoft.Modelora.Extensions;

namespace JSSoft.Modelora.DynamicConverters;

internal sealed class EnumModelConverter : ModelConverterBase<object>
{
    public override bool CanConvert(Type type) => type.IsEnum;

    protected override object? Read(BinaryReader reader, Type type, ModelOptions options)
        => reader.ReadEnum(type);

    protected override void Write(BinaryWriter writer, object value, ModelOptions options)
        => writer.WriteEnum(value, value.GetType());
}
