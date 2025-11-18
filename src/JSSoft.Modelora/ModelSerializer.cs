// <copyright file="ModelSerializer.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Buffers;
using static JSSoft.Modelora.ModelResolver;

namespace JSSoft.Modelora;

public static class ModelSerializer
{
    private enum DataType : byte
    {
        Null,

        Default,

        Value,

        Header,
    }

    public static byte[] Serialize<T>(T? obj)
        where T : notnull
        => Serialize(obj, ModelOptions.Empty);

    public static byte[] Serialize<T>(T? obj, ModelOptions options)
        where T : notnull
        => Serialize(obj, obj?.GetType() ?? typeof(T), options);

    public static byte[] Serialize(object? obj, Type type) => Serialize(obj, type, ModelOptions.Empty);

    public static byte[] Serialize(object? obj, Type type, ModelOptions options)
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new ModelWriter(buffer);
        Serialize(ref writer, obj, type, options);
        return writer.ToByteArray();
    }

    public static void Serialize(ref ModelWriter writer, object? value, Type type, ModelOptions options)
    {
        if (value is null)
        {
            writer.Write((byte)DataType.Null);
            return;
        }

        if (!ModelTypeScope.CanOmitTypeInfo(type, options))
        {
            var data = new ModelData(type);
            writer.Write((byte)DataType.Header);
            data.Write(ref writer);
        }

        if (TypeUtility.IsDefault(value) && ModelTypeScope.CanWriteDefaultValue(options))
        {
            writer.Write((byte)DataType.Default);
        }
        else if (TryGetConverter(type, out var converter))
        {
            writer.Write((byte)DataType.Value);
            SerializeByConverter(ref writer, value, options, converter);
        }
        else
        {
            throw new ModelException($"Unsupported type {type}");
        }
    }

    public static object? Deserialize(ReadOnlySpan<byte> bytes) => Deserialize(bytes, ModelOptions.Empty);

    public static object? Deserialize(ReadOnlySpan<byte> bytes, ModelOptions options)
        => Deserialize(bytes, typeof(object), options);

    public static object? Deserialize(ReadOnlySpan<byte> bytes, Type type)
        => Deserialize(bytes, type, ModelOptions.Empty);

    public static object? Deserialize(ReadOnlySpan<byte> bytes, Type type, ModelOptions options)
    {
        var sequence = new ReadOnlySequence<byte>(bytes.ToArray());
        var reader = new ModelReader(sequence);
        return Deserialize(ref reader, type, options);
    }

    public static object? Deserialize(ref ModelReader reader, Type type, ModelOptions options)
    {
        var dataType = (DataType)reader.ReadByte();
        if (dataType == DataType.Null)
        {
            return null;
        }

        if (ModelTypeScope.CanOmitTypeInfo(type, options) && dataType == DataType.Header)
        {
            throw new ModelException("Type information is required but not found.");
        }

        var actualType = type;
        if (dataType == DataType.Header)
        {
            var header = ModelData.GetData(ref reader);
            var headerType = TypeUtility.GetType(header.TypeName);
            actualType = ModelResolver.GetType(headerType, header.Version);
            dataType = (DataType)reader.ReadByte();
        }

        if (actualType == typeof(object))
        {
            throw new ModelException("Type information is not found. Specify the exact type explicitly.");
        }

        if (dataType == DataType.Default)
        {
            return TypeUtility.GetDefault(actualType);
        }
        else if (dataType == DataType.Value && TryGetConverter(actualType, out var converter))
        {
            return converter.Read(ref reader, actualType, options);
        }

        throw new ModelException($"Invalid data type {actualType}.");
    }

    public static T Deserialize<T>(ref ModelReader reader, ModelOptions options)
        where T : notnull
    {
        if (Deserialize(ref reader, typeof(T), options) is T obj)
        {
            return obj;
        }

        throw new ModelException($"Failed to deserialize {typeof(T)}.");
    }

    public static T Deserialize<T>(ReadOnlySpan<byte> bytes)
        where T : notnull
        => Deserialize<T>(bytes, ModelOptions.Empty);

    public static T Deserialize<T>(ReadOnlySpan<byte> bytes, ModelOptions options)
        where T : notnull
    {
        var sequence = new ReadOnlySequence<byte>(bytes.ToArray());
        var reader = new ModelReader(sequence);
        return Deserialize<T>(ref reader, options);
    }

    public static T Clone<T>(T obj)
        where T : notnull
        => Clone(obj, ModelOptions.Empty);

    public static T Clone<T>(T obj, ModelOptions options)
        where T : notnull
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new ModelWriter(buffer);
        Serialize(ref writer, obj, typeof(T), options);
        var reader = new ModelReader(new ReadOnlySequence<byte>(buffer.WrittenMemory));
        return Deserialize<T>(ref reader, options);
    }

    private static void SerializeByConverter(
        ref ModelWriter writer, object obj, ModelOptions options, IModelConverter converter)
    {
        try
        {
            converter.Write(ref writer, obj, options);
        }
        catch (Exception e) when (e is not ModelException)
        {
            var message = $"An exception occurred while serializing {obj.GetType()} by {converter.GetType()}.";
            throw new ModelException(message, e);
        }
    }
}
