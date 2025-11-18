// <copyright file="ModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

#pragma warning disable SA1402 // File may only contain a single type
using System.Diagnostics;

namespace JSSoft.Modelora;

public abstract class ModelConverter(Type type) : IModelConverter
{
    public Type Type { get; } = type;

    bool IModelConverter.CanConvert(Type type) => type == Type;

    object? IModelConverter.Read(ref ModelReader reader, Type type, ModelOptions options)
        => Read(ref reader, type, options);

    void IModelConverter.Write(ref ModelWriter writer, object value, ModelOptions options)
        => Write(ref writer, value, options);

    protected abstract object? Read(ref ModelReader reader, Type type, ModelOptions options);

    protected abstract void Write(ref ModelWriter writer, object value, ModelOptions options);
}

public abstract class ModelConverter<T> : IModelConverter
    where T : notnull
{
    public Type Type { get; } = typeof(T);

    public virtual bool CanConvert(Type type) => type == Type;

    object? IModelConverter.Read(ref ModelReader reader, Type type, ModelOptions options)
        => Read(ref reader, type, options);

    void IModelConverter.Write(ref ModelWriter writer, object value, ModelOptions options)
    {
        if (value is T t)
        {
            Write(ref writer, t, options);
        }
        else
        {
            throw new UnreachableException("The object is not of the expected type.");
        }
    }

    protected abstract T? Read(ref ModelReader reader, Type type, ModelOptions options);

    protected abstract void Write(ref ModelWriter writer, T value, ModelOptions options);
}
