// <copyright file="ModelScalarConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;
using System.Reflection;
using JSSoft.Modelora.Extensions;

namespace JSSoft.Modelora.DynamicConverters;

internal sealed class ModelScalarConverter : ModelConverterBase<object>
{
    protected override object? Read(BinaryReader reader, Type type, ModelOptions options)
    {
        if (type.GetCustomAttribute<ModelScalarAttribute>() is not { } attribute)
        {
            var message = $"Type '{type}' does not have the {nameof(ModelScalarAttribute)}";
            throw new ArgumentException(message, nameof(type));
        }

        var kind = attribute.Kind;
        object scalarValue = kind switch
        {
            ModelScalarKind.String => reader.ReadString(),
            ModelScalarKind.Boolean => reader.ReadBoolean(),
            ModelScalarKind.Int32 => reader.ReadZigZagEncodedInt32(),
            ModelScalarKind.Int64 => reader.ReadZigZagEncodedInt64(),
            ModelScalarKind.Hex => reader.ReadBytes(reader.ReadZigZagEncodedInt32()),
            _ => throw new NotSupportedException("The scalar value is of an unsupported type."),
        };
        return ModelScalarUtility.GetObjectFromScalarValue(options, type, scalarValue);
    }

    protected override void Write(BinaryWriter writer, object value, ModelOptions options)
    {
        var scalarValue = ModelScalarUtility.GetScalarValue(value);
        switch (scalarValue)
        {
            case string s:
                writer.Write(s);
                break;
            case bool b:
                writer.Write(b);
                break;
            case int i:
                writer.WriteZigZagEncodedInt32(i);
                break;
            case long l:
                writer.WriteZigZagEncodedInt64(l);
                break;
            case byte[] bytes:
                writer.WriteZigZagEncodedInt32(bytes.Length);
                writer.Write(bytes);
                break;
            default:
                throw new NotSupportedException("The scalar value is of an unsupported type.");
        }
    }
}
