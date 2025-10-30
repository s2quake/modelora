// <copyright file="ByteYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace JSSoft.Modelora.Yaml.StaticConverters;

internal sealed class ByteYamlTypeConverter : YamlTypeConverter<byte>
{
    public override byte Read(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        => parser.ReadByteValue();

    public override void Write(IEmitter emitter, byte value, Type type, ObjectSerializer serializer)
        => emitter.WriteNumberValue(value);
}
