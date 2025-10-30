// <copyright file="BigIntegerYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace JSSoft.Modelora.Yaml.StaticConverters;

internal sealed class BigIntegerYamlTypeConverter : YamlTypeConverter<BigInteger>
{
    public override BigInteger Read(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        => BigInteger.Parse(parser.ReadStringValue(), NumberFormatInfo.InvariantInfo);

    public override void Write(IEmitter emitter, BigInteger value, Type type, ObjectSerializer serializer)
    {
        if (value > long.MaxValue || value < long.MinValue)
        {
            emitter.WriteStringValue(value.ToString("D", NumberFormatInfo.InvariantInfo));
        }
        else
        {
            emitter.WriteNumberValue((long)value);
        }
    }
}
