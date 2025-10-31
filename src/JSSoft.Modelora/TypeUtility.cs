// <copyright file="TypeUtility.cs" company="JSSoft">
//   Copyright (c) 2025 Jeesu Choi. All Rights Reserved.
//   Licensed under the MIT License. See LICENSE.md in the project root for license information.
// </copyright>

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JSSoft.Modelora;

public static class TypeUtility
{
    private static readonly ConcurrentDictionary<string, Type> _typeByFullName = [];
    private static readonly ConcurrentDictionary<Type, object> _defaultByType = [];
    private static readonly KnownTypes _knownTypes = new();
    private static readonly HashSet<Assembly> _addedAssemblies = [];
    private static readonly object _lock = new();

    static TypeUtility()
    {
        _knownTypes.AddType(typeof(object), "obj");
        _knownTypes.AddType(typeof(BigInteger), "bi");
        _knownTypes.AddType(typeof(bool), "b");
        _knownTypes.AddType(typeof(byte), "y");
        _knownTypes.AddType(typeof(char), "c");
        _knownTypes.AddType(typeof(DateTimeOffset), "dt");
        _knownTypes.AddType(typeof(Guid), "id");
        _knownTypes.AddType(typeof(int), "i");
        _knownTypes.AddType(typeof(long), "l");
        _knownTypes.AddType(typeof(string), "s");
        _knownTypes.AddType(typeof(TimeSpan), "ts");
        _knownTypes.AddType(typeof(Array), "ar");
        _knownTypes.AddType(typeof(List<>), "li<>");
        _knownTypes.AddType(typeof(HashSet<>), "hs<>");
        _knownTypes.AddType(typeof(SortedSet<>), "shs<>");
        _knownTypes.AddType(typeof(Dictionary<,>), "di<,>");
        _knownTypes.AddType(typeof(SortedDictionary<,>), "sdi<,>");
        _knownTypes.AddType(typeof(ImmutableArray<>), "imar<>");
        _knownTypes.AddType(typeof(ImmutableList<>), "imli<>");
        _knownTypes.AddType(typeof(ImmutableHashSet<>), "imhs<>");
        _knownTypes.AddType(typeof(ImmutableSortedSet<>), "imss<>");
        _knownTypes.AddType(typeof(ImmutableDictionary<,>), "imdi<,>");
        _knownTypes.AddType(typeof(ImmutableSortedDictionary<,>), "imsd<,>");
        _knownTypes.AddType(typeof(Tuple<,>), "tp<,>");
        _knownTypes.AddType(typeof(Tuple<,,>), "tp<,,>");
        _knownTypes.AddType(typeof(Tuple<,,,>), "tp<,,,>");
        _knownTypes.AddType(typeof(Tuple<,,,,>), "tp<,,,,>");
        _knownTypes.AddType(typeof(Tuple<,,,,,>), "tp<,,,,,>");
        _knownTypes.AddType(typeof(Tuple<,,,,,,>), "tp<,,,,,,>");
        _knownTypes.AddType(typeof(Tuple<,,,,,,,>), "tp<,,,,,,,>");
        _knownTypes.AddType(typeof(ValueTuple<,>), "vtp<,>");
        _knownTypes.AddType(typeof(ValueTuple<,,>), "vtp<,,>");
        _knownTypes.AddType(typeof(ValueTuple<,,,>), "vtp<,,,>");
        _knownTypes.AddType(typeof(ValueTuple<,,,,>), "vtp<,,,,>");
        _knownTypes.AddType(typeof(ValueTuple<,,,,,>), "vtp<,,,,,>");
        _knownTypes.AddType(typeof(ValueTuple<,,,,,,>), "vtp<,,,,,,>");
        _knownTypes.AddType(typeof(ValueTuple<,,,,,,,>), "vtp<,,,,,,,>");

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            AddAssembly(assembly);
        }

        AppDomain.CurrentDomain.AssemblyLoad += (sender, args) =>
        {
            if (args.LoadedAssembly is not null)
            {
                AddAssembly(args.LoadedAssembly);
            }
        };
    }

    public static Type GetType(string typeName)
    {
        var match = Regex.Match(
            typeName, @"^(?<type>[a-zA-Z][a-zA-Z0-9_]*)(?:(?<array>\[,*\])|(?<generic>\<.+\>))?(?<nullable>\??)$");
        if (!match.Success)
        {
            throw new ArgumentException($"Invalid type name: {typeName}", nameof(typeName));
        }

        var isNullable = match.Groups["nullable"].Value == "?";
        var name = match.Groups["type"].Value;
        var genericPart = Regex.Replace(match.Groups["generic"].Value, "^<(.+)>$", "$1");
        var arrayPart = match.Groups["array"].Value;

        if (genericPart != string.Empty)
        {
            var genericArgumentNames = genericPart.Contains('<') ? [genericPart] : genericPart.Split(',');
            var genericArgumentList = new List<Type>(genericArgumentNames.Length);
            var separators = string.Empty.PadRight(genericArgumentNames.Length - 1, ',');
            var typeDefinitionName = $"{name}<{separators}>";
            var typeDefinition = _knownTypes.GetType(typeDefinitionName);
            foreach (var genericArgumentName in genericArgumentNames)
            {
                var genericArgument = GetType(genericArgumentName);
                genericArgumentList.Add(genericArgument);
            }

            var type = typeDefinition.MakeGenericType([.. genericArgumentList]);
            if (isNullable)
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }

            return type;
        }
        else if (arrayPart != string.Empty)
        {
            var elementTypeName = name;
            var elementType = _knownTypes.GetType(elementTypeName);
            if (isNullable)
            {
                elementType = typeof(Nullable<>).MakeGenericType(elementType);
            }

            var rank = arrayPart.Count(c => c == ',') + 1;
            return Array.CreateInstance(elementType, new int[rank]).GetType();
        }
        else
        {
            var type = _knownTypes.GetType(name);
            if (isNullable)
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }

            return type;
        }
    }

    public static bool TryGetType(string typeName, [MaybeNullWhen(false)] out Type type)
    {
        if (!_typeByFullName.TryGetValue(typeName, out type))
        {
            type = Type.GetType(typeName);
            if (type is not null)
            {
                _typeByFullName[typeName] = type;
            }
        }

        return type is not null;
    }

    public static bool IsNullableType(Type type)
        => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

    public static string GetTypeName(Type type)
    {
        lock (_lock)
        {
            return GetTypeNameInternal(type);
        }
    }

    public static bool HasDefaultValue(Type type)
    {
        if (type == typeof(string))
        {
            return true;
        }

        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
        {
            return HasDefaultValue(underlyingType);
        }

        return type.IsValueType && !type.IsEnum;
    }

    public static bool IsDefault(object? value)
    {
        if (value is null || !HasDefaultValue(value.GetType()))
        {
            return false;
        }

        var defaultValue = _defaultByType.GetOrAdd(value.GetType(), CreateDefault);
        return ReferenceEquals(value, defaultValue) || Equals(value, defaultValue);
    }

    public static object GetDefault(Type type)
    {
        if (HasDefaultValue(type))
        {
            return _defaultByType.GetOrAdd(type, CreateDefault);
        }

        throw new ArgumentException(
            $"Type '{type.FullName}' is not a value type and does not have a default value.",
            nameof(type));
    }

    public static bool TryGetDefault(Type type, [MaybeNullWhen(false)] out object defaultValue)
    {
        if (HasDefaultValue(type))
        {
            defaultValue = _defaultByType.GetOrAdd(type, CreateDefault);
            return true;
        }

        defaultValue = null;
        return false;
    }

    public static bool IsKnownType(Type type) => _knownTypes.Contains(type);

    public static bool IsKnownType(string typeName) => _knownTypes.Contains(typeName);

    public static Type GetActualType(object? value, Type type)
    {
        if (value is null)
        {
            return type;
        }

        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
        {
            return typeof(Nullable<>).MakeGenericType(underlyingType);
        }

        return value.GetType();
    }

    public static object CreateInstance(Type type, params object?[] args)
    {
        try
        {
            if (Activator.CreateInstance(type, args: args) is { } obj)
            {
                return obj;
            }
        }
        catch (Exception e)
        {
            throw new ModelCreationException(type, e);
        }

        throw new ModelCreationException(type);
    }

    public static string GetFullName(Type type)
    {
        if (type.Name is null)
        {
            throw new ArgumentException("Type does not have FullName", nameof(type));
        }

        var name = type.Name;
        if (type.IsGenericType)
        {
            var genericArguments = type.GetGenericArguments();
            var nameList = new List<string>(genericArguments.Length);
            foreach (var genericArgument in genericArguments)
            {
                var genericArgumentName = $"[{GetFullName(genericArgument)}]";
                nameList.Add(genericArgumentName);
            }

            name = $"{name}[{string.Join(',', nameList)}]";
        }

        if (type.DeclaringType is null)
        {
            return type.Name;
        }

        return $"{GetFullName(type.DeclaringType)}+{name}";
    }

    private static void AddAssembly(Assembly assembly)
    {
        if (!_addedAssemblies.Add(assembly))
        {
            return;
        }

        var query = from type in assembly.GetTypes()
                    where type.IsDefined(typeof(ModelAttribute)) ||
                          type.IsDefined(typeof(ModelConverterAttribute))
                    select type;

        foreach (var item in query)
        {
            if (item.IsDefined(typeof(ModelAttribute)))
            {
                var attribute = item.GetCustomAttribute<ModelAttribute>()
                    ?? throw new UnreachableException($"{nameof(ModelAttribute)} cannot be null.");
                _knownTypes.AddType(item, attribute.TypeName);
            }
            else if (item.IsDefined(typeof(ModelConverterAttribute)))
            {
                var attribute = item.GetCustomAttribute<ModelConverterAttribute>()
                    ?? throw new UnreachableException($"{nameof(ModelConverterAttribute)} cannot be null.");
                _knownTypes.AddType(item, attribute.TypeName);
            }

            if (item.IsDefined(typeof(ModelKnownTypeAttribute)))
            {
                var knownTypeAttributes = item.GetCustomAttributes<ModelKnownTypeAttribute>();
                foreach (var knownTypeAttribute in knownTypeAttributes)
                {
                    if (item.IsAssignableFrom(knownTypeAttribute.Type))
                    {
                        throw new InvalidModelException(
                            $"Type '{knownTypeAttribute.Type.FullName}' must not be " +
                            $"assignable to '{item.FullName}'.",
                            item);
                    }

                    _knownTypes.AddType(knownTypeAttribute.Type, knownTypeAttribute.TypeName);
                }
            }
        }

        _addedAssemblies.Add(assembly);
    }

    private static object CreateDefault(Type type)
    {
        if (type == typeof(string))
        {
            return string.Empty;
        }

        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
        {
            return CreateDefault(underlyingType);
        }

        return Activator.CreateInstance(type) ?? throw new UnreachableException("ValueType cannot be null");
    }

    private static string GetTypeNameInternal(Type type)
    {
        if (!_addedAssemblies.Contains(type.Assembly))
        {
            AddAssembly(type.Assembly);
        }

        if (Nullable.GetUnderlyingType(type) is { } underlyingType)
        {
            return $"{GetTypeName(underlyingType)}?";
        }
        else if (_knownTypes.TryGetTypeName(type, out var typeName))
        {
            return typeName;
        }
        else if (type.IsGenericType)
        {
            var typeDefinition = type.GetGenericTypeDefinition();
            var typeDefinitionName = _knownTypes.GetTypeName(typeDefinition);
            var genericArguments = type.GetGenericArguments();
            var genericArgumentList = new List<string>(genericArguments.Length);
            foreach (var genericArgument in genericArguments)
            {
                var genericArgumentName = GetTypeName(genericArgument);
                genericArgumentList.Add(genericArgumentName);
            }

            var genericArgumentString = string.Join(',', genericArgumentList);
            return Regex.Replace(typeDefinitionName, "<.*>", $"<{genericArgumentString}>");
        }
        else if (type.IsArray)
        {
            var elementType = type.GetElementType()
                ?? throw new UnreachableException("Array type does not have an element type");
            var elementTypeName = GetTypeName(elementType);
            var rank = type.GetArrayRank();
            return $"{elementTypeName}[{new string(',', rank - 1)}]";
        }
        else if (type.IsDefined(typeof(OriginModelAttribute)))
        {
            var attribute = type.GetCustomAttribute<OriginModelAttribute>()!;
            return GetTypeName(attribute.Type);
        }

        throw new NotSupportedException($"Type '{type}' is not supported or not registered in known types.");
    }
}
