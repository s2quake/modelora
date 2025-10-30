// <copyright file="DateTimeOffsetYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace JSSoft.Modelora.Yaml.StaticConverters;

internal sealed class DateTimeOffsetYamlTypeConverter : YamlTypeConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        => DateTimeOffset.Parse(parser.ReadStringValue(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

    public override void Write(IEmitter emitter, DateTimeOffset value, Type type, ObjectSerializer serializer)
        => emitter.WriteStringValue(value.ToString("o", CultureInfo.InvariantCulture));
}
