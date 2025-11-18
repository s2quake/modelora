// <copyright file="ModelScalarConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Reflection;

namespace JSSoft.Modelora.DynamicConverters;

internal sealed class ModelScalarConverter : ModelConverter<object>
{
    protected override object? Read(ref ModelReader reader, Type type, ModelOptions options)
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
            ModelScalarKind.Int32 => reader.ReadInt32(),
            ModelScalarKind.Int64 => reader.ReadInt64(),
            ModelScalarKind.Hex => reader.ReadBytes(reader.ReadInt32()).ToArray(),
            _ => throw new NotSupportedException("The scalar value is of an unsupported type."),
        };
        return ModelScalarUtility.GetObjectFromScalarValue(options, type, scalarValue);
    }

    protected override void Write(ref ModelWriter writer, object value, ModelOptions options)
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
                writer.Write(i);
                break;
            case long l:
                writer.Write(l);
                break;
            case byte[] bytes:
                writer.Write(bytes.Length);
                writer.Write(bytes);
                break;
            default:
                throw new NotSupportedException("The scalar value is of an unsupported type.");
        }
    }
}
