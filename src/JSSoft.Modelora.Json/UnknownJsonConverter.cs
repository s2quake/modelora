// <copyright file="UnknownJsonConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Text.Json;
using System.Text.Json.Serialization;

namespace JSSoft.Modelora.Json;

internal sealed class UnknownJsonConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Cannot find type information.");
        }

        var readerBackup = reader;
        reader.Expect(JsonTokenType.StartObject);
        reader.Read();
        var typeName = reader.ReadString("type");
        var version = reader.ReadInt32("version");
        var type = TypeUtility.GetType(typeName);
        var modelType = ModelResolver.GetType(type, version);
        reader = readerBackup;

        var obj = JsonSerializer.Deserialize(ref reader, modelType, options);
        reader.Read();
        reader.Expect(JsonTokenType.EndObject);
        return obj;
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        throw new NotSupportedException($"Type '{value.GetType()}' is not supported.");
    }
}
