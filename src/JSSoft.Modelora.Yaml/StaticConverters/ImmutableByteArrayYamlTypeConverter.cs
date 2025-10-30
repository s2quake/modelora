// <copyright file="ImmutableByteArrayYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace JSSoft.Modelora.Yaml.StaticConverters;

internal sealed class ImmutableByteArrayYamlTypeConverter : YamlTypeConverter<ImmutableArray<byte>>
{
    public override ImmutableArray<byte> Read(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        => ImmutableArray.Create(ByteUtility.Parse(parser.ReadStringValue()));

    public override void Write(IEmitter emitter, ImmutableArray<byte> value, Type type, ObjectSerializer serializer)
        => emitter.WriteStringValue(ByteUtility.Hex(value.AsSpan()));
}
