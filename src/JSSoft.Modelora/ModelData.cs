// <copyright file="ModelData.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

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

    public void Write(ref ModelWriter writer)
    {
        writer.Write(TypeName);
        writer.Write(Version);
    }

    public static ModelData GetData(ref ModelReader reader) => new()
    {
        TypeName = reader.ReadString(),
        Version = reader.ReadInt32(),
    };
}
