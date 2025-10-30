// <copyright file="CharJsonConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Text.Json;
using System.Text.Json.Serialization;

namespace JSSoft.Modelora.Json.StaticConverters;

internal sealed class CharJsonConverter : JsonConverter<char>
{
    public override char Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetString() is string s && s.Length == 1
            ? s[0]
            : throw new JsonException("Expected a single character string.");

    public override void Write(Utf8JsonWriter writer, char value, JsonSerializerOptions options)
    {
        if (char.IsSurrogate(value))
        {
            throw new JsonException("Cannot serialize a surrogate character.");
        }

        writer.WriteStringValue(value.ToString());
    }
}
