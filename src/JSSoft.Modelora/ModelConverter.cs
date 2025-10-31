// <copyright file="ModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type
using System.Diagnostics;
using System.IO;

namespace JSSoft.Modelora;

public abstract class ModelConverter(Type type) : IModelConverter
{
    public Type Type { get; } = type;

    bool IModelConverter.CanConvert(Type type) => type == Type;

    object? IModelConverter.Read(BinaryReader reader, Type type, ModelOptions options)
        => Read(reader, type, options);

    void IModelConverter.Write(BinaryWriter writer, object value, ModelOptions options)
        => Write(writer, value, options);

    protected abstract object? Read(BinaryReader reader, Type type, ModelOptions options);

    protected abstract void Write(BinaryWriter writer, object value, ModelOptions options);
}

public abstract class ModelConverterBase<T> : IModelConverter
    where T : notnull
{
    public Type Type { get; } = typeof(T);

    public virtual bool CanConvert(Type type) => type == Type;

    object? IModelConverter.Read(BinaryReader reader, Type type, ModelOptions options)
        => Read(reader, type, options);

    void IModelConverter.Write(BinaryWriter writer, object value, ModelOptions options)
    {
        if (value is T t)
        {
            Write(writer, t, options);
        }
        else
        {
            throw new UnreachableException("The object is not of the expected type.");
        }
    }

    protected abstract T? Read(BinaryReader reader, Type type, ModelOptions options);

    protected abstract void Write(BinaryWriter writer, T value, ModelOptions options);
}
