// <copyright file="NullableModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;

namespace JSSoft.Modelora.DynamicConverters;

internal sealed class NullableModelConverter : IModelConverter
{
    public bool CanConvert(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    public object? Read(BinaryReader reader, Type type, ModelOptions options)
    {
        if (Nullable.GetUnderlyingType(type) is not { } underlyingType)
        {
            throw new ModelException($"Type '{type}' is not a nullable type.");
        }

        if (!ModelResolver.TryGetConverter(underlyingType, out var converter))
        {
            throw new ModelException($"Unsupported type {underlyingType}");
        }

        return converter.Read(reader, underlyingType, options);
    }

    public void Write(BinaryWriter writer, object value, ModelOptions options)
    {
        var type = value.GetType();
        if (!ModelResolver.TryGetConverter(type, out var converter))
        {
            throw new ModelException($"Unsupported type {type}");
        }

        converter.Write(writer, value, options);
    }
}
