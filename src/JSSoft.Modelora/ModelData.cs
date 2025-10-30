// <copyright file="ModelData.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;
using JSSoft.Modelora.Extensions;

namespace JSSoft.Modelora;

internal sealed record class ModelData
{
    public ModelData()
    {
    }

    public ModelData(Type type)
    {
        (TypeName, Version) = ModelResolver.GetTypeInfo(type);
    }

    public string TypeName { get; init; } = string.Empty;

    public int Version { get; set; }

    public void Write(BinaryWriter writer)
    {
        writer.Write(TypeName);
        writer.WriteZigZagEncodedInt32(Version);
    }

    public static ModelData GetData(BinaryReader reader) => new()
    {
        TypeName = reader.ReadString(),
        Version = reader.ReadZigZagEncodedInt32(),
    };
}
