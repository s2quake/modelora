// <copyright file="IModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;

namespace JSSoft.Modelora;

public interface IModelConverter
{
    bool CanConvert(Type type);

    void Write(BinaryWriter writer, object value, ModelOptions options);

    object? Read(BinaryReader reader, Type type, ModelOptions options);
}
