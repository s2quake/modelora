// <copyright file="EnumJsonConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Text.Json;
using System.Text.Json.Serialization;

namespace JSSoft.Modelora.Json.DynamicConverters;

internal sealed class EnumJsonConverter : JsonConverter<object>
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is { } s)
        {
            if (Enum.TryParse(typeToConvert, s, out object? value))
            {
                return value;
            }

            throw new JsonException($"Failed to parse '{s}' to enum '{typeToConvert}'.");
        }

        throw new JsonException("Enum value is null");
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString() ?? throw new JsonException("Enum value is null"));
}
