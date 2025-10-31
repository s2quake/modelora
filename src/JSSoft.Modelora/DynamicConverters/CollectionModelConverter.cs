// <copyright file="CollectionModelConverter.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections;
using System.IO;

namespace JSSoft.Modelora.DynamicConverters;

internal abstract class CollectionModelConverter : ModelConverterBase<IEnumerable>, IModelComparer
{
    protected abstract Type GenericTypeDefinition { get; }

    protected abstract bool IsDictionary { get; }

    public override bool CanConvert(Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == GenericTypeDefinition;

    public bool Equals(object obj1, object obj2, Type type)
    {
        if (obj1.GetType() != obj2.GetType())
        {
            return false;
        }

        if (type.IsValueType)
        {
            if (TypeUtility.IsDefault(obj1) && TypeUtility.IsDefault(obj2))
            {
                return true;
            }

            if (TypeUtility.IsDefault(obj1) || TypeUtility.IsDefault(obj2))
            {
                return false;
            }
        }

        if (obj1 is ICollection collection1
            && obj2 is ICollection collection2
            && collection1.Count != collection2.Count)
        {
            return false;
        }

        var items1 = (IEnumerable)obj1;
        var items2 = (IEnumerable)obj2;

        var elementType = GetElementType(type);
        var enumerator1 = items1.GetEnumerator();
        var enumerator2 = items2.GetEnumerator();
        while (enumerator1.MoveNext() && enumerator2.MoveNext())
        {
            if (!ModelResolver.Equals(enumerator1.Current, enumerator2.Current, elementType))
            {
                return false;
            }
        }

        return true;
    }

    public int GetHashCode(object obj, Type type)
    {
        if (TypeUtility.IsDefault(obj))
        {
            return 0;
        }

        var items = (IEnumerable)obj;
        var elementType = GetElementType(type);
        HashCode hash = default;
        foreach (var item in items)
        {
            hash.Add(ModelResolver.GetHashCode(item, elementType));
        }

        return hash.ToHashCode();
    }

    protected sealed override IEnumerable Read(BinaryReader reader, Type type, ModelOptions options)
    {
        var elementType = GetElementType(type);
        var length = reader.ReadInt32();
        var listInstance = CreateListInstance(elementType);
        using var _ = ModelTypeScope.Push(elementType);
        for (var i = 0; i < length; i++)
        {
            var value = ModelSerializer.Deserialize(reader, elementType, options);
            listInstance.Add(value);
        }

        return CreateInstance(type, elementType, listInstance);
    }

    protected sealed override void Write(BinaryWriter writer, IEnumerable value, ModelOptions options)
    {
        var enumerator = value.GetEnumerator();
        var elementType = GetElementType(value.GetType());
        var position = writer.BaseStream.Position;
        var length = 0;
        writer.Write(length);
        using var _ = ModelTypeScope.Push(elementType);
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            var currentType = TypeUtility.GetActualType(current, elementType);
            ModelSerializer.Serialize(writer, current, currentType, options);
            length++;
        }

        var currentPosition = writer.BaseStream.Position;
        writer.BaseStream.Position = position;
        writer.Write(length);
        writer.BaseStream.Position = currentPosition;
    }

    protected virtual IEnumerable CreateInstance(Type type, Type elementType, IList listInstance)
        => (IEnumerable)TypeUtility.CreateInstance(type, args: [listInstance]);

    protected virtual Type GetElementType(Type type)
    {
        if (CanConvert(type))
        {
            if (IsDictionary)
            {
                return typeof(KeyValuePair<,>).MakeGenericType(type.GetGenericArguments());
            }

            return type.GetGenericArguments()[0];
        }

        throw new NotSupportedException("The type is not supported.");
    }

    private static IList CreateListInstance(Type elementType)
    {
        var listType = typeof(List<>).MakeGenericType(elementType);
        return (IList)TypeUtility.CreateInstance(listType, args: [])!;
    }
}
