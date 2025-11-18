// <copyright file="NullableModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

namespace JSSoft.Modelora.DynamicConverters;

internal sealed class NullableModelConverter : IModelConverter
{
    public bool CanConvert(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    public object? Read(ref ModelReader reader, Type type, ModelOptions options)
    {
        if (Nullable.GetUnderlyingType(type) is not { } underlyingType)
        {
            throw new ModelException($"Type '{type}' is not a nullable type.");
        }

        var converter = ModelResolver.GetConverter(underlyingType);
        return converter.Read(ref reader, underlyingType, options);
    }

    public void Write(ref ModelWriter writer, object value, ModelOptions options)
    {
        var type = value.GetType();
        var converter = ModelResolver.GetConverter(type);
        converter.Write(ref writer, value, options);
    }
}
