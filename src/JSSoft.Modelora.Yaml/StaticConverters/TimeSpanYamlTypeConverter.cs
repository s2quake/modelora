// <copyright file="TimeSpanYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace JSSoft.Modelora.Yaml.StaticConverters;

internal sealed class TimeSpanYamlTypeConverter : YamlTypeConverter<TimeSpan>
{
    public override TimeSpan Read(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        => TimeSpan.FromTicks(parser.ReadInt64Value());

    public override void Write(IEmitter emitter, TimeSpan value, Type type, ObjectSerializer serializer)
        => emitter.WriteNumberValue(value.Ticks);
}
