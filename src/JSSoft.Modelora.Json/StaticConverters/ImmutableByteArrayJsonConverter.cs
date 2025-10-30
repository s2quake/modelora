// <copyright file="ImmutableByteArrayJsonConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Text.Json;
using System.Text.Json.Serialization;

namespace JSSoft.Modelora.Json.StaticConverters;

internal sealed class ImmutableByteArrayJsonConverter : JsonConverter<ImmutableArray<byte>>
{
    public override ImmutableArray<byte> Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.GetString() is not { } hex)
        {
            throw new JsonException("Expected a string.");
        }

        return ImmutableArray.Create(ByteUtility.Parse(hex));
    }

    public override void Write(Utf8JsonWriter writer, ImmutableArray<byte> value, JsonSerializerOptions options)
        => writer.WriteStringValue(ByteUtility.Hex(value.AsSpan()));
}
