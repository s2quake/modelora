// <copyright file="StringYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace JSSoft.Modelora.Yaml.StaticConverters;

internal sealed class StringYamlTypeConverter : YamlTypeConverter<string>
{
    public override string? Read(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        => parser.ReadStringValue();

    public override void Write(IEmitter emitter, string value, Type type, ObjectSerializer serializer)
        => emitter.WriteStringValue(value);
}
