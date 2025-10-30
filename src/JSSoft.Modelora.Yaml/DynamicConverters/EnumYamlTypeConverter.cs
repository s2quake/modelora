// <copyright file="EnumYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace JSSoft.Modelora.Yaml.DynamicConverters;

internal sealed class EnumYamlTypeConverter : YamlTypeConverter<object>
{
    public override bool Accepts(Type type) => type.IsEnum;

    public override object? Read(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        => Enum.Parse(type, parser.ReadStringValue());

    public override void Write(IEmitter emitter, object value, Type type, ObjectSerializer serializer)
        => emitter.WriteStringValue($"{value}");
}
