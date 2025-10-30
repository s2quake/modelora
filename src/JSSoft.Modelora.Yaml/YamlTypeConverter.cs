// <copyright file="YamlTypeConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace JSSoft.Modelora.Yaml;

internal abstract class YamlTypeConverter<T> : IYamlTypeConverter
    where T : notnull
{
    public virtual bool Accepts(Type type) => type == typeof(T);

    public abstract T? Read(IParser parser, Type type, ObjectDeserializer rootDeserializer);

    public abstract void Write(IEmitter emitter, T value, Type type, ObjectSerializer serializer);

    object? IYamlTypeConverter.ReadYaml(IParser parser, Type type, ObjectDeserializer rootDeserializer)
        => Read(parser, type, rootDeserializer);

    void IYamlTypeConverter.WriteYaml(IEmitter emitter, object? value, Type type, ObjectSerializer serializer)
    {
        if (value is T typedValue)
        {
            Write(emitter, typedValue, type, serializer);
        }
        else
        {
            throw new ArgumentException($"Expected {typeof(T)}, but got {value?.GetType()}.", nameof(value));
        }
    }
}
