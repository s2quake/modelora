// <copyright file="BooleanYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace JSSoft.Modelora.Yaml.StaticConverters;

internal sealed class BooleanYamlTypeConverter : YamlTypeConverter<bool>
{
    public override bool Read(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        => parser.ReadBooleanValue();

    public override void Write(IEmitter emitter, bool value, Type type, ObjectSerializer serializer)
        => emitter.WriteBooleanValue(value);
}
