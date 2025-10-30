// <copyright file="GuidYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace JSSoft.Modelora.Yaml.StaticConverters;

internal sealed class GuidYamlTypeConverter : YamlTypeConverter<Guid>
{
    public override Guid Read(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        => Guid.Parse(parser.ReadStringValue());

    public override void Write(IEmitter emitter, Guid value, Type type, ObjectSerializer serializer)
        => emitter.WriteStringValue(value.ToString("D", CultureInfo.InvariantCulture));
}
