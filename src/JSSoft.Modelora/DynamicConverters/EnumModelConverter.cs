// <copyright file="EnumModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.DynamicConverters;

internal sealed class EnumModelConverter : ModelConverter<object>
{
    public override bool CanConvert(Type type) => type.IsEnum;

    protected override object? Read(ref ModelReader reader, Type type, ModelOptions options)
        => reader.ReadEnum(type);

    protected override void Write(ref ModelWriter writer, object value, ModelOptions options)
        => writer.WriteEnum(value, value.GetType());
}
