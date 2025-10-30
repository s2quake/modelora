// <copyright file="ScalarValueYamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Reflection;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using static JSSoft.Modelora.ModelScalarUtility;

namespace JSSoft.Modelora.Yaml.DynamicConverters;

internal sealed class ScalarValueYamlTypeConverter : IYamlTypeConverter
{
    public bool Accepts(Type type)
        => type.IsDefined(typeof(ModelScalarAttribute));

    public object? ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
    {
        var modelOptions = ModelOptionsScope.Current;
        var scalarKind = GetScalarKind(type);
        var value = ReadValue(parser, scalarKind);

        return GetObjectFromScalarValue(modelOptions, type, value);

        static object ReadValue(IParser parser, ModelScalarKind scalarKind)
        {
            return scalarKind switch
            {
                ModelScalarKind.String => parser.ReadStringValue(),
                ModelScalarKind.Boolean => parser.ReadBooleanValue(),
                ModelScalarKind.Int32 => parser.ReadInt32Value(),
                ModelScalarKind.Int64 => parser.ReadInt64Value(),
                ModelScalarKind.Hex => ByteUtility.Parse(parser.ReadStringValue()),
                _ => throw new NotSupportedException($"The scalar kind '{scalarKind}' is not supported."),
            };
        }
    }

    public void WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var scalarValue = GetScalarValue(value);
        if (scalarValue is string stringValue)
        {
            emitter.WriteStringValue(stringValue);
        }
        else if (scalarValue is bool boolValue)
        {
            emitter.WriteBooleanValue(boolValue);
        }
        else if (scalarValue is int int32Value)
        {
            emitter.WriteNumberValue(int32Value);
        }
        else if (scalarValue is long int64Value)
        {
            emitter.WriteNumberValue(int64Value);
        }
        else if (scalarValue is byte[] byteArrayValue)
        {
            emitter.WriteStringValue(ByteUtility.Hex(byteArrayValue));
        }
        else
        {
            throw new NotSupportedException($"The scalar value type '{scalarValue.GetType()}' is not supported.");
        }
    }
}
