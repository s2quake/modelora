// <copyright file="Int32YamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace JSSoft.Modelora.Yaml.StaticConverters;

internal sealed class Int32YamlTypeConverter : YamlTypeConverter<int>
{
    public override int Read(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        => parser.ReadInt32Value();

    public override void Write(IEmitter emitter, int value, Type type, ObjectSerializer serializer)
        => emitter.WriteNumberValue(value);
}
