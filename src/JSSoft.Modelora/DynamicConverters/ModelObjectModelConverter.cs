// <copyright file="ModelObjectModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.IO;
using System.Reflection;

namespace JSSoft.Modelora.DynamicConverters;

internal sealed class ModelObjectModelConverter : ModelConverterBase<object>, IModelComparer
{
    public override bool CanConvert(Type type)
        => type.IsDefined(typeof(ModelAttribute)) || type.IsDefined(typeof(OriginModelAttribute));

    public bool Equals(object obj1, object obj2, Type type)
    {
        var properties = ModelResolver.GetProperties(type);
        foreach (var property in properties)
        {
            var value1 = property.GetValue(obj1);
            var value2 = property.GetValue(obj2);
            if (!ModelResolver.Equals(value1, value2, property.PropertyType))
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(object obj, Type type)
    {
        var properties = ModelResolver.GetProperties(type);
        HashCode hash = default;
        foreach (var property in properties)
        {
            var value = property.GetValue(obj);
            hash.Add(ModelResolver.GetHashCode(value, property.PropertyType));
        }

        return hash.ToHashCode();
    }

    protected override object? Read(BinaryReader reader, Type type, ModelOptions options)
    {
        var obj = TypeUtility.CreateInstance(type);
        var properties = ModelResolver.GetProperties(type);
        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            var propertyType = property.PropertyType;
            using var _ = ModelTypeScope.Push(property.PropertyType);
            var propertyValue = ModelSerializer.Deserialize(reader, propertyType, options);
            SetPropertyValue(property, obj, propertyValue);
        }

        if (type.GetCustomAttribute<OriginModelAttribute>() is { } originModelAttribute)
        {
            var originType = originModelAttribute.Type;
            var originVersion = ModelResolver.GetVersion(originType);
            var version = ModelResolver.GetVersion(type);
            while (version < originVersion)
            {
                var args = new object[] { obj };
                type = ModelResolver.GetType(originType, version + 1);
                obj = TypeUtility.CreateInstance(type, args: args);
                version++;
            }
        }

        if (options.IsValidationEnabled)
        {
            ModelResolver.Validate(obj, options);
        }

        return obj;
    }

    protected override void Write(BinaryWriter writer, object value, ModelOptions options)
    {
        var type = value.GetType();
        if (options.IsValidationEnabled && !TypeUtility.IsDefault(value))
        {
            ModelResolver.Validate(value, options);
        }

        if (type.GetCustomAttribute<OriginModelAttribute>() is { } originModelAttribute
            && !originModelAttribute.AllowSerialization)
        {
            var message = $"The type '{type}' is a legacy model and is not allowed to be serialized. " +
                          $"Because it is marked with 'OriginModelAttribute' with 'AllowSerialization = false'.";
            throw new ModelException(message);
        }

        var properties = ModelResolver.GetProperties(type);
        for (var i = 0; i < properties.Count; i++)
        {
            var property = properties[i];
            var propertyType = property.PropertyType;
            var propertyValue = GetPropertyValue(property, value, options);
            var propertyActualType = TypeUtility.GetActualType(propertyValue, propertyType);
            using var _ = ModelTypeScope.Push(propertyType, emitDefaultValue: property.EmitDefaultValue);
            ModelSerializer.Serialize(writer, propertyValue, propertyActualType, options);
        }
    }

    private static object? GetPropertyValue(ModelProperty property, object obj, ModelOptions options)
    {
        if (property.InspectOnly && options.Purpose is SerializationPurpose.Contract)
        {
            return null;
        }

        return property.GetValue(obj);
    }

    private static void SetPropertyValue(ModelProperty property, object obj, object? value)
    {
        if (!property.InspectOnly)
        {
            property.SetValue(obj, value);
        }
    }
}
